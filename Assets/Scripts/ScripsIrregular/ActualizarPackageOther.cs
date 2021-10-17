using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ActualizarPackageOther : MonoBehaviour
{
    // cube
    public GameObject cube;
    //Cylinder
    public GameObject cylinder;
   //texto del titulo
    public Text title;
    //color de la imagen
    public Image colorImage;
    //Elementos de UI
    public GameObject package;
    public InputField lenght_Input;
    public InputField height_Input;
    public InputField width_Input;
    public InputField Name;
    public TextMesh LabelName;
    //text for the id
    public Text id;
    //Text for weight
    public InputField weight_Input;
    //Text for pcs
    public InputField pcs_Input;
    //Package manager
    public PackageManajer packageManajer;
    //Other amanager
    public IrregularManager irManager;
   
    //Rotaciones
    public Toggle rotacion;
    //type of object
    public Toggle cubeToggle;
    public Toggle cylinderToggle;
    public Toggle otherToggle;

    public GameObject alerta;
    public Text AlertText;
    //listy of object to be loading
    private List<GameObject> packages1;
    //Language
    public GlobalLanguage globalLan;

    // Start is called before the first frame update

    void Start()
    {
        //Crete new list of pacakges
        packages1 = new List<GameObject>();
        //add the current package
        packages1.Add(package);
    }

    // Update is called once per frame
    void Update()
    {
        try
        {
            if (globalLan.returnLanguage() == "Español")
            {
                AlertText.text = "Por favor introducir un número entero ";
            }
            else if (globalLan.returnLanguage() == "English")
            {
                AlertText.text = "Please enter a whole number ";
            }
            //Update the values 
            int packageIdP = int.Parse(id.text);
            bool rot = true;
            int classIr = 0;
            Vector3 packageSizeP = new Vector3();
            Vector3 verticalPostP = new Vector3();
            //Size
           
            packageSizeP.x = int.Parse(width_Input.text);
            packageSizeP.y = int.Parse(height_Input.text);
            packageSizeP.z = int.Parse(lenght_Input.text);
            //Validate Rotation
            if (rotacion.isOn)
            {
                rot = true;
            }
            else
            {
                rot = false;
            }

            //Validate Type
            if(cubeToggle.isOn)
            {
               classIr= 1;
            }
            else if(cylinderToggle.isOn)
            {
                classIr = 2;
            }
            else if(otherToggle.isOn)
            {
                classIr = 3;
            }
            //Name, quantuty,weigh, client
            string Name_Value1 = Name.text + "";
            int quantityP = int.Parse(pcs_Input.text);
            float weightP = 0f;
            if (!weight_Input.text.Contains("."))
            {
                weightP = float.Parse(weight_Input.text) ;
            }
            else
            {
                if (globalLan.returnLanguage() == "Español")
                {
                    AlertText.text = "Por favor utilizar ( , ) como separador decimal";
                }
                else if (globalLan.returnLanguage() == "English")
                {
                    AlertText.text = "Please use (,) as decimal separator";
                }
                throw new InvalidOperationException("Logfile cannot be read-only");
            }
            alerta.SetActive(false);
            //update the values for the pType in IrregularManger
            irManager.pTypesIR[packageIdP].updatePType(packageSizeP, quantityP,weightP,Name_Value1,classIr,rot);


        }
        catch (System.Exception)
        {
            //Show the error mesage boc
            alerta.SetActive(true);
            int packageIdP = int.Parse(id.text);
            
        }
        string Name_Value = Name.text + "";
        //Change the name in the mesh on the box
        LabelName.text = Name_Value + "";

        if (lenght_Input.text != "" | height_Input.text != "" | width_Input.text != "")
        {
            
            //Size Variables
            float lenght_value = float.Parse(lenght_Input.text);
            float height_value = float.Parse(height_Input.text);
            float width_value = float.Parse(width_Input.text);
             
            //adjust cube
            //Adjust the size of the preview 
            if (lenght_value >= 50f || height_value >= 50f || width_value >= 50f)
            {
                float proporcion = (Mathf.Max(lenght_value, height_value, width_value)) / 50f;
                lenght_value = lenght_value / proporcion;
                height_value = height_value / proporcion;
                width_value = width_value / proporcion;
                cube.transform.localScale = new Vector3(lenght_value, height_value, width_value);
            }

            else
            {
                cube.transform.localScale = new Vector3(lenght_value, height_value, width_value);
            }
            //adjust cylinder
            if (lenght_value >= 50f || height_value >= 50f )
            {
                float proporcion = (Mathf.Max(lenght_value, height_value)) / 100f;
                lenght_value = lenght_value / proporcion;
                height_value = height_value / (proporcion*2);
                cylinder.transform.localScale = new Vector3(lenght_value, height_value, lenght_value);
            }

            else
            {
                cylinder.transform.localScale = new Vector3(lenght_value, height_value/2, lenght_value);
            }
        }

       
        //transform.localScale = new Vector3(float.Parse(lenght_Input.text), float.Parse(height_Input.text), float.Parse(width_Input.text));
    }
    /**
     * This method updates the values in the UI
     */
    public void updateValues(Color packageColorP, int packageIdP, Vector3 packageSizeP, int quantityP, float weightP, bool isEmpty)
    {
        if (!isEmpty)
        {
            lenght_Input.text = packageSizeP.z + "";
            height_Input.text = packageSizeP.y + "";
            width_Input.text = packageSizeP.x + "";
            pcs_Input.text = quantityP + "";
            weight_Input.text = weightP + "";
        }
        title.text = "Package " + packageIdP;
        id.text = packageIdP + "";
        colorImage.color = packageColorP;
    }

   

}
