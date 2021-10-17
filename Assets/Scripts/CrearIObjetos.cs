using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;

public class CrearIObjetos : MonoBehaviour, IPointerClickHandler
{
    private  int NumPacks;
    public GameObject modelo;
    public RectTransform Lista;
    public float height;
    public int NumCarga;
    public Image ColorImage;
    public Color[] packageColor;
    public Renderer myRenderer;
    public GameObject Cube;
    public Text numero;
    //Size of the container in m
    public Vector3 containerSize;
    //The transform of the container
    public Transform containerTransform;

    // Start is called before the first frame update
    void Start()
    {
        NumPacks = 1;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

   
    public void OnPointerClick(PointerEventData eventData)
    {

    }
    public void nuevo()
    {
        var pos = modelo.transform.localPosition;

        Material newMaterial = new Material(myRenderer.GetComponent<MeshRenderer>().material);
        myRenderer.material = newMaterial;
        myRenderer.material.SetColor("_Color", packageColor[NumPacks]);

       
        //cambiar imagen y color 
        ColorImage.color = packageColor[NumPacks];
        numero.text = NumPacks + "";

        //Crear el nuevo objeto
        GameObject nuevo = Instantiate(modelo);
        nuevo.SetActive(true);
        nuevo.transform.SetParent(Lista);

        //Asiganer material al cubo
        Cube.GetComponent<MeshRenderer>().material = newMaterial;

       

       //poner en posición
          
        Debug.Log(""+height);
        nuevo.transform.rotation = new Quaternion(0f, 0f, 0f,0f);
        nuevo.transform.localScale = new Vector3(1f,1f,1f) ;
        nuevo.transform.localPosition = new Vector3(pos.x, pos.y -height, pos.z);
        //añadir uno nuevo
        NumPacks++;
        height += nuevo.GetComponent<RectTransform>().sizeDelta.y;


        
    }
    public void resizeContainer(Vector3 newSize)
    {
        containerSize = newSize;
        containerTransform.localScale = containerSize;
        containerTransform.position = Vector3.zero;
    }

}
