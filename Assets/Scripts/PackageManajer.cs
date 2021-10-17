using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Runtime.InteropServices;
using System;
using System.IO;
using System.Diagnostics;
using Debug = UnityEngine.Debug;
using System.Threading;
using System.Threading.Tasks;

//-----------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using Common;
using UnityEditor;

public class PackageManajer : MonoBehaviour
{

    //**************************************************************************
    //The definiton of a package type
    public class PackageType
    {

        //----------------------------------
        //PUBLIC VARIABLES
        //----------------------------------

        //Is possible vertical 0 no 1 yes
        public Vector3 verticalPos;
        public Vector3 maxForce;
        //Client and group id
        public int clientId;
        public int groupId;
        public Vector3 packageSize;
        //The Color of the Package
        public Color packageColor;
        //The id of the package
        public int packageId;
        //the number of packages
        public int quantity;
        //weight of the package
        public float weight;
        //The ui package controller
        public ActualizarPackage uiPackage;
        //Positions for each package of this group
        public List<Vector3> packagePositions;
        //End position for each package
        public List<Vector3> endPositions;
        //orden de cargue 
        //Nombre del paquete
        public string Name1;
        //----------------------------
        //Methods
        //----------------------------
        public PackageType(Color packageColorP, int packageIdP, Vector3 packageSizeP, Vector3 verticalPosP, Vector3 maxForceP, int quantityP, float weightP, ActualizarPackage uiPackageP,int clientID,  string name)
        {
            packageColor = packageColorP;
            packageSize = packageSizeP;
            packageId = packageIdP;
            quantity = quantityP;
            weight = weightP;
            uiPackage = uiPackageP;
            verticalPos = verticalPosP;
            maxForce = maxForceP;
            packagePositions = new List<Vector3>();
            endPositions = new List<Vector3>();
            clientId = clientID;
            Name1 = name;
        }
        public void updatePType(Color packageColorP, int packageIdP, Vector3 packageSizeP, Vector3 verticalPosP, Vector3 maxForceP, int quantityP, float weightP, ActualizarPackage uiPackageP, String NameValue, float client)
        {

            packageColor = packageColorP;
            packageSize = packageSizeP;
            packageId = packageIdP;
            quantity = quantityP;
            weight = weightP;
            uiPackage = uiPackageP;
            Name1 = NameValue;
            verticalPos = verticalPosP;
            maxForce = maxForceP;
            clientId = (int)client;
        }
        public void AddPosition(Vector3 initialPosition, Vector3 endPosition)
        {
            packagePositions.Add(initialPosition);
            endPositions.Add(endPosition);
        }


    }
    //**************************************************************************
    //----------------------------------
    // VARIABLES
    //----------------------------------
    //--------------
    // ActiveFuntion Cargar
    public bool irregular;
    public bool activeFuntion = false;
    public RectTransform parent;
    //UI Package Model
    public GameObject modelo;
    //Space for list of Model intance
    public RectTransform Lista;
    //Scroll bar of the list
    public RectTransform Scroll;
    //List Height 
    public float height;
    //Number of pacakge
    public int NumCarga;
    //Text of the active package
    public Text controlador;
    //example .\Assets\Chep\chep.txt
    public string filePath;
    //Color image in the general UI of the model
    public Image ColorImage;
    //Color image in the miniature UI of the model
    public Image ColorMiniatura;
    //Specefic color of the ID
    public Color[] packageColor;

    public Renderer myRenderer;
    //Preview of the package
    public GameObject Cube;
    //Numbre in General UI model
    public Text numero;
    //Numbre in miniature UI model
    public Text numeroMiniatura;
    //The prefab of a package
    public GameObject packagePrefab;
    // Nombre 
    public InputField Name;
    //Size of the container in m
    public Vector3 containerSize;
    //The transform of the container
    public Transform containerTransform;
    //Singleton instance
    public static PackageManajer instance;
    //The package types for each package
    public Dictionary<int, PackageType> pTypes;
    //The package types for each package
    public Dictionary<int, PackageType> CopypTypes;
    //lista de objetos a cargar
    private List<GameObject> packages;
    private List<GameObject> packagesUI;
    public List<GameObject> listUi;

    //inputfields size
    public InputField lenght_Input;
    public InputField height_Input;
    public InputField width_Input;
    //inputfield Name
    public InputField NameText;
    //Text for weight
    public InputField weight_Input;
    //Text for pcs
    public InputField pcs_Input;
    //Nombre proyecto
    public InputField Nombre_proyecto;
    //Inputfields fuerza
    public InputField Fuerza_x_input;
    public InputField Fuerza_y_input;
    public InputField Fuerza_z_input;
    // Menu containers 
    public GameObject menuContainers;
    //Cliente
    public InputField client_input;

    //File, reader and text for reading purposes
    private FileInfo theSourceFile = null;
    private StreamReader reader = null;
    private StreamReader reader1 = null;
    private string text = " "; // assigned to allow first line to be read below
    private string text1 = " "; // assigned to allow first line to be read below
    //last id used
    private int lastId;
    //Resgistro en tabla;
    public GameObject registro;
    //Tabla de registros
    public RectTransform tablaRegistros;
    //Nombre de archivo
    public InputField Titulo;
    //Nombre registro
    public Text Nombre_Registro;
    //Fecha regsitro
    public Text Fecha_Registro;
    //Container resgistro
    public Text tamaño_Contenedor;
    //Quantity Array of the package 
    int[] array_quantity = new int[100];
    //Indicator of total volume
    public Text total_Volume;
    //Indicator of free volume
    public Text free_Volume;
    //Indicator of total weight
    public Text label_peso;
    //Rotation in the model
    public Toggle rotacion_x;
    public Toggle rotacion_z;
    public Toggle panel_Rotacion;
    //Force in the model
    public Toggle panel_Fuerza;
    public GameObject panel_Fuerza_object;
    public Toggles toggles;
    public float peso = 0;
    //Nombre contenedor
    public Text nombre_container;
    //Volumne de contenedor
    private float volumen_total;
    public float volumen_ocupado = 0;
    public TxtManager txtManager;
    //Loading bar 
    public Image LoadBar;
    private bool loading_bar = false;
    private float TimeLeft;

    //Lista de objetos con cargue intermedio o basio
    public Dictionary<float, float> intermedio;
    //Camara de carga
    public SnapShot snapCam;
    public int numGroups;
    public int numClient;
    //Archivo de carga CSV
    public TextAsset textAssetData;
    //Menu csv
    public GameObject menuCsv;
    //No package message
    public GameObject NoPackage;
    public GameObject NoTypes;
    public GameObject saveMsg;
    //Empty message
    public GameObject emptyMsg;
    //toogle of Group View
    public Toggle VistaGrupos;
    //toogle of Client View
    public Toggle VistaClient;
    //Bool of the color depent of the group of the package
    private bool groupColor;
    //Controller of the taps
    public TabGroup tapGroup;
    //Carga tap
    public TapButtom CargaTap;
    //new loading Button
    public GameObject newLoadingButtom;
    // Main Camera
    public Transform Camera;
    //Counter of new loading
    private int counter_title = 1;
    //msgBox
    public GameObject alerta;
    public Text alertext;
    //RactiveFuntion
    public  int counterActive;
    //private Process process;
    private Process process = new Process();
    private bool inProcess=false;
    public GameObject bloking;
    //wieght limit
    public InputField maxWeight_iF;
    private float maxWeight;
    private bool maxWeight_bol;
    //----------------------------------
    //METHODS
    //----------------------------------
    //Singleton
    void Awake()
    {
        //Check if instance already exists
        if (instance == null)
            //if not, set instance to this
            instance = this;
        //If instance already exists and it's not this:
        else if (instance != this)
            //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
            Destroy(gameObject);

        //Sets this to not be destroyed when reloading scene
        //DontDestroyOnLoad(gameObject);
    }
    // Use this for initialization

    // Start is called before the first frame updateDf
    void Start()
    {
        tapGroup.OnTabSelected(CargaTap);
        pTypes = new Dictionary<int, PackageType>();
        intermedio = new Dictionary<float, float>();
        packages = new List<GameObject>();
        packagesUI = new List<GameObject>();

        lenght_Input.text = 50 + "";
        height_Input.text = 50 + "";
        width_Input.text = 50 + "";
        weight_Input.text = 1 + "";
        pcs_Input.text = 1 + "";
        client_input.text = 0 + "";
        Nombre_proyecto.text = "Proyecto_1";
        resizeContainer(new Vector3(10.2f,2.27f,2.43f));
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
            if (process.HasExited)
            {
                runAlgorithm1();
            }

        }
       
        /*/
        //Animar barra de cargue
        if (loading_bar)
        {
            if (TimeLeft < 2)
            {
                TimeLeft += Time.deltaTime;
                LoadBar.fillAmount = TimeLeft / 2;
            }
            else
            {
                LoadBar.enabled = false;
            }
        } */

