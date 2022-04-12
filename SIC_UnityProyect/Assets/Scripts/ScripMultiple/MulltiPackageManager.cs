using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class MulltiPackageManager : MonoBehaviour
{
    public PackageManajer pm;
    public MultUIContainer multiC;
    public GameObject containerPrefab;
    public GameObject FirstContainer;
    //File, reader and text for reading purposes
    private FileInfo theSourceFile = null;
    private StreamReader reader = null;
    private StreamReader reader1 = null;
    private string text = " "; // assigned to allow first line to be read below
    private string text1 = " "; // assigned to allow first line to be read below
    //Lista de Contenedores
    public List<GameObject> containersList;
    //Lista Pallets
    public List<GameObject> palletsList;
    //Lista de paquetes
    public List<GameObject> packages;
 
    public int currentContainer=1;
    public Color[] arrowColor;
    public Image leftarrow;
    public Image rightarrow;
    public TwoStagePacking twoStage;
    //load bar
    public bool inProcess;
    public bool waitingForText;
    private float TimeLeft;
    public Image LoadBar;
    public GameObject bloking;
    public TwoStageManager stageManager;

    //Pallet prefab
    public GameObject palletPrefab;

    //Save MultiContainer
    public MultTxtManager multTxtManager;
    public MultRecordContainerManager multRecordContainerManager;

    //Save TwoStage
    public TwoStageTxtManager twoStageTxtManager;


    //Size Pallet
    public Vector3 palletSize;

    // Start is called before the first frame update
    void Start()
    {
        inProcess = false;
        containersList = new List<GameObject>();
        containersList.Add(pm.containerGO);
        TimeLeft = 0;
    }

    // Update is called once per frame
    void Update()
    {
        //Animar barra de cargue
        if (inProcess)
        {
            if (TimeLeft < 1)
            {
                TimeLeft += Time.deltaTime;
                LoadBar.fillAmount = TimeLeft;
            }
            else
            {
                TimeLeft = 0;
            }
            if (waitingForText)
            {
                //inProcess = false;
                //MultiLoadingAsync("tempPallet.txt");
            }

        }
    }
    //----------------------------------------------
    //Correr algoritmo escena de Multiple contenedor
    //----------------------------------------------
    public async void multiRunAlgorithm(string path)
    {
        inProcess = true;
        multiC.vehicles("MultipleContainer");
        pm.createMultiFile("MultipleContainer");
        twoStage.runTwoStagePacking(path);
        MultiLoadingAsync("tempPallet.txt");
    }

    //----------------------------------------------
    //Correr algoritmo escena de Dos etapas
    //----------------------------------------------
    public async void TwoStageRunAlgorithm()
    {
        inProcess = true;
        string path= "File.txt";
        multiC.vehicles("TwoStage");
        bloking.SetActive(true);
        pm.createMultiFile("TwoStage");
        stageManager.runFirstStage(path);
        //Cargar Pallets en camion y paquetes en pallets
        //loadingPalletsInContainers("tempPallet.txt");
    }
    public async void MultiLoadingAsync(string path)
    {
        
        bloking.SetActive(true);
        
        deletePackage();
        string path1 = string.Format("{0}/StreamingAssets/MultipleContainer/Algorithm/{1}",
        UnityEngine.Application.dataPath,path);

        

        // pm.validateDelete();
        //------------------------
        //Initialize the reading
        //-------------------------
        theSourceFile = new FileInfo(path1);
        reader = theSourceFile.OpenText();


        //First Line
        text = reader.ReadLine();
        string[] split = text.Split('\t');
        int totalPcs = int.Parse(split[0]);


        //Second Line
        //Second Line
        text = reader.ReadLine();
        split = text.Split('\t');
        pm.volumen_ocupado = float.Parse(split[0]) / 1000000;
        //Read the packages
        string[] lines = System.IO.File.ReadAllLines(path1);
        //creara contador de grupo actual
        pm.numGroups = 0;
        //Validate maxWieght
        bool maxWeightValidator = true;
        float pesoaux = 0;
        List<string> container = new List<string>();
        //----
        for (int i = 2; i < lines.Length; i++)
        {
            //Read Line
            string[] split1 = lines[i].Split('\t');
            //ContainerType
            int containerType = int.Parse(split1[9]);
            //number of the same ContainerType
            int numContainer = int.Parse(split1[10]);
            //create container
            if (containerType== 0 && numContainer == 0 && !container.Contains(split1[9] + split1[10]))
            {
                pm.resizeContainer(multiC.containerArray[0].containerSize);
                FirstContainer.GetComponent<Container>().updateVolume(multiC.containerArray[0].containerSize);
                container.Add(split1[9] + split1[10]);

                //pesoaux = 0;
            }
            else if(numContainer == 0 && !container.Contains(split1[9] + split1[10]))
            {
                pm.resizeContainer(multiC.containerArray[int.Parse(split1[9])].containerSize);
                FirstContainer.GetComponent<Container>().updateVolume(multiC.containerArray[int.Parse(split1[9])].containerSize);
                container.Add(split1[9] + split1[10]);

                //pesoaux = 0;
            }
            else if(!container.Contains(split1[9]+ split1[10]))
            {
                createContainers(int.Parse(split1[9]));
                container.Add(split1[9] + split1[10]);
                maxWeightValidator = true;
                pesoaux = 0;
            }

            float ajustX = 1500f *( float.Parse(split1[10]) -1);
            //Read Package
            Vector3 endPosition = new Vector3(float.Parse(split1[3]), float.Parse(split1[5]), float.Parse(split1[4])) / 100;
            Vector3 packPosition = new Vector3(float.Parse(split1[0]), float.Parse(split1[2]), float.Parse(split1[1])) / 100;
            Vector3 packSize = (endPosition - packPosition) * 100;
            //Evitar error de decimales
            packSize = new Vector3(Convert.ToSingle(Math.Round(packSize.x, 2)), Convert.ToSingle(Math.Round(packSize.y, 2)), Convert.ToSingle(Math.Round(packSize.z, 2)));
            //Read packageId
            int packId = int.Parse(split1[6]);
            //Read packageGroup
            int packGroup = int.Parse(split1[7]);
            //Read packageCostumer
            int packClient = int.Parse(split1[8]);
            //Read container
            int containerID = int.Parse(split1[10]);

            //Read pType
            
            pesoaux += (pm.pTypes[packId].weight) / 10000;
            if (multiC.containerArray[containerType].containerWLimit < pesoaux)
            {
                maxWeightValidator = false;
            }
            print("paquete "+ packPosition.x+" contenedor "+ multiC.containerArray[containerType].containerSize.x);
            if (packPosition.x < multiC.containerArray[containerType].containerSize.x && maxWeightValidator)
            {

                if (packGroup != pm.numGroups)
                {

                    pm.snapCam.group = packGroup - 1;
                    pm.snapCam.callTakeSnapShot();
                    await Task.Delay(5);
                    pm.numGroups = packGroup;
                }
                // Crear paquetes en base al prefab
                GameObject go = GameObject.Instantiate(pm.packagePrefab);
                packages.Add(go);
               
                go.GetComponent<Package>().setPackageValues(endPosition - packPosition
                    , packPosition, packId, pm.packageColor[packId], 10, packClient, packGroup, pm.pTypes[packId].Name1);
                Vector3 size1 = endPosition - packPosition;
                go.transform.SetParent(containersList[containerID].transform);
                //Actualizar indicadores del contenedor
                containersList[containerID].GetComponent<Container>().updateKPI(pm.pTypes[packId].packageSize,pm.pTypes[packId].weight);
                if (i == lines.Length - 1)
                {

                    pm.snapCam.group = packGroup;
                    pm.snapCam.callTakeSnapShot();
                    await Task.Delay(5);
                    pm.numGroups = packGroup;
                }

                
            }
            else
            {
                bool keyExists = pm.intermedio.ContainsKey(packId);
                if (!keyExists)
                {
                    pm.intermedio.Add(packId, 0f);
                }
                pm.intermedio[packId] += 1f;
            }
            
        }
        pm.label_peso.text = pm.peso + " kg";
        reader.Close();
        //Give the option to add a new loading
        //pm.showNewLoading();
        //ajustar canvas
        pm.EstadoCargado();
        //Actualiz<ar contador
        pm.esEste.text="1/" + containersList.Count;
        inProcess = false;
        bloking.SetActive(false);
        recalculateKPI(FirstContainer);
    }
    public async void loadingPalletsInContainers(string path)
    {
       
        bloking.SetActive(true);

        deletePallets();
        string path1 = string.Format("{0}/StreamingAssets/TwoStage/Algorithm/{1}",
        UnityEngine.Application.dataPath, path);


        // pm.validateDelete();
        //------------------------
        //Initialize the reading
        //-------------------------
        theSourceFile = new FileInfo(path1);
        reader = theSourceFile.OpenText();


        //First Line
        text = reader.ReadLine();
        string[] split = text.Split('\t');
        int totalPcs = int.Parse(split[0]);


        //Second Line
        //Second Line
        text = reader.ReadLine();
        split = text.Split('\t');
        pm.volumen_ocupado = float.Parse(split[0]) / 1000000;
        //Read the packages
        string[] lines = System.IO.File.ReadAllLines(path1);
        //creara contador de grupo actual
        pm.numGroups = 0;
        //Validate maxWieght
        bool maxWeightValidator = true;
        float pesoaux = 0;
        List<string> container = new List<string>();
        //----
        for (int i = 2; i < lines.Length; i++)
        {

            //Read Line
            string[] split1 = lines[i].Split('\t');

            //create container
            if (split1[9] == "0" && split1[10] == "0" && !container.Contains(split1[9] + split1[10]))
            {
                pm.resizeContainer(multiC.containerArray[0].containerSize);
                FirstContainer.GetComponent<Container>().updateVolume(multiC.containerArray[0].containerSize);
                container.Add(split1[9] + split1[10]);

            }
            else if (split1[10] == "0" && !container.Contains(split1[9] + split1[10]))
            {
                pm.resizeContainer(multiC.containerArray[int.Parse(split1[9])].containerSize);
                FirstContainer.GetComponent<Container>().updateVolume(multiC.containerArray[int.Parse(split1[9])].containerSize);
                container.Add(split1[9] + split1[10]);

            }
            else if (!container.Contains(split1[9] + split1[10]))
            {
                createContainers(int.Parse(split1[9]));
                container.Add(split1[9] + split1[10]);
                await Task.Delay(5);
            }

            float ajustX = 1500f * (float.Parse(split1[10]) - 1);
            //Read Package
            Vector3 endPosition = new Vector3(float.Parse(split1[3]), float.Parse(split1[5]), float.Parse(split1[4])) / 100;
            Vector3 packPosition = new Vector3(float.Parse(split1[0]), float.Parse(split1[2]), float.Parse(split1[1])) / 100;
            Vector3 packSize = (endPosition - packPosition) * 100;
            //Evitar error de decimales
            packSize = new Vector3(Convert.ToSingle(Math.Round(packSize.x, 2)), Convert.ToSingle(Math.Round(packSize.y, 2)), Convert.ToSingle(Math.Round(packSize.z, 2)));
            //Read packageId
            int packId = int.Parse(split1[6]);
            //Read packageGroup
            int packGroup = int.Parse(split1[7]);
            //Read packageCostumer
            int packClient = int.Parse(split1[8]);
            //Read container
            int containerID = int.Parse(split1[10]);
            if (packPosition.x < pm.containerSize.x && containerID< containersList.Count)
            {

                if (packGroup != pm.numGroups)
                {

                    pm.snapCam.group = packGroup - 1;
                    pm.snapCam.callTakeSnapShot();
                    await Task.Delay(5);
                    pm.numGroups = packGroup;
                }
                //setnamePackage
                int auxCounter = i - 2;
                // Crear paquetes en base al prefab
                GameObject go = GameObject.Instantiate(palletPrefab);
                go.name = "Pallet_" + auxCounter;
                palletsList.Add(go);
                go.transform.position = packPosition;
                go.transform.localScale = new Vector3(packSize.x /100f, packSize.y/100f, packSize.z/100f);
               
               // go.GetComponent<Package>().setPackageValues(endPosition - packPosition , packPosition, packId, pm.packageColor[packId], 10, packClient, packGroup, "NOMBRECAJA");
               Vector3 size1 = endPosition - packPosition;
                go.transform.SetParent(containersList[containerID].transform);
                if (i == lines.Length - 1)
                {

                    pm.snapCam.group = packGroup;
                    pm.snapCam.callTakeSnapShot();
                    await Task.Delay(5);
                    pm.numGroups = packGroup;
                }

            }
            else
            {
                bool keyExists = pm.intermedio.ContainsKey(packId);
                if (!keyExists)
                {
                    pm.intermedio.Add(packId, 0f);
                }
                pm.intermedio[packId] += 1f;
            }

        }
        pm.label_peso.text = pm.peso + " kg";
        reader.Close();
        //Give the option to add a new loading
        //pm.showNewLoading();
        //ajustar canvas
        pm.EstadoCargado();
        //Actualiz<ar contador
        pm.esEste.text = "1/" + containersList.Count;
        //inProcess = false;
        loadingPackagesInPallets("Pallets.txt");
    }

    public  void loadingPackagesInPallets(string path)
    {
        //Crear pallet para rotar
        //GameObject palletRo = GameObject.Instantiate(palletPrefab);
        //palletRo.name = "palletRo";
        //palletRo.SetActive(false);
        //palletRo.transform.position = new Vector3(0f,0f,0f);

        string path1 = string.Format("{0}/StreamingAssets/TwoStage/Algorithm/{1}",
        UnityEngine.Application.dataPath, path);


        // pm.validateDelete();
        //------------------------
        //Initialize the reading
        //-------------------------
        theSourceFile = new FileInfo(path1);
        reader = theSourceFile.OpenText();


        //First Line
        text = reader.ReadLine();
        string[] split = text.Split('\t');
        int totalPcs = int.Parse(split[0]);


        //Second Line
        //Second Line
        text = reader.ReadLine();
        split = text.Split('\t');
        pm.volumen_ocupado = float.Parse(split[0]) / 1000000;
        //Read the packages
        string[] lines = System.IO.File.ReadAllLines(path1);
        //creara contador de grupo actual
        pm.numGroups = 0;
        //Validate maxWieght
        bool maxWeightValidator = true;
        float pesoaux = 0;

        //----
        for (int i = 2; i < lines.Length; i++)
        {
            //Read Line
            string[] split1 = lines[i].Split('\t');

           
            float ajustX = 1500f * (float.Parse(split1[10]) - 1);
            //Read Package
            Vector3 endPosition = new Vector3(float.Parse(split1[3]), float.Parse(split1[5]), float.Parse(split1[4])) / 100;
            Vector3 packPosition = new Vector3(float.Parse(split1[0]), float.Parse(split1[2]), float.Parse(split1[1])) / 100;
            Vector3 packSize = (endPosition - packPosition) * 100;
            //Evitar error de decimales
            packSize = new Vector3(Convert.ToSingle(Math.Round(packSize.x, 2)), Convert.ToSingle(Math.Round(packSize.y, 2)), Convert.ToSingle(Math.Round(packSize.z, 2)));
            //Read packageId
            int packId = int.Parse(split1[6]);
            //Read packageGroup
            int packGroup = int.Parse(split1[7]);
            //Read packageCostumer
            int packClient = int.Parse(split1[8]);
            //Read container
            int palletID = int.Parse(split1[10]);
            if (packPosition.x < pm.containerSize.x && palletID<palletsList.Count)
            {

                //setnamePackage
                int auxCounter = i - 2;
                // Crear paquetes en base al prefab
                GameObject go = GameObject.Instantiate(pm.packagePrefab);
                go.name = "paquete_" + auxCounter;
                packages.Add(go);

              Vector3 size1 = endPosition - packPosition;
                //go.transform.localPosition = new Vector3(packPosition.x + palletsList[containerID].transform.position.x, packPosition.y + palletsList[containerID].transform.position.y, packPosition.z + palletsList[containerID].transform.position.z);
                //go.transform.SetParent(palletsList[containerID].transform);
                //Calcular escala real del pallet

             
                float realx = palletsList[palletID].transform.localScale.x * palletsList[palletID].transform.parent.localScale.x;
                float realz = palletsList[palletID].transform.localScale.z * palletsList[palletID].transform.parent.localScale.z;
                float palletSizeXm = (palletSize.x / 100);
                print("la medida de x" + realx + " el pallet size"+ palletSizeXm +"y la diferencia es"+ (realx - palletSizeXm));
                if(Mathf.Abs((realx-palletSizeXm))<0.0001f)
                {

                    go.GetComponent<Package>().setPackageValues(endPosition - packPosition
                 , packPosition, packId, pm.packageColor[packId], 10, packClient, packGroup, pm.pTypes[packId].Name1);

                    go.transform.localPosition = new Vector3(packPosition.x + palletsList[palletID].transform.position.x, packPosition.y + palletsList[palletID].transform.position.y, packPosition.z + palletsList[palletID].transform.position.z);
                    go.transform.SetParent(palletsList[palletID].transform);
                    palletsList[palletID].transform.parent.GetComponent<Container>().updateKPI(pm.pTypes[packId].packageSize, pm.pTypes[packId].weight);


                }
                else
                {

                    print("entro en el pallet:" + palletID);
                    go.GetComponent<Package>().setPackageValues(new Vector3(size1.z, size1.y, size1.x),
                   new Vector3(packPosition.z, packPosition.y, packPosition.x), packId, pm.packageColor[packId], 10, packClient, packGroup, pm.pTypes[packId].Name1);

                    //Transform parent = go.transform.parent;
                    //go.transform.RotateAround(transform.TransformPoint(0.5f, 0.5f, 0.5f), Vector3.up, 90f);

                    go.transform.localPosition = new Vector3(packPosition.z + palletsList[palletID].transform.position.x, packPosition.y + palletsList[palletID].transform.position.y, packPosition.x + palletsList[palletID].transform.position.z);

                    //go.transform.SetParent(palletRo.transform);
                    //palletRo.transform.RotateAround(transform.TransformPoint(0.5f, 0.5f, 0.5f), Vector3.up, 90f);
                    //go.transform.SetParent(parent);
                    go.transform.SetParent(palletsList[palletID].transform);
                    palletsList[palletID].transform.parent.GetComponent<Container>().updateKPI(pm.pTypes[packId].packageSize, pm.pTypes[packId].weight);

                    //palletRo.transform.RotateAround(transform.TransformPoint(0.5f, 0.5f, 0.5f), Vector3.up, -90f);

                }
                //go.transform.SetParent(palletsList[containerID].transform);
            }
            else
            {
                bool keyExists = pm.intermedio.ContainsKey(packId);
                if (!keyExists)
                {
                    pm.intermedio.Add(packId, 0f);
                }
                pm.intermedio[packId] += 1f;
            }

        }
        pm.label_peso.text = pm.peso + " kg";
        reader.Close();
        //Give the option to add a new loading
        //pm.showNewLoading();
        //ajustar canvas
        pm.EstadoCargado();
        //Actualiz<ar contador
        pm.esEste.text = "1/" + containersList.Count;
        inProcess = false;
        bloking.SetActive(false);
        recalculateKPI(FirstContainer);
    }

    public void restart()
    {
        deletePackage();
        pm.reestablecer();
        pm.emptyMsg.SetActive(false);
    }

    public void restartTwoStage()
    {
        deletePallets();
        pm.reestablecer();
        pm.emptyMsg.SetActive(false);
    }
    public void deletePackage()
    {
        for (int i = 0; i < packages.Count; i++)
        {
            GameObject.Destroy(packages[i]);
        }
        packages.Clear();
        for (int i = 1; i < containersList.Count; i++)
        {
            GameObject.Destroy(containersList[i]);
        }
        containersList.Clear();
        containersList.Add(pm.containerGO);
        currentContainer = 1;
        containersList[0].SetActive(true);
        leftarrow.color = arrowColor[1];
        rightarrow.color = arrowColor[1];
        foreach (Transform object1 in containersList[0].transform)
        {
            if (object1.name == "Objective")
            {
                pm.cameraControl.currentObjective = object1;
                pm.esEste.text = currentContainer + "/1";
                break;
            }
        }
        pm.volumen_ocupado = 0;
        pm.peso = 0;
        pm.label_peso.text = pm.peso + " kg";

    }
    public void deletePallets()
    {
        for (int i = 0; i < palletsList.Count; i++)
        {
            GameObject.Destroy(palletsList[i]);
        }
        palletsList.Clear();
        for (int i = 1; i < containersList.Count; i++)
        {
            GameObject.Destroy(containersList[i]);
        }
        containersList.Clear();
        containersList.Add(pm.containerGO);
        currentContainer = 1;
        containersList[0].SetActive(true);
        leftarrow.color = arrowColor[1];
        rightarrow.color = arrowColor[1];
        foreach (Transform object1 in containersList[0].transform)
        {
            if (object1.name == "Objective")
            {
                pm.cameraControl.currentObjective = object1;
                pm.esEste.text = currentContainer + "/1";
                break;
            }
        }
        pm.volumen_ocupado = 0;
        pm.peso = 0;
        pm.label_peso.text = pm.peso + " kg";

    }
    public void createContainers(int Type)
    {
        rightarrow.color = arrowColor[0];
        pm.numContainers++;
        GameObject newContainer = Instantiate(containerPrefab);
        containersList.Add(newContainer);
        newContainer.transform.position = new Vector3(pm.containerGO.transform.position.x, pm.containerGO.transform.position.y, pm.containerGO.transform.position.z);
        newContainer.transform.SetParent(pm.ParentContainer.transform);
        print(multiC.containerArray.Count);
        newContainer.transform.localScale = multiC.containerArray[Type].containerSize;
        newContainer.SetActive(false);
        newContainer.GetComponent<Container>().updateVolume(newContainer.transform.localScale);
        

    }
    
    public void rightContainer()
    {
        if (currentContainer < containersList.Count())
        {
            
            currentContainer++;
            leftarrow.color = arrowColor[0];
            if (currentContainer== containersList.Count())
            {
                rightarrow.color = arrowColor[1];
            }
            
                for (int i = 0 ; i < containersList.Count; i++)
            {
                containersList[i].SetActive(false);
            }
                containersList[currentContainer - 1].SetActive(true);

            //indicadores
            recalculateKPI(containersList[currentContainer - 1]);


            foreach (Transform object1 in containersList[currentContainer - 1].transform)
            {
                if (object1.name == "Objective")
                {
                    pm.cameraControl.currentObjective = object1;
                    pm.esEste.text = currentContainer + "/" + containersList.Count;
                    
                }
                else if(object1.name =="Pallet")
                {
                    object1.gameObject.SetActive(false);
                    break;
                }
            }
        }

    }

    public void leftContainer()
    {
        if (currentContainer > 1)
        {

            currentContainer--;
            rightarrow.color = arrowColor[0];
            if (currentContainer == 1)
            {
                leftarrow.color = arrowColor[1];
            }

            for (int i = 0; i < containersList.Count; i++)
            {
                containersList[i].SetActive(false);
            }
            containersList[currentContainer - 1].SetActive(true);
            recalculateKPI(containersList[currentContainer - 1]);
            foreach (Transform object1 in containersList[currentContainer - 1].transform)
            {
                if (object1.name == "Objective")
                {
                    pm.cameraControl.currentObjective = object1;
                    pm.esEste.text = currentContainer + "/" + containersList.Count;
                }
                else if (object1.name == "Pallet")
                {
                    object1.gameObject.SetActive(false);
                    break;
                }
            }
        }   
    }
    void recalculateKPI(GameObject container)
    {
        pm.total_Volume.text = container.GetComponent<Container>().total_volume + " m3";
        pm.free_Volume.text = container.GetComponent<Container>().free_volume + " m3";
        pm.used_Volume.text = container.GetComponent<Container>().used_volume + " m3";
        pm.label_peso.text = container.GetComponent<Container>().weight + " kg";
        pm.nombre_container.text = container.transform.localScale.x + " m x " +
         container.transform.localScale.y + " m x " +
         container.transform.localScale.z + " m  ";

    }
    public void addNames(string route, string file)
    {
        string path = string.Format("{0}/StreamingAssets/{1}/Algorithm/{2}",
             UnityEngine.Application.dataPath,route, "file_aux.txt");
        string[] file2 = File.ReadAllLines(string.Format("{0}/StreamingAssets/{1}/Algorithm/{2}",
        UnityEngine.Application.dataPath,route, file));


        File.Delete(path);
        //Create file axiliar with the names
        using (StreamWriter writer = new StreamWriter(path, true))
        {
            string output = file2[0] + "\n" + "\n";
            writer.Write(output);
            output = file2[2] + "\n" + "\n";
            writer.Write(output);
            for (int i = 4; i < file2.Length; i = i + 2)
            {
                string[] split1 = file2[i].Split('\t');
                int id = int.Parse(split1[0]);
                output = file2[i] + pm.pTypes[id].Name1 + "\n" + "\n";
                writer.Write(output);
            }
            writer.Close();
        }
    }
    public void MultSave()
    {
        if (!pm.activeFuntion)
        {
            pm.activeFuntion = true;
            addNames("MultipleContainer","file.txt");


            //arachivos de origen
            string[] file1 = File.ReadAllLines(string.Format("{0}/StreamingAssets/MultipleContainer/Algorithm/{1}",
                UnityEngine.Application.dataPath, "tempPallet.txt"));
            string[] file2 = File.ReadAllLines(string.Format("{0}/StreamingAssets/MultipleContainer/Algorithm/{1}",
          UnityEngine.Application.dataPath, "vehiculos.txt"));
            string[] file3 = File.ReadAllLines(string.Format("{0}/StreamingAssets/MultipleContainer/Algorithm/{1}",
             UnityEngine.Application.dataPath, "file_aux.txt"));

            //Archivo de destino

            string fileName = string.Format("{0}.txt", pm.Titulo.text);

            string destFile = string.Format("{0}/StreamingAssets/MultipleContainer/Reports/{1}",
                   UnityEngine.Application.dataPath, fileName);
            if (File.Exists(destFile))
            {
                File.Delete(destFile);
            }
          
            using (TextWriter tw = new StreamWriter(destFile, true))
            {
                for (int i = 0; i < file1.Length; i++)
                {
                    tw.WriteLine(file1[i]);
                }
                tw.WriteLine();
                for (int i = 0; i < file2.Length; i++)
                {
                    tw.WriteLine(file2[i]);
                }
                tw.WriteLine();
                for (int i = 0; i < file3.Length; i++)
                {
                    tw.WriteLine(file3[i]);
                }
                tw.WriteLine();
            }

            pm.Nombre_Registro.text = pm.Titulo.text;
            pm.Fecha_Registro.text = "" + DateTime.Now;
            pm.tamaño_Contenedor.text = "(" + pm.containerTransform.localScale.x + "m x" + pm.containerTransform.localScale.y + "m x" + pm.containerTransform.localScale.z + "m )";
            multTxtManager.precargar();
            pm.saveMsg.SetActive(true);
            pm.activeFuntion = false;
        }
    }

    public void TwoStageSave()
    {
        if (!pm.activeFuntion)
        {
            pm.activeFuntion = true;
            addNames("TwoStage", "file.txt");


            //arachivos de origen
            string[] file1 = File.ReadAllLines(string.Format("{0}/StreamingAssets/TwoStage/Algorithm/{1}",
                UnityEngine.Application.dataPath, "tempPallet.txt"));
            string[] file2 = File.ReadAllLines(string.Format("{0}/StreamingAssets/TwoStage/Algorithm/{1}",
          UnityEngine.Application.dataPath, "vehiculos.txt"));
            string[] file3 = File.ReadAllLines(string.Format("{0}/StreamingAssets/TwoStage/Algorithm/{1}",
             UnityEngine.Application.dataPath, "file_aux.txt"));
            string[] file4 = File.ReadAllLines(string.Format("{0}/StreamingAssets/TwoStage/Algorithm/{1}",
         UnityEngine.Application.dataPath, "pallets.txt"));

            //Archivo de destino

            string fileName = string.Format("{0}.txt", pm.Titulo.text);

            string destFile = string.Format("{0}/StreamingAssets/TwoStage/Reports/{1}",
                   UnityEngine.Application.dataPath, fileName);
            if (File.Exists(destFile))
            {
                File.Delete(destFile);
            }

            using (TextWriter tw = new StreamWriter(destFile, true))
            {
                for (int i = 0; i < file1.Length; i++)
                {
                    tw.WriteLine(file1[i]);
                }
                tw.WriteLine();
                for (int i = 0; i < file2.Length; i++)
                {
                    tw.WriteLine(file2[i]);
                }
                tw.WriteLine();
                for (int i = 0; i < file4.Length; i++)
                {
                    tw.WriteLine(file4[i]);
                }
                tw.WriteLine();
                for (int i = 0; i < file3.Length; i++)
                {
                    tw.WriteLine(file3[i]);
                }
                tw.WriteLine();
            }

            pm.Nombre_Registro.text = pm.Titulo.text;
            pm.Fecha_Registro.text = "" + DateTime.Now;
            pm.tamaño_Contenedor.text = "(" + pm.containerTransform.localScale.x + "m x" + pm.containerTransform.localScale.y + "m x" + pm.containerTransform.localScale.z + "m )";
            twoStageTxtManager.precargar();
            pm.saveMsg.SetActive(true);
            pm.activeFuntion = false;
        }
    }
    public async void ReadMultiTxt(Text path)
    {
        restart();
        //Change the tap selected
        pm.tapGroup.OnTabSelected(pm.CargaTap);
        //Claer intermedio
        pm.intermedio.Clear();
        //Path del arrcivo a leer
        string path1 = string.Format("{0}/StreamingAssets/MultipleContainer/Reports/{1}",
        UnityEngine.Application.dataPath, path.text);
        //Archivo temp
        string temp = string.Format("{0}/StreamingAssets/MultipleContainer/Algorithm/{1}",
       UnityEngine.Application.dataPath, "tempPallet.txt");

        //-------
        //Leer lineas del archivo
        string[] lines = System.IO.File.ReadAllLines(path1);

        //Calcular el limite del archivo que divide temp.txt de vehciulos.txt y file.txt 
        int limite = 0;
        int limite1 = 0;
        int counter=0;
        for (int i = 0; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i]) && counter==0)
            {
                limite = i-1;
                counter++;
            }
            else if (string.IsNullOrWhiteSpace(lines[i]) && counter == 1)
            {
                limite1 = i-1;
                break;
            }
        }
        print("los limites son "+ limite  +" "+ limite1);
        
        //Actualizar temp
        File.Delete(temp);
        using (TextWriter tw = new StreamWriter(temp, true))
        {
            for (int i = 0; i <= limite; i++)
            {
                tw.WriteLine(lines[i]);
            }
        }
        //traer datos de vehiculos.txt
        multRecordContainerManager.DeleteRecordsCargaTap();
        Vector3 size;
        for (int i = limite+3; i <= limite1; i++)
        {
            string[] split = lines[i].Split('\t');
            size = new Vector3(float.Parse(split[1])/100f, float.Parse(split[2]) / 100f, float.Parse(split[3]) / 100f);
            multiC.saveNewCustomCanvasValues(split[7],size.x+"", size.y+"", size.z+"", split[4],split[5],split[6]);

            //  multiC.saveNewCustomCanvasValues(ContainerName.text, lenghtLabel.text, highLabel.text, widthLabel.text, quantityLabel.text, weightLabel.text, priceLabel.text);
        }
              
        
        //traer datos de file.txt
        for (int i = limite1+2; i <lines.Length; i++)
        {
            if (i == limite1 + 2)
            {
                string[] split1 = lines[i].Split('\t');
                pm.numClient = int.Parse(split1[1]);
            }
            else if (i == limite1 + 4)
            {

            }
            else if (!string.IsNullOrWhiteSpace(lines[i]))
            {
                //Dividir valores
                string[] split1 = lines[i].Split('\t');
                //Cargar datos
                int packId = int.Parse(split1[0]);
                Vector3 packSize = new Vector3(float.Parse(split1[1]), float.Parse(split1[5]), float.Parse(split1[3]));
                Vector3 verticalPostP = new Vector3(float.Parse(split1[2]), float.Parse(split1[6]), float.Parse(split1[4]));
                //maxForce
                Vector3 maxForceP = new Vector3();
                maxForceP.x = Mathf.Floor(float.Parse(split1[9]) * (packSize.x * packSize.y) / 10000f);
                maxForceP.y = Mathf.Floor(float.Parse(split1[10]) * (packSize.y * packSize.z) / 10000f);
                maxForceP.z = Mathf.Floor(float.Parse(split1[11]) * (packSize.x * packSize.z) / 10000f);
                int quantity = int.Parse(split1[7]);
                int weight = int.Parse(split1[8]);
                int client = int.Parse(split1[12]);
                string name = split1[14];
                pm.newPackage(packSize, verticalPostP, maxForceP, quantity, weight, false, packId, client, name);
                pm.updatePackageValues(packId, packSize, verticalPostP, new Vector3(float.Parse(split1[9]), float.Parse(split1[10]), float.Parse(split1[11])), quantity, weight, name, client);
            }
        }
        //creara contador de grupo actual
        pm.numGroups = 0;
        //----


        //cahnge the state of the canvas
        MultiLoadingAsync("tempPallet.txt");
        string path_string = path.text;
        pm.Titulo.text = path_string.Substring(0, path_string.IndexOf("."));
    }
    public async void ReadTwoStageTxt(Text path)
    {
        restart();
        //Change the tap selected
        pm.tapGroup.OnTabSelected(pm.CargaTap);
        //Claer intermedio
        pm.intermedio.Clear();
        //Path del arrcivo a leer
        string path1 = string.Format("{0}/StreamingAssets/TwoStage/Reports/{1}",
        UnityEngine.Application.dataPath, path.text);
        //Archivo temp
        string temp = string.Format("{0}/StreamingAssets/TwoStage/Algorithm/{1}",
       UnityEngine.Application.dataPath, "tempPallet.txt");
        //Pallets
        string pallets = string.Format("{0}/StreamingAssets/TwoStage/Algorithm/{1}",
       UnityEngine.Application.dataPath, "pallets.txt");
        //-------
        //Leer lineas del archivo
        string[] lines = System.IO.File.ReadAllLines(path1);

        //Calcular el limite del archivo que divide temp.txt de vehciulos.txt y file.txt 
        int limite = 0;
        int limite1 = 0;
        int limite2 = 0;
        int counter = 0;
        for (int i = 0; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i]) )
            {
                if(counter==0)
                {
                    limite = i - 1;
                    counter++;
                }
                else if ( counter == 1)
                {
                    limite1 = i - 1;
                    counter++;
                }
                else if (counter == 2)
                {
                    limite2 = i - 1;
                    break;
                }
            }
            
            
        }
        print("los limites son " + limite + " " + limite1+" "+ limite2);

        //Actualizar temp
        File.Delete(temp);
        using (TextWriter tw = new StreamWriter(temp, true))
        {
            for (int i = 0; i <= limite; i++)
            {
                tw.WriteLine(lines[i]);
            }
        }
        //traer datos de vehiculos.txt
        multRecordContainerManager.DeleteRecordsCargaTap();
        Vector3 size;
        for (int i = limite + 3; i <= limite1; i++)
        {
           
            string[] split = lines[i].Split('\t');
            size = new Vector3(float.Parse(split[1]) / 100f, float.Parse(split[2]) / 100f, float.Parse(split[3]) / 100f);
            multiC.saveNewCustomCanvasValues(split[7], size.x + "", size.y + "", size.z + "", split[4], split[5], split[6]);

            //  multiC.saveNewCustomCanvasValues(ContainerName.text, lenghtLabel.text, highLabel.text, widthLabel.text, quantityLabel.text, weightLabel.text, priceLabel.text);
        }

        //Actualizar pallets
        File.Delete(pallets);
        using (TextWriter tw = new StreamWriter(pallets, true))
        {
            for (int i = limite1+2; i <= limite2; i++)
            {
                tw.WriteLine(lines[i]);
            }
        }

        //traer datos de file.txt
        for (int i = limite2 + 2; i < lines.Length; i++)
        {
           
            if (i == limite2 + 2)
            {
                string[] split1 = lines[i].Split('\t');
                pm.numClient = int.Parse(split1[1]);
            }
            else if (i == limite2 + 4)
            {

            }
            else if (!string.IsNullOrWhiteSpace(lines[i]))
            {
                print("la linea es. " + i);
                //Dividir valores
                string[] split1 = lines[i].Split('\t');
                //Cargar datos
                int packId = int.Parse(split1[0]);
                Vector3 packSize = new Vector3(float.Parse(split1[1]), float.Parse(split1[5]), float.Parse(split1[3]));
                Vector3 verticalPostP = new Vector3(float.Parse(split1[2]), float.Parse(split1[6]), float.Parse(split1[4]));
                //maxForce
                Vector3 maxForceP = new Vector3();
                maxForceP.x = Mathf.Floor(float.Parse(split1[9]) * (packSize.x * packSize.y) / 10000f);
                maxForceP.y = Mathf.Floor(float.Parse(split1[10]) * (packSize.y * packSize.z) / 10000f);
                maxForceP.z = Mathf.Floor(float.Parse(split1[11]) * (packSize.x * packSize.z) / 10000f);
                int quantity = int.Parse(split1[7]);
                int weight = int.Parse(split1[8]);
                int client = int.Parse(split1[12]);
                string name = split1[14];
                pm.newPackage(packSize, verticalPostP, maxForceP, quantity, weight, false, packId, client, name);
                pm.updatePackageValues(packId, packSize, verticalPostP, new Vector3(float.Parse(split1[9]), float.Parse(split1[10]), float.Parse(split1[11])), quantity, weight, name, client);
            }
        }
        //creara contador de grupo actual
        pm.numGroups = 0;
        //----


        //cahnge the state of the canvas
        loadingPalletsInContainers("TempPallet.txt");
        string path_string = path.text;
        pm.Titulo.text = path_string.Substring(0, path_string.IndexOf("."));
    }

    public void multGroupColorFuntion()
    {

        float var = (1f / pm.numGroups);
        Debug.Log("el numero de grupos es " + pm.numGroups + "y las var " + var);
        for (int i = 0; i < containersList.Count; i++)
        {
            foreach (Transform package in containersList[i].transform)
            {
                int id;
                GameObject go = package.gameObject;
                if ( go.name.Contains("Package"))
                {
                    id = go.GetComponent<Package>().groupId;
                    go.GetComponent<Renderer>().material.color = new Color(1 - var * id, var * id, 0.2f);
                }
            }
        }
    }
    //----------------------------------------------------------
    //Change the color of the packege in base of the ID
    //----------------------------------------------------------
    public void multPackColorFuntion()
    {
        for (int i = 0; i < containersList.Count; i++)
        {
            foreach (Transform package in containersList[i].transform)
            {
                int id;
                GameObject go = package.gameObject;
                if (go.name.Contains("Package"))
                {
                    id = go.GetComponent<Package>().itemId;
                    go.GetComponent<Renderer>().material.color = pm.packageColor[id];
                }
            }
        }

    }
    //----------------------------------------------------------
    //Identify the type of change of color to apply
    //----------------------------------------------------------
    public void multChangeColor()
    {
        if (pm.groupColor && !pm.VistaClient.isOn)
        {
            multPackColorFuntion();
            pm.groupColor = false;
        }
        else
        {
            multGroupColorFuntion();
            pm.groupColor = true;
            pm.VistaClient.isOn = false;
        }
    }
    /// <summary>
    //Change the color of the package in base of the group
    /// </summary>
    public void multClientColorFuntion()
    {
        Vector3 firtsColor = new Vector3(0, 0.4f, 1f);
        Vector3 lastColor = new Vector3(0.9f, 0.7f, 0.1f);
        Vector3 diferenceColor = new Vector3(0f, 0f, 0f);
        if (pm.numClient > 1)
        {
            diferenceColor = (lastColor - firtsColor) / (pm.numClient - 1);
        }

        print("numero de clienest es " + pm.numClient);
        for (int i = 0; i < containersList.Count; i++)
        {
            foreach (Transform package in containersList[i].transform)
            {
                int id;
                GameObject go = package.gameObject;
                if (go.name.Contains("Package"))
                {
                    id = go.GetComponent<Package>().client;
                    go.GetComponent<Renderer>().material.color = new Color(firtsColor.x + diferenceColor.x * id, firtsColor.y + diferenceColor.y * id, firtsColor.z + diferenceColor.z * id);
                    Vector3 printCOlor = new Vector3(firtsColor.x + diferenceColor.x * id, firtsColor.y + diferenceColor.y * id, firtsColor.z + diferenceColor.z * id);
                    print(printCOlor);
                }
            }
        }


    }

    //----------------------------------------------------------
    //Identify the type of change of color to apply
    //----------------------------------------------------------
    public void multChangeColorClient()
    {
        if (pm.groupColor && !pm.VistaGrupos.isOn)
        {
            multPackColorFuntion();
            pm.groupColor = false;
        }
        else
        {
            multClientColorFuntion();
            pm.groupColor = true;
            pm.VistaGrupos.isOn = false;
        }
    }

    public void TwoStageGroupColorFuntion()
    {
        float var = (1f / pm.numGroups);
        Debug.Log("el numero de grupos es " + pm.numGroups + "y las var " + var);
        for (int i = 0; i < palletsList.Count; i++)
        {
            foreach (Transform package in palletsList[i].transform)
            {
                int id;
                GameObject go = package.gameObject;
                if (go.name.Contains("paquete"))
                {
                    id = go.GetComponent<Package>().groupId;
                    go.GetComponent<Renderer>().material.color = new Color(1 - var * id, var * id, 0.2f);
                }
            }
        }        
    }
    //----------------------------------------------------------
    //Change the color of the packege in base of the ID
    //----------------------------------------------------------
    public void TwoStagePackColorFuntion()
    {
        for (int i = 0; i < palletsList.Count; i++)
        {
            foreach (Transform package in palletsList[i].transform)
            {
                int id;
                GameObject go = package.gameObject;
                if (go.name.Contains("paquete"))
                {
                    id = go.GetComponent<Package>().itemId;
                    go.GetComponent<Renderer>().material.color = pm.packageColor[id];
                }
            }
        }

    }
    //----------------------------------------------------------
    //Identify the type of change of color to apply
    //----------------------------------------------------------
    public void TwoStageChangeColor()
    {
        if (pm.groupColor && !pm.VistaClient.isOn)
        {
            TwoStagePackColorFuntion();
            pm.groupColor = false;
        }
        else
        {
            TwoStageGroupColorFuntion();
            pm.groupColor = true;
            pm.VistaClient.isOn = false;
        }
    }
    /// <summary>
    //Change the color of the package in base of the group
    /// </summary>
    public void TwoStageClientColorFuntion()
    {
        Vector3 firtsColor = new Vector3(0, 0.4f, 1f);
        Vector3 lastColor = new Vector3(0.9f, 0.7f, 0.1f);
        Vector3 diferenceColor = new Vector3(0f, 0f, 0f);
        if (pm.numClient > 1)
        {
            diferenceColor = (lastColor - firtsColor) / (pm.numClient - 1);
        }

        print("numero de clienest es " + pm.numClient);
        for (int i = 0; i < palletsList.Count; i++)
        {
            foreach (Transform package in palletsList[i].transform)
            {
                int id;
                GameObject go = package.gameObject;
                if (go.name.Contains("paquete"))
                {
                    id = go.GetComponent<Package>().client;
                    go.GetComponent<Renderer>().material.color = new Color(firtsColor.x + diferenceColor.x * id, firtsColor.y + diferenceColor.y * id, firtsColor.z + diferenceColor.z * id);
                    Vector3 printCOlor = new Vector3(firtsColor.x + diferenceColor.x * id, firtsColor.y + diferenceColor.y * id, firtsColor.z + diferenceColor.z * id);
                    print(printCOlor);
                }
            }
        }


    }

    //----------------------------------------------------------
    //Identify the type of change of color to apply
    //----------------------------------------------------------
    public void TwoStageChangeColorClient()
    {
        if (pm.groupColor && !pm.VistaGrupos.isOn)
        {
            TwoStagePackColorFuntion();
            pm.groupColor = false;
        }
        else
        {
            TwoStageClientColorFuntion();
            pm.groupColor = true;
            pm.VistaGrupos.isOn = false;
        }
    }
}
