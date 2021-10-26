using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/*
 *This class controls the UI element of containers to resize the container
 */
public class UIContainer : MonoBehaviour
{
    //----------------------------------
    //PUBLIC VARIABLES
    //----------------------------------
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
    //----------------------------------
    //PRIVATE VARIABLES
    //----------------------------------
    //The pacakge Manager
    PackageManajer packageManager;
    public int numContainers;
    public Image ContainerImage;
    public RecordContainerManager recordContainerManager;
    //Main Camera
    public Transform cam;


    //----------------------------------
    //METHODS
    //----------------------------------

    // Use this for initialization
    void Start()
    {
         //Show the size of the container in the indicator space
        size_txt.text = "(" + containerSize.x + "m ," + containerSize.y + "m ," + containerSize.z + "m )";
        
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
            alerta.SetActive(false);
            try
            {
                //validate if data exist
                if (string.IsNullOrEmpty(Input_title.text) || string.IsNullOrEmpty(height_Input.text) || string.IsNullOrEmpty(lenght_Input.text) || string.IsNullOrEmpty(width_Input.text))
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
                //Set value of the container
                containerSize.x = float.Parse(lenght_Input.text);
                containerSize.y = float.Parse(height_Input.text);
                containerSize.z = float.Parse(width_Input.text);
              
                Label_title.text = Input_title.text;
                size_txt.text = "(" + lenght_Input.text + "m ," + height_Input.text + "m ," + width_Input.text + "m )";
                uiContainer.containerSize = new Vector3(containerSize.x, containerSize.y, containerSize.z);
                //Adjut Container Icon
                adjustImage(containerSize.x);
                //Create a new UI
                GameObject newUiContainer = GameObject.Instantiate(customUICanvas);
                newUiContainer.SetActive(true);
                newUiContainer.transform.SetParent(panelContainer);
                newUiContainer.transform.localRotation = Quaternion.identity;
                newUiContainer.transform.localScale = new Vector3(1f, 1f, 1f);
                newUiContainer.transform.localPosition = new Vector3(newUiContainer.transform.localPosition.x, newUiContainer.transform.localPosition.y, 0f);
                recordContainerManager.newContainerRecord(Input_title.text, lenght_Input.text, height_Input.text, width_Input.text); 
              
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
    public void saveNewCustomCanvasValues(string name,string lenght,string height,string width)
    {
        if (isCustomSize)
        {
            alerta.SetActive(false);
            try
            {
                containerSize.x = float.Parse(lenght);
                containerSize.y = float.Parse(height);
                containerSize.z = float.Parse(width);
                Label_title.text = name;
                size_txt.text = "(" + lenght+ "m ," + height+ "m ," + width + "m )";
                uiContainer.containerSize = new Vector3(containerSize.x, containerSize.y, containerSize.z);
                adjustImage(containerSize.x);
                GameObject newUiContainer = GameObject.Instantiate(customUICanvas);
                newUiContainer.SetActive(true);
                newUiContainer.transform.SetParent(panelContainer);
                newUiContainer.transform.localRotation = Quaternion.identity;
                newUiContainer.transform.localScale = new Vector3(1f, 1f, 1f);
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
        int numConteiner1 = admContianer.GetComponent<UIContainer>().numContainers;
        Debug.Log("El numero  a eliminar es " + numConteiner1);
        if (numConteiner1 > 4)
        {
            panelContainer.sizeDelta = new Vector2(panelContainer.sizeDelta.x, panelContainer.sizeDelta.y - 65f);
        }
        admContianer.GetComponent<UIContainer>().numContainers--;
    }

    /**
      * Set custom size for the container
      */
    public void setCustomValues(Vector3 newSize, string newTitle)
    {
        containerSize = newSize;
        size_txt.text = "(" + containerSize.x + "m ," + containerSize.y + "m ," + containerSize.z + "m )";
    }
}
