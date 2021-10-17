using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Toggles : MonoBehaviour
{
    public Color[] UiColors;
    public GameObject UIgeneral;
    public GameObject UIminiatura;
    public GameObject panel_rotacion;
    public GameObject panel_fuerza;
    public GameObject modelo;
    public GameObject F_Y;
    public GameObject F_X;
    public InputField F_X_text;
    public InputField F_Y_text;
    public InputField F_Z_text;
    public Toggle fuerza_tgl;
    public Toggle r_X;
    public Toggle r_Z;
    public Text num;
    public Text controlador;
    public GameObject Image_R;
    public GameObject Image_F;
    public GameObject cube;
    public PackageManajer packageManajer;
    private bool rotar;
    float angle = 360.0f; // Degree per time unit
    float time = 1.0f; // Time unit in sec
    Vector3 axis = Vector3.up; // Rotation axis, here it the yaw axis

  
    // Start is called before the first frame update
    void Start()
    {
       
       // Image_F.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        bool cargado=true;
        if (controlador.text != num.text)
        {
            UIminiatura.SetActive(true);
            UIgeneral.SetActive(false);
            RectTransform rt = modelo.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(153f, 26f);
            
        }
        foreach(float i in packageManajer.intermedio.Keys)
        {
            float num_f = float.Parse(num.text);
            if (num_f== i && packageManajer.pTypes[int.Parse(num_f+"")].quantity==packageManajer.intermedio[i])
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
        if(cargado)
        {
            UIminiatura.GetComponent<Image>().color = UiColors[2];
        }
        


    }
     public void Rotacion ( bool mostrar)
    {
        panel_rotacion.SetActive(mostrar);
        if (r_X.isOn) {F_X.SetActive(mostrar); }
        if (r_Z.isOn) { F_Y.SetActive(mostrar); }
        Image_R.SetActive(mostrar);
        rotar = mostrar;
        //var rot = cube.transform.rotation;
        
      
        
    }
    public void fuerza(bool mostrar)
    {
        panel_fuerza.SetActive(mostrar);
        Image_F.SetActive(mostrar);
        
        if (mostrar)
        {
            F_X_text.text = 0f + "";
            F_Y_text.text = 0f + "";
            F_Z_text.text = 0f + "";
        }
        else
        {
            F_X_text.text = 4000000f + "";
            F_Y_text.text = 4000000f + "";
            F_Z_text.text = 4000000f + " ";
        }
    }
    public void R_X(bool mostrar)
    {
        if (mostrar && fuerza_tgl.isOn)
        {
            F_X_text.text = 0f + " ";
        }
        else
        {
            F_X_text.text = 40000f + " ";
        }
        F_X.SetActive(mostrar);
    }
    public void R_Z(bool mostrar)
    {
        if (mostrar && fuerza_tgl.isOn)
        {
            F_Y_text.text = 0f + " ";
        }
        else
        {
            F_Y_text.text = 40000f + " ";
        }
        F_Y.SetActive(mostrar);
    }
    public void deletePackage()
    {
        GameObject.Destroy(modelo);
        PackageManajer.instance.deletePackageType(int.Parse(num.text));
    }
 }
