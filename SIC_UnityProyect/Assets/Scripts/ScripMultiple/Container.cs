using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Container : MonoBehaviour
{
    public float total_volume;
    public float free_volume;
    public float used_volume;
    public float weight;
    public Vector3 scale;
    // Start is called before the first frame update
    void Start()
    {
        //total_volume = (transform.localScale.x * transform.localScale.y * transform.localScale.z);
        //free_volume = total_volume;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void updateVolume(Vector3 size)
    {
        total_volume = (size.x * size.y * size.z);
        free_volume = total_volume;
        used_volume = 0;
        weight = 0;
    }
    public void updateKPI(Vector3 size, float weightPackage)
    {
        free_volume = free_volume - ((size.x * size.y * size.z)/1000000);
        used_volume=used_volume + ((size.x * size.y * size.z) / 1000000);
        weight = weight + (weightPackage/10000);

    }
}
