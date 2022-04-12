using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
/*
 *This class controls the UI element of containers to resize the container
 */
public class MultUIContainer : MonoBehaviour
{


    public class ContainerType
    {
        //----------------------------------
        //PUBLIC VARIABLES
        //----------------------------------

        //The id of the container
        public Vector3 containerSize;
        public string containerName;
        public int containerQuantity;
        public float containerWLimit;
        public int price;
        public int id;
        public ContainerType( Vector3 containerSizeP, string containerNameP, int containerQuantityP, float containerWLimitP ,  int priceP, int idP)
        {
            containerSize= containerSizeP;
            containerName= containerNameP;
            containerQuantity= containerQuantityP;
            containerWLimit= containerWLimitP;
            price= priceP;
            id=idP;
        }

        public void updateContainerType(int containerIDP, Vector3 containerSizeP, string containerNameP, int containerQuantityP, float containerWLimitP, int priceP)
        {
            containerSize = containerSizeP;
            containerName = containerNameP;
            containerQuantity = containerQuantityP;
            containerWLimit = containerWLimitP;
            price = priceP;
        }

    }
    //----------------------------------
    //PUBLIC VARIABLES
    //----------------------------------
    //Dictionary of Containers
    public Dictionary<int, ContainerType> containerArray;
    //This UI Container Size
    public Vector3 containerSize;
    //The title of this container
    public Text size_txt;
    //Is this a custom size?
    public bool isCustomSize;
    //Canvas panel for custom UI
    public GameObject customUICanvas;
    //Canvas panel adm
    public GameObject admContianer;
    //Panel lista de contenedores
    public RectTransform panelContainer;
    //The text of the  height, width, lenght
    public InputField lenght_Input;
    public InputField height_Input;
    public InputField width_Input;
    public InputField quantity_Input;
    public InputField weight_Input;
    public InputField price_Input;
    public UIContainer uiContainer;

    public Scrollbar scrollbar;
    //Titulo de contenedor
    public InputField Input_title;
    public Text Label_title;
    //MsgBox de info erronea
    public GameObject alerta;
    public GameObject separadorDecimal;
    public GameObject MissingData;
    public GameObject validData;
    public GameObject EnteroMsg;

    //----------------------------------
    //PRIVATE VARIABLES
    //----------------------------------
    //The pacakge Manager
    PackageManajer packageManager;
    public int numContainers;
    public Image ContainerImage;
    public MultRecordContainerManager recordContainerManager;
    //Main Camera
    public Transform cam;
    //ID 
    public Text idText;



        //----------------------------------
        //METHODS
        //----------------------------------

        // Use this for initialization
        void Start()
    {
        containerArray = new Dictionary<int, ContainerType>();
        //Show the size of the container in the indicator space
        //size_txt.text = "(" + containerSize.x + "m ," + containerSize.y + "m ," + containerSize.z + "m )";
        
        packageManager = PackageManajer.instance;
        
    }

    private void Update()
    {
        if(isCustomSize)
        {
            if(lenght_Input.text.Contains("."))
            {
                separadorDecimal.SetActive(true);
            }
            else if (height_Input.text.Contains("."))
            {
                separadorDecimal.SetActive(true);
            }
            else if (width_Input.text.Contains("."))
            {
                separadorDecimal.SetActive(true);
            }
        }
    }
    /**
      * Listener for pallet toggle. Swaps pallet mesh on and off
      */

