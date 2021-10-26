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

public class View : MonoBehaviour
{
    // VARIABLES GLOBALES

    public string fileName1 = "";
    public string fileName2 = "";
    private float error = 0.0001f;
    public PackageManajer pm;
    public IrregularManager ir;

    // 1

    // de las piezas

    private int nPi; //número de piezas
    private int nPa; //número de partes
    private List<List<int>> Pa; //partes que forman a cada tipo de pieza
    private List<List<int>> VPa; //vertices que forman a cada parte
    private List<Vector3> V; //vértices x, y, z

    // de las posiciones

    private Vector3 escalaCubo; // escala del cubo x, y, z
    private List<int> idPi; // id de cada pieza que se debe graficar
    private List<int> sePone; // Indica si la pieza va dentro o no del contenedor
    private List<Vector3> posPi; // posición de cada pieza
    private List<Quaternion> rotPi; // rotación de la pieza

    // FUNCIONES
    public void ReadDataPiezas()
    {
        // Inicializar

        int nR = 0; //número de rotaciones
        Pa = new List<List<int>>();
        VPa = new List<List<int>>();
        V = new List<Vector3>();

        // Variables

        int fila = 0;
        List<int> tempListInt;

        // Lectura de los datos
        //StreamReader reader = new StreamReader(fileName1);
        StreamReader reader = new StreamReader(fileName1);
        while (!reader.EndOfStream)
        {
            string line = reader.ReadLine();
            string[] values;
            tempListInt = new List<int>();
            if (line != null && line != "")
            {
                values = line.Split(' ');
                if (fila == 0) nPi = int.Parse(values[0]);
                else if (fila == 1) nR = int.Parse(values[0]);
                else if (fila == 2) nPa = int.Parse(values[0]);
                else if (fila >= 5 + nPi + nR && fila < 5 + nPi + nR + nPi + nPa) for (int i = 0; i < values.Length; i++) tempListInt.Add(int.Parse(values[i]));
                else if (fila >= 5 + nPi + nR) V.Add(new Vector3(float.Parse(values[0]), float.Parse(values[1]), float.Parse(values[2])));
            }
            if (fila >= 5 + nPi + nR)
            {
                if (fila < 5 + nPi + nR + nPi) Pa.Add(tempListInt);
                else if (fila < 5 + nPi + nR + nPi + nPa) VPa.Add(tempListInt);
            }
            fila++;
        }
        reader.Close();

        // Imprimir la lectura de datos
        /*
        print("Tipos de piezas = " + nPi);
        print("Número de rotaciones = " + nR);
        print("Cantidad de partes = " + nPa);
        string tempString = "";
        print("Pieza: índices de partes");
        for (int i = 0; i < nPi; i++)
        {
            tempString = i + ":\t";
            for (int j = 0; j < Pa[i].Count; j++) tempString += Pa[i][j] + "\t";
            print(tempString);
        }
        print("Parte: índices de vértices");
        for (int i = 0; i < nPa; i++)
        {
            tempString = i + ":\t";
            for (int j = 0; j < VPa[i].Count; j++) tempString += VPa[i][j] + "\t";
            print(tempString);
        }
        print("Vértice:\tx\ty\tz");
        for (int i = 0; i < V.Count; i++) print(i + ":\t" + V[i].x + "\t" + V[i].y + "\t" + V[i].z);
        */
    }

