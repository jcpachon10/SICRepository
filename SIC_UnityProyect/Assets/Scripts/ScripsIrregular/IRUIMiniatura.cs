using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IRUIMiniatura : MonoBehaviour
{
    public InputField Client;
    public InputField lenght_Input;
    public InputField height_Input;
    public InputField width_Input;
    public InputField weight_input;
    public InputField quantity_input;
    public InputField Name;
    public Text LabelName;
    public Text LabelMed;
    //Cube Toggle
    public Toggle cube_Toggle;
    //cylinder Toggle
    public Toggle cylinder_Toggle;
    //other Toggle
    public Toggle other_Toggle;
    public GameObject general;
    public GameObject miniatura;
    public GameObject modelo;
    public Text Controlador;
    public Text numAct;
    public PackageManajer packageManajer;


    // Start is called before the first frame update
    void Start()
    {
      
    }

    // Update is called once per frame
    void Update()
    {
        LabelName.text = Name.text;
        if (cube_Toggle.isOn) { LabelName.text = Name.text + "(Cube)"; }
        else if (cylinder_Toggle.isOn) { LabelName.text = Name.text + "(Cylinder)"; }
        else if (other_Toggle.isOn) { LabelName.text = Name.text + "(Other)"; }
        try
        {
            LabelMed.text = lenght_Input.text + "x" + height_Input.text + "x" + width_Input.text + " cm   " + weight_input.text + " kg  " + packageManajer.intermedio[float.Parse(numAct.text)] + "/" + quantity_input.text + " und";
        }
        catch
        {
            
            LabelMed.text = lenght_Input.text + "x" + height_Input.text + "x" + width_Input.text + " cm   " + weight_input.text + " kg  " + quantity_input.text + " und";
        }
    }
    public void abrir()
    {
        general.SetActive(true);
        miniatura.SetActive(false);
        RectTransform rt = modelo.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(153f, 126.4577f);
        Controlador.text = numAct.text;
    }
    public void MinimizeAll()
    {
        
        Controlador.text = "-1";
    }
}
