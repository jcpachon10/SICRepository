using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
/*
 * This class models one package
 */
public class Package : MonoBehaviour
{
    //----------------------------------
    //PUBLIC VARIABLES
    //----------------------------------
    //Mesh of the name   
    public TextMeshPro Nombre1;
    public TextMeshPro Nombre2;
    public TextMeshPro Nombre3;
    public TextMeshPro Nombre4;
    //Grouo and item ID
    public int groupId;
    public int itemId;
    public bool broken;
    public int client;
    public Vector3 initialPosition;
    public Vector3 initialEuler;
    public static int count = 0;

    //------------------------
    //METHODS
    //------------------------
    /*
     * This method sets the values of a package
     * @param size the Size of the package in meters
     * @param position the position of the package in world space
     * @param packageId the id to be desplayed on the package box
     * @param packageColorP  the color of the package 
     */
    public void setPackageValues(Vector3 size, Vector3 position, int packageId, Color packageColor, float weight,int clientID,int groupID, string Name1)
    {
       
        broken = false;
        //JCMF
        //Set the position in space
        transform.position = position;
        initialPosition=position;
        initialEuler = this.transform.eulerAngles;

    //Set the dimensions of the package
    transform.localScale = size;
        //Set the color of the package
        GetComponent<Renderer>().material.color = packageColor;
        //Set the mass on the rigidbody
        //Set itemId
        itemId = packageId;
        //Set gropu id
        groupId = groupID;
        //SetClientID
        client = clientID;
       
        // GetComponentInChildren<Rigidbody>().mass = weight;
        //update the name     
        Nombre1.text = Name1 + "";
        Nombre2.text = Name1 + "";
        Nombre3.text = Name1 + "";
        Nombre4.text = Name1 + "";
    }
    public void setPackagePosition( Vector3 position)
    {
        //Set the position in space
        transform.position = position;
       
    }

    /**
    *Shows the package Information on clicked
    */
    public void showPackageInfo()
  {
     
       // PackageManager.instance.showPackageInfo(int.Parse(GetComponentInChildren<TextMesh>().text));
   }


    //JCMF
    /**
    *Returns the global velocity of the package
    */
    public Vector3 comparePackageVelocity()
    {
        return GetComponentInChildren<Rigidbody>().velocity;
    }

    /**
    *Changes the material of the package to reflect itś state in the DBC
    */
    public void breakPackage()
    {
        broken = true;
    }

    /**
    *Changes the material of the package to reflect itś state in the DBC
    */
    public bool isBroken()
    {
        return broken;
    }
    //JCMF
    
}
