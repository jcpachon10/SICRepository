using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class planeCollider : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other is BoxCollider)
        {
            GameObject box = other.gameObject;
            box.transform.position= box.GetComponent<Package>().initialPosition;
            box.transform.eulerAngles = box.GetComponent<Package>().initialEuler;
        }
    }
}
