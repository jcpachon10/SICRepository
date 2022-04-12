using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Diagnostics;
using Common;
using Debug = UnityEngine.Debug;

public class TwoStageManager : MonoBehaviour
{
    public PackageManajer chief;
    public MulltiPackageManager mainManager;
    Vector3 containerCapacity;

    public Vector3 palletCapacity;

    public float palletThickness = 10f;

    public int[,] cargoInfo;

    public int[,] containerInfo;

    public bool processing;

    bool processed;

    int[] indices;

    int[] leftToPack;

    int[] leftToPallet;

    string[] packageTypes;

    string allPallets;

    int packageCount;

    int customerCount;

    string inFilename = "file.txt";

    int currentContainer = 0;

    int currentPallet = 0;

    string palletHeader;
    private Process process = new Process();

    int numVehi;

    int typeID;

    int generalInstance;

    int palletInstance = 0;

    int maxWeight;

    bool hayContainers;

    public string infoTemp = "";

    public int executionLimit;

    int iteNumber;

    bool nextStep;

    void Start()
    {
        processing = false;
        processed = false;
        nextStep = false;
    }

    void Update()
    {

        if (nextStep == true)
        {
            if (!processed)
            {

                checkUnpalleted(1);
            }
            else
            {
                checkUnpackaged(1);
            }
            nextStep = false;
        }

    }

    private void OnProcessExit(object sender, EventArgs e)
    {
        nextStep = true;
    }

    public async void runTwoStagePacking(string inputFilename)
    {
        runFirstStage(inputFilename);
        runSecondStage();
    }
    public void runFirstStage(string inputFilename)
    {
        //Poner falso vairables
        processing = false;
        processed = false;
        nextStep = false;
        //IMPORTANTE ESTA LÍNEA HAY QUE CAMBIARLA
        palletCapacity = new Vector3(120f, 100f, 160f);
        allPallets = "";
        maxWeight = 400000;
        loadStartingInfo(inputFilename);
        runMultidrop();
        //checkUnpalleted(1);
    }


    public void runSecondStage()
    {
        loadContainersInfo();
        selectContainer(0);
        createStartingInfo();
        runMultidrop();
        //checkUnpackaged(1);
    }

    void loadContainersInfo()
    {
        generalInstance = 0;
        string path1 = string.Format("{0}/StreamingAssets/MultipleContainer/Algorithm/{1}",
        UnityEngine.Application.dataPath, "vehiculos.txt");

        string[] lines = System.IO.File.ReadAllLines(path1);
        numVehi = int.Parse(lines[0]);

        containerInfo = new int[numVehi, 8];
        string[] lineaC;

        for (int i = 0; i < numVehi; i++)
        {
            lineaC = lines[i + 1].Split('\t');
            containerInfo[i, 0] = (int)float.Parse(lineaC[0]);
            containerInfo[i, 1] = (int)float.Parse(lineaC[1]);
            containerInfo[i, 2] = (int)float.Parse(lineaC[2]);
            containerInfo[i, 3] = (int)float.Parse(lineaC[3]);
            containerInfo[i, 4] = (int)float.Parse(lineaC[4]);
            containerInfo[i, 5] = (int)float.Parse(lineaC[5]);
            containerInfo[i, 6] = (int)float.Parse(lineaC[6]);
            containerInfo[i, 7] = 0;
        }
        hayContainers = true;
    }

