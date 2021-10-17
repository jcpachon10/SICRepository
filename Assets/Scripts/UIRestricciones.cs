using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIRestricciones : MonoBehaviour
{
    public GameObject panel_peso;
    public GameObject panel_distribucion;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Peso(bool mostrar)
    {
        panel_peso.SetActive(mostrar);
        panel_distribucion.SetActive(false);

    }
    public void Distribucion(bool mostrar)
    {
        panel_distribucion.SetActive(mostrar);
        panel_peso.SetActive(false);

    }
}