        volumen_total = containerSize.x * containerSize.y * containerSize.z;
        total_Volume.text = volumen_total + " m3";
        free_Volume.text = volumen_total - volumen_ocupado + " m3";
        nombre_container.text = "Contenedor "+containerTransform.localScale.x+" m x "+
            containerTransform.localScale.y + " m x "+
            containerTransform.localScale.z + " m  " ;
     
            
    }

    //--------------------------------------------------------------
    //Update data, play multidrop y print the results
    //--------------------------------------------------------------
    public async void runAlgorith()
    { if (!activeFuntion)
        {
            try
            {
                activeFuntion = true;
                //vaciar contenedor
                vaciar();
                if (pTypes.Count == 0)
                {
                    activeFuntion = false;
                    NoTypes.SetActive(true);
                    Debug.LogError("Por favor cargar paquete");
                    throw new NullReferenceException("Exception Message");
                }
                //-----------------
                //Reiniciar valores
                //-----------------
                peso = 0;
                float num = 0;
                intermedio.Clear();
                
                //reset number of clients
                numClient=0;

                //Array.Clear(intermedio, 0, intermedio.Length);
                foreach (PackageType pType in pTypes.Values)

                {
                    pType.packagePositions.Clear();
                    pType.endPositions.Clear();

                    if (numClient <= pType.clientId)
                    {
                        numClient = pType.clientId+1;
                        
                    }
                }

                //----------------------------
                //Crear archivo para multridop
                //-----------------------------
                //try { p6 = int.Parse(UIRunAlgorithm[0].text); p7 = int.Parse(UIRunAlgorithm[1].text); } catch (Exception) { }
                ArrayList mymessage = new ArrayList();

                //Number of packages and number of clients
                mymessage.Add(pTypes.Count + "\t" + numClient + "\n");
                //Container Size
                mymessage.Add(containerSize.x * 100 + "\t" + containerSize.z * 100 + "\t" + containerSize.y * 100 + "\n");
                foreach (PackageType ptype in pTypes.Values)
                {
                    /*
                    if (ptype.maxForce.x >= 4000000)
                    {
                        ptype.maxForce.x = 4000000;
                    }
                    if (ptype.maxForce.y >= 4000000)
                    {
                        ptype.maxForce.y = 4000000;
                    }
                    if (ptype.maxForce.z >= 4000000)
                    {
                        ptype.maxForce.z = 4000000;
                    }
                    */
                    //pId, Largo, Largo Vertical, Ancho, Ancho vertical, Alto, Alto vertical, Cantidad,Peso, Soporte largo, Soporte Ancho, Soporte Alto, Group Id,Destino
                    mymessage.Add(ptype.packageId + "\t" + ptype.packageSize.x + "\t" + ptype.verticalPos.x + "\t" + ptype.packageSize.z + "\t" + ptype.verticalPos.z + "\t" + ptype.packageSize.y + "\t" + ptype.verticalPos.y + "\t"
                            + ptype.quantity + "\t" + ptype.weight + "\t" + ptype.maxForce.x + "\t" + ptype.maxForce.y + "\t" + ptype.maxForce.z + "\t" + ptype.clientId + "\t" + 0 + "\t" + "\n");

                    
                }
                string fileWrite = string.Format("{0}/StreamingAssets/Multidrop/{1}",
                 UnityEngine.Application.dataPath, "file.txt");

                StreamWriter sw = new StreamWriter(fileWrite);
                foreach (string message in mymessage)
                {

                    sw.WriteLine(message);
                }
                sw.Close();
               
                ProcessStartInfo startInfo =  process.StartInfo;

                startInfo.FileName = string.Format("{0}/StreamingAssets/Multidrop/{1}",
                UnityEngine.Application.dataPath, "Multidrop.exe");
                startInfo.WorkingDirectory= string.Format("{0}/StreamingAssets/Multidrop",
                UnityEngine.Application.dataPath);
                //Daneses (Visibilidad y apilamiento)
                //Sesquia (Alcanzable)
                //Juanqueria (pared)
                //Nombre opcion(1-Clientes(Se pueden apilar) 2-Pesos 3-Pared invisibles)
                startInfo.Arguments = string.Format("{0}/StreamingAssets/Multidrop/{1}",
                UnityEngine.Application.dataPath, "file.txt 1   1   1   3   2  1000");
                //StartInfo.Arguments = "Extra Arguments to Pass to the Program";
                startInfo.CreateNoWindow = true;
                startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                //startInfo.UseShellExecute = true;
                // *** Redirect the output ***
                //startInfo.RedirectStandardError = true;
                //startInfo.RedirectStandardOutput = true;

                process.Start();

                //hasAnswer = true;
                Debug.Log("inicio");

                //Animación cargue
                LoadBar.enabled = true;
                TimeLeft = 0;
                loading_bar = true;
                await Task.Delay(2000);
                Debug.Log("fin");
                process.WaitForExit();


                string path1 = string.Format("{0}/StreamingAssets/Multidrop/{1}",
                UnityEngine.Application.dataPath, "temp.txt");
             

                validateDelete();
                //------------------------
                //Initialize the reading
                //-------------------------
                theSourceFile = new FileInfo(path1);
                reader = theSourceFile.OpenText();


                //First Line
                text = reader.ReadLine();
                string[] split = text.Split('\t');
                int totalPcs = int.Parse(split[0]);
                resizeContainer(new Vector3(float.Parse(split[2]), float.Parse(split[4]), float.Parse(split[3])) / 100);
                ;
                //Second Line
                text = reader.ReadLine();
                split = text.Split('\t');
                volumen_ocupado = float.Parse(split[0]) / 1000000;
                //Read the packages
                string[] lines = System.IO.File.ReadAllLines(path1);
                //creara contador de grupo actual
                numGroups = 0;
                
                //----
                for (int i = 2; i < lines.Length; i++)
                {
                    string[] split1 = lines[i].Split('\t');




                    //Read Package
                    Vector3 endPosition = new Vector3(float.Parse(split1[3]), float.Parse(split1[5]), float.Parse(split1[4])) / 100;
                    Vector3 packPosition = new Vector3(float.Parse(split1[0]), float.Parse(split1[2]), float.Parse(split1[1])) / 100;
                    Vector3 packSize = (endPosition - packPosition) * 100;
                    //Evitar error de decimales
                    packSize = new Vector3(Convert.ToSingle(Math.Round(packSize.x, 2)), Convert.ToSingle(Math.Round(packSize.y, 2)), Convert.ToSingle(Math.Round(packSize.z, 2)));

                    int packId = int.Parse(split1[6]);
                    int packGroup = int.Parse(split1[7]);
                    int packClient = int.Parse(split1[8]);

                    //Check If package type exist 
                    //newPackage(packSize, array_quantity[packId], 1, false, packId);


                    PackageType pType = pTypes[packId];
                    //pType.AddPosition(packPosition, endPosition);
                    //updatePackageValues(packId, packSize,pTypes[packId].verticalPos, pTypes[packId].maxForce, pType.quantity, pType.weight, pTypes[packId].Name1,packGroup);
                    //Debug.Log(packId + " tiene " + pType.quantity);

                    //Calcular peso
                    if (packPosition.x < containerSize.x)
                    {
                        peso += (pType.weight) / 10000;
                        
                    }
                    //Contar paquetes que no se cargaron
                    else
                    {
                        Vector3 adjust = new Vector3(1f, 0f, 0f);
                        //add position
                        packPosition = packPosition + adjust;
                        endPosition= endPosition + adjust;
                        bool keyExists = intermedio.ContainsKey(packId);
                        if (!keyExists)
                        {
                            intermedio.Add(packId, 0f);
                        }
                        intermedio[packId] += 1f;
                    }
                    //add position
                    pType.AddPosition(packPosition, endPosition);
                    //----------------------------
                    //Cargar los paquetes
                    //-----------------------------

                    if (packGroup != numGroups)
                    {

                        snapCam.group = packGroup - 1;
                        snapCam.callTakeSnapShot();
                        await Task.Delay(5);
                        numGroups = packGroup;
                    }
                    // Crear paquetes en base al prefab
                    GameObject go = GameObject.Instantiate(packagePrefab);
                    go.transform.SetParent(parent);
                    packages.Add(go);
                    go.GetComponent<Package>().setPackageValues(endPosition - packPosition
                        , packPosition, packId, packageColor[packId], pType.weight, packClient, packGroup, pType.Name1);
                    Vector3 size1 = endPosition - packPosition;

                    if (i == lines.Length - 1)
                    {

                        snapCam.group = packGroup;
                        snapCam.callTakeSnapShot();
                        await Task.Delay(5);
                        numGroups = packGroup;
                    }

                    //----

                }
                label_peso.text = peso + " kg";
                reader.Close();
                //Give the option to add a new loading
                showNewLoading();
                //ajustar canvas
                EstadoCargado();
                //Reactive funtion 
                activeFuntion = false;
            }
            
            catch(System.Exception)
            {
                Debug.Log("callo en error");
                activeFuntion = false;
                return;
            }
        }
    }
   
    public  void runAlgorithm()
    {
        if (!activeFuntion)
        {
            try
            {
                inProcess = true;
                activeFuntion = true;
                //Reiniciar intermeido
                intermedio.Clear();
                //vaciar contenedor
                vaciar();
                if (pTypes.Count == 0)
                {
                    activeFuntion = false;
                    NoTypes.SetActive(true);
                    Debug.LogError("Por favor cargar paquete");
                    throw new NullReferenceException("Exception Message");
                }
                //-----------------
                //Reiniciar valores
                //-----------------
                peso = 0;
                float num = 0;
                intermedio.Clear();

                //reset number of clients
                numClient = 0;

                //Array.Clear(intermedio, 0, intermedio.Length);
                foreach (PackageType pType in pTypes.Values)

                {
                    pType.packagePositions.Clear();
                    pType.endPositions.Clear();

                    if (numClient <= pType.clientId)
                    {
                        numClient = pType.clientId + 1;

                    }
                }

                //----------------------------
                //Crear archivo para multridop
                //-----------------------------
                //try { p6 = int.Parse(UIRunAlgorithm[0].text); p7 = int.Parse(UIRunAlgorithm[1].text); } catch (Exception) { }
                ArrayList mymessage = new ArrayList();

                //Number of packages and number of clients
                mymessage.Add(pTypes.Count + "\t" + numClient + "\n");
                //Container Size
                mymessage.Add(containerSize.x * 100 + "\t" + containerSize.z * 100 + "\t" + containerSize.y * 100 + "\n");
                foreach (PackageType ptype in pTypes.Values)
                {
                    
                    if (ptype.maxForce.x >= 40000)
                    {
                        ptype.maxForce.x = 4000000;
                    }
                    if (ptype.maxForce.y >= 40000)
                    {
                        ptype.maxForce.y = 4000000;
                    }
                    if (ptype.maxForce.z >= 40000)
                    {
                        ptype.maxForce.z = 4000000;
                    }

                    //pId, Largo, Largo Vertical, Ancho, Ancho vertical, Alto, Alto vertical, Cantidad,Peso, Soporte largo, Soporte Ancho, Soporte Alto, Group Id,Destino
                    mymessage.Add(ptype.packageId + "\t" + ptype.packageSize.x + "\t" + ptype.verticalPos.x + "\t" + ptype.packageSize.z + "\t" + ptype.verticalPos.z + "\t" + ptype.packageSize.y + "\t" + ptype.verticalPos.y + "\t"
                            + ptype.quantity + "\t" + ptype.weight + "\t" + ptype.maxForce.x + "\t" + ptype.maxForce.y + "\t" + ptype.maxForce.z + "\t" + ptype.clientId + "\t" + 0 + "\t" + "\n");


                }
                string fileWrite = string.Format("{0}/StreamingAssets/Multidrop/{1}",
                 UnityEngine.Application.dataPath, "file.txt");

                StreamWriter sw = new StreamWriter(fileWrite);
                foreach (string message in mymessage)
                {

                    sw.WriteLine(message);
                }
                sw.Close();

                ProcessStartInfo startInfo = process.StartInfo;

                startInfo.FileName = string.Format("{0}/StreamingAssets/Multidrop/{1}",
                UnityEngine.Application.dataPath, "Multidrop.exe");
                startInfo.WorkingDirectory = string.Format("{0}/StreamingAssets/Multidrop",
                UnityEngine.Application.dataPath);
                //Daneses (Visibilidad y apilamiento)
                //Sesquia (Alcanzable)
                //Juanqueria (pared)
                //Nombre opcion(1-Clientes(Se pueden apilar) 2-Pesos 3-Pared invisibles)
                startInfo.Arguments = string.Format("{0}/StreamingAssets/Multidrop/{1}",
                UnityEngine.Application.dataPath, "file.txt 1   1   1   3   2  20");
                //StartInfo.Arguments = "Extra Arguments to Pass to the Program";
                startInfo.CreateNoWindow = true;
                startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                //startInfo.UseShellExecute = true;
                // *** Redirect the output ***
                //startInfo.RedirectStandardError = true;
                //startInfo.RedirectStandardOutput = true;

                process.Start();

                //hasAnswer = true;
                Debug.Log("inicio");

                //Animación cargue
               

                Debug.Log("fin");
                
                //Reactive funtion 
                activeFuntion = false;
                //active bloking
                bloking.SetActive(true);
                TimeLeft = 0;

            }

            catch (System.Exception)
            {
                Debug.Log("callo en error");
                activeFuntion = false;
                return;
            }
        }
    }
    public async void runAlgorithm1()
    {
        inProcess = false;
        bloking.SetActive(false);
        string path1 = string.Format("{0}/StreamingAssets/Multidrop/{1}",
        UnityEngine.Application.dataPath, "temp.txt");


        validateDelete();
        //------------------------
        //Initialize the reading
        //-------------------------
        theSourceFile = new FileInfo(path1);
        reader = theSourceFile.OpenText();


        //First Line
        text = reader.ReadLine();
        string[] split = text.Split('\t');
        int totalPcs = int.Parse(split[0]);
        resizeContainer(new Vector3(float.Parse(split[2]), float.Parse(split[4]), float.Parse(split[3])) / 100);
        ;
        //Second Line
        text = reader.ReadLine();
        split = text.Split('\t');
        volumen_ocupado = float.Parse(split[0]) / 1000000;
        //Read the packages
        string[] lines = System.IO.File.ReadAllLines(path1);
        //creara contador de grupo actual
        numGroups = 0;
        //Validate maxWieght
        bool maxWeightValidator=true;
        //----
        for (int i = 2; i < lines.Length; i++)
        {
            //Read Line
            string[] split1 = lines[i].Split('\t');

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

            //Read pType
            PackageType pType = pTypes[packId];
            //Calcular peso
           
            if (packPosition.x < containerSize.x && maxWeightValidator)
            {
                peso += (pType.weight) / 10000;
                if(maxWeight_bol && maxWeight<peso)
                {
                    peso -= (pType.weight) / 10000;
                    maxWeightValidator = false;
                    Vector3 adjust = new Vector3(10f, 0f, 0f);
                    //add position
                    packPosition = packPosition + adjust;
                    endPosition = endPosition + adjust;
                    bool keyExists = intermedio.ContainsKey(packId);
                    if (!keyExists)
                    {
                        intermedio.Add(packId, 0f);
                    }
                    intermedio[packId] += 1f;
                }
            }
            //Contar paquetes que no se cargaron
            else
            {
                Vector3 adjust = new Vector3(10f, 0f, 0f);
                //add position
                packPosition = packPosition + adjust;
                endPosition = endPosition + adjust;
                bool keyExists = intermedio.ContainsKey(packId);
                if (!keyExists)
                {
                    intermedio.Add(packId, 0f);
                }
                intermedio[packId] += 1f;
            }
            //add position
            pType.AddPosition(packPosition, endPosition);
            //----------------------------
            //Cargar los paquetes
            //-----------------------------

            if (packGroup != numGroups)
            {

                snapCam.group = packGroup - 1;
                snapCam.callTakeSnapShot();
                await Task.Delay(5);
                numGroups = packGroup;
            }
            // Crear paquetes en base al prefab
            GameObject go = GameObject.Instantiate(packagePrefab);
            go.transform.SetParent(parent);
            packages.Add(go);
            go.GetComponent<Package>().setPackageValues(endPosition - packPosition
                , packPosition, packId, packageColor[packId], pType.weight, packClient, packGroup, pType.Name1);
            Vector3 size1 = endPosition - packPosition;

            if (i == lines.Length - 1)
            {

                snapCam.group = packGroup;
                snapCam.callTakeSnapShot();
                await Task.Delay(5);
                numGroups = packGroup;
            }

            //----

        }
        label_peso.text = peso + " kg";
        reader.Close();
        //Give the option to add a new loading
        showNewLoading();
        //ajustar canvas
        EstadoCargado();
    }
    //--------------------------------------------------------------
    //Update data wiht New Package button
    //--------------------------------------------------------------
    public void NewPackageButtom()
    {
        int idP = 0;
        while (pTypes.ContainsKey(idP))
        {
            idP++;
        }
        newPackage(new Vector3(), new Vector3(), new Vector3(),1,0,true,idP,0,"");
    }

    
    public void newPackage(Vector3 packageSizeP, Vector3 verticalPosP, Vector3 maxForceP, int quantityP, float weightP, bool isEmpty, int Id,int clientID, string name)
    {
        var pos = modelo.transform.localPosition;
        Material material;
        material = Instantiate(Cube.GetComponent<Renderer>().material);
        Cube.GetComponent<Renderer>().material = material;
        Name.text = "Item" + Id;

        if (packageSizeP.x > 0)
        {

            lenght_Input.text = (packageSizeP.z) + "";
            height_Input.text = (packageSizeP.y) + "";
            width_Input.text = (packageSizeP.x) + "";
            weight_Input.text = (weightP / 10000) + "";
            pcs_Input.text = (quantityP) + "";
           
            Name.text = name;
            client_input.text = clientID+"";
            if (maxForceP.x < 1000 || maxForceP.y < 1000 || maxForceP.z < 1000) {  panel_Fuerza.isOn = true; panel_Fuerza_object.SetActive(true); } else { panel_Fuerza.isOn = false; }
            if (verticalPosP.x == 0 && verticalPosP.z == 0) { panel_Rotacion.isOn = false; } else { panel_Rotacion.isOn = true; }
            if (verticalPosP.x > 0) { rotacion_x.isOn = true; toggles.R_X(true); } else { rotacion_x.isOn = false; toggles.R_X(false); }
            if (verticalPosP.z > 0) { rotacion_z.isOn = true; toggles.R_Z(true); } else { rotacion_z.isOn = false; toggles.R_Z(false); }
            Fuerza_x_input.text = maxForceP.x + "";
            Fuerza_y_input.text = maxForceP.y + "";
            Fuerza_z_input.text = maxForceP.z + "";

        }
        else
        {
            client_input.text = 0f + "";
            lenght_Input.text = 50 + "";
            height_Input.text = 50+ "";
            width_Input.text = 50 + "";
            weight_Input.text = 1 + "";
            pcs_Input.text = 1 + "";

            panel_Rotacion.isOn = true;
            panel_Fuerza.isOn = false;
            maxForceP = new Vector3(400000,400000,400000);
            Fuerza_x_input.text =400000f + " ";
            Fuerza_y_input.text = 400000f + " ";
            Fuerza_z_input.text = 400000f + " ";
        }
            
        
        
        //Ampliar lista
        if (Id > 8)
        {
            Lista.sizeDelta=new Vector2 (Lista.sizeDelta.x, Lista.sizeDelta.y+25f);
        }
            //Asiganer material al cubo
            material.color = packageColor[Id];

        //cambiar imagen y color 
        ColorImage.color = packageColor[Id];
        ColorMiniatura.color = packageColor[Id];
        numero.text = Id + "";
        controlador.text = Id + "";
        numeroMiniatura.text = Id + "";



        //Crear el nuevo objeto
        GameObject nuevo = Instantiate(modelo);
        packagesUI.Add(nuevo);
        nuevo.SetActive(true);
        nuevo.transform.SetParent(Lista);
        ActualizarPackage uiPackageP = nuevo.GetComponent<ActualizarPackage>();
        //Crear nuevo PackageType
        PackageType p = new PackageType(packageColor[Id], Id, packageSizeP, verticalPosP, maxForceP, quantityP, weightP, uiPackageP, clientID, Name.text);
        
        pTypes.Add(Id, p);
       // updatePackageValues(Id, packageSizeP, quantityP, weightP, Name.text);

        //poner en posición
        nuevo.transform.rotation = new Quaternion(0f, 0f, 0f, 0f);
        nuevo.transform.localScale = new Vector3(1f, 1f, 1f);
        nuevo.transform.localPosition = new Vector3(pos.x, pos.y - height, pos.z);
        //añadir uno nuevo

        height += nuevo.GetComponent<RectTransform>().sizeDelta.y;
    

    }
    /**
    * Method that creates a new package type
    * @param packageColorP The color of the package type
    * @param packageIdP    The id of the package type
    * @param packageSizeP The size of the package type width, height, lenght in meters
    * @param quantityP The number of packages of these package type
    * @param weightP The weight of the package type in Kg
    * @param uiPackageP the UI controller of the package
    *@param determines if isEmpty
    */
    //--------------------------------------------------------------
    //Create a new package in base to prefab
    //--------------------------------------------------------------
    public void createNewPackageType(Vector3 packageSizeP, Vector3 verticalPosP, Vector3 maxForceP, int quantityP, float weightP, bool isEmpty, int idP, int clientid)
    {
        Color packageColorP = packageColor[0];
        if (lastId < packageColor.Length)
            packageColorP = packageColor[lastId];


        //        char packageIdP = ALPHABET[lastId+1];
        Debug.Log("Creating package " + idP);
        lastId++;
        GameObject go = GameObject.Instantiate(modelo);
        go.transform.SetParent(Lista);
        go.SetActive(true);
        listUi.Add(go);
        ActualizarPackage uiPackageP = go.GetComponent<ActualizarPackage>();
        //Create a new package type then add it to the list
        PackageType p = new PackageType(packageColorP, idP, packageSizeP,verticalPosP, maxForceP, quantityP, weightP, uiPackageP, clientid, "item"+idP);
        uiPackageP.updateValues(packageColorP, idP, packageSizeP, quantityP, weightP, isEmpty);
        pTypes.Add(idP, p);

        
    }
    /**
    * Method that updates a package type
    * @param packageSizeP The size of the package type width, height, lenght in meters
    * @param quantityP The number of packages of these package type
    * @param weightP The weight of the package type in Kg
    * @param uiPackageP the UI controller of the package
    */

    //--------------------------------------------------------------
    //Update the package Values
    //--------------------------------------------------------------
    public void updatePackageValues(int packageIdP, Vector3 packageSizeP, Vector3 verticalPosP, Vector3 maxForceP, int quantityP, float weightP,String NameValue, float client)
    {

        try
        {
           // Debug.Log("Updating package " + packageIdP + " # " + quantityP);
            ActualizarPackage uiPackageP = pTypes[packageIdP].uiPackage;
            pTypes[packageIdP].updatePType(pTypes[packageIdP].packageColor, packageIdP, packageSizeP,  verticalPosP,  maxForceP, quantityP, weightP, uiPackageP,NameValue,client);

            //infoPanel.updateValues(pTypes[packageIdP].packageColor, packageIdP, packageSizeP, quantityP, weightP, false);
            return;
        }
        catch (Exception)
        {
            Debug.Log("Error on update package ");
        }

    }

    //--------------------------------------------------------------
    //Resize the container
    //--------------------------------------------------------------
    public void resizeContainer(Vector3 newSize)
    {
        containerSize = newSize;
        if(irregular)
        {
            containerTransform.localScale = new Vector3(containerSize.z,containerSize.y, containerSize.x);
        }
        else
        {
            containerTransform.localScale = containerSize;
        }
        containerTransform.position = Vector3.zero;
        Camera.position = new Vector3(10f+newSize.x,Camera.position.y,Camera.position.z);
        
    }

    //Unhabilited
    public async void loadPackages()
    {
      
        foreach (GameObject pack in packages)
        {
            GameObject.Destroy(pack);
        }
        packages.Clear();

        //Load the packages
        foreach (PackageType pType in pTypes.Values)

        {
            Debug.Log(pType.packageId+" tiene "+ pType.quantity);


            for (int i = 0; i < pType.quantity; i++)
            {
                Debug.Log(pType.packageId+"HASTA " + i);
                GameObject go = GameObject.Instantiate(packagePrefab);
                packages.Add(go);
                //go.GetComponent<Package>().setPackageValues(pType.endPositions[i] - pType.packagePositions[i], pType.packagePositions[i], pType.packageId, pType.packageColor, pType.weight,1,pType.Name1);
                Vector3 size1 = pType.endPositions[i] - pType.packagePositions[i];
               
                //await Task.Delay(100);

            }
        }

        Debug.Log("Loaded " + packages.Count + " packages");
        //Cerrar menu containers
        LeanTween.scale(menuContainers, new Vector3(0, 0, 0), 0.5f);
        menuContainers.GetComponent<MenuObjetos>().EstaCerrado = true;
        controlador.text = -1+"";
    }
    public  void resetPositions()
    {
        if (!activeFuntion)
        {
            activeFuntion = true;
            ArePackage();
            foreach (GameObject pack in packages)
            {
                GameObject.Destroy(pack);
            }
            packages.Clear();
            //Unactive toggles
            VistaGrupos.isOn = false;
            VistaClient.isOn = false;
            //------------------------
            //Initialize the reading
            //-------------------------
            string path1 = string.Format("{0}/StreamingAssets/Multidrop/{1}",
                UnityEngine.Application.dataPath, "temp.txt");
            theSourceFile = new FileInfo(path1);
            //Read the packages
            string[] lines = System.IO.File.ReadAllLines(path1);

            //creara contador de grupo actual
            numGroups = 0;
            //----
            for (int i = 2; i < lines.Length; i++)
            {
                string[] split1 = lines[i].Split('\t');


                //Read Package
                Vector3 endPosition = new Vector3(float.Parse(split1[3]), float.Parse(split1[5]), float.Parse(split1[4])) / 100;
                Vector3 packPosition = new Vector3(float.Parse(split1[0]), float.Parse(split1[2]), float.Parse(split1[1])) / 100;
                Vector3 eulerAngles = new Vector3(0, 0, 0);
                Vector3 packSize = (endPosition - packPosition) * 100;
                if(split1.Length>9)
                {
                     eulerAngles = new Vector3(float.Parse(split1[9]), float.Parse(split1[10]), float.Parse(split1[11]));
                }

                //Evitar error de decimales
                packSize = new Vector3(Convert.ToSingle(Math.Round(packSize.x, 2)), Convert.ToSingle(Math.Round(packSize.y, 2)), Convert.ToSingle(Math.Round(packSize.z, 2)));

                int packId = int.Parse(split1[6]);
                int packGroup = int.Parse(split1[7]);
                int packClient = int.Parse(split1[8]);

                //Check If package type exist 
                //newPackage(packSize, array_quantity[packId], 1, false, packId);
                PackageType pType = pTypes[packId];

                //Calcular peso
                if (split1[8] != "0") { peso += (pType.weight) / 10000; }
                //Save the nymber of groups
                if (packGroup != numGroups) { numGroups = packGroup; }

                //----------------------------
                //Cargar los paquetes
                //-----------------------------


                // Crear paquetes en base al prefab
                GameObject go = GameObject.Instantiate(packagePrefab);
                go.transform.SetParent(parent);
                go.transform.eulerAngles = eulerAngles;
                packages.Add(go);
                go.GetComponent<Package>().groupId = packGroup;
                go.GetComponent<Package>().itemId = packId;

                go.GetComponent<Package>().setPackageValues(endPosition - packPosition
                    , packPosition, packId, packageColor[packId], pType.weight, packClient,packGroup, pType.Name1);
                Vector3 size1 = endPosition - packPosition;
                //----
                // Añadir Upper
                Renderer rende2 = go.GetComponent<Renderer>();
                Bounds brende2 = rende2.bounds;
                Destroy(go.transform.Find("Upper").gameObject);
                GameObject newUpper = GameObject.CreatePrimitive(PrimitiveType.Cube);
                newUpper.name = "Upper";
                newUpper.GetComponent<MeshRenderer>().enabled = false;
                var script = newUpper.AddComponent<UpperDetector>();
                script.package = go.GetComponent<OnMouseClick>();
                newUpper.transform.parent = go.transform;
                newUpper.transform.localPosition = new Vector3(0f, 0f, 0f);
                newUpper.transform.localScale = new Vector3(1f, 1f, 1f);
                newUpper.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
                newUpper.GetComponent<BoxCollider>().center = new Vector3(0.5f, 0.5f, 0.5f);
                newUpper.GetComponent<BoxCollider>().size = new Vector3(0.9f, 0.9f, 0.9f);
                newUpper.GetComponent<BoxCollider>().isTrigger = true;
                newUpper.GetComponent<BoxCollider>().enabled = false;
                newUpper.transform.parent = null;
                newUpper.transform.position = new Vector3(newUpper.transform.position.x, newUpper.transform.position.y + 0.05f * brende2.size.y, newUpper.transform.position.z);
                newUpper.transform.parent = go.transform;
                newUpper.GetComponent<BoxCollider>().enabled = true;


            }
            //reader.Close();
            activeFuntion = false;
        }
    }
    //------------------------------------------------------------------
    //Update the canvas to improve the container visibility 
    //------------------------------------------------------------------
    public void EstadoCargado()
    {
        //Cerrar menu containers
        if (!menuContainers.GetComponent<MenuObjetos>().EstaCerrado)
        { 
            menuContainers.GetComponent<MenuObjetos>().OnCloseContainer();
        }
        //Unactive toggle of GroupView
        VistaGrupos.isOn = false;
        VistaClient.isOn = false;
        //minizar lista de paquetes
        controlador.text = -1 + "";
    }
    //-----------------------------------------------------------------
    //Print the .PDF
    //-----------------------------------------------------------------
    public void imprimir()
    {
            ArePackage();
            Debug.Log(pTypes.Count + "");

            foreach (PackageType pType1 in pTypes.Values)
            {
                Debug.Log("Quantity" + pType1.quantity + "");
                for (int i = 0; i < pType1.quantity - 1; i++)
                {
                    Debug.Log(pType1.packagePositions[i].x);
                }
            }
        
    }
    //--------------------------------------------------------------
    //Package load from txt file
    //--------------------------------------------------------------
    public void ReadTxt(Text path)
    {
        reestablecer();
        //Path del arrcivo a leer
        string path1 = string.Format("{0}/StreamingAssets/txt/{1}",
        UnityEngine.Application.dataPath, path.text);
        //-------
        //Leer lineas del archivo
        string[] lines = System.IO.File.ReadAllLines(path1);

        //Calcular el limite del archivo que divide temp.txt de file.txt
        int limite = 0;
        for (int i = 2; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i]))
            {
                limite = i;
                Debug.Log("el limite es " + limite);
                break;
            }
        }
        for (int i = limite; i < lines.Length; i++)
        {
            string[] split1 = lines[i].Split('\t');

            //if (split1[8] != "0")
            //{
                // Use a tab to indent each line of the file.
                int packId = int.Parse(split1[6]);
        }
        for (int i = 2; i <limite; i++)
        {
            Debug.Log("Leyendo " + i);
            string[] split1 = lines[i].Split('\t');

            //if (split1[8] != "0")
            //{
                // Use a tab to indent each line of the file.
                int packId = int.Parse(split1[6]);
                array_quantity[packId] = array_quantity[packId] + 1;
            //}
               
          
           
        }
        for (int i = 0; i < 3; i++)
        {
            Debug.Log("\t" + i + "\t" + array_quantity[i]);
        }
            //-----
            //Initialize the reading

        theSourceFile = new FileInfo(path1);
        reader = theSourceFile.OpenText();

        //First Line
        text = reader.ReadLine();
        string[] split = text.Split('\t');
        int totalPcs = int.Parse(split[0]);
        resizeContainer(new Vector3(float.Parse(split[2]), float.Parse(split[4]), float.Parse(split[3])) / 100);
        Debug.Log("Total pcs = " + split[0] + " # Destinations = " + split[1]);
        Debug.Log("Container size = " + split[2] + "mm " + split[4] + "mm " + split[3] + "mm");
        //Second Line
        text = reader.ReadLine();
        split = text.Split('\t');
        Debug.Log("Total Volume = " + split[0] + " Used Volume = " + split[1]);
        float total_vaolume_ = float.Parse(split[0])/1000000;
        float free_vaolume_ = (float.Parse(split[0]) - float.Parse(split[1]))/1000000;
        total_Volume.text = total_vaolume_ + " m3";
        free_Volume.text = free_vaolume_ + " m3";
        //Read the packages
        string[] lines1 = System.IO.File.ReadAllLines(path1);


        for (int i = 2; i < limite; i++)
        {
           
            text = reader.ReadLine();
            split = text.Split('\t');
            /*Is it over?
             if (split[8]== "0")
                return;*/
            //Read Package
            //Debug.Log(text);
            Vector3 endPosition = new Vector3(float.Parse(split[3]), float.Parse(split[5]), float.Parse(split[4])) / 100;
            Vector3 packPosition = new Vector3(float.Parse(split[0]), float.Parse(split[2]), float.Parse(split[1])) / 100;
            Vector3 packSize = endPosition - packPosition;
            Vector3 maxForceP = new Vector3();
            Vector3 verticalPostP = new Vector3(1,1,1);
            int packId = int.Parse(split[6]);
            int packClient = int.Parse(split[7]);
            int packGroup = int.Parse(split[8]);

            //Check If package type exist 
            if (!pTypes.ContainsKey(packId))
                newPackage(packSize, verticalPostP,maxForceP, array_quantity[packId], 1, false, packId,packClient,"item");
            PackageType pType = pTypes[packId];
            pType.AddPosition(packPosition, endPosition);
            updatePackageValues(packId, packSize,verticalPostP,maxForceP, pType.quantity , pType.weight, pTypes[packId].Name1,packClient);
            Debug.Log(packId + " tiene " + pType.quantity);

            //Calcular peso
            if (split[8] != "0")
            {
                peso += (pType.weight) / 10000;
            }
            //Contar paquetes que no se cargaron
            else
            {
                bool keyExists = intermedio.ContainsKey(packId);
                if (!keyExists)
                {
                    intermedio.Add(packId, 0f);
                }
                intermedio[packId] += 1f;
            }

        }

        loadPackages();
        string path_string = path.text;
        Titulo.text = path_string.Substring(0, path_string.IndexOf("."));

    }
    //--------------------------------------------------------------
    //Package load from txt file
    //--------------------------------------------------------------

    public async void ReadTxt1(Text path)

    {
        reestablecer();
        //Change the tap selected
        tapGroup.OnTabSelected(CargaTap);

        //
        intermedio.Clear();
        //Path del arrcivo a leer
        string path1 = string.Format("{0}/StreamingAssets/txt/{1}",
        UnityEngine.Application.dataPath, path.text);
        //Archivo temp
        string temp = string.Format("{0}/StreamingAssets/Multidrop/{1}",
       UnityEngine.Application.dataPath, "temp.txt");
        //-------
        //Leer lineas del archivo
        string[] lines = System.IO.File.ReadAllLines(path1);

        //Calcular el limite del archivo que divide temp.txt de file.txt
        int limite = 0;
        for (int i = 2; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i]))
            {
                limite = i;
                break;
            }
        }

        //Actualizar temp
        File.Delete(temp);
        using (StreamWriter writer = new StreamWriter(temp, true))
        {
            
            for (int i = 0; i < limite-1; i++)
            {
                writer.WriteLine(lines[i]);
            }
            writer.Close();
        }
        //traer datos de file.txt
        for (int i = limite-1; i < lines.Length; i++)
        {
            if (i == limite - 1)
            {
                string[] split1 = lines[i].Split('\t');
                numClient = int.Parse(split1[1]);
            }
            else if (i == limite + 1)
            {
                string[] split1 = lines[i].Split('\t');
                resizeContainer(new Vector3(float.Parse(split1[0]), float.Parse(split1[2]), float.Parse(split1[1])) / 100);
       
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
                maxForceP.x = Mathf.Floor(float.Parse(split1[9])*(packSize.x * packSize.y) / 10000f);
                maxForceP.y = Mathf.Floor(float.Parse(split1[10]) * (packSize.y * packSize.z) / 10000f);
                maxForceP.z = Mathf.Floor(float.Parse(split1[11]) * (packSize.x * packSize.z) / 10000f);
                int quantity= int.Parse(split1[7]);
                int weight = int.Parse(split1[8]);
                int client = int.Parse(split1[12]);
                string name = split1[14];
                newPackage(packSize, verticalPostP, maxForceP, quantity, weight, false, packId,client,name);
                updatePackageValues(packId, packSize, verticalPostP, new Vector3(float.Parse(split1[9]), float.Parse(split1[10]), float.Parse(split1[11])), quantity, weight,name, client);
            }
        }
        //vaciar contenedor
        vaciar();
        //creara contador de grupo actual
        numGroups = 0;
        //----

        //read temp
        //leer volumen
        string[] split = lines[1].Split('\t');
        volumen_ocupado = float.Parse(split[0]) / 1000000;
        print(volumen_ocupado);
        for (int i = 0; i < limite - 3; i++)
        {
            string[] split1 = lines[i + 2].Split('\t');
            Vector3 endPosition = new Vector3(float.Parse(split1[3]), float.Parse(split1[5]), float.Parse(split1[4])) / 100;
            Vector3 packPosition = new Vector3(float.Parse(split1[0]), float.Parse(split1[2]), float.Parse(split1[1])) / 100;
            Vector3 eulerAngles = new Vector3(float.Parse(split1[9]), float.Parse(split1[10]), float.Parse(split1[11]));
            int packId = int.Parse(split1[6]);
            int packGroup = int.Parse(split1[7]);
            int packClient = int.Parse(split1[8]);
            PackageType pType = pTypes[packId];
            //pType.AddPosition(packPosition, endPosition);
            //Calcular peso
            if (packPosition.x < containerSize.x)
            {
                peso += (pType.weight) / 10000;

            }
            //Contar paquetes que no se cargaron
            else
            {
                Vector3 adjust = new Vector3(1f, 0f, 0f);
                //add position
                packPosition = packPosition + adjust;
                endPosition = endPosition + adjust;
                bool keyExists = intermedio.ContainsKey(packId);
                if (!keyExists)
                {
                    intermedio.Add(packId, 0f);
                }
                intermedio[packId] += 1f;
            }
            //add position
            pType.AddPosition(packPosition, endPosition);
            //----
            //loading 
            if (packGroup != numGroups)
            {

                snapCam.group = packGroup - 1;
                snapCam.callTakeSnapShot();
                await Task.Delay(400);
                numGroups = packGroup;
            }
            GameObject go = GameObject.Instantiate(packagePrefab);
            go.transform.SetParent(parent);
            packages.Add(go);
            go.transform.eulerAngles = eulerAngles;
            go.GetComponent<Package>().setPackageValues(endPosition - packPosition
                , packPosition, packId, packageColor[packId], pType.weight, packClient, packGroup, pType.Name1);
            Vector3 size1 = endPosition - packPosition;
            if (i == limite - 4)
            {
                snapCam.group = packGroup;
                snapCam.callTakeSnapShot();
                await Task.Delay(400);
                numGroups = packGroup;
            }

            // Añadir Upper
            Renderer rende2 = go.GetComponent<Renderer>();
            Bounds brende2 = rende2.bounds;
            Destroy(go.transform.Find("Upper").gameObject);
            GameObject newUpper = GameObject.CreatePrimitive(PrimitiveType.Cube);
            newUpper.name = "Upper";
            newUpper.GetComponent<MeshRenderer>().enabled = false;
            var script = newUpper.AddComponent<UpperDetector>();
            script.package = go.GetComponent<OnMouseClick>();
            newUpper.transform.parent = go.transform;
            newUpper.transform.localPosition = new Vector3(0f, 0f, 0f);
            newUpper.transform.localScale = new Vector3(1f, 1f, 1f);
            newUpper.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
            newUpper.GetComponent<BoxCollider>().center = new Vector3(0.5f, 0.5f, 0.5f);
            newUpper.GetComponent<BoxCollider>().size = new Vector3(0.9f, 0.9f, 0.9f);
            newUpper.GetComponent<BoxCollider>().isTrigger = true;
            newUpper.GetComponent<BoxCollider>().enabled = false;
            newUpper.transform.parent = null;
            newUpper.transform.position = new Vector3(newUpper.transform.position.x, newUpper.transform.position.y + 0.05f *brende2.size.y, newUpper.transform.position.z);
            newUpper.transform.parent = go.transform;
            newUpper.GetComponent<BoxCollider>().enabled = true;

            //----
        }
        label_peso.text = peso + " kg";
        //Give the option to add a new loading
        showNewLoading();
        //cahnge the state of the canvas
        EstadoCargado();
        string path_string = path.text;
        Titulo.text = path_string.Substring(0, path_string.IndexOf("."));

    }
    //----------------------------------
    //Reset the values of the scene
    //----------------------------------
    public void reestablecer()
    {
        int idP = 0;
        //Conocer el numero de paqutes antes de borrar
        while (pTypes.ContainsKey(idP))
        {
            idP++;
        }
        
        pTypes.Clear();
        //Elimiar UIs del panel de objetos
        foreach (Transform child in Lista)
        {
            GameObject.Destroy(child.gameObject);
        }
        //reinicar cantidades
        for (int j = 0; j < array_quantity.Length; j++)
        {

            array_quantity[j] = 0;
        }
        //Resize the list of objects
        Lista.sizeDelta = new Vector2(Lista.sizeDelta.x, 370.4893f);
       
    }
    //---------------------------------------------------- 
    //Save the record of the current Scenario
    //-----------------------------------------------------
    public  void Save()
    {
        if (!activeFuntion)
        {
            activeFuntion = true;

            ArePackage();

            //arachivos de origen
            string[] file1 = File.ReadAllLines(string.Format("{0}/StreamingAssets/Multidrop/{1}",
                UnityEngine.Application.dataPath, "temp.txt"));
            string[] file2 = File.ReadAllLines(string.Format("{0}/StreamingAssets/Multidrop/{1}",
             UnityEngine.Application.dataPath, "file.txt"));
            string path = string.Format("{0}/StreamingAssets/Multidrop/{1}",
             UnityEngine.Application.dataPath, "file_aux.txt");
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
                    output = file2[i] + pTypes[id].Name1 + "\n" + "\n";
                    writer.Write(output);
                }
                writer.Close();
            }

            string[] file3 = File.ReadAllLines(string.Format("{0}/StreamingAssets/Multidrop/{1}",
             UnityEngine.Application.dataPath, "file_aux.txt"));
            //Archivo de destino

            string fileName = string.Format("{0}.txt", Titulo.text);

            string destFile = string.Format("{0}/StreamingAssets/txt/{1}",
                   UnityEngine.Application.dataPath, fileName);
            if (File.Exists(destFile))
            {
                File.Delete(destFile);
            }
            /*
            using (TextWriter tw = new StreamWriter(destFile, true))
            {
                Debug.Log("file tiene " + file1.Length + "y temp " + file3.Length);
                for (int i = 0; i < file1.Length; i++)
                {
                    tw.WriteLine(file1[i]);
                }
                for (int i = 0; i < file3.Length; i++)
                {
                    tw.WriteLine(file3[i]);
                }
            }
            //prueba
            string destFile1 = string.Format("{0}/StreamingAssets/txt/{1}.txt",
                   UnityEngine.Application.dataPath, "PruebaTemp");*/
            using (TextWriter tw = new StreamWriter(destFile, true))
            {
                tw.WriteLine(file1[0]);
                tw.WriteLine(file1[1]);
                string toWrite="";
                foreach (Transform child in parent)
                {
                    Vector3 endPosition = (child.transform.position + child.transform.localScale) * 100;
                    toWrite = 
                        Mathf.Round(child.transform.position.x*100)+ "\t"
                        + Mathf.Round(child.transform.position.z*100) + "\t" 
                        + Mathf.Round(child.transform.position.y*100) + "\t"
                        + Mathf.Round(endPosition.x) + "\t" +
                        Mathf.Round(endPosition.z) + "\t" +
                        Mathf.Round(endPosition.y) + "\t" +
                        child.GetComponent<Package>().itemId + "\t" + child.GetComponent<Package>().groupId + "\t" 
                        + child.GetComponent<Package>().client + "\t"+
                        Mathf.Round(child.transform.eulerAngles.x) + "\t" +
                        Mathf.Round(child.transform.eulerAngles.y) + "\t" +
                        Mathf.Round(child.transform.eulerAngles.z) ;
                    tw.WriteLine(toWrite);
                }
                for (int i = 0; i < file3.Length; i++)
                {
                    tw.WriteLine(file3[i]);
                }
            }

            Nombre_Registro.text = Titulo.text;
            Fecha_Registro.text = "" + DateTime.Now;
            tamaño_Contenedor.text = "(" + containerTransform.localScale.x + "m x" + containerTransform.localScale.y + "m x" + containerTransform.localScale.z + "m )";
            txtManager.precargar();
            saveMsg.SetActive(true);
            activeFuntion = false;
        }
    }

    //--------------------------------------------------------
    //*Deletes a package type with and ID
    //*@id the id of the package
    //--------------------------------------------------------
    public void deletePackageType(int id)
    {
        int idP = pTypes.Count;
       
        PackageType item;
        pTypes.TryGetValue(id, out item);
        if(idP>8)
        {

            Lista.sizeDelta = new Vector2(Lista.sizeDelta.x, Lista.sizeDelta.y - 25);
        }
        if (pTypes.ContainsKey(id))
        {
            pTypes.Remove(id);
            return;
        }
    }
    //---------------------------------------------------------
    //Empty Container
    //---------------------------------------------------------
    public void vaciar()
    {
        foreach (GameObject pack in packages)
        {
            GameObject.Destroy(pack);
        }
        packages.Clear();
        volumen_ocupado =0;
        peso = 0;
        label_peso.text = peso + " kg";
    }
    //----------------------------------------------------------
    //Game out
    //----------------------------------------------------------
    public void salir()
    {
        UnityEngine.Application.Quit();
        Debug.Log("Ha salido");
    }
     //----------------------------------------------------------
    //Read CSV
    //----------------------------------------------------------
    public void ReadCsv()
    {
        menuCsv.SetActive(false);
        vaciar();
        reestablecer();
        OpenFileDlg pth = new OpenFileDlg();
        pth.structSize = System.Runtime.InteropServices.Marshal.SizeOf(pth);
        //pth.filter = "CSV files(*.csv)| *.csv | All files(*.*) | *.* "; 
        pth.file = new string(new char[256]);
        pth.maxFile = pth.file.Length;
        pth.fileTitle = new string(new char[64]);
        pth.maxFileTitle = pth.fileTitle.Length;
        pth.initialDir = "c:\\";  // default path  
        pth.title = "Open project";
        pth.defExt = "csv";
        pth.flags = 0x00080000 | 0x00001000 | 0x00000800 | 0x00000200 | 0x00000008;
        // 0x00080000 Whether to use the new file selection window
        // Is it possible to select multiple files at 0x00000200 ?
        string type = pth.file.Substring(pth.file.Length - 3);
        Debug.Log(type + type.Length);
        if (OpenFileDialog.GetOpenFileName(pth))
        {
            string filepath = pth.file;

            if (filepath.Substring(pth.file.Length - 3) == "csv")
            {
                    textAssetData = new TextAsset(File.ReadAllText(filepath));
                    string[] lines = textAssetData.text.Split(new string[] { "\n" }, StringSplitOptions.None);
                int line = 0;
                try
                {
                    for (int i = 1; i < lines.Length; i++)
                    {
                        line = i;
                        if (!string.IsNullOrEmpty(lines[i]))
                        {
                            
                            string[] split1 = lines[i].Split(';');

                            //Cargar datos
                            int packId = int.Parse(split1[0]);
                            Vector3 packSize = new Vector3(int.Parse(split1[5]), int.Parse(split1[4]), int.Parse(split1[3]));
                            Vector3 verticalPostP = new Vector3(float.Parse(split1[9]), 1, float.Parse(split1[8]));
                            if (verticalPostP.x > 1 || verticalPostP.y > 1 || verticalPostP.z > 1)
                            {
                                throw new InvalidOperationException("Logfile cannot be read-only");
                            }

                            Vector3 maxForceP = new Vector3(4000000, 4000000, 4000000);
                            if (!string.IsNullOrEmpty(split1[10]))
                             {
                                if (split1[10].Contains("."))
                                {
                                    split1[10] = split1[10].Replace(".", ",");
                                }
                                maxForceP.z = float.Parse(split1[10]);
                                maxForceP.z = Mathf.Ceil(maxForceP.z);
                                if (verticalPostP.x==1 && !string.IsNullOrEmpty(split1[12]))
                                {
                                    if (split1[12].Contains("."))
                                    {
                                        split1[12] = split1[12].Replace(".", ",");
                                    }

                                    maxForceP.x = float.Parse(split1[12]);
                                    maxForceP.x= Mathf.Ceil(maxForceP.x);
                                }
                                if (verticalPostP.z ==1 && !string.IsNullOrEmpty(split1[11]))
                                {
                                    if (split1[11].Contains("."))
                                    {
                                        split1[11] = split1[11].Replace(".", ",");
                                    }
                                    maxForceP.y = float.Parse(split1[11]);
                                    maxForceP.y = Mathf.Ceil(maxForceP.y);
                                }
                            }


                            int quantity = int.Parse(split1[7]);
                            if (split1[6].Contains("."))
                            {
                                split1[6] = split1[6].Replace(".", ",");
                            }
                            float weight = float.Parse(split1[6]) * 10000;
                            int client = int.Parse(split1[2]);
                            int clientID = int.Parse(split1[2]);
                            string name = split1[1];
                            newPackage(packSize, verticalPostP, maxForceP, quantity, weight, false, packId, clientID, name);
                            /* updatePackageValues(packId, packSize, verticalPostP,
                                 new Vector3(Mathf.Floor(maxForceP.x * 10000f / (packSize.x * packSize.y)),
                                 Mathf.Floor(maxForceP.y * 10000f / (packSize.y * packSize.z)),
                                 Mathf.Floor(maxForceP.z * 10000f / (packSize.x * packSize.z))
                                 ), quantity, weight, name, client);*/
                        }

                    }
                }

                catch (System.Exception)
                {
                    alertext.text = "El archivo .csv contiene un error en sus datos en la linea " + (line + 1);
                    alerta.SetActive(true);
                    vaciar();
                    reestablecer();

                }

            }
            else
            {
                alertext.text = "Por favor cargue un archivo tipo .csv";
                alerta.SetActive(true);
            }
        }
    }
    //----------------------------------------------------------
    //Create the model of the CSV
    //----------------------------------------------------------
    public void csvModelo()
    {
        menuCsv.SetActive(false);
       // FileUtil.MoveFileOrDirectory("sourcepath/YourFileOrFolder", "destpath/YourFileOrFolder");
        String path = Application.dataPath + "/StreamingAssets/Modelo(1).csv";
        int counter = 1;
        String path_copy = Application.dataPath + "/StreamingAssets/Modelo.csv";
        while (File.Exists(path_copy))
        {
            path_copy = string.Format("{0}/StreamingAssets/Modelo({1}).csv",
                Application.dataPath,
                counter);
            counter++;
        }
       
        File.Copy(path, path_copy);
        System.Diagnostics.Process process = new System.Diagnostics.Process();
        process.StartInfo.CreateNoWindow = true;
        process.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal;
        process.StartInfo.UseShellExecute = true;
        process.StartInfo.FileName = path_copy;
        //process.StartInfo.Verb = "print";

        process.Start();
    }
    public void csvModeloIR()
    {
        menuCsv.SetActive(false);
        // FileUtil.MoveFileOrDirectory("sourcepath/YourFileOrFolder", "destpath/YourFileOrFolder");
        String path = Application.dataPath + "/StreamingAssets/ModeloIr(1).csv";
        int counter = 1;
        String path_copy = Application.dataPath + "/StreamingAssets/ModeloIr.csv";
        while (File.Exists(path_copy))
        {
            path_copy = string.Format("{0}/StreamingAssets/ModeloIr({1}).csv",
                Application.dataPath,
                counter);
            counter++;
        }

        File.Copy(path, path_copy);
        System.Diagnostics.Process process = new System.Diagnostics.Process();
        process.StartInfo.CreateNoWindow = true;
        process.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal;
        process.StartInfo.UseShellExecute = true;
        process.StartInfo.FileName = path_copy;
        //process.StartInfo.Verb = "print";

        process.Start();
    }
    //----------------------------------------------------------
    //Validate if exist package already
    //----------------------------------------------------------
    public void ArePackage()
    {
        if (packages.Count == 0)
        {
            NoPackage.SetActive(true);
            Debug.LogError("Por favor cargar paquete");
            //throw new NullReferenceException("Exception Message");
            activeFuntion = false;
            throw new NullReferenceException("Exception Message");
        }

    }
    /// <summary>
    //Change the color of the package in base of the group
    /// </summary>
    public void groupColorFuntion()
    {
      
         float var = (1f / numGroups);
        Debug.Log("el numero de grupos es " + numGroups+"y las var "+ var);
        foreach (Transform child in parent)
        {
            int id;
            GameObject go = child.gameObject;
            id = go.GetComponent<Package>().groupId;

            go.GetComponent<Renderer>().material.color = new Color(1-var*id,var*id,0.2f);
        }
        

    }
    //----------------------------------------------------------
    //Change the color of the packege in base of the ID
    //----------------------------------------------------------
    public void packColorFuntion()
    {
        foreach (Transform child in parent)
        {
            int id;
            GameObject go = child.gameObject;
            id = go.GetComponent<Package>().itemId;

            go.GetComponent<Renderer>().material.color = packageColor[id];
        }

    }
   //----------------------------------------------------------
    //Identify the type of change of color to apply
    //----------------------------------------------------------
    public void changeColor()
    {
       if(groupColor && !VistaClient.isOn)
        {
            packColorFuntion();
            groupColor = false;
        }
        else
        {
            groupColorFuntion();
            groupColor = true;
            VistaClient.isOn = false;
        }
    }
    /// <summary>
    //Change the color of the package in base of the group
    /// </summary>
    public void ClientColorFuntion()
    {
        Vector3 firtsColor = new Vector3(0, 0.4f, 1f);
        Vector3 lastColor = new Vector3(0.9f, 0.7f, 0.1f);
        Vector3 diferenceColor = new Vector3(0f, 0f, 0f);
        if (numClient>1)
        {
             diferenceColor = (lastColor - firtsColor) / (numClient - 1);
        }
        
        print("numero de clienest es " + numClient);
        
        foreach (Transform child in parent)
        {
            int id;
            GameObject go = child.gameObject;
            id = go.GetComponent<Package>().client;
            print(diferenceColor+"");
            go.GetComponent<Renderer>().material.color = new Color(firtsColor.x+diferenceColor.x*id, firtsColor.y + diferenceColor.y* id, firtsColor.z + diferenceColor.z * id);
            Vector3 printCOlor = new Vector3(firtsColor.x + diferenceColor.x * id, firtsColor.y + diferenceColor.y * id, firtsColor.z + diferenceColor.z * id);
            print(printCOlor);
        }


    }
  
    //----------------------------------------------------------
    //Identify the type of change of color to apply
    //----------------------------------------------------------
    public void changeColorClient()
    {
        if (groupColor && !VistaGrupos.isOn)
        {
            packColorFuntion();
            groupColor = false;
        }
        else
        {
            ClientColorFuntion();
            groupColor = true;
            VistaGrupos.isOn = false;
        }
    }
    //----------------------------------------------------------
    //Identify if its necesary a new loading 
    //----------------------------------------------------------
    public void showNewLoading()
    {
       
        foreach(Transform child in Lista)
        {
           if(child.gameObject.name.Contains( "newLoadingButtom"))
            {
                GameObject.Destroy(child.gameObject);
            }
        }

        if (intermedio.Count > 0)
        {
            Vector3 pos = modelo.transform.localPosition;
            GameObject instanceNewLoadingButton = Instantiate(newLoadingButtom);
            instanceNewLoadingButton.SetActive(true);
            instanceNewLoadingButton.transform.SetParent(Lista);
            instanceNewLoadingButton.transform.rotation = new Quaternion(0f, 0f, 0f, 0f);
            instanceNewLoadingButton.transform.localScale = new Vector3(1f, 1f, 1f);
            instanceNewLoadingButton.transform.localPosition = new Vector3(pos.x, pos.y - height, pos.z);
        }
    }
    //---------------------------------------------------------
    //Add new loading
    //---------------------------------------------------------
    public void createNewLoading()
    {
        //Save active scenary
        Save();
        CopypTypes = new Dictionary<int, PackageType>();
        //Make a copy of ptype
        foreach (int key in pTypes.Keys)
        {
            CopypTypes.Add(key, pTypes[key]);
        }
        reestablecer();
        vaciar();

        //int counter = pTypes.Count;
        int counter = 0;
        string code = "(" + counter_title + ")";
        if(Titulo.text.Contains(code))
        {
            Titulo.text = Titulo.text.Remove(Titulo.text.Length - 3);
            counter_title++;
        }
        else
        {
            counter_title = 1;
        }
        foreach (float j in intermedio.Keys)
        {
            
            int i = (int)j;
            int quantity = (int)intermedio[j];
            CopypTypes[i].maxForce.x = Mathf.Floor(CopypTypes[i].maxForce.x * (CopypTypes[i].packageSize.x * CopypTypes[i].packageSize.y) / 10000f);
            CopypTypes[i].maxForce.y = Mathf.Floor(CopypTypes[i].maxForce.y * (CopypTypes[i].packageSize.y * CopypTypes[i].packageSize.z) / 10000f);
            CopypTypes[i].maxForce.z = Mathf.Floor(CopypTypes[i].maxForce.z * (CopypTypes[i].packageSize.x * CopypTypes[i].packageSize.z) / 10000f);
            Debug.Log("EL que se va a añadir es " + counter);
            newPackage(CopypTypes[i].packageSize, CopypTypes[i].verticalPos, CopypTypes[i].maxForce,quantity, CopypTypes[i].weight,true, counter,CopypTypes[i].clientId, CopypTypes[i].Name1);
            counter++;
        }
        Titulo.text = Titulo.text + "(" + counter_title + ")";
        CopypTypes.Clear();
        intermedio.Clear();
    }
    //Validate if ptype was delete
    public void validateDelete()
    {   
        string path1 = string.Format("{0}/StreamingAssets/Multidrop/{1}",
        UnityEngine.Application.dataPath, "temp.txt"); 
        List<int> ids =new List<int>();
        int contador = 0;
        bool changefile = false;
        //------------------------
        //Initialize the reading
        //-------------------------
       string[] lines = System.IO.File.ReadAllLines(path1);
        
        foreach(int key in pTypes.Keys)
        {
            ids.Add(key);   
        }
        for (int i = 2; i < lines.Length; i++)
        {
            string[] split1 = lines[i].Split('\t');
            int packId = int.Parse(split1[6]);
            if(!pTypes.ContainsKey(packId))
            {
                changefile = true;
            }
        }
       if(changefile)
        {
            StreamWriter sw = new StreamWriter(path1);
            sw.WriteLine(lines[0]);
            sw.WriteLine(lines[1]);
            for (int i = 2; i < lines.Length; i++)
            {
                string[] split1 = lines[i].Split('\t');
                int packId = int.Parse(split1[6]);
                string message = "";
                message = split1[0] + '\t' + split1[1] + '\t' + split1[2] + '\t' + split1[3] + '\t' + split1[4] + '\t' + split1[5] + '\t' + ids[packId] + '\t' 
                    + split1[7] + '\t' + split1[8];
                sw.WriteLine(message);

            }
            
            sw.Close();
        }
       
    }
   //empty everything
    public void empty()
    {
        vaciar();
        reestablecer();
        emptyMsg.SetActive(false);
    }
    //Kill the process
    public void killProcess()
    {
        inProcess = false;
        process.Kill();
        bloking.SetActive(false);
    }
    public void activeMaxWeight( bool active)
    {
        maxWeight_bol =active;
    }
    public void changeMaxWeight()
    {
        if(maxWeight_iF.text.Contains("."))
        {

        }
        else
        {
            maxWeight= float.Parse(maxWeight_iF.text);
        }
    }
    
}




