using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationLimit : MonoBehaviour
{
    
    //-------------------------------
    //VARIABLES
    //-------------------------------

    //Packge
    public GameObject package;
    //Select
    public GameObject select;
    //Position of package
    public Vector3 pos;
    //Position of package
    public  Vector3 eulerAngles;
    //counte
    private int counter=0;
    //bool
    private bool inCounting=false;
    private bool outCounting = false;
    // Start is called before the first frame update

    //METHODS
    void Start()
    {
        pos = package.transform.position;
        eulerAngles = package.transform.eulerAngles;
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        if (inCounting && select.activeSelf)
        {
            counter++;
            if (counter > 5 )
            {
                
                //Unfreeze the movement
                package.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
                package.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
                package.transform.position = pos;
                package.transform.eulerAngles = eulerAngles;
                inCounting = false;
                //create upper
                Renderer rende2 = package.GetComponent<Renderer>();
                Bounds brende2 = rende2.bounds;
                Destroy(package.transform.Find("Upper").gameObject);
                GameObject newUpper = GameObject.CreatePrimitive(PrimitiveType.Cube);
                newUpper.name = "Upper";
                newUpper.GetComponent<MeshRenderer>().enabled = false;
                var script = newUpper.AddComponent<UpperDetector>();
                script.package = package.GetComponent<OnMouseClick>();
                newUpper.transform.parent = package.transform;
                newUpper.transform.localPosition = new Vector3(0f, 0f, 0f);
                newUpper.transform.localScale = new Vector3(1f, 1f, 1f);
                newUpper.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
                newUpper.GetComponent<BoxCollider>().center = new Vector3(0.5f, 0.5f, 0.5f);
                newUpper.GetComponent<BoxCollider>().size = new Vector3(0.9f, 0.9f, 0.9f);
                newUpper.GetComponent<BoxCollider>().isTrigger = true;
                newUpper.GetComponent<BoxCollider>().enabled = false;
                newUpper.transform.parent = null;
                newUpper.transform.position = new Vector3(newUpper.transform.position.x, newUpper.transform.position.y + 0.05f * brende2.size.y, newUpper.transform.position.z);
                newUpper.transform.parent = package.transform;
                newUpper.GetComponent<BoxCollider>().enabled = true;
                

                counter = 0;
            }
        }
        else
        {
            counter = 0;
        }

    }
    //Validate if the box have something on
    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.name!="Upper")
        {
            inCounting = true;
        }
    }
    /*void OnTriggerExit(Collider other)
    {
        inCounting = false;
        print("Salio");
    }*/
}