    void selectPallet()
    {
        generalInstance = 0;
    }
    void selectContainer(int pCriterium)
    { //0 para precio, 1 para volumen, 2 para peso, 3 para personalizado (que no existe aun)
        int[] criteriumList = new int[numVehi];
        int greatestIndex = 0;
        int greatestValue = -1;
        int multiplier = 1;
        int field = 0;
        int currentVal = 0;

        switch (pCriterium)
        {
            case 0:
                multiplier = -1;
                field = 6;
                break;
            case 1:
                multiplier = 1;
                field = 0;
                break;
            case 2:
                multiplier = 1;
                field = 5;
                break;
        }
        bool started = false;

        for (int i = 0; i < numVehi; i++)
        {
            if (containerInfo[i, 4] > 0)
            { //donde se guarda el n�mero de disponibles

                if (field == 1)
                {
                    currentVal = containerInfo[i, 1] * containerInfo[i, 2] * containerInfo[i, 3];
                }
                else
                {
                    currentVal = multiplier * containerInfo[i, field];
                }
                if (!started)
                {
                    greatestValue = currentVal;
                    greatestIndex = i;
                    started = true;
                    Debug.Log("Valor mayor es " + currentVal);
                }
                else if (currentVal > greatestValue)
                {
                    greatestValue = currentVal;
                    greatestIndex = i;
                }
                Debug.Log("Valor es " + currentVal);
            }
        }

        if (started == false)
        {
            hayContainers = false;
            Debug.Log("No quedan contenedores");
        }
        else
        {
            palletCapacity = new Vector3(containerInfo[greatestIndex, 1], containerInfo[greatestIndex, 2], containerInfo[greatestIndex, 3]);
            containerInfo[greatestIndex, 4]--;
            containerInfo[greatestIndex, 7]++;
            palletHeader = "" + containerInfo[greatestIndex, 1] + '\t' + containerInfo[greatestIndex, 2] + '\t' + containerInfo[greatestIndex, 3] + "\n";
            generalInstance = greatestIndex;
            maxWeight = containerInfo[greatestIndex, 5];
            //buildTextInput(1);
            Debug.Log("Se selecciono el contenedor " + greatestIndex + ", quedan " + containerInfo[greatestIndex, 4]);
        }


    }


    void loadStartingInfo(string pFileName)
    { //Container size en del txt de entrada es el tama�o de los pallets
        currentPallet = 0;
        string path1 = string.Format("{0}/StreamingAssets/TwoStage/Algorithm/{1}",
        UnityEngine.Application.dataPath, pFileName);
        //-------
        //Leer lineas del archivo
        string[] lines = System.IO.File.ReadAllLines(path1);
        string[] linesWithText = new string[lines.Length / 2];


        leftToPack = new int[int.Parse(lines[0].Split('\t')[0])];
        indices = new int[leftToPack.Length];

        //Calcular el limite del archivo que divide temp.txt de file.txt
        int limite = 0;
        int lineIndex = 0;

        Debug.Log("Lineas: " + lines.Length);

        for (int i = 4; i < lines.Length; i = i + 2)
        {
            linesWithText[lineIndex] = lines[i];

            lineIndex++;
            if (string.IsNullOrWhiteSpace(lines[i]))
            {
                limite = i;
                break;
            }

        }

        palletHeader = "" + palletCapacity.x + '\t' + palletCapacity.y + '\t' + palletCapacity.z;




        cargoInfo = new int[lineIndex, 14];

        string[] infoLine = new string[14];
        string newText = "";
        for (int i = 0; i < lineIndex; i++)
        {
            newText = newText + linesWithText[i] + '\n' + '\n';
            infoLine = linesWithText[i].Split('\t');

            cargoInfo[i, 0] = int.Parse(infoLine[0]);
            cargoInfo[i, 1] = int.Parse(infoLine[1]);
            cargoInfo[i, 2] = int.Parse(infoLine[2]);
            cargoInfo[i, 3] = int.Parse(infoLine[3]);
            cargoInfo[i, 4] = int.Parse(infoLine[4]);
            cargoInfo[i, 5] = int.Parse(infoLine[5]);
            cargoInfo[i, 6] = int.Parse(infoLine[6]);
            cargoInfo[i, 7] = int.Parse(infoLine[7]);
            cargoInfo[i, 8] = int.Parse(infoLine[8]);
            cargoInfo[i, 9] = int.Parse(infoLine[9]);
            cargoInfo[i, 10] = int.Parse(infoLine[10]);
            cargoInfo[i, 11] = int.Parse(infoLine[11]);
            cargoInfo[i, 12] = int.Parse(infoLine[12]);
            cargoInfo[i, 13] = int.Parse(infoLine[13]);
            leftToPack[i] = int.Parse(infoLine[7]);
            indices[i] = i;
        }


        packageCount = int.Parse(lines[0].Split('\t')[0]);
        customerCount = int.Parse(lines[0].Split('\t')[1]);



        string intertex;

        intertex = "" + lines[0] + '\n' + '\n' + palletHeader + '\n' + '\n' + newText;


        string fileWrite = string.Format("{0}/StreamingAssets/TwoStage/Algorithm/{1}",
                 UnityEngine.Application.dataPath, "intermediate.txt");

        File.WriteAllText(fileWrite, intertex);
        fileWrite = string.Format("{0}/StreamingAssets/TwoStage/Algorithm/{1}",
                 UnityEngine.Application.dataPath, "pallets.txt");
        File.WriteAllText(fileWrite, lines[0] + '\t' + palletHeader + '\n' + "0	99999");
    }


