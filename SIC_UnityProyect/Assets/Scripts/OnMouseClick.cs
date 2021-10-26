using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using System.Collections;

public class OnMouseClick : MonoBehaviour
{
    //---------------------------------
    //VARIABLES
    //---------------------------------
    private Vector3 mOffset;
    //Parent of all the package
    public Transform parent;
    //Coord in Z
    private float mZCoord;
    //Cube of select
    public GameObject select;
    //Rigidbody Constrains
    private RigidbodyConstraints originalConstrains;
    //Boolean that determinate if it`s a viable  movement  
    public bool movementAllowed;
    //Biggest side of the box
    private float max;
    //RigidBody of the pacakge selected
    private Rigidbody rigidbody;
    public float speed = 2f;



    //---------------------------------
    //METHODS
    //---------------------------------

    void Start()
    {
        //Initialize of varaibles
        rigidbody = GetComponent<Rigidbody>();
        movementAllowed = true;
    }
  
    private void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            select.SetActive(false);
        }
        //Validate if its select and its a viable movement
    
        if (select.activeSelf)
        {
            int numRot = 0;
            //Movement
            if (Input.GetKey(KeyCode.LeftArrow))
                this.rigidbody.velocity = 2 * Vector3.back;
            //this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z - 0.01f);
            else if (Input.GetKey(KeyCode.RightArrow))
                this.rigidbody.velocity = 2 * Vector3.forward;
            //this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z + 0.01f);
            else if (Input.GetKey(KeyCode.UpArrow))
                this.rigidbody.velocity = 2 * Vector3.left;
            //this.transform.position = new Vector3(this.transform.position.x - 0.01f, this.transform.position.y, this.transform.position.z);
            else if (Input.GetKey(KeyCode.DownArrow))
                this.rigidbody.velocity = 2 * Vector3.right;
            //this.transform.position = new Vector3(this.transform.position.x + 0.01f, this.transform.position.y, this.transform.position.z);
            else if (Input.GetKeyDown(KeyCode.Alpha1)) numRot = 1;
            else if (Input.GetKeyDown(KeyCode.Alpha2)) numRot = 2;
            else if (Input.GetKeyDown(KeyCode.Alpha3)) numRot = 3;
            else if (Input.GetKeyDown(KeyCode.Alpha4)) numRot = 4;
            else if (Input.GetKeyDown(KeyCode.Alpha5)) numRot = 5;
            else if (Input.GetKeyDown(KeyCode.Alpha6)) numRot = 6;
            if (numRot > 0) // Rota
            {
                Renderer rende1 = GetComponent<Renderer>();
                Bounds brende1 = rende1.bounds;
                if (numRot == 1)
                    this.transform.RotateAround(transform.TransformPoint(0.5f, 0.5f, 0.5f), Vector3.up, 90f);
                else if (numRot == 2)
                    this.transform.RotateAround(transform.TransformPoint(0.5f, 0.5f, 0.5f), Vector3.down, 90f);
                else if (numRot == 3)
                    this.transform.RotateAround(transform.TransformPoint(0.5f, 0.5f, 0.5f), Vector3.forward, 90f);
                else if (numRot == 4)
                    this.transform.RotateAround(transform.TransformPoint(0.5f, 0.5f, 0.5f), Vector3.back, 90f);
                else if (numRot == 5)
                    this.transform.RotateAround(transform.TransformPoint(0.5f, 0.5f, 0.5f), Vector3.right, 90f);
                else if (numRot == 6)
                    this.transform.RotateAround(transform.TransformPoint(0.5f, 0.5f, 0.5f), Vector3.left, 90f);
                Renderer rende2 = GetComponent<Renderer>();
                Bounds brende2 = rende2.bounds;
                this.transform.position = new Vector3(transform.position.x, transform.position.y - (brende1.size.y - brende2.size.y) / 2, transform.position.z);

                // Upper

                Destroy(transform.Find("Upper").gameObject);
                GameObject newUpper = GameObject.CreatePrimitive(PrimitiveType.Cube);
                newUpper.name = "Upper";
                newUpper.GetComponent<MeshRenderer>().enabled = false;
                var script = newUpper.AddComponent<UpperDetector>();
                script.package = GetComponent<OnMouseClick>();
                newUpper.transform.parent = transform;
                newUpper.transform.localPosition = new Vector3(0f, 0f, 0f);
                newUpper.transform.localScale = new Vector3(1f, 1f, 1f);
                newUpper.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
                newUpper.GetComponent<BoxCollider>().center = new Vector3(0.5f, 0.5f, 0.5f);
                newUpper.GetComponent<BoxCollider>().size = new Vector3(0.9f, 0.9f, 0.9f);
                newUpper.GetComponent<BoxCollider>().isTrigger = true;
                newUpper.GetComponent<BoxCollider>().enabled = false;
                newUpper.transform.parent = null;
                newUpper.transform.position = new Vector3(newUpper.transform.position.x, newUpper.transform.position.y + 0.05f* brende2.size.y, newUpper.transform.position.z);
                newUpper.transform.parent = transform;
                newUpper.GetComponent<BoxCollider>().enabled = true;
            }
            //Unfreeze the movement
            this.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
            this.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;

        }
        else
        {
            this.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        }


    }
    //Select the package
    void  OnMouseDown()

    {
        if (movementAllowed)
        {

            parent = this.transform.parent;
            //Unactive the select in other package
            foreach (Transform child in parent)
            {
                child.GetComponent<OnMouseClick>().select.SetActive(false);
            }
            mZCoord = Camera.main.WorldToScreenPoint(gameObject.transform.position).z;
            mOffset = gameObject.transform.position - GetMouseWordPoss();

            select.SetActive(true);
            //
        }

    }

    //Know the mouse position in the Screen
    private Vector3 GetMouseWordPoss()
    {
        //pixelCordinate (x,y)
        Vector3 mousePoint = Input.mousePosition;
        //Z cordinate of the game
        mousePoint.z = mZCoord;
        return Camera.main.ScreenToWorldPoint(mousePoint);

    }

    //Adjuts the position when mousedrag
    void OnMouseDrag()
    {

        Vector3 wordposition = GetMouseWordPoss() + mOffset;
        if (movementAllowed)
        //wordposition.x>=0 && wordposition.y>=0 && wordposition.z>=0)
        {
            transform.position = wordposition;
        }

    }
}