    /*
     *Function Called when this UI element is Pressed
     */
    public void buttonPressed()
    {
        packageManager.vaciar();
        //Know if the container is custome
        if (isCustomSize)
        {
            customUICanvas.SetActive(true);
        }
        //Resize the container
        packageManager.resizeContainer(containerSize);
    }
    /*
     *Function Called when this UI element is Pressed
     */
    public void buttonPressedIR()
    {
        packageManager.vaciar();
        //Know if the container is custome
        if (isCustomSize)
        {

            customUICanvas.SetActive(true);
        }
        //Resize the container
        Vector3 containerIR = new Vector3(containerSize.z, containerSize.y,containerSize.x);

        packageManager.resizeContainer(containerSize);
    }
    /**
      *Close custom canvas
      */
    public void closeCustomCanvas()
    {
        if (isCustomSize)
            customUICanvas.SetActive(false);
    }
    /**
     *Saves custom canvas values
     */
    public void saveCustomCanvasValues()
    {
        if (isCustomSize)

        {

            try
            {
                alerta.SetActive(false);
           
                //validate if data exist
                if (string.IsNullOrEmpty(quantity_Input.text) || string.IsNullOrEmpty(price_Input.text) || string.IsNullOrEmpty(weight_Input.text) || string.IsNullOrEmpty(Input_title.text) || string.IsNullOrEmpty(height_Input.text) || string.IsNullOrEmpty(lenght_Input.text) || string.IsNullOrEmpty(width_Input.text))
                {
                    MissingData.SetActive(true);
                    return;
                }
                //Validate the decimal separator
                if (lenght_Input.text.Contains(".") || height_Input.text.Contains(".") || width_Input.text.Contains("."))
                {
                    Debug.Log("Usar ',' como separador decimal");
                    separadorDecimal.SetActive(true);
                    return;
                }
                //variables for the new container
                float lenght_flt = float.Parse(lenght_Input.text);
                float height_flt = float.Parse(height_Input.text);
                float width_flt = float.Parse(width_Input.text);
                //validate if its integer
                int quantity_int = 0;
                int price_int = 0;
                int weight_int = 0;
                try {

                     quantity_int = int.Parse(quantity_Input.text);
                     price_int = int.Parse(price_Input.text);
                     weight_int = int.Parse(weight_Input.text);
                }
                catch (System.Exception)
                {
                    EnteroMsg.SetActive(true);
                    return;
                }
                //Set value of the container
                containerSize.x = float.Parse(lenght_Input.text);
                containerSize.y = float.Parse(height_Input.text);
                containerSize.z = float.Parse(width_Input.text);
                //add element of containerType
                ContainerType c = new ContainerType(containerSize,Input_title.text,quantity_int,weight_int, price_int, containerArray.Count);
                //calculate ID
                containerArray.Add(containerArray.Count, c);
               

                Label_title.text = Input_title.text + "  " + quantity_int + " und  " + weight_int + " kg  $" + price_int;
                size_txt.text = "(" + lenght_Input.text + "m ," + height_Input.text + "m ," + width_Input.text + "m )" ;
                uiContainer.containerSize = new Vector3(containerSize.x, containerSize.y, containerSize.z);
                //Adjut Container Icon
                adjustImage(containerSize.x);
                //Create a new UI
                GameObject newUiContainer = GameObject.Instantiate(customUICanvas);
                newUiContainer.SetActive(true);
                newUiContainer.transform.SetParent(panelContainer);
                newUiContainer.transform.localRotation = Quaternion.identity;
                newUiContainer.transform.localScale = new Vector3(1f, 1f, 1f);
                //add id
                newUiContainer.GetComponent<MultUIContainer>().idText.text = c.id+"";
                newUiContainer.transform.localPosition = new Vector3(newUiContainer.transform.localPosition.x, newUiContainer.transform.localPosition.y, 0f);
                recordContainerManager.newContainerRecord(Input_title.text, lenght_Input.text, height_Input.text, width_Input.text, quantity_Input.text
                    ,weight_Input.text, price_Input.text); 
              
                numContainers++;
                //Put Empty
                packageManager.vaciar();
                //Add space to the list
                if (numContainers>4)
                {
                    panelContainer.sizeDelta = new Vector2(panelContainer.sizeDelta.x, panelContainer.sizeDelta.y + 65f);
                }
                packageManager.resizeContainer(containerSize);

                //Delete labels of inputfields
                lenght_Input.text = "";
                height_Input.text = "";
                width_Input.text = "";
                Input_title.text = "";

            }
            catch (System.Exception)
            {
                validData.SetActive(true);
                Debug.Log("Información erronea");
            }

        }
    }
    public void saveNewCustomCanvasValues(string name,string lenght,string height,string width, string quantity, string weight, string price)
    {
        if (isCustomSize)
        {
            alerta.SetActive(false);
            try
            {
                containerSize.x = float.Parse(lenght);
                containerSize.y = float.Parse(height);
                containerSize.z = float.Parse(width);
                int quantityP = int.Parse(quantity);
                int weightP = int.Parse(weight);
                int priceP = int.Parse(price);
                //add element of containerType
                ContainerType c = new ContainerType(containerSize, name, quantityP, weightP, priceP, containerArray.Count);
                //calculate ID
                containerArray.Add(containerArray.Count, c);


                Label_title.text = name + "  " + quantity + " und  " + weight + " kg  $" + price;
                size_txt.text = "(" + lenght+ "m ," + height+ "m ," + width + "m )";
                uiContainer.containerSize = new Vector3(containerSize.x, containerSize.y, containerSize.z);
                adjustImage(containerSize.x);
                GameObject newUiContainer = GameObject.Instantiate(customUICanvas);
                newUiContainer.SetActive(true);
                newUiContainer.transform.SetParent(panelContainer);
                newUiContainer.transform.localRotation = Quaternion.identity;
                newUiContainer.transform.localScale = new Vector3(1f, 1f, 1f);
                //add id
                newUiContainer.GetComponent<MultUIContainer>().idText.text = c.id + "";

                newUiContainer.transform.localPosition = new Vector3(newUiContainer.transform.localPosition.x, newUiContainer.transform.localPosition.y, 0f);
                numContainers++;
                if (numContainers > 4)
                {
                    panelContainer.sizeDelta = new Vector2(panelContainer.sizeDelta.x, panelContainer.sizeDelta.y + 65f);
                }
                packageManager.resizeContainer(containerSize);
                scrollbar.value = 0.00f;
            }
            catch (System.Exception)
            {
                alerta.SetActive(true);
                Debug.Log("Información erronea");
            }

        }
    }