    void createStartingInfo()
    { //Container size en del txt de entrada es el tama�o de los pallets
        currentContainer = 0;
        string path1 = string.Format("{0}/StreamingAssets/TwoStage/Algorithm/{1}",
        UnityEngine.Application.dataPath, "palletDefinitions.txt");
        //-------
        //Leer lineas del archivo
        string[] lines = System.IO.File.ReadAllLines(path1);
        string[] linesWithText = new string[lines.Length / 2];


        leftToPack = new int[int.Parse(lines[0].Split('\t')[0])];
        indices = new int[leftToPack.Length];

        //Calcular el limite del archivo que divide temp.txt de file.txt
        int limite = 0;
        int lineIndex = 0;

        Debug.Log("Lineas dos: " + lines.Length);

        for (int i = 4; i < lines.Length; i = i + 2)
        {
            linesWithText[lineIndex] = lines[i];

            lineIndex++;
            if (string.IsNullOrWhiteSpace(lines[i]))
            {
                limite = i;
                break;
            }

        }

        palletHeader = "" + palletCapacity.x + '\t' + palletCapacity.y + '\t' + palletCapacity.z;

        cargoInfo = new int[lineIndex, 14];

        string[] infoLine = new string[14];
        string newText = "";
        for (int i = 0; i < lineIndex; i++)
        {
            newText = newText + linesWithText[i] + '\n' + '\n';
            infoLine = linesWithText[i].Split('\t');

            cargoInfo[i, 0] = int.Parse(infoLine[0]);
            cargoInfo[i, 1] = int.Parse(infoLine[1]);
            cargoInfo[i, 2] = int.Parse(infoLine[2]);
            cargoInfo[i, 3] = int.Parse(infoLine[3]);
            cargoInfo[i, 4] = int.Parse(infoLine[4]);
            cargoInfo[i, 5] = int.Parse(infoLine[5]);
            cargoInfo[i, 6] = int.Parse(infoLine[6]);
            cargoInfo[i, 7] = int.Parse(infoLine[7]);
            cargoInfo[i, 8] = int.Parse(infoLine[8]);
            cargoInfo[i, 9] = int.Parse(infoLine[9]);
            cargoInfo[i, 10] = int.Parse(infoLine[10]);
            cargoInfo[i, 11] = int.Parse(infoLine[11]);
            cargoInfo[i, 12] = int.Parse(infoLine[12]);
            cargoInfo[i, 13] = int.Parse(infoLine[13]);
            leftToPack[i] = int.Parse(infoLine[7]);
            indices[i] = i;
        }


        packageCount = int.Parse(lines[0].Split('\t')[0]);
        customerCount = int.Parse(lines[0].Split('\t')[1]);



        string intertex;

        intertex = "" + lines[0] + '\n' + '\n' + palletHeader + '\n' + '\n' + newText;


        string fileWrite = string.Format("{0}/StreamingAssets/TwoStage/Algorithm/{1}",
                 UnityEngine.Application.dataPath, "intermediate.txt");

        File.WriteAllText(fileWrite, intertex);
        fileWrite = string.Format("{0}/StreamingAssets/TwoStage/Algorithm/{1}",
                 UnityEngine.Application.dataPath, "tempPallet.txt");
        File.WriteAllText(fileWrite, lines[0] + '\t' + palletHeader + '\n' + "0	99999");

    }