    // de las posiciones
    public void ReadDataPos()
    {
        // Inicializar

        idPi = new List<int>();
        sePone = new List<int>();
        posPi = new List<Vector3>();
        rotPi = new List<Quaternion>();

        // Variables

        int fila = 0;

        // Lectura de los datos
        //StreamReader reader = new StreamReader(fileName2);
        StreamReader reader = new StreamReader(fileName2);

        //StreamReader reader = new StreamReader(System.IO.Directory.GetParent(Application.dataPath) + "./Assets/Resources/Texto/" + fileName2);
        while (!reader.EndOfStream)
        {
            string line = reader.ReadLine();
            string[] values;
            float factor = 0.5f;
            if (line != null && line != "")
            {
                values = line.Split('\t');
                if (fila == 2) escalaCubo = new Vector3(float.Parse(values[0], CultureInfo.InvariantCulture) + factor, float.Parse(values[1], CultureInfo.InvariantCulture) + factor, float.Parse(values[2], CultureInfo.InvariantCulture) + factor);
                else if (fila >= 4)
                {
                    idPi.Add(int.Parse(values[1]));
                    sePone.Add(int.Parse(values[2]));
                    posPi.Add(new Vector3(float.Parse(values[3], CultureInfo.InvariantCulture), float.Parse(values[4], CultureInfo.InvariantCulture), float.Parse(values[5], CultureInfo.InvariantCulture)));
                    rotPi.Add(new Quaternion(float.Parse(values[7], CultureInfo.InvariantCulture), float.Parse(values[8], CultureInfo.InvariantCulture), float.Parse(values[9], CultureInfo.InvariantCulture), float.Parse(values[6], CultureInfo.InvariantCulture)));
                }
            }
            fila++;
        }
        reader.Close();

        // Imprimir la lectura de los datos
        /*
        print("Dimensiones del cubo: " + escalaCubo.x + "\t" + escalaCubo.y + "\t" + escalaCubo.z);
        print("Piezas que se grafican: posx\tposy\tposz\tqw\tqx\tqy\tqz");
        String tempString = "";
        for (int i = 0; i < idPi.Count; i++) { tempString = idPi[i] + ":\t" + posPi[i].x + "\t" + posPi[i].y + "\t" + posPi[i].z + "\t" + rotPi[i].w + "\t" + rotPi[i].x + "\t" + rotPi[i].y + "\t" + rotPi[i].z; print(tempString); }
        */
    }

    
    // 2 Crear objetos y darles un mesh
     public void Objetos()
    {
        float weight = 0;
        float posAx = escalaCubo.x * 2;
        for (int i = 0; i < nPi; i++)
        {
            List<int> verticesPieza = new List<int>();
            List<Vector3> verticesPiezaMesh = new List<Vector3>();
            List<int> triangulosPiezaMesh = new List<int>();
            for (int j = 0; j < Pa[i].Count; j++)
            {
                int numV = VPa[Pa[i][j]].Count;
                List<List<float>> listaPlanos = new List<List<float>>();
                Vector3 c = new Vector3(0.0f, 0.0f, 0.0f);
                for (int k = 0; k < numV; k++)
                {
                    int v = VPa[Pa[i][j]][k];
                    verticesPieza.Add(v);
                    c.x += V[v].x; c.y += V[v].y; c.z += V[v].z;
                }
                c.x /= numV; c.y /= numV; c.z /= numV;
                for (int k1 = 0; k1 < numV - 2; k1++)
                {
                    int v1 = VPa[Pa[i][j]][k1];
                    for (int k2 = k1 + 1; k2 < numV - 1; k2++)
                    {
                        int v2 = VPa[Pa[i][j]][k2];
                        for (int k3 = k2 + 1; k3 < numV; k3++)
                        {
                            int v3 = VPa[Pa[i][j]][k3];

                            // Se determina si el plano que forma v1, v2 y v3 es una cara de la parte

                            Vector3 p12 = new Vector3(V[v2].x - V[v1].x, V[v2].y - V[v1].y, V[v2].z - V[v1].z);
                            Vector3 p13 = new Vector3(V[v3].x - V[v1].x, V[v3].y - V[v1].y, V[v3].z - V[v1].z);
                            Vector3 face = new Vector3(p12.y * p13.z - p12.z * p13.y, -p12.x * p13.z + p12.z * p13.x, p12.x * p13.y - p12.y * p13.x);
                            face.Normalize();
                            float faceD = -face.x * V[v1].x - face.y * V[v1].y - face.z * V[v1].z;
                            float d = c.x * face.x + c.y * face.y + c.z * face.z + faceD;
                            if (d < 0) // El centro está en la parte positiva del plano
                            {
                                face.x = -face.x;
                                face.y = -face.y;
                                face.z = -face.z;
                                faceD = -faceD;
                            }

                            // Se determina si el plano es cara

                            bool esCara = true;
                            List<int> puntosCara = new List<int>();
                            puntosCara.Add(v1); puntosCara.Add(v2); puntosCara.Add(v3);
                            for (int k = 0; k < numV; k++)
                                if (k != k1 && k != k2 && k != k3)
                                {
                                    int v = VPa[Pa[i][j]][k];
                                    d = V[v].x * face.x + V[v].y * face.y + V[v].z * face.z + faceD;
                                    if (Mathf.Abs(d) < error) puntosCara.Add(v);
                                    else if (d < error)
                                    {
                                        esCara = false;
                                        break;
                                    }
                                }
                            if (esCara)
                            {
                                // Se determina si el plano ya existe

                                bool yaExiste = false;
                                for (int n = 0; n < listaPlanos.Count; n++)
                                    if (Mathf.Abs(listaPlanos[n][0] - face.x) < error && Mathf.Abs(listaPlanos[n][1] - face.y) < error && Mathf.Abs(listaPlanos[n][2] - face.z) < error && Mathf.Abs(listaPlanos[n][3] - faceD) < error)
                                    {
                                        yaExiste = true; break;
                                    }
                                if (!yaExiste)
                                {
                                    List<float> tempListaPlanos = new List<float>();
                                    tempListaPlanos.Add(face.x); tempListaPlanos.Add(face.y); tempListaPlanos.Add(face.z); tempListaPlanos.Add(faceD);
                                    listaPlanos.Add(tempListaPlanos);

                                    // Se organizan los puntos, de forma contigua

                                    if (puntosCara.Count > 3)
                                    {
                                        // Encontrar el plano en que se proyecta la cara

                                        float anguloXY = Mathf.Acos(Mathf.Abs(face.z));
                                        float anguloXZ = Mathf.Acos(Mathf.Abs(face.y));
                                        float anguloYZ = Mathf.Acos(Mathf.Abs(face.x));
                                        int plano = 2; // Plano YZ
                                        if (Mathf.Abs(anguloXY - Mathf.PI / 2) >= Mathf.Abs(anguloXZ - Mathf.PI / 2) && Mathf.Abs(anguloXY - Mathf.PI / 2) >= Mathf.Abs(anguloYZ - Mathf.PI / 2))
                                            plano = 0;//Plano XY
                                        else if (Mathf.Abs(anguloXZ - Mathf.PI / 2) >= Mathf.Abs(anguloXY - Mathf.PI / 2) && Mathf.Abs(anguloXZ - Mathf.PI / 2) >= Mathf.Abs(anguloYZ - Mathf.PI / 2))
                                            plano = 1;//Plano XZ

                                        // Se usa la función D para determinar el vértice que forma una línea en la que todos los otros puntos están de un lado de la línea

                                        int pos = 0;
                                        for (int ñ = 0; ñ < puntosCara.Count - 1; ñ++)
                                        {
                                            int a = puntosCara[ñ];
                                            pos++;
                                            for (int n = ñ + 1; n < puntosCara.Count; n++)
                                            {
                                                int b = puntosCara[n];
                                                bool estaBien = true;
                                                for (int m = 0; m < puntosCara.Count; m++)
                                                {
                                                    if (m != ñ && m != n)
                                                    {
                                                        int r = puntosCara[m];
                                                        float funD = 0.0f;
                                                        if (plano == 0)//XY
                                                            funD = (V[a].x - V[b].x) * (V[a].y - V[r].y) - (V[a].y - V[b].y) * (V[a].x - V[r].x);
                                                        else if (plano == 1)//XZ
                                                            funD = (V[a].x - V[b].x) * (V[a].z - V[r].z) - (V[a].z - V[b].z) * (V[a].x - V[r].x);
                                                        else//YZ
                                                            funD = (V[a].y - V[b].y) * (V[a].z - V[r].z) - (V[a].z - V[b].z) * (V[a].y - V[r].y);
                                                        if (funD < -error)
                                                        {
                                                            estaBien = false;
                                                            break;
                                                        }
                                                    }
                                                }
                                                if (estaBien && pos != n)
                                                {
                                                    int temp = puntosCara[pos];
                                                    puntosCara[pos] = b;
                                                    puntosCara[n] = temp;
                                                }
                                            }
                                        }
                                    }

                                    // Imprimir las caras

                                    int p1 = puntosCara[0];
                                    int vert1 = -1;
                                    for (int m = 0; m < verticesPieza.Count; m++)
                                        if (vert1 == -1 && p1 == verticesPieza[m])
                                        {
                                            vert1 = m;
                                            break;
                                        }

                                    for (int n = 0; n < puntosCara.Count - 2; n++)
                                    {
                                        int p2 = puntosCara[n + 1];
                                        int p3 = puntosCara[n + 2];
                                        int vert2 = -1;
                                        int vert3 = -1;
                                        for (int m = 0; m < verticesPieza.Count; m++)
                                            if (vert2 == -1 && p2 == verticesPieza[m]) vert2 = m;
                                            else if (vert3 == -1 && p3 == verticesPieza[m]) vert3 = m;
                                            else if (vert2 != -1 && vert3 != -1) break;
                                        triangulosPiezaMesh.Add(vert1);
                                        triangulosPiezaMesh.Add(vert2);
                                        triangulosPiezaMesh.Add(vert3);
                                        triangulosPiezaMesh.Add(vert1);
                                        triangulosPiezaMesh.Add(vert3);
                                        triangulosPiezaMesh.Add(vert2);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            // Asignar vértices y triángulos al mesh

            for (int ii = 0; ii < idPi.Count; ii++)
            {
                if (idPi[ii] == i && sePone[ii] == 1)
                {
                    for (int j = 0; j < verticesPieza.Count; j++)
                        verticesPiezaMesh.Add(V[verticesPieza[j]]);

                    print("los indicadores"+idPi[ii]+"");
                    weight += ir.pTypesIR[idPi[ii]].weight;
                    GameObject piece = new GameObject();
                    piece.name = "Piece" + i;
                    Mesh mesh = piece.AddComponent<MeshFilter>().mesh;
                    Vector3[] vertices = verticesPiezaMesh.ToArray(); ;
                    int[] triangles = triangulosPiezaMesh.ToArray();
                    mesh.vertices = vertices;
                    mesh.triangles = triangles;
                    Renderer rend = piece.AddComponent<MeshRenderer>();
                    rend.material = new Material(Shader.Find("Standard"));
                    piece.transform.SetParent(pm.parent);
                    Material mat = rend.material;
                    mat.SetFloat("_Mode", 3);
                    mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    mat.SetInt("_ZWrite", 0);
                    mat.DisableKeyword("_ALPHATEST_ON");
                    mat.DisableKeyword("_ALPHABLEND_ON");
                    mat.EnableKeyword("_ALPHAPREMULTIPLY_ON");
                    mat.renderQueue = 3000;
                    //mat.SetOverrideTag("RenderType", "Transparent");
                    //mat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
                   Color ColorT= pm.packageColor[idPi[ii]];
                    ColorT.a = 0.50f;
                    mat.color = ColorT;

                    piece.AddComponent<MeshCollider>();
                    piece.GetComponent<MeshCollider>().convex=true;
                    piece.AddComponent<Rigidbody>();
                    piece.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;

                    // Posición y escala

                    Transform TPiece = piece.GetComponent<Transform>();
                    TPiece.localPosition = posPi[ii]/100;
                    TPiece.localScale = new Vector3(0.01f, 0.01f, 0.01f);
                    TPiece.rotation = rotPi[ii];
                }
            }
        }
        pm.label_peso.text = weight+"";

        // Posición y escala del cubo
   }
    void run()
    {
        UnityEngine.Random.seed = 123;
        //string fileName1 = fileName + ".txt";
        string fileName2 = "Res" + fileName1;
        ReadDataPiezas();
        ReadDataPos();
        Objetos();
    }
}
