using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Text;
using System.Diagnostics;
using UnityEngine.UI;
using System.Linq;
using System.Globalization;
using UnityEditor;
using System.Runtime.InteropServices;
using System.Collections.Specialized;
using System.Security.Cryptography;
using Common;

public class IrregularManager : MonoBehaviour
{
   //-----------------------
   //Class of type of objects
   //-----------------------
    public class objectsType
    {

        //----------------------------------
        //PUBLIC VARIABLES
        //----------------------------------
        //Nombre del paquete
        public string Name1;
        //type
        public int classIR;
        //size
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
        //ui
        public GameObject UIpanel;
        //Vectors of the object
        public Vector3[] verticesMesh;
        //Triangles of the object
        public int[] trianglesMesh;
        //vlidate rotation
        public bool rotation;
        //----------------------------
        //Methods
        //----------------------------
        public objectsType(GameObject newUi, int classir,Color packageColorP, int packageIdP, Vector3 packageSizeP,int quantityP, float weightP, ActualizarPackage uiPackageP, string name, bool rot)
        {
            UIpanel = newUi;
            classIR = classir;
            packageColor = packageColorP;
            packageSize = packageSizeP;
            packageId = packageIdP;
            quantity = quantityP;
            weight = weightP;
            uiPackage = uiPackageP;
            packagePositions = new List<Vector3>();
            endPositions = new List<Vector3>();
            Name1 = name;
            rotation = rot;
        }
        public void updatePType( Vector3 packageSizeP,  int quantityP, float weightP, String NameValue, int classIr, bool rot)
        {
            classIR=classIr;
            packageSize = packageSizeP;
            quantity = quantityP;
            weight = weightP;
            rotation = rot;
            Name1 = NameValue;
        }
        public void AddMesh(Vector3[] VMesh, int[] TMesh)
        {
            verticesMesh = VMesh;
            trianglesMesh = TMesh;
        }
        public void AddPosition(Vector3 initialPosition, Vector3 endPosition)
        {
            packagePositions.Add(initialPosition);
            endPositions.Add(endPosition);
        }
    }

    // VARIABLES
    private bool activeFuntion;
    //types of objects
    public Dictionary<int, objectsType> pTypesIR;
    //Model of ui
    public GameObject model;
    //List ui of objects
    public RectTransform Lista;
    //use packageManager
    public PackageManajer pm;
    //List Height 
    public float height;
    //Numbre in General UI model
    public Text numero;
    //Numbre in miniature UI model
    public Text numeroMiniatura;
    //Text of the active package
    public Text controlador;
    //Color image in the general UI of the model
    public Image ColorImage;
    //Color image in the miniature UI of the model
    //Preview of the package
    public GameObject CubePreview;
    //Preview of the cylinder
    public GameObject cylinderPreview;
    public Image ColorMiniatura;
    //Archivo de carga CSV
    public TextAsset textAssetData;
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
    //Rotation in the model
    public Toggle panel_Rotacion;
    //Cube Toggle
    public Toggle cube_Toggle;
    //cylinder Toggle
    public Toggle cylinder_Toggle;
    //other Toggle
    public Toggle other_Toggle;
    // Nombre 
    public InputField Name;
    //Scrip irToggel
    public IRToggle irToggle;
    public View view;
    //Loading bar 
    public Image LoadBar;
    private bool loading_bar = false;
    private float TimeLeft;
    //process exe
    private Process process = new Process();
    //define if proces is playing
    //0-No run 
    //1-run
    bool inProcess = false;
    public string fileName = "";
    private float error = 0.0001f;
    //Blocked panel
    public GameObject bloking;
    //txtManagerIr
    public TxtManagerIR txtManagerIR;

    // Variables Cubo
    public float Lx = 0;
    public float Ly = 0;
    public float Lz = 0;

    // Variables Cilindro
    public float Diametro = 0;
    public float Altura = 0;
    private int Refinamiento = 32;

    // 1 Read data
    private List<int> idPieza; // id de la pieza
    private List<List<Vector3>> vertices; // (x,y,z) de cada vértice para cada pieza

    // 2 GameObjects

    private List<GameObject> myGameObjects;