    void buildTextInput(int execMode)
    {

        int remainingTypes = 0;
        string intermedio = "";
        string newText = "";
        string infoLine = "";

        for (int i = 0; i < leftToPack.Length; i++)
        {
            if (leftToPack[i] > 0)
            {
                infoLine = "" + i + '\t' + cargoInfo[i, 1] + '\t' + cargoInfo[i, 2] + '\t' + cargoInfo[i, 3] + '\t' + cargoInfo[i, 4] + '\t' + cargoInfo[i, 5] + '\t' + cargoInfo[i, 6]
                + '\t' + leftToPack[i] + '\t' + cargoInfo[i, 8] + '\t' + cargoInfo[i, 9] + '\t' + cargoInfo[i, 10] + '\t' + cargoInfo[i, 11] + '\t' + cargoInfo[i, 12] + '\t' + cargoInfo[i, 13] + '\n' + '\n';


                leftToPack[i] = 0;
                indices[remainingTypes] = i;

                newText = newText + infoLine;
                remainingTypes++;
            }
        }
        intermedio = "" + remainingTypes + '\t' + customerCount + '\t' + '\n' + '\n' + palletHeader + '\n' + '\n' + newText;

        string fileWrite = string.Format("{0}/StreamingAssets/TwoStage/Algorithm/{1}",
                  UnityEngine.Application.dataPath, "intermediate.txt");

        File.WriteAllText(fileWrite, intermedio);



    }
    void runMultidrop()
    {

        string path1 = string.Format("{0}/StreamingAssets/TwoStage/Algorithm/{1}",
        UnityEngine.Application.dataPath, "intermediate.txt");



        //-------
        //Leer lineas del archivo
        string[] lines = System.IO.File.ReadAllLines(path1);
        
       


        //Debug.Log("Peso max es " + maxWeight);
        print("hay "+ lines.Length);
        //Validar si es una sola caja
        if (lines.Length != 6 )
        {
            ProcessStartInfo startInfo = process.StartInfo;


            //        chief.containerSize=palletCapacity;

            startInfo.FileName = string.Format("{0}/StreamingAssets/TwoStage/Algorithm/{1}",
            UnityEngine.Application.dataPath, "Multidrop.exe");
            startInfo.WorkingDirectory = string.Format("{0}/StreamingAssets/TwoStage/Algorithm",
            UnityEngine.Application.dataPath);

            //Daneses (Visibilidad y apilamiento)
            //Sesquia (Alcanzable)
            //Juanqueria (pared)
            //Nombre opcion(1-Clientes(Se pueden apilar) 2-Pesos 3-Pared invisibles)
            startInfo.Arguments = "intermediate.txt 1   1   1   3   2  20";
            //StartInfo.Arguments = "Extra Arguments to Pass to the Program";
            startInfo.CreateNoWindow = true;
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            //startInfo.UseShellExecute = true;
            // *** Redirect the output ***
            //startInfo.RedirectStandardError = true;
            //startInfo.RedirectStandardOutput = true;
            process.EnableRaisingEvents = true;
            process.Exited += new EventHandler(OnProcessExit);
            process.Start();
        }
        else
        {

            string[] split = lines[4].Split('\t');
            if (split[7] == "1")
            {
                //ultima opcion
                //Poner falso vairables
                processing = false;
                processed = false;
                nextStep = false;
                mainManager.loadingPalletsInContainers("tempPallet.txt");
                /*
                string tempWrite = string.Format("{0}/StreamingAssets/TwoStage/Algorithm/{1}",
                UnityEngine.Application.dataPath, "temp.txt");
                int uniqueIndex = int.Parse(split[0]);
                string h1 = "0" + '\t' + customerCount + '\t' + palletHeader + '\n' + "0" + "0" + '\n';
                string theLine = h1 + "0" + '\t' + "0" + '\t' + "0" + '\t' + cargoInfo[uniqueIndex, 1] + '\t' + cargoInfo[uniqueIndex, 3] + '\t' + cargoInfo[uniqueIndex, 5] + '\t' + split[0] + '\t' + "1" + '\t' + "0";
                string theLine1 = "\n";
                File.WriteAllText(tempWrite, theLine + theLine1);
                print("Solo hay una");
                checkUnpalleted(1);
                //nextStep = false;*/
            }
            else
            {
                ProcessStartInfo startInfo = process.StartInfo;


                //        chief.containerSize=palletCapacity;

                startInfo.FileName = string.Format("{0}/StreamingAssets/TwoStage/Algorithm/{1}",
                UnityEngine.Application.dataPath, "Multidrop.exe");
                startInfo.WorkingDirectory = string.Format("{0}/StreamingAssets/TwoStage/Algorithm",
                UnityEngine.Application.dataPath);

                //Daneses (Visibilidad y apilamiento)
                //Sesquia (Alcanzable)
                //Juanqueria (pared)
                //Nombre opcion(1-Clientes(Se pueden apilar) 2-Pesos 3-Pared invisibles)
                startInfo.Arguments = "intermediate.txt 1   1   1   3   2  20";
                //StartInfo.Arguments = "Extra Arguments to Pass to the Program";
                startInfo.CreateNoWindow = true;
                startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                //startInfo.UseShellExecute = true;
                // *** Redirect the output ***
                //startInfo.RedirectStandardError = true;
                //startInfo.RedirectStandardOutput = true;
                process.EnableRaisingEvents = true;
                process.Exited += new EventHandler(OnProcessExit);
                process.Start();
            }
        }
    }

