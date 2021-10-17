using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class RecordManager : MonoBehaviour
{
    //----------------------------------
    // VARIABLES
    //----------------------------------
    public Text nombre;
    public GameObject registro;
    public TxtManager txtManager;
    public RectTransform tablaRegistros;
    //----------------------------------
    //METHODS
    //----------------------------------

    //Delete specefici record
    public void deleteRecord()
    {
        string namePdf;
        namePdf = nombre.text.Remove(nombre.text.Length - 4);

        //Txt file
        string deletefileTxt = string.Format("{0}/StreamingAssets/txt/{1}",
                  UnityEngine.Application.dataPath, nombre.text);
        //PDF file
        string deletefilePdf = string.Format("{0}/StreamingAssets/pdf/{1}.pdf",
                  UnityEngine.Application.dataPath, namePdf);
        
        //Validate if exist
        if (File.Exists(deletefileTxt))
        {
            File.Delete(deletefileTxt);
        }
        if (File.Exists(deletefilePdf))
        {
            File.Delete(deletefilePdf);
        }
        //Adjust the size of the list
        if(txtManager.txt_quantity>12)
        {
            tablaRegistros.sizeDelta = new Vector2(tablaRegistros.sizeDelta.x, tablaRegistros.sizeDelta.y - 30f);
            txtManager.txt_quantity--;
        }
        GameObject.Destroy(registro);
    }
    //Open the PDF file of record
    public void printPdf()
    {
        string name = nombre.text.Remove(nombre.text.Length - 4);
        string printFile = string.Format("{0}/StreamingAssets/pdf/{1}.pdf",
                  UnityEngine.Application.dataPath, name);
        if (printFile == null)
            return;

        if (File.Exists(printFile))
        {
            Debug.Log("file found");
        }
        else
        {
            Debug.Log("file not found");
            return;
        }
        System.Diagnostics.Process process = new System.Diagnostics.Process();
        process.StartInfo.CreateNoWindow = true;
        process.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal;
        process.StartInfo.UseShellExecute = true;
        process.StartInfo.FileName = printFile;

        process.Start();
    }
}