    // FUNCIONES
    void Start()
    {

        //tapGroup.OnTabSelected(CargaTap);
        pTypesIR = new Dictionary<int, objectsType>();
        //intermedio = new Dictionary<float, float>();
        //packages = new List<GameObject>();
        //packagesUI = new List<GameObject>();

        //lenght_Input.text = 50 + "";
        //height_Input.text = 50 + "";
        //width_Input.text = 50 + "";
        //weight_Input.text = 1 + "";
        //pcs_Input.text = 1 + "";
        //client_input.text = 0 + "";
        //Nombre_proyecto.text = "Proyecto_1";
        //resizeContainer(new Vector3(2.2f, 2.27f, 2.43f));
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
                runalgorim1();
            }

        }
        else
	    {
            LoadBar.enabled = false;
        }
    }

        //--------------------------------------------------------------
        //Calculate 
        //--------------------------------------------------------------
        float puntoCorte(Vector3 face, Vector3 vec)
    {
        return -face.x * vec.x - face.y * vec.y - face.z * vec.z;
    }
    float distance(Vector3 punto, Vector4 plano)
    {
        return punto.x * plano.x + punto.y * plano.y + punto.z * plano.z + plano.w;
    }
    Vector3 productoCruz(Vector3 v1, Vector3 v2, Vector3 v3)
    {
        Vector3 ab = v2 - v1;
        Vector3 ac = v3 - v1;
        return new Vector3(ab.y * ac.z - ab.z * ac.y,
                         -(ab.x * ac.z - ab.z * ac.x),
                           ab.x * ac.y - ab.y * ac.x);
    }
    int indiceVec(Vector3 vec, List<Vector3> lista)
    {
        int resp = 0;
        for (int i = 0; i < lista.Count; i++)
            if (Mathf.Abs(vec.x - lista[i].x) < error && Mathf.Abs(vec.y - lista[i].y) < error && Mathf.Abs(vec.z - lista[i].z) < error)
            {
                resp = i;
                break;
            }
        return resp;
    }
    //--------------------------
    // Obtener vértices Cubo:
    //-------------------------
    List<Vector3> verticesCubo()
    {
        List<Vector3> vertices = new List<Vector3>();
        vertices.Add(new Vector3(0, 0, 0));
        vertices.Add(new Vector3(Lx, 0, 0));
        vertices.Add(new Vector3(0, Ly, 0));
        vertices.Add(new Vector3(Lx, Ly, 0));
        vertices.Add(new Vector3(0, 0, Lz));
        vertices.Add(new Vector3(Lx, 0, Lz));
        vertices.Add(new Vector3(0, Ly, Lz));
        vertices.Add(new Vector3(Lx, Ly, Lz));
        return vertices;
    }

    // Obtener vértices cilindro

    List<Vector3> verticesCilindro()
    {
        List<Vector3> vertices = new List<Vector3>();
        float angulo = 2 / (float)Refinamiento * Mathf.PI; // Radianes
        float Radio = Diametro / 2;
        float myRadio = Radio / Mathf.Cos(angulo / 2);
        for (int i = 0; i < Refinamiento; i++)
        {
            float valAngulo = angulo * i;
            float valx = myRadio * Mathf.Cos(valAngulo) - myRadio;
            float valy = myRadio * Mathf.Sin(valAngulo);
            if (i == 0)
            {
                vertices.Add(new Vector3(0, 0, 0));
                vertices.Add(new Vector3(0, 0, Altura));
            }
            else
            {
                vertices.Add(new Vector3(valx, valy, 0));
                vertices.Add(new Vector3(valx, valy, Altura));
            }
        }
        return vertices;
    }
    //--------------------------
    // Funtion asing to new pacakage butttom
    //-------------------------
    public void NewPackageButtom()
    {
        int idP = 0;
        while (pTypesIR.ContainsKey(idP))
        {
            idP++;
        }
        newObject(new Vector3(),1, 1, 0, true, idP, "");
    }

    //--------------------------
    // Create new objects and add to otype
    //-------------------------
    public void newObject(Vector3 packageSizeP, int classIR, int quantityP, float weightP, bool rotation, int Id, string name)
    {

        var pos = model.transform.localPosition;
        Material material;
        Material material1;
        
        Name.text = "Item" + Id;
        //Chncage color previwe
        material = Instantiate(CubePreview.GetComponent<Renderer>().material);
        CubePreview.GetComponent<Renderer>().material = material;
        material1 = Instantiate(cylinderPreview.GetComponent<Renderer>().material);
        cylinderPreview.GetComponent<Renderer>().material = material;
   
        if (!string.IsNullOrEmpty(name))
        {
            if (classIR==1)
            {
                cube_Toggle.isOn=true;
                cylinder_Toggle.isOn = false;
                other_Toggle.isOn = false;
                irToggle.cube(true);
            }
            else if (classIR == 2)
            {
                cylinder_Toggle.isOn = true;
                other_Toggle.isOn = false;
                cube_Toggle.isOn = false;
                irToggle.cylinder(true);
            }
            else if (classIR == 3)
            {
                other_Toggle.isOn = true;
                irToggle.other(true);
            }
            
            Name.text = name;
            lenght_Input.text = (packageSizeP.z) + "";
            height_Input.text = (packageSizeP.y) + "";
            width_Input.text = (packageSizeP.x) + "";
            weight_Input.text = (weightP) + "";
            pcs_Input.text = (quantityP) + "";
            panel_Rotacion.isOn = rotation;
            //Name.text = name;

        }
        else
        {
            lenght_Input.text = 50 + "";
            height_Input.text = 50 + "";
            width_Input.text = 50 + "";
            weight_Input.text = 1 + "";
            pcs_Input.text = 1 + "";
            panel_Rotacion.isOn = true;
        }

        //Ampliar lista
        if (Id > 8)
        {
            Lista.sizeDelta = new Vector2(Lista.sizeDelta.x, Lista.sizeDelta.y + 35f);
        }
        
        //Asiganer material al cubo
        material.color = pm.packageColor[Id];
        material1.color = pm.packageColor[Id];
        //cambiar imagen y color 
        ColorImage.color = pm.packageColor[Id];
        ColorMiniatura.color = pm.packageColor[Id]; 
        numero.text = Id + "";
        controlador.text = Id + "";
        numeroMiniatura.text = Id + "";

       

        //Crear el nuevo objeto
        GameObject nuevo = Instantiate(model);
        nuevo.SetActive(true);
        nuevo.transform.SetParent(Lista);
        ActualizarPackage uiPackageP = nuevo.GetComponent<ActualizarPackage>();
        //Crear nuevo PackageType
        objectsType p = new objectsType(nuevo,classIR,pm.packageColor[Id], Id, packageSizeP,quantityP, weightP, uiPackageP, Name.text, rotation);

        pTypesIR.Add(Id, p);
        // updatePackageValues(Id, packageSizeP, quantityP, weightP, Name.text);

        //poner en posición
        nuevo.transform.rotation = new Quaternion(0f, 0f, 0f, 0f);
        nuevo.transform.localScale = new Vector3(1f, 1f, 1f);
        nuevo.transform.localPosition = new Vector3(pos.x, pos.y - height, pos.z);
        //añadir uno nuevo

        height += nuevo.GetComponent<RectTransform>().sizeDelta.y;

    }
    //--------------------------
    // wait opt use
    //-------------------------
    public void ReadData()
    {
        // Variables

        idPieza = new List<int>();
        vertices = new List<List<Vector3>>();
        List<Vector3> tempVertices = new List<Vector3>();

        // Leer datos
        string path = string.Format("{0}/StreamingAssets/{1}",
                UnityEngine.Application.dataPath, "prueba.txt");
        StreamReader reader = new StreamReader(path);
        bool primeraFila = true;
        while (!reader.EndOfStream)
        {
            string line = reader.ReadLine();
            if (primeraFila)
            {
                primeraFila = false;
                continue;
            }
            string[] values;
            if (line != null && line != "")
            {
                values = line.Split(';');
                if (idPieza.Count == 0)
                {
                    idPieza.Add(int.Parse(values[0]));
                }
                else if (idPieza[idPieza.Count - 1] != int.Parse(values[0]))
                {
                    idPieza.Add(int.Parse(values[0]));
                    vertices.Add(tempVertices);
                    tempVertices = new List<Vector3>();
                }
                tempVertices.Add(new Vector3(float.Parse(values[1]), float.Parse(values[2]), float.Parse(values[3])));
                print(tempVertices[tempVertices.Count - 1]);
                //                tempVertices.Add(new Vector3(float.Parse(values[1], CultureInfo.InvariantCulture), float.Parse(values[2], CultureInfo.InvariantCulture), float.Parse(values[3], CultureInfo.InvariantCulture)));
            }
           
        }
        vertices.Add(tempVertices);
        reader.Close();

        // Imprimir datos
        /*
        print("READ DATA\n");
        print("Number of pieces = " + idPieza.Count);
        print("row\tidPiece\tx\ty\tz");
        int fila = 0;
        String linea = "";
        for (int i = 0; i < vertices.Count; i++)
        {
            linea = fila + ": " + idPieza[i] + "\t";
            for (int j = 0; j < vertices[i].Count; j++)
                print(linea + vertices[i][j].x + "\t" + vertices[i][j].y + "\t" + vertices[i][j].z);
        }
        */
    }
    //----------------------------
    //Read csv file
    //----------------------------
    public void ReadCsv()
    {
        //delete dates
        reestablecer();
        //active CSV menu
        pm.menuCsv.SetActive(false);
        

        //Read the fileDlg
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
        string type = pth.file.Substring(pth.file.Length - 3);
 
        if (OpenFileDialog.GetOpenFileName(pth))
        {
            string filepath = pth.file;

            //Validate if its a csv file
            if (filepath.Substring(pth.file.Length - 3) == "csv")
            {
                textAssetData = new TextAsset(File.ReadAllText(filepath));
                string[] lines = textAssetData.text.Split(new string[] { "\n" }, StringSplitOptions.None);
                createUi(lines);

            }
            else
            {
                //alertext.text = "Por favor cargue un archivo tipo .csv";
                //alerta.SetActive(true);
            }
        }
    }
    public void createUi(string[] lines)
    {
        int line = 0;
        List<Vector3> tempVertices = new List<Vector3>();
        try
        {
            //read lines
            for (int i = 1; i < lines.Length; i++)
            {
                line = i;
                string[] split1 = lines[i].Split(';');
                //if not have id dosnt read
                if (!string.IsNullOrEmpty(split1[0]))
                {

                    //Cargar datos
                    //id
                    int packId = int.Parse(split1[0]);
                    //name
                    string name = split1[2];
                    //quantity
                    int quantity = int.Parse(split1[3]);
                    if (split1[4].Contains("."))
                    {
                        split1[4] = split1[4].Replace(".", ",");
                    }
                    //weight
                    float weight = float.Parse(split1[4]);
                    //rotation
                    bool b_rot = false;
                    int rotation = int.Parse(split1[5]);
                    if (rotation == 1)
                    {
                        b_rot = true;
                    }
                    else if (rotation == 0)
                    {
                        b_rot = false;
                    }
                    else if (rotation > 1)
                    {
                        throw new InvalidOperationException("Logfile cannot be read-only");
                    }
                    //Size
                    Vector3 packSize = new Vector3(int.Parse(split1[6]), int.Parse(split1[7]), int.Parse(split1[8]));
                    //Class of other object
                    int packClass = int.Parse(split1[1]);

                    newObject(packSize, packClass, quantity, weight, b_rot, packId, name);
                    //Read vertex for OTHER objects
                    if (packClass == 3)
                    {
                        //Restart
                        tempVertices.Clear();
                        bool run = true;
                        tempVertices.Add(new Vector3(int.Parse(split1[6]), int.Parse(split1[7]), int.Parse(split1[8])));
                        int contador = i + 1;

                        while (contador < lines.Length)
                        {
                            string[] split2 = lines[contador].Split(';');
                            tempVertices.Add(new Vector3(int.Parse(split2[6]), int.Parse(split2[7]), int.Parse(split2[8])));
                            contador++;
                            if (!string.IsNullOrEmpty(split2[0]) || contador == lines.Length - 1)
                            {
                                break;
                            }
                        }
                        Vector3[] vecMesh = tempVertices.ToArray();
                        pTypesIR[packId].verticesMesh = vecMesh;
                    }

                }

            }
        }

        catch (System.Exception)
        {
            pm.alertext.text = "El archivo .csv contiene un error en sus datos en la linea " + (line + 1);
            pm.alerta.SetActive(true);
            //pm.vaciar();
            //pm.reestablecer();

        }

    }    
    //--------------------------
    // wiat to use
    //-------------------------
    public void GenerateGameObjects()
    {
        myGameObjects = new List<GameObject>();
        for (int i = 0; i < idPieza.Count; i++) // tipos de pieza
        {
            List<int> triangulosPiezaMesh = new List<int>();
            float volumenPieza = 0f;
            // Determinar centroide de la pieza

            Vector3 c = new Vector3(0, 0, 0);
            int nV = vertices[i].Count;
            for (int k = 0; k < nV; k++)
                c += vertices[i][k];
            c /= nV;

            // Encontrar caras de la pieza

            List<Vector4> caras = new List<Vector4>();
            for (int k1 = 0; k1 < nV - 2; k1++) // vértice 1
            {
                Vector3 v1 = vertices[i][k1];
                for (int k2 = k1 + 1; k2 < nV - 1; k2++) // vértice 2
                {
                    Vector3 v2 = vertices[i][k2];
                    for (int k3 = k2 + 1; k3 < nV; k3++) // vértice 3
                    {
                        Vector3 v3 = vertices[i][k3];

                        // Se determina el plano de v1,v2,v3 y se lo orienta

                        Vector3 tempFace = productoCruz(v1, v2, v3);
                        tempFace = tempFace.normalized;
                        Vector4 face = new Vector4(tempFace.x, tempFace.y, tempFace.z, puntoCorte(tempFace, v1));
                        float d = distance(c, face);
                        if (d < 0) // El centro debe estar en el lado positivo del plano
                            face *= -1;

                        // Se determina si el plano es cara

                        bool esCara = true;
                        List<Vector3> verticesCara = new List<Vector3>();
                        verticesCara.Add(v1); verticesCara.Add(v2); verticesCara.Add(v3);
                        for (int k = 0; k < nV; k++)
                            if (k != k1 && k != k2 && k != k3)
                            {
                                Vector3 v = vertices[i][k];
                                d = distance(v, face);
                                if (Mathf.Abs(d) < error)
                                    verticesCara.Add(v);
                                else if (d < 0)
                                {
                                    esCara = false;
                                    break;
                                }
                            }
                        if (esCara)
                        {
                            // Se determina si la cara ya se consideró

                            bool yaExiste = false;
                            for (int k = 0; k < caras.Count; k++)
                                if (Mathf.Abs(caras[k].x - face.x) < error && Mathf.Abs(caras[k].y - face.y) < error && Mathf.Abs(caras[k].z - face.z) < error && Mathf.Abs(caras[k].w - face.w) < error)
                                {
                                    yaExiste = true;
                                    break;
                                }
                            if (!yaExiste)
                            {
                                caras.Add(face);

                                // Se organizan los puntos, de forma contigua

                                if (verticesCara.Count > 3)
                                {
                                    // Encontrar el plano cartesiano en el que mejor se proyecta la cara

                                    float anguloXY = Mathf.Acos(Mathf.Abs(face.z));
                                    float anguloXZ = Mathf.Acos(Mathf.Abs(face.y));
                                    float anguloYZ = Mathf.Acos(Mathf.Abs(face.x));
                                    int plano = 2; // Plano YZ
                                    if (Mathf.Abs(anguloXY - Mathf.PI / 2) >= Mathf.Abs(anguloXZ - Mathf.PI / 2) && Mathf.Abs(anguloXY - Mathf.PI / 2) >= Mathf.Abs(anguloYZ - Mathf.PI / 2))
                                        plano = 0;//Plano XY
                                    else if (Mathf.Abs(anguloXZ - Mathf.PI / 2) >= Mathf.Abs(anguloXY - Mathf.PI / 2) && Mathf.Abs(anguloXZ - Mathf.PI / 2) >= Mathf.Abs(anguloYZ - Mathf.PI / 2))
                                        plano = 1;//Plano XZ

                                    // Se determinan los bordes con la funciónD

                                    int nVC = verticesCara.Count;
                                    for (int n1 = 0; n1 < nVC - 1; n1++)
                                    {
                                        Vector3 a = verticesCara[n1];
                                        for (int n2 = n1 + 1; n2 < nVC; n2++)
                                        {
                                            Vector3 b = verticesCara[n2];
                                            bool formanBorde = true;
                                            for (int n3 = 0; n3 < nVC; n3++)
                                                if (n3 != n1 && n3 != n2)
                                                {
                                                    Vector3 r = verticesCara[n3];
                                                    float funD = 0f;
                                                    if (plano == 0) //XY
                                                        funD = (a.x - b.x) * (a.y - r.y) - (a.y - b.y) * (a.x - r.x);
                                                    else if (plano == 1) // XZ
                                                        funD = (a.x - b.x) * (a.z - r.z) - (a.z - b.z) * (a.x - r.x);
                                                    else // YZ
                                                        funD = (a.y - b.y) * (a.z - r.z) - (a.z - b.z) * (a.y - r.y);
                                                    if (funD < -error)
                                                    {
                                                        formanBorde = false;
                                                        break;
                                                    }
                                                }
                                            if (formanBorde && n2 != n1 + 1)
                                            {
                                                Vector3 temp = verticesCara[n1 + 1];
                                                verticesCara[n1 + 1] = verticesCara[n2];
                                                verticesCara[n2] = temp;
                                                break;
                                            }
                                        }
                                    }
                                }

                                // Volumen de la pieza y mesh

                                float altura = Mathf.Abs(distance(c, face));
                                Vector3 vec1 = verticesCara[0];
                                int ivec1 = indiceVec(vec1, vertices[i]);
                                for (int k = 0; k < verticesCara.Count - 2; k++)
                                {
                                    Vector3 vec2 = verticesCara[k + 1];
                                    Vector3 vec3 = verticesCara[k + 2];
                                    Vector3 tempArea = productoCruz(vec1, vec2, vec3);
                                    float area = tempArea.sqrMagnitude / 2;
                                    volumenPieza += area * altura / 3;
                                    int ivec2 = indiceVec(vec2, vertices[i]);
                                    int ivec3 = indiceVec(vec3, vertices[i]);
                                    triangulosPiezaMesh.Add(ivec1);
                                    triangulosPiezaMesh.Add(ivec2);
                                    triangulosPiezaMesh.Add(ivec3);
                                    triangulosPiezaMesh.Add(ivec1); // triángulos en 2 sentidos porque no si es clockwise o countercloackwise
                                    triangulosPiezaMesh.Add(ivec3);
                                    triangulosPiezaMesh.Add(ivec2);
                                }
                            }
                        }
                    }
                }
            }

            // Crear el gameObject

            GameObject go = new GameObject();
            go.name = "Piece" + i;
            Mesh mesh = go.AddComponent<MeshFilter>().mesh;
            Vector3[] verticesMesh = vertices[i].ToArray();
            int[] trianglesMesh = triangulosPiezaMesh.ToArray();
            go.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            mesh.vertices = verticesMesh;
            mesh.triangles = trianglesMesh;
            Renderer rend = go.AddComponent<MeshRenderer>();
            rend.material = new Material(Shader.Find("Standard"));
            Material mat = rend.material;
            mat.color = new Color(UnityEngine.Random.RandomRange(0f, 1f),
                UnityEngine.Random.RandomRange(0f, 1f),
                UnityEngine.Random.RandomRange(0f, 1f));
            mat.SetOverrideTag("RenderType", "Opaque");
            mat.SetFloat("_Glossiness", 0f);
            go.SetActive(true);
            
            myGameObjects.Add(go);
        }
    }

    //--------------------------
    // create the objects
    //-------------------------
    public void create()
    {
        //Animación cargue
        LoadBar.enabled = true;
        TimeLeft = 0;
        loading_bar = true;

        //Read each otype
        foreach (objectsType oType in pTypesIR.Values)
        {
            if (oType.classIR == 1)
            {
                Lx = oType.packageSize.x;
                Ly = oType.packageSize.y;
                Lz = oType.packageSize.z;
                saveVertex(oType.packageId, oType.classIR, verticesCubo());
            }
            else if (oType.classIR == 2)
            {
                Diametro = oType.packageSize.z;
                Altura = oType.packageSize.y;
                saveVertex(oType.packageId, oType.classIR, verticesCilindro());
            }
            else if (oType.classIR == 3)
            {
                List<Vector3> vertices2 = oType.verticesMesh.ToList();
                                
                saveVertex(oType.packageId, oType.classIR, vertices2 );
            }
        }
        runalgorim();
        pm.EstadoCargado();
    }
    //--------------------------
    // wiat to use
    //-------------------------
    public void ContinueWriting( int idPieza, int idTipo, List<Vector3> vertices)
    {
        string fileWrite = string.Format("{0}/StreamingAssets/{1}",
               UnityEngine.Application.dataPath, "prueba.txt");

        StreamWriter sw = new StreamWriter(fileWrite);
        sw.WriteLine("idPieza;idTipo;x;y;z");
        for (int i = 0; i < vertices.Count; i++)
        {
            sw.WriteLine(idPieza + ";"+
                System.Math.Round(vertices[i].x,2) + ";" +
                System.Math.Round(vertices[i].y,2) + ";" +
                System.Math.Round(vertices[i].z,2) + ";"+ idTipo );
        }
        sw.Close();
    }
    //--------------------------
    // Create the vertex
    //-------------------------
    public void saveVertex(int idPieza, int idTipo, List<Vector3> vertices1)
    {

        vertices= new List<List<Vector3>>();
             myGameObjects = new List<GameObject>();

            List<int> triangulosPiezaMesh = new List<int>();
            float volumenPieza = 0f;
            // Determinar centroide de la pieza

            Vector3 c = new Vector3(0, 0, 0);
            int nV = vertices1.Count;
            for (int k = 0; k < nV; k++)
                c += vertices1[k];
            c /= nV;

            // Encontrar caras de la pieza

            List<Vector4> caras = new List<Vector4>();
            for (int k1 = 0; k1 < nV - 2; k1++) // vértice 1
            {
                Vector3 v1 = vertices1[k1];
                for (int k2 = k1 + 1; k2 < nV - 1; k2++) // vértice 2
                {
                    Vector3 v2 = vertices1[k2];
                    for (int k3 = k2 + 1; k3 < nV; k3++) // vértice 3
                    {
                        Vector3 v3 = vertices1[k3];

                        // Se determina el plano de v1,v2,v3 y se lo orienta

                        Vector3 tempFace = productoCruz(v1, v2, v3);
                        tempFace = tempFace.normalized;
                        Vector4 face = new Vector4(tempFace.x, tempFace.y, tempFace.z, puntoCorte(tempFace, v1));
                        float d = distance(c, face);
                        if (d < 0) // El centro debe estar en el lado positivo del plano
                            face *= -1;

                        // Se determina si el plano es cara

                        bool esCara = true;
                        List<Vector3> verticesCara = new List<Vector3>();
                        verticesCara.Add(v1); verticesCara.Add(v2); verticesCara.Add(v3);
                        for (int k = 0; k < nV; k++)
                            if (k != k1 && k != k2 && k != k3)
                            {
                                Vector3 v = vertices1[k];
                                d = distance(v, face);
                                if (Mathf.Abs(d) < error)
                                    verticesCara.Add(v);
                                else if (d < 0)
                                {
                                    esCara = false;
                                    break;
                                }
                            }
                        if (esCara)
                        {
                            // Se determina si la cara ya se consideró

                            bool yaExiste = false;
                            for (int k = 0; k < caras.Count; k++)
                                if (Mathf.Abs(caras[k].x - face.x) < error && Mathf.Abs(caras[k].y - face.y) < error && Mathf.Abs(caras[k].z - face.z) < error && Mathf.Abs(caras[k].w - face.w) < error)
                                {
                                    yaExiste = true;
                                    break;
                                }
                            if (!yaExiste)
                            {
                                caras.Add(face);

                                // Se organizan los puntos, de forma contigua

                                if (verticesCara.Count > 3)
                                {
                                    // Encontrar el plano cartesiano en el que mejor se proyecta la cara

                                    float anguloXY = Mathf.Acos(Mathf.Abs(face.z));
                                    float anguloXZ = Mathf.Acos(Mathf.Abs(face.y));
                                    float anguloYZ = Mathf.Acos(Mathf.Abs(face.x));
                                    int plano = 2; // Plano YZ
                                    if (Mathf.Abs(anguloXY - Mathf.PI / 2) >= Mathf.Abs(anguloXZ - Mathf.PI / 2) && Mathf.Abs(anguloXY - Mathf.PI / 2) >= Mathf.Abs(anguloYZ - Mathf.PI / 2))
                                        plano = 0;//Plano XY
                                    else if (Mathf.Abs(anguloXZ - Mathf.PI / 2) >= Mathf.Abs(anguloXY - Mathf.PI / 2) && Mathf.Abs(anguloXZ - Mathf.PI / 2) >= Mathf.Abs(anguloYZ - Mathf.PI / 2))
                                        plano = 1;//Plano XZ

                                    // Se determinan los bordes con la funciónD

                                    int nVC = verticesCara.Count;
                                    for (int n1 = 0; n1 < nVC - 1; n1++)
                                    {
                                        Vector3 a = verticesCara[n1];
                                        for (int n2 = n1 + 1; n2 < nVC; n2++)
                                        {
                                            Vector3 b = verticesCara[n2];
                                            bool formanBorde = true;
                                            for (int n3 = 0; n3 < nVC; n3++)
                                                if (n3 != n1 && n3 != n2)
                                                {
                                                    Vector3 r = verticesCara[n3];
                                                    float funD = 0f;
                                                    if (plano == 0) //XY
                                                        funD = (a.x - b.x) * (a.y - r.y) - (a.y - b.y) * (a.x - r.x);
                                                    else if (plano == 1) // XZ
                                                        funD = (a.x - b.x) * (a.z - r.z) - (a.z - b.z) * (a.x - r.x);
                                                    else // YZ
                                                        funD = (a.y - b.y) * (a.z - r.z) - (a.z - b.z) * (a.y - r.y);
                                                    if (funD < -error)
                                                    {
                                                        formanBorde = false;
                                                        break;
                                                    }
                                                }
                                            if (formanBorde && n2 != n1 + 1)
                                            {
                                                Vector3 temp = verticesCara[n1 + 1];
                                                verticesCara[n1 + 1] = verticesCara[n2];
                                                verticesCara[n2] = temp;
                                                break;
                                            }
                                        }
                                    }
                                }

                                // Volumen de la pieza y mesh

                                float altura = Mathf.Abs(distance(c, face));
                                Vector3 vec1 = verticesCara[0];
                                int ivec1 = indiceVec(vec1, vertices1);
                                for (int k = 0; k < verticesCara.Count - 2; k++)
                                {
                                    Vector3 vec2 = verticesCara[k + 1];
                                    Vector3 vec3 = verticesCara[k + 2];
                                    Vector3 tempArea = productoCruz(vec1, vec2, vec3);
                                    float area = tempArea.sqrMagnitude / 2;
                                    volumenPieza += area * altura / 3;
                                    int ivec2 = indiceVec(vec2, vertices1);
                                    int ivec3 = indiceVec(vec3, vertices1);
                                    triangulosPiezaMesh.Add(ivec1);
                                    triangulosPiezaMesh.Add(ivec2);
                                    triangulosPiezaMesh.Add(ivec3);
                                    triangulosPiezaMesh.Add(ivec1); // triángulos en 2 sentidos porque no si es clockwise o countercloackwise
                                    triangulosPiezaMesh.Add(ivec3);
                                    triangulosPiezaMesh.Add(ivec2);
                                }
                            }
                        }
                    }
                }
            }

            // Crear el gameObject

            //GameObject go = new GameObject();

            //go.name = "Piece "+ idPieza;
            //Mesh mesh = go.AddComponent<MeshFilter>().mesh;
            Vector3[] verticesMesh = vertices1.ToArray();
            int[] trianglesMesh = triangulosPiezaMesh.ToArray();
            //go.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
            //mesh.vertices = verticesMesh;
            //mesh.triangles = trianglesMesh;
            //Renderer rend = go.AddComponent<MeshRenderer>();
            //rend.material = new Material(Shader.Find("Standard"));
          //  Material mat = rend.material;
        //mat.color = pm.packageColor[idPieza];
            //mat.SetOverrideTag("RenderType", "Opaque");
            //mat.SetFloat("_Glossiness", 0f);
          //  go.SetActive(true);
        //go.transform.SetParent(pm.parent);
     

        //Save 
        pTypesIR[idPieza].AddMesh(verticesMesh, trianglesMesh);
        //myGameObjects.Add(go);
        }

    //--------------------------
    // Create teh txt input Call the .exe and use view for read the output the iobjects
    //-------------------------
    public void runalgorim()
    {
        //initialitate values
        vaciarContenedor();
        int totalVertex=0;
        int counterVertex = 0;
        string msg="";
        string fileWrite = string.Format("{0}/StreamingAssets/OtherObjects/{1}",
            UnityEngine.Application.dataPath, "prueba.txt");

        //write the input values
        StreamWriter sw = new StreamWriter(fileWrite);
        //number of types
        sw.WriteLine(pTypesIR.Keys.Count);
        //rotations
        sw.WriteLine("64");
        //number of types
        sw.WriteLine(pTypesIR.Keys.Count);
        //Number of vertex
        foreach (objectsType oType in pTypesIR.Values)
        {
            totalVertex += oType.verticesMesh.Length;
        }
        sw.WriteLine(totalVertex);
        //Quantity of objects
        foreach (objectsType oType in pTypesIR.Values)
        {
            msg += oType.quantity + " ";
        }
        msg = msg.Remove(msg.Length - 1);
        sw.WriteLine(msg);
        msg = "";
        //rotations of each o   
        foreach (objectsType oType in pTypesIR.Values)
        { 
          if(oType.rotation)
            {
                sw.WriteLine("0 1 2 3 4 5 6 7 8 9 10 11 12 13 14 15 16 17 18 19 20 21 22 23 24 25 26 27 28 29 30 31 32 33 34 35 36 37 38 39 40 41 42 43 44 45 46 47 48 49 50 51 52 53 54 55 56 57 58 59 60 61 62 63");
            }
          else if (oType.classIR==2)
            {
                sw.WriteLine("16");
            }
            else
            {
                sw.WriteLine("0");
            }
         
        }

        //Rotaciones
        for(int rx=0; rx<=3; rx++)
        {
            for (int ry = 0; ry <= 3; ry++)
            {
                for (int rz = 0; rz <= 3; rz++)
                {
                    sw.WriteLine((rx*90)+" "+(ry*90)+" "+(rz*90));
                }
            }
        }
        foreach (objectsType oType in pTypesIR.Values)
        {
            sw.WriteLine(oType.packageId);
        }
        foreach (objectsType oType in pTypesIR.Values)
        {
           for(int i = 0; i <oType.verticesMesh.Length; i++)
            {
                msg += counterVertex + " ";
                counterVertex++;
            }
            msg = msg.Remove(msg.Length - 1);
            sw.WriteLine(msg);
            msg = "";
        }

        foreach (objectsType oType in pTypesIR.Values)
        {
            foreach (Vector3 tempVec in oType.verticesMesh)
            {
                sw.WriteLine(tempVec.x + " " + tempVec.y + " " + tempVec.z);
            }
        }
        sw.Close();


        //Start process
        ProcessStartInfo startInfo =process.StartInfo;

        startInfo.FileName = string.Format("{0}/StreamingAssets/OtherObjects/{1}",
        UnityEngine.Application.dataPath, "ProjectCplex.exe");
        startInfo.WorkingDirectory = string.Format("{0}/StreamingAssets/OtherObjects",
        UnityEngine.Application.dataPath);
        
        
        startInfo.Arguments = "-t 1000 -nThreads 4 -ins prueba -crit 1 -Lx 200 -Ly 200";
        process.StartInfo.WindowStyle = ProcessWindowStyle.Minimized;
        process.EnableRaisingEvents = true;
        process.Exited += new EventHandler(myProcess_Exited);
        // Starts the process
        process.Start();
        inProcess = true;
        bloking.SetActive(true);
    }
    public void runalgorim1 ()
    {
        inProcess = false;
        bloking.SetActive(false);
        view.fileName1 = string.Format("{0}/StreamingAssets/OtherObjects/{1}",
            UnityEngine.Application.dataPath, "prueba.txt");
        view.fileName2 = string.Format("{0}/StreamingAssets/OtherObjects/{1}",
            UnityEngine.Application.dataPath, "respuesta.txt");
        //process.WaitForExit(1000 * 60 * 5);    // Wait up to five minutes.
        view.ReadDataPiezas();
        view.ReadDataPos();
        view.Objetos();

    }
    private void myProcess_Exited(object sender, System.EventArgs e)
    {
        Console.WriteLine(
                          $"Exit time    : {process.ExitTime}\n" +
                          $"Exit code    : {process.ExitCode}\n");
    }
    //--------------------------
    // Delete the packages
    //----------------------------
    public void vaciarContenedor()
    {
        //Elimiar UIs del panel de objetos
        foreach (Transform child in pm.parent)
        {
            GameObject.Destroy(child.gameObject);
        }
    }
    //--------------------------
    // Restablece todo
    //----------------------------
    public void reestablecer()
    {
        vaciarContenedor();
        int idP = 0;
        //Conocer el numero de paqutes antes de borrar
        while (pTypesIR.ContainsKey(idP))
        {
            idP++;
        }
        //Elimiar UIs del panel de objetos
        foreach (Transform child in Lista)
        {
            GameObject.Destroy(child.gameObject);
        }

        pTypesIR.Clear();
        
        /*reinicar cantidades
        for (int j = 0; j < array_quantity.Length; j++)
        {

            array_quantity[j] = 0;
        }*/
        //Resize the list of objects
        Lista.sizeDelta = new Vector2(Lista.sizeDelta.x, 370.4893f);
    }

    //--------------------------
    //SAVE
    //--------------------------
    public void Save()
    {
        if (!activeFuntion)
        {
            activeFuntion = true;

            ArePackage();


            //arachivos de origen
            string[] file1 = File.ReadAllLines(string.Format("{0}/StreamingAssets/OtherObjects/{1}",
                UnityEngine.Application.dataPath, "prueba.txt"));
            string[] file2 = File.ReadAllLines(string.Format("{0}/StreamingAssets/OtherObjects/{1}",
             UnityEngine.Application.dataPath, "respuesta.txt"));
            string path = string.Format("{0}/StreamingAssets/OtherObjects/Save/{1}{2}",
             UnityEngine.Application.dataPath,pm.Titulo.text, ".txt");
            File.Delete(path);
            //saveUI
            using (StreamWriter writer = new StreamWriter(path, true))
            {
                writer.WriteLine(pm.containerSize.x + " " + pm.containerSize.y + " " + pm.containerSize.z);
                writer.WriteLine("IdPieza; Tipo; nombre; Cantidad; Peso; Rotación; x; y; z");
                int rot = 0;
                string msg = "";
                foreach( objectsType oType in pTypesIR.Values)
                {
                    if(oType.rotation) { rot = 1; } else { rot = 0; }
                    msg+= oType.packageId+";"+oType.classIR + ";" +
                        oType.Name1+";" +oType.quantity + ";" +
                        oType.weight + ";" +rot +";";
                    //write vectors
                    if(oType.classIR==3)
                    {
                        foreach(Vector3 v in oType.verticesMesh)
                        {
                            msg += v.x+";"+ v.y+";"+v.z;
                            msg += "\n";
                            msg += ";;;;;;";
                        }
                        msg = msg.Remove(msg.Length - 6);

                    }
                    else 
                    {
                        msg += oType.packageSize.x + ";" + oType.packageSize.y + ";" + oType.packageSize.z;
                        msg += "\n";
                    }
                }
                writer.Write(msg);
              
                writer.Close();
            }

            
            //Create file axiliar with the names
            using (StreamWriter writer = new StreamWriter(path, true))
            {
                writer.WriteLine("\t");
               foreach (String line in file1)
                {
                    writer.WriteLine(line);
                }
                writer.WriteLine("\t");
                foreach (String line in file2)
                {
                    writer.WriteLine(line);
                }
                writer.Close();
            }


            pm.Nombre_Registro.text = pm.Titulo.text;
            pm.Fecha_Registro.text = "" + DateTime.Now;
            pm.tamaño_Contenedor.text = "(" + pm.containerTransform.localScale.x + "m x" + pm.containerTransform.localScale.y + "m x" + pm.containerTransform.localScale.z + "m )";
            txtManagerIR.precargar();
            pm.saveMsg.SetActive(true);
            activeFuntion = false;
        }
    }

    public void readtxt(Text path)
    {
        //delete dates
        reestablecer();
        //Change the tap selected
        pm.tapGroup.OnTabSelected(pm.CargaTap);
        //Read register
        string[] lines = File.ReadAllLines(string.Format("{0}/StreamingAssets/OtherObjects/Save/{1}",
                UnityEngine.Application.dataPath, path.text));
        string ui = string.Format("{0}/StreamingAssets/OtherObjects/{1}",
            UnityEngine.Application.dataPath, "ui_aux.txt");
        string input = string.Format("{0}/StreamingAssets/OtherObjects/{1}",
            UnityEngine.Application.dataPath, "input_aux.txt");
        string output = string.Format("{0}/StreamingAssets/OtherObjects/{1}",
            UnityEngine.Application.dataPath, "output_aux.txt");
        File.Delete(input);
        File.Delete(ui);
        File.Delete(output);

        //Determinate limits
        int limit1 = 0;
        int limit2 = 0;
        for (int i = 0; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i]) && limit1 == 0)
            {
                limit1 = i;
            }
            else if (string.IsNullOrWhiteSpace(lines[i]))
            {
                limit2 = i;
                break;
            }
        }
        print("l1 " + limit1 + " l2 " + limit2);
        using (StreamWriter writer = new StreamWriter(ui, true))
        {
            for (int i = 1; i < limit1; i++)
            {
                writer.WriteLine(lines[i]);
            }
            writer.Close();
        }
        using (StreamWriter writer = new StreamWriter(input, true))
        {
            for (int i = limit1 + 1; i < limit2; i++)
            {
                writer.WriteLine(lines[i]);
            }
            writer.Close();
        }
        using (StreamWriter writer = new StreamWriter(output, true))
        {
            for (int i = limit2 + 1; i < lines.Length; i++)
            {
                writer.WriteLine(lines[i]);
            }
            writer.Close();
        }
        view.fileName1 = input;
        view.fileName2 = output;
        string[] lines1 = System.IO.File.ReadAllLines(ui);
        createUi(lines1);
        pm.Titulo.text = path.text.Substring(0, path.text.IndexOf("."));
        //process.WaitForExit(1000 * 60 * 5);    // Wait up to five minutes.
        view.ReadDataPiezas();
        view.ReadDataPos();
        view.Objetos();

    }
    public void ArePackage()
    {
        if (pm.parent.childCount == 0)
        {
            pm.NoPackage.SetActive(true);

            //throw new NullReferenceException("Exception Message");
            activeFuntion = false;
            throw new NullReferenceException("Exception Message");
        }

    }

    public void killProcess()
    {
        inProcess = false;
        process.Kill();
        bloking.SetActive(false);
    }
}




