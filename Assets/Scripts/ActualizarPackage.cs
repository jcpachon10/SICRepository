using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ActualizarPackage : MonoBehaviour
{
   //texto del titulo
    public Text title;
    //Cliente
    public InputField client_input;
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
    //Inputfields fuerza
    public Toggle fuerza;
    //Input of force
    public InputField Fuerza_x_input;
    public InputField Fuerza_y_input;
    public InputField Fuerza_z_input;
    //Rotaciones
    public Toggle rotacion;
    public Toggle rot_x;
    public Toggle rot_z;
    public GameObject alerta;
    public GameObject ClientAlert;
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
        if (packageManajer.controlador.text == id.text)
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
                Vector3 packageSizeP = new Vector3();
                Vector3 maxForceP = new Vector3();
                Vector3 verticalPostP = new Vector3();
                //Size

                packageSizeP.x = int.Parse(width_Input.text);
                packageSizeP.y = int.Parse(height_Input.text);
                packageSizeP.z = int.Parse(lenght_Input.text);
                //Force
                maxForceP.x = float.Parse(Fuerza_x_input.text);
                maxForceP.y = float.Parse(Fuerza_y_input.text);
                maxForceP.z = float.Parse(Fuerza_z_input.text);
                if (fuerza.isOn)
                {
                    if (float.Parse(Fuerza_x_input.text) < 40000)
                    {
                        maxForceP.x = Mathf.Floor(float.Parse(Fuerza_x_input.text) * 10000f / (packageSizeP.x * packageSizeP.y));
                    }
                    if (float.Parse(Fuerza_y_input.text) < 40000)
                    {
                        maxForceP.y = Mathf.Floor(float.Parse(Fuerza_y_input.text) * 10000f / (packageSizeP.y * packageSizeP.z));
                    }
                    if (float.Parse(Fuerza_z_input.text) < 40000)
                    {
                        maxForceP.z = Mathf.Floor(float.Parse(Fuerza_z_input.text) * 10000f / (packageSizeP.x * packageSizeP.z));
                    }
                }
                /*
                maxForceP.x = float.Parse(Fuerza_x_input.text) *4 ;
                maxForceP.y = float.Parse(Fuerza_y_input.text) *4;
                maxForceP.z = float.Parse(Fuerza_z_input.text) *4;
                */
                //Define the rotation of the object
                verticalPostP.y = 1;
                if (rotacion.isOn)
                {
                    if (rot_x.isOn) { verticalPostP.x = 1; } else { verticalPostP.x = 0; }
                    if (rot_z.isOn) { verticalPostP.z = 1; } else { verticalPostP.z = 0; }
                }
                else
                {
                    verticalPostP.x = 0;
                    verticalPostP.z = 0;
                }
                //Name, quantuty,weigh, client
                string Name_Value1 = Name.text + "";
                int quantityP = int.Parse(pcs_Input.text);
                float weightP = 0f;
                if (!weight_Input.text.Contains("."))
                {
                    weightP = float.Parse(weight_Input.text) * 10000;
                }
                else
                {
                    print(globalLan.returnLanguage());
                    string language = globalLan.returnLanguage();
                    if (language == "Español")
                    {
                        AlertText.text = "Por favor utilizar ( , ) como separador decimal";
                    }
                    else if (language == "English")
                    {
                        AlertText.text = "Please use (,) as decimal separator";
                    }
                    throw new InvalidOperationException("Logfile cannot be read-only");
                }
                int client = int.Parse(client_input.text);


                alerta.SetActive(false);
                //update the values for the pType in PackageManajer
                PackageManajer.instance.updatePackageValues(packageIdP, packageSizeP, verticalPostP, maxForceP, quantityP, weightP, Name_Value1, client);

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

                //Adjust the size of the preview 
                if (lenght_value >= 50f || height_value >= 50f || width_value >= 50f)
                {
                    float proporcion = (Mathf.Max(lenght_value, height_value, width_value)) / 50f;
                    lenght_value = lenght_value / proporcion;
                    height_value = height_value / proporcion;
                    width_value = width_value / proporcion;
                    transform.localScale = new Vector3(lenght_value, height_value, width_value);
                }

                else
                {
                    transform.localScale = new Vector3(lenght_value, height_value, width_value);
                }
            }

        }
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
