using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuObjetos : MonoBehaviour
{

    //-----------------------
    //VARAIBLES
    //-----------------------
    private float anterior;
    public bool EstaCerrado;


    //Generic method to close using LeanTween
    public void OnClose()
    {
       
        if (EstaCerrado == false)
        {
            anterior = gameObject.transform.localPosition.x;
            Debug.Log("" + anterior);
            LeanTween.scale(gameObject, new Vector3(1, 0,0), 0.1f);
            LeanTween.moveLocalX(gameObject,0, 0.1f);
            EstaCerrado = true;
       
        }
        else
        {
            LeanTween.moveLocalX(gameObject, anterior, 0.1f);
            LeanTween.scale(gameObject, new Vector3(1, 1, 1), 0.1f);
            EstaCerrado = false;
        }
    }

    //Close the preview of the pacakage  
    public void OnClosePreview()
    {

        if (EstaCerrado == false)
        {
            anterior = gameObject.transform.localPosition.x;
            Debug.Log("" + anterior);
            LeanTween.scale(gameObject, new Vector3(0, 1, 0), 0.1f);
            LeanTween.moveLocalX(gameObject, 80, 0.1f);
            EstaCerrado = true;

            //.setOnComplete(DestroyMe);
        }
        else
        {
            LeanTween.moveLocalX(gameObject, anterior, 0.1f);
            LeanTween.scale(gameObject, new Vector3(1, 1, 1), 0.1f);
            EstaCerrado = false;
        }
    }
    //Close the list of the pacakages  
    public void OnCloseList()
    {

        if (EstaCerrado == false)
        {
            anterior = gameObject.transform.localPosition.y;
            Debug.Log("" + anterior);
            LeanTween.scale(gameObject, new Vector3(1, 0, 0), 0.1f);
            LeanTween.moveLocalY(gameObject, 170, 0.1f);
            EstaCerrado = true;

            //.setOnComplete(DestroyMe);
        }
        else
        {
            LeanTween.moveLocalY(gameObject, anterior, 0.1f);
            LeanTween.scale(gameObject, new Vector3(1, 1, 1), 0.1f);
            EstaCerrado = false;
        }
    }
    //Close the list of the containers  
    public void OnCloseContainer()
    {

        if (EstaCerrado == false)
        {
            anterior = gameObject.transform.localPosition.y;
            LeanTween.scale(gameObject, new Vector3(1, 0, 0), 0.1f);
            LeanTween.moveLocalY(gameObject, 170, 0.1f);
            EstaCerrado = true;

            //.setOnComplete(DestroyMe);
        }
        else
        {
            LeanTween.moveLocalY(gameObject, anterior, 0.1f);
            LeanTween.scale(gameObject, new Vector3(1, 1, 1), 0.1f);
            EstaCerrado = false;
        }
    }
    void DestroyMe()
    {
        Destroy(gameObject);
          
    }
}
