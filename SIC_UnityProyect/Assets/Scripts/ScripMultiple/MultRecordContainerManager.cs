using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class MultRecordContainerManager : MonoBehaviour
{  //------------------------
   //VARIABLES
   //------------------------

 
    //txt with the record
    private string path;
    //Table
    public RectTransform ContainerRecordTable;
    //Panel of the containers
    public RectTransform panelContainer;
    //Model of record
    public GameObject record;
    public GameObject admRecords;
    //Inputs of the container
    public Text ContainerName;
    public Text lenghtLabel;
    public Text highLabel;
    public Text widthLabel;
    public Text quantityLabel;
    public Text weightLabel;
    public Text priceLabel;

    public MultUIContainer uIContainer;
    //If active the container
    public Toggle incluir;
    //ScrollVar
    public Scrollbar _sRect;

    public int numContainer;
    private int toggleActive;
    //Edit Menu
    public GameObject editMenu;
    public Text titleName;
    public InputField lenghtIF;
    public InputField heightIF;
    public InputField widthIF;
    public InputField quantityIF;
    public InputField weightIF;
    public InputField priceIF;
    public Toggle incluirTgl;
    public GameObject MsgError;
    //------------------------
    //METHODS
    //------------------------
    void Start()
    {
        path = Application.dataPath + "/StreamingAssets/MultipleContainer/Containers/";
      
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Create all the records
    public void createRecords()
    {

        numContainer = 0;
        path = Application.dataPath + "/StreamingAssets/MultipleContainer/Containers/";


        //Delete all the records
        DeleteRecords();
        //EmptyContainersArray
        uIContainer.containerArray.Clear();
        //Read all the txt files
        string[] dirs = Directory.GetFiles(path, "*.txt");
        int cantidad_txt = dirs.Length;

       // if (cantidad_txt > 0)
       //{


            for (int i = 0; i < dirs.Length; i++)
            {
                //
                string nameTxt = new DirectoryInfo(dirs[i]).Name;
                nameTxt = nameTxt.Remove(nameTxt.Length - 4);
                ContainerName.text = nameTxt;

                string[] lines = System.IO.File.ReadAllLines(dirs[i]);
                incluir.isOn = false;
                string[] split = lines[0].Split('\t');

                //bring the continer info
                lenghtLabel.text = split[1];
                highLabel.text = split[2];
                widthLabel.text = split[3];
            quantityLabel.text = split[5];
            weightLabel.text = split[6];
            priceLabel.text = split[7];


            //Evaluate the toggle and create the container
            if (int.Parse(split[4]) == 1)
                {
                    incluir.isOn = true;
                    uIContainer.saveNewCustomCanvasValues(ContainerName.text, lenghtLabel.text, highLabel.text, widthLabel.text,quantityLabel.text,weightLabel.text,priceLabel.text);
                }
                else
                {
                    incluir.isOn = false;
                }
                //crate the new record 
                GameObject newRecord = GameObject.Instantiate(record);
                newRecord.SetActive(true);
                newRecord.transform.SetParent(ContainerRecordTable);
                newRecord.transform.localRotation = Quaternion.identity;
                newRecord.transform.localScale = new Vector3(1f, 1f, 1f);
                newRecord.transform.localPosition = new Vector3(newRecord.transform.localPosition.x, newRecord.transform.localPosition.y, 0f);
                numContainer++;

                //Add space to the table
                if (numContainer > 12)
                {
                    ContainerRecordTable.sizeDelta = new Vector2(ContainerRecordTable.sizeDelta.x, ContainerRecordTable.sizeDelta.y + 30f);
                }
                _sRect.value=1;
            }
      //  }
    }
   //Create a unique record
    public void newContainerRecord(string name, string lenght, string height, string width, string quantity, string weight, string price)
    {
      
        string destFile = string.Format("{0}/StreamingAssets/MultipleContainer/Containers/{1}.txt",
               UnityEngine.Application.dataPath, name);
        int counter=0;

        //Evaluate if exist other container with the same name
        while(File.Exists(destFile))
        {
            counter++;
            destFile = string.Format("{0}/StreamingAssets/MultipleContainer/Containers/{1}_{2}.txt",
                UnityEngine.Application.dataPath, name,counter);
        }

        if (counter > 0) { ContainerName.text = name+"_"+counter; } else { ContainerName.text = name; }

        //give the dates to the record
        lenghtLabel.text = lenght;
        highLabel.text = height;
        widthLabel.text = width;
        quantityLabel.text = quantity;
        weightLabel.text = weight;
        priceLabel.text = price;
        incluir.isOn= true;
        GameObject newRecord = GameObject.Instantiate(record);
        newRecord.SetActive(true);
        newRecord.transform.SetParent(ContainerRecordTable);
        newRecord.transform.localRotation = Quaternion.identity;
        newRecord.transform.localScale = new Vector3(1f, 1f, 1f);
        newRecord.transform.localPosition = new Vector3(newRecord.transform.localPosition.x, newRecord.transform.localPosition.y, 0f);
        numContainer++;

        //add space
        if (numContainer > 12)
        {
            ContainerRecordTable.sizeDelta = new Vector2(ContainerRecordTable.sizeDelta.x, ContainerRecordTable.sizeDelta.y + 30f);
        }
        //create the .txt file
        using (StreamWriter writer = new StreamWriter(destFile, true))
        {
            {
                string output =  name + "\t" + lenght + "\t" + height + "\t" + width + "\t" + "1" + "\t" +quantity + "\t" +weight + "\t" +price;
                writer.Write(output);
            }
            writer.Close();
        }
        
        
    }
   //Evalate the toggle of the conteiner record
    public void activeContainer(bool incluir)
    {
       
        if (record.name.Contains("(Clone)"))
        {
            string destFile = string.Format("{0}/StreamingAssets/MultipleContainer/Containers/{1}.txt",
              UnityEngine.Application.dataPath, ContainerName.text);
         
            if (File.Exists(destFile))
            {
                File.Delete(destFile);
            }
            //create the UI container in the list in principal window
            if (incluir)
            {
                using (StreamWriter writer = new StreamWriter(destFile, true))
                {
                    {
                        string output = ContainerName.text + "\t" + lenghtLabel.text + "\t" + highLabel.text + "\t" + widthLabel.text + "\t" + "1" + "\t" + quantityLabel.text + "\t"+ weightLabel.text + "\t" +priceLabel.text;
                        writer.Write(output);
                    }
                    writer.Close();
                }
            }
            else
            {
                using (StreamWriter writer = new StreamWriter(destFile, true))
                {
                    {
                        string output = ContainerName.text + "\t" + lenghtLabel.text + "\t" + highLabel.text + "\t" + widthLabel.text + "\t" + "0" + "\t" + quantityLabel.text + "\t" + weightLabel.text + "\t" + priceLabel.text;
                        writer.Write(output);
                    }
                    writer.Close();
                }
              
            }
            
        }

    }
    //Delate all the records
    public void DeleteRecords()
    {
        panelContainer.sizeDelta = new Vector2(panelContainer.sizeDelta.x, 333.2187f);
        uIContainer.GetComponent<MultUIContainer>().numContainers =0;
        foreach (Transform child in panelContainer)
        {
            if(child.name.Contains("Clone"))
            {
                GameObject.Destroy(child.gameObject);
            }
            
        }
        foreach (Transform child in ContainerRecordTable)
        {
            GameObject.Destroy(child.gameObject);
        }
        ContainerRecordTable.sizeDelta = new Vector2(ContainerRecordTable.sizeDelta.x, 360f);

    }
    public void DeleteRecordsCargaTap()
    {
        panelContainer.sizeDelta = new Vector2(panelContainer.sizeDelta.x, 333.2187f);
        uIContainer.GetComponent<MultUIContainer>().numContainers = 0;
        foreach (Transform child in panelContainer)
        {
            if (child.name.Contains("Clone"))
            {
                GameObject.Destroy(child.gameObject);
            }

        }

    }
    //Delete a  record
    public void deleteRecord()
    {
       
        string destFile = string.Format("{0}/StreamingAssets/MultipleContainer/Containers/{1}.txt",
              UnityEngine.Application.dataPath, ContainerName.text);
        File.Delete(destFile);
        GameObject.Destroy(record);
        int recordNumber = admRecords.GetComponent<MultRecordContainerManager>().numContainer;
        if(recordNumber > 12)
        {
            ContainerRecordTable.sizeDelta = new Vector2(ContainerRecordTable.sizeDelta.x, ContainerRecordTable.sizeDelta.y - 30f);
            admRecords.GetComponent<MultRecordContainerManager>().numContainer--;
        }
    }
    public void openEditMenu()
    {
        editMenu.SetActive(true);
        titleName.text= ContainerName.text;
        lenghtIF.text= lenghtLabel.text;
        heightIF.text= highLabel.text;
        widthIF.text=widthLabel.text;
        quantityIF.text=quantityLabel.text;
        weightIF.text=weightLabel.text;
        priceIF.text=priceLabel.text;
        incluirTgl.isOn = incluir.isOn;
        MsgError.SetActive(false);
    }

    public void saveChangesContainer()
    {
        string destFile = string.Format("{0}/StreamingAssets/MultipleContainer/Containers/{1}.txt",
               UnityEngine.Application.dataPath, titleName.text);
        MsgError.SetActive(false);
        try
        {
            //validate 
            Vector3 containerSize = new Vector3();
            containerSize.x = float.Parse(lenghtIF.text);
            containerSize.y = float.Parse(heightIF.text);
            containerSize.z = float.Parse(widthIF.text);
            int quantityP = int.Parse(quantityIF.text);
            int weightP = int.Parse(weightIF.text);
            int priceP = int.Parse(priceIF.text);
            if (string.IsNullOrEmpty(lenghtIF.text) || string.IsNullOrEmpty(heightIF.text) || string.IsNullOrEmpty(widthIF.text) || string.IsNullOrEmpty(quantityIF.text) || string.IsNullOrEmpty(weightIF.text) || string.IsNullOrEmpty(priceIF.text) )
            {
                MsgError.SetActive(true);
                return;
            }
            //Validate the decimal separator
            if (lenghtIF.text.Contains(".") || heightIF.text.Contains(".") || widthIF.text.Contains("."))
            {
                MsgError.SetActive(true);
                return;
            }
        }
        catch (System.Exception)
        {
            MsgError.SetActive(true);
            return;
        }

        using (StreamWriter writer = new StreamWriter(destFile))
        {  
            if(incluirTgl.isOn)
            {
                string output = titleName.text + "\t" + lenghtIF.text + "\t" + heightIF.text + "\t" + widthIF.text + "\t" + "1" + "\t" + quantityIF.text + "\t" + weightIF.text + "\t" + priceIF.text;
                writer.WriteLine(output);
            }
            else
            {
                string output = titleName.text + "\t" + lenghtIF.text + "\t" + heightIF.text + "\t" + widthIF.text + "\t" + "0" + "\t" + quantityIF.text + "\t" + weightIF.text + "\t" + priceIF.text;
                writer.WriteLine(output);
            }
            writer.Close();
            createRecords();
            editMenu.SetActive(false);
        }
    }


}