    void checkUnpackaged(int executionMode)
    {//Execution mode 0 para armar pallets, 1 para armar containers
        string path1 = string.Format("{0}/StreamingAssets/TwoStage/Algorithm/{1}",
        UnityEngine.Application.dataPath, "temp.txt");
        string path2 = string.Format("{0}/StreamingAssets/TwoStage/Algorithm/{1}",
        UnityEngine.Application.dataPath, "tempPallet.txt");


        for (int i = 0; i < leftToPack.Length; i++)
        {
            leftToPack[i] = 0;

        }

        string bodyText = "";
        //-------
        //Leer lineas del archivo
        string[] lines = System.IO.File.ReadAllLines(path1);
        string[] packInfo = new string[lines.Length - 2];
        string[] packInfo1 = new string[lines.Length - 2];
        string[] exceedWeight = new string[lines.Length - 2];

        int[] boxParams;
        int finalPacked = -1;
        int packW;
        float totalPalletWeight = 0f;

        //Debug.Log("Peso max es " + maxWeight);

        if (lines.Length > 3 && hayContainers)
        {
            infoTemp = "";
            Debug.Log("Hay " + lines.Length + " cajas");
            for (int i = 2; i < lines.Length; i++)
            {
                boxParams = Array.ConvertAll(lines[i].Split('\t'), int.Parse);
                string linea = "" + boxParams[0] + '\t' + boxParams[1] + '\t' + boxParams[2] + '\t' + boxParams[3] + '\t'
                    + boxParams[4] + '\t' + boxParams[5] + '\t' + indices[boxParams[6]] + '\t' + boxParams[7] + '\t' + boxParams[8];
                packInfo1[i - 2] = linea;
            }
            for (int i = 2; i < lines.Length; i++)
            {
                boxParams = Array.ConvertAll(lines[i].Split('\t'), int.Parse);
                string linea = "" + boxParams[0] + '\t' + boxParams[1] + '\t' + boxParams[2] + '\t' + boxParams[3] + '\t'
                    + boxParams[4] + '\t' + boxParams[5] + '\t' + indices[boxParams[6]] + '\t' + boxParams[7] + '\t' + boxParams[8];
                packW = cargoInfo[indices[boxParams[6]], 8] / 10000;
                packInfo[i - 2] = linea;
                print("Entro a la monda :" + totalPalletWeight + " " + packW + " " + maxWeight);
                if (boxParams[3] > palletCapacity.x)
                {
                    finalPacked = i - 2;
                    Debug.Log("Fuera! " + i);
                    break;
                }
                else if (totalPalletWeight + packW > maxWeight)
                {

                    exceedWeight[i - 2] = packInfo[i - 2];
                    packInfo[i - 2] = "";

                }
                else
                {
                    totalPalletWeight = totalPalletWeight + packW;

                }
            }
            //Copy

            if (finalPacked == -1)
            {
                finalPacked = lines.Length - 2;
            }

            //Debug.Log("El final es "+finalPacked);

            int packedCount = 0;

            // for (int i = 0; i < finalPacked; i++){
            //     if (packInfo[i]!=""){
            //         packedCount++;
            //         bodyText=bodyText+packInfo[i] + '\n';
            //     }

            // }

            //Debug.Log("checo " + packInfo.Length + "lineas");

            for (int i = 0; i < packInfo.Length; i++)
            {
                if (packInfo[i] != "" && i < finalPacked)
                {
                    packedCount++;
                    bodyText = bodyText + '\n' + packInfo[i] + '\t' + generalInstance + '\t' + currentContainer;
                    //Debug.Log("Checado todo "+i);
                }
                else
                {
                    boxParams = Array.ConvertAll(packInfo1[i].Split('\t'), int.Parse);
                    int packType = boxParams[6];
                    leftToPack[packType]++;
                    infoTemp = infoTemp + '\n' + packInfo1[i] + '\t' + generalInstance + '\t' + currentPallet;

                    //Debug.Log("Del "+packType+" hay "+leftToPack[packType]);
                }

            }


            File.AppendAllText(path2, bodyText);
            currentContainer++;

            for (int i = 0; i < leftToPack.Length; i++)
            {
                indices[i] = 0;
            }

            selectContainer(0);



            //if (hayContainers)
            //{
            buildTextInput(0);


            print("algo marica");

            runMultidrop();
            //checkUnpackaged(executionMode);
            //}

        }

        else if (hayContainers == false)
        {
            Debug.Log("No hay mas containers");
            processing = false;
            File.AppendAllText(path2, infoTemp);
            //Poner falso vairables
            processing = false;
            processed = false;
            nextStep = false;
            mainManager.loadingPalletsInContainers("tempPallet.txt");
        }

        else
        {
            //Poner falso vairables
            processing = false;
            processed = false;
            nextStep = false;
            mainManager.loadingPalletsInContainers("tempPallet.txt");
        }

    }


