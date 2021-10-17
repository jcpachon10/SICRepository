using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    //----------------------------------
    //PUBLIC VARIABLES
    //----------------------------------
    //The static point to look at
    public Transform objective;
    //The speed of the rotation
    public float camSpeed;
    //Zoom of the  camera
    public float zoom;
    //para que rote sobre el objeto 
    Vector3 amount = Vector3.zero;

    
    
    //----------------------------------
    //PRIVATE VARIABLES
    //----------------------------------
    private Camera cam;
    //Current objective
    private Transform currentObjective;
    //----------------------------------
    //METHODS
    //----------------------------------


    // Use this for initialization
    void Start()
    {
        amount.z = 100;
        cam = GetComponent<Camera>();
        currentObjective = objective;
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        //Check if collides with something when click
        if (Input.GetMouseButtonDown(1))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;


            if (Physics.Raycast(ray, out hit, 100))
            {

                if (hit.transform.gameObject.name.Contains("Package"))
                {

                    Package p = hit.transform.gameObject.GetComponentInParent<Package>();

                    p.showPackageInfo();
                    //currentObjective = hit.transform;
                }

            }
        }
       // Movement of the camera
        if (Input.GetMouseButton(1))
        {
            transform.RotateAround(currentObjective.position, Vector3.up, camSpeed * Input.GetAxis("Mouse X") * -1);
            transform.RotateAround(currentObjective.position, Vector3.forward, camSpeed * Input.GetAxis("Mouse Y") * 1);

        }
        transform.LookAt(currentObjective);
        //Zoom

        transform.Translate(Vector3.forward * Input.GetAxis("Mouse ScrollWheel") * zoom, Space.Self);
        
       
      
            
}
    
    /**
    *Reset the camera
    */
    public void resetCameraObjective()
    {
        currentObjective = objective;
    }
}
