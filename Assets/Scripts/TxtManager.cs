using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System;

public class TxtManager : MonoBehaviour
{
    public string path;
    public string pathPdf;
    public GameObject registro;
    public RectTransform tablaRegistros;
    public Text nombre;
    public Text fecha;
    public Text container;
    public GameObject imprimir;
    public int txt_quantity;
    public RecordContainerManager recordContainerManager;
    private GameObject[] registros;
    // Start is called before the first frame update
    void Start()
    {
        path= Application.dataPath + "/StreamingAssets/txt/";
        pathPdf= Application.dataPath + "/StreamingAssets/pdf/";
       
        recordContainerManager.createRecords();
        precargar();
    }
    public void pb()
    {
        precargar();
        recordContainerManager.createRecords();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void precargar()

    {

        txt_quantity = 0;
        // Restar size of the table
        tablaRegistros.sizeDelta = new Vector2(tablaRegistros.sizeDelta.x, 360f);
        //Delete all records
        Debug.Log("Eliminados Regsitros");
        eliminarRegistros();

        //Read the files in the Directory
        string[] dirs = Directory.GetFiles(path, "*.txt");
        txt_quantity = dirs.Length;
        string[] dirs1 = Directory.GetFiles(pathPdf, "*.pdf");
        int cantidad_pdf = dirs1.Length;
        for (int i = 0; i < txt_quantity; i++)
        {
            //Read the lines
            
            string[] lines = System.IO.File.ReadAllLines(dirs[i]);
            string[] split1 = lines[0].Split('\t');
            Vector3 tamano = new Vector3(float.Parse(split1[2]) / 100f, float.Parse(split1[3]) / 100f, float.Parse(split1[4]) / 100f);
            container.text = "(" + tamano.x + " m, " + tamano.y + " m, " + tamano.z + " m)";
            nombre.text = new DirectoryInfo(dirs[i]).Name;
            fecha.text = "" + new DirectoryInfo(dirs[i]).LastWriteTime;
            //Identify a .pdf with the same name
            for (int j = 0; j < cantidad_pdf; j++)
            {
                string namePdf;
                namePdf = new DirectoryInfo(dirs1[j]).Name;
                namePdf = namePdf.Remove(namePdf.Length - 4);
                string nameTxt = nombre.text.Remove(nombre.text.Length - 4);

                if (nameTxt == namePdf)
                {
                    imprimir.SetActive(true);
                    break;
                }
                else
                {
                    imprimir.SetActive(false);
                }


            }
            //Create Instan of the Record Model
            GameObject newRegistro = GameObject.Instantiate(registro);
            newRegistro.SetActive(true);
            newRegistro.tag = "Registro";
            newRegistro.transform.SetParent(tablaRegistros);
            newRegistro.transform.localRotation = Quaternion.identity;
            newRegistro.transform.localScale = new Vector3(1f, 1f, 1f);
            newRegistro.transform.localPosition = new Vector3(newRegistro.transform.localPosition.x, newRegistro.transform.localPosition.y, 0f);
            
           
                //then of the 11th record add space to the table
                if (i > 11)
            {
                tablaRegistros.sizeDelta = new Vector2(tablaRegistros.sizeDelta.x, tablaRegistros.sizeDelta.y + 30f);
            }
        }
        
    }
   
    public void eliminarRegistros()
    {
        foreach (Transform child in tablaRegistros)
        {
            GameObject.Destroy(child.gameObject);
        }

    }

   
}
