using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpperDetector : MonoBehaviour
{ 
    // Conect with the OnMouseClick script in package
    public OnMouseClick package;
   
    //Validate if other pacakge is on
    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Package")
        {
            package.movementAllowed = false;
        }
        
    }
    void OnTriggerExit(Collider other)
    {
        package.movementAllowed = true;
     }
}