    void checkUnpalleted(int executionMode)
    {//Execution mode 0 para no apilar, 1 para apilables
        print("aaaaaaaaa");
        string path1 = string.Format("{0}/StreamingAssets/TwoStage/Algorithm/{1}",
        UnityEngine.Application.dataPath, "temp.txt");
        string path2 = string.Format("{0}/StreamingAssets/TwoStage/Algorithm/{1}",
        UnityEngine.Application.dataPath, "pallets.txt");

        print("casi");
        for (int i = 0; i < leftToPack.Length; i++)
        {
            leftToPack[i] = 0;

        }

        string bodyText = "";
        //-------
        //Leer lineas del archivo
        string[] lines = System.IO.File.ReadAllLines(path1);
        string[] packInfo = new string[lines.Length - 2];
        string[] packInfo1 = new string[lines.Length - 2];
        string[] exceedWeight = new string[lines.Length - 2];

        int[] boxParams;
        int finalPacked = -1;
        int packW; 
        float totalPalletWeight = 0f;

        //Debug.Log("Peso max es " + maxWeight);

        int[] maxes = new int[3];
        int[] mins = new int[3];
        int lowestBS;

        iteNumber++;

        print(lines.Length);
        //print("esta verga es: "+lines[2]);
        if(lines.Length>3)
        {
            //if (!string.IsNullOrWhiteSpace(lines[2]))// && iteNumber < executionLimit)
            //{
                infoTemp = "";
                Debug.Log("Hay " + lines.Length + " cajas");
                for (int i = 2; i < lines.Length; i++)
                {
                    boxParams = Array.ConvertAll(lines[i].Split('\t'), int.Parse);
                    string linea = "" + boxParams[0] + '\t' + boxParams[1] + '\t' + boxParams[2] + '\t' + boxParams[3] + '\t'
                        + boxParams[4] + '\t' + boxParams[5] + '\t' + indices[boxParams[6]] + '\t' + boxParams[7] + '\t' + boxParams[8];
                    packInfo1[i - 2] = linea;
                }
                for (int i = 2; i < lines.Length; i++)
                {
                    boxParams = Array.ConvertAll(lines[i].Split('\t'), int.Parse);
                    string linea = "" + boxParams[0] + '\t' + boxParams[1] + '\t' + boxParams[2] + '\t' + boxParams[3] + '\t'
                        + boxParams[4] + '\t' + boxParams[5] + '\t' + indices[boxParams[6]] + '\t' + boxParams[7] + '\t' + boxParams[8];
                    packW = cargoInfo[indices[boxParams[6]], 8] / 10000;
                    packInfo[i - 2] = linea;
                    if (boxParams[3] > palletCapacity.x)
                    {
                        //packInfo[i-2]="";
                        // era i-3
                        if (i == 2)
                        {
                            //No se pudo cargar ningun packete 
                            processed = true;
                            print("SE PUTEO");
                            mainManager.bloking.SetActive(false);
                            mainManager.inProcess = false;
                            return;
                        }
                        finalPacked = i - 2;
                        Debug.Log("Fuera! " + i);
                        break;
                    }
                    else if (totalPalletWeight + packW > maxWeight)
                    {
                        exceedWeight[i - 2] = packInfo[i - 2];
                        packInfo[i - 2] = "";

                    }
                    else
                    {
                        totalPalletWeight = totalPalletWeight + packW;
                        if (i == 2)
                        {
                            mins[0] = boxParams[0];
                            mins[1] = boxParams[1];
                            mins[2] = boxParams[2];
                            maxes[0] = boxParams[3];
                            maxes[1] = boxParams[4];
                            maxes[2] = boxParams[5];
                            //lowestBS
                        }
                        else
                        {
                            for (int j = 0; j < 3; j++)
                            {
                                if (boxParams[j] < mins[j])
                                {
                                    mins[j] = boxParams[j];
                                }
                                if (boxParams[j + 3] > maxes[j])
                                {
                                    maxes[j] = boxParams[j + 3];
                                }
                            }
                        }

                    }
                }

                int bearingS = 0;

                if (executionMode == 1)
                {

                    bearingS = 0;
                }

                // string palType = "0" + '\t' + (maxes[0] - mins[0]) + '\t' + "0" + '\t' + (maxes[1] - mins[1]) + '\t' + "0" + '\t'
                //                 + (maxes[2] - mins[2] + (int)palletThickness) + '\t' + "1" + '\t' + "1" + '\t' + totalPalletWeight + '\t' + bearingS + '\t' + bearingS + '\t' + bearingS + '\t' + "0" + '\t' + "0" + '\n' + '\n';
                string palType = "0" + '\t' + palletCapacity.x + '\t' + "0" + '\t' + palletCapacity.y + '\t' + "0" + '\t'
                                + palletCapacity.z + '\t' + "1" + '\t' + "1" + '\t' + totalPalletWeight * 10000 + '\t' + bearingS + '\t' + bearingS + '\t' + bearingS + '\t' + "0" + '\t' + "0" + '\n' + '\n';

                //uno de esos números es el cliente
                allPallets = allPallets + palType;
                palType = "";

                print(palType);

                if (finalPacked == -1)
                {
                    finalPacked = lines.Length - 2;
                }

                Debug.Log("El final es " + finalPacked);

                int packedCount = 0;


                for (int i = 0; i < packInfo.Length; i++)
                {
                    if (packInfo[i] != "" && i < finalPacked)
                    {
                        packedCount++;
                        bodyText = bodyText + '\n' + packInfo[i] + '\t' + generalInstance + '\t' + currentPallet;
                        //Debug.Log("Checado todo "+i);
                    }
                    else
                    {
                        boxParams = Array.ConvertAll(packInfo1[i].Split('\t'), int.Parse);
                        int packType = boxParams[6];
                        leftToPack[packType]++;
                        infoTemp = infoTemp + '\n' + packInfo1[i] + '\t' + generalInstance + '\t' + currentPallet;

                        //Debug.Log("Del "+packType+" hay "+leftToPack[packType]);
                    }

                }


                File.AppendAllText(path2, bodyText);

                //  for (int i = 0; i < leftToPack.Length; i++)
                // {
                //     indices[i] = 0;
                // }


                currentPallet++;
                selectPallet();

                //if (hayContainers)
                //{
                buildTextInput(0);

                runMultidrop();
                //checkUnpalleted(executionMode);
                //}

            //}

        }

        else
        {
            string h = "" + currentPallet + '\t' + customerCount + '\n' + '\n' + palletHeader + '\n' + '\n';
            allPallets = h + allPallets;
            print("sucker");
            string path3 = string.Format("{0}/StreamingAssets/TwoStage/Algorithm/{1}",
            UnityEngine.Application.dataPath, "palletDefinitions.txt");
            File.WriteAllText(path3, allPallets);
            processed = true;
            runSecondStage();
        }

        //Debug.Log("Checado todo");

        // packageCount=int.Parse(linesWithText[0].Split('\t')[0]);
        // customerCount=int.Parse(linesWithText[0].Split('\t')[1]);

    }


}
