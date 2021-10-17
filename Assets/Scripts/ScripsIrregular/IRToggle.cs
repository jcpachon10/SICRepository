using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IRToggle : MonoBehaviour
{
    public GameObject UIgeneral;
    public GameObject UIminiatura;
    public GameObject modelo;
    //inputfields size
    public GameObject lenght_Input;
    public GameObject height_Input;
    public GameObject width_Input;
    public GameObject cm_label;
    public GameObject cubePreview;
    public GameObject cylinderPreview;
    public Text controlador;
    public Color[] UiColors;
    public PackageManajer packageManajer;
    public IrregularManager irregularManager;
    public Text num;
   private int PackageId;
   public Text L_label;
    public GameObject cube_Image;
    public GameObject cylinder_Image;
    public GameObject other_Image;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        bool cargado = true;
        if (controlador.text != num.text)
        {
            UIminiatura.SetActive(true);
            UIgeneral.SetActive(false);
            RectTransform rt = modelo.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(153f, 26f);

        }
        foreach (float i in packageManajer.intermedio.Keys)
        {
            float num_f = float.Parse(num.text);
            if (num_f == i && packageManajer.pTypes[int.Parse(num_f + "")].quantity == packageManajer.intermedio[i])
            {
                UIminiatura.GetComponent<Image>().color = UiColors[0];
                cargado = false;
            }
            if (num_f == i && packageManajer.pTypes[int.Parse(num_f + "")].quantity > packageManajer.intermedio[i])
            {
                UIminiatura.GetComponent<Image>().color = UiColors[1];
                cargado = false;
            }

        }
        if (cargado)
        {
            UIminiatura.GetComponent<Image>().color = UiColors[2];
        }


    }
    public void cube(bool mostrar)
    {
        if(mostrar)
        {

            PackageId = int.Parse(num.text);
            L_label.text = "L:";
            
            width_Input.SetActive(mostrar);
            height_Input.SetActive(mostrar);
            lenght_Input.SetActive(mostrar);
            
            cm_label.SetActive(mostrar);
        }
        cubePreview.SetActive(mostrar);
        cube_Image.SetActive(mostrar);
    }
    public void cylinder(bool mostrar)
    {
        if( mostrar)
        {
            PackageId = int.Parse(num.text);
            L_label.text = "D:";
            width_Input.SetActive(!mostrar);
            height_Input.SetActive(mostrar);
            lenght_Input.SetActive(mostrar);
            cm_label.SetActive(mostrar);
        }
        cylinderPreview.SetActive(mostrar);
        cylinder_Image.SetActive(mostrar);
    }
    public void other(bool mostrar)
    {
        if (mostrar)
        {
            PackageId = int.Parse(num.text);
            width_Input.SetActive(!mostrar);
            height_Input.SetActive(!mostrar);
            lenght_Input.SetActive(!mostrar);
            cm_label.SetActive(!mostrar);
        }
        other_Image.SetActive(mostrar);
    }
    public void deletePackage()
    {
        GameObject.Destroy(modelo);
        //PackageManajer.instance.deletePackageType(int.Parse(num.text));
    }
}