    public void adjustImage(float length)
    {
        float fill;
        if (length < 4.5f) { fill = 0.3f; }
        else if (length > 15){ fill = 1; }
        else { fill=length / 15; }
        ContainerImage.fillAmount = fill;
    }
    public void eliminar()
    {
        GameObject.Destroy(customUICanvas);
        int numConteiner1 = int.Parse(idText.text);
        admContianer.GetComponent<MultUIContainer>().containerArray.Remove(int.Parse(idText.text));
        Debug.Log("El numero  a eliminar es " + numConteiner1);
        if (numConteiner1 > 4)
        {
            panelContainer.sizeDelta = new Vector2(panelContainer.sizeDelta.x, panelContainer.sizeDelta.y - 65f);
        }
        admContianer.GetComponent<MultUIContainer>().numContainers--;
        
    }

    /**
      * Set custom size for the container
      */
    public void setCustomValues(Vector3 newSize, string newTitle)
    {
        containerSize = newSize;
        size_txt.text = "(" + containerSize.x + "m ," + containerSize.y + "m ," + containerSize.z + "m )";
    }
    public void vehicles(string path)
    {
        string fileWrite = string.Format("{0}/StreamingAssets/{1}/Algorithm/{2}",
                UnityEngine.Application.dataPath, path, "vehiculos.txt");

        StreamWriter sw = new StreamWriter(fileWrite);
        sw.WriteLine(containerArray.Count);
        string message="";
        int counter=0;
        foreach (ContainerType container in containerArray.Values)
        {
            message=counter+ "\t" + container.containerSize.x*100+"\t" + container.containerSize.y*100 + "\t" + container.containerSize.z*100 + "\t" + container.containerQuantity + "\t" + container.containerWLimit + "\t" + container.price + "\t" +container.containerName;
            sw.WriteLine(message);
            counter++;
        }
        sw.Close();
    }
}
