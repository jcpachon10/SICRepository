using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Transparencia2 : MonoBehaviour
{
    public GameObject caja;
    public GameObject nombre;
    public GameObject nombre1;
    public GameObject nombre2;
    public GameObject nombre3;
    private SphereCollider head;
    // Start is called before the first frame update
    void Start()
    {
      
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "MainCamera")
        {
            caja.GetComponent<Renderer>().enabled = false;
            nombre.SetActive(false);
            nombre1.SetActive(false);
            nombre2.SetActive(false);
            nombre3.SetActive(false);
        }
       


    }
    void OnTriggerExit(Collider other)
    {
        if ( other.tag == "MainCamera")
        {
            caja.GetComponent<Renderer>().enabled = true;
            nombre.SetActive(true);
            nombre1.SetActive(true);
            nombre2.SetActive(true);
            nombre3.SetActive(true);
        }
       
           
    }
    
    void Update()
    {
        
    }
}
