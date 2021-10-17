using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Camera))]
public class SnapShot : MonoBehaviour
{
    //------------------------
    //VARIABLES
    //------------------------
    public GameObject cara_contenedor;
    Camera SnapCamera;
    int resWidth = 256;
    int resHeigth = 256;
    public int group;

    //------------------------
    //METHODS
    //------------------------
    
    //Take the photo
    void Awake()
    {
        SnapCamera = GetComponent<Camera>();
        if (SnapCamera.targetTexture == null)
        {
            SnapCamera.targetTexture = new RenderTexture(resWidth, resHeigth,24);
        }
        else
        {
            resWidth = SnapCamera.targetTexture.width;
            resHeigth = SnapCamera.targetTexture.height;
        }
        SnapCamera.gameObject.SetActive(false);
    }

    // Start is called before the first frame update
    void Start()
    {
       
        SnapCamera.fieldOfView = 60f;
    }

    // Update is called once per frame
    public void callTakeSnapShot()
    {
        SnapCamera.gameObject.SetActive(true);

    }
   
    void LateUpdate()
    {
        //Validate the biggest side of the conteiner
        float mayor = Mathf.Max(cara_contenedor.transform.localScale.x, cara_contenedor.transform.localScale.y, cara_contenedor.transform.localScale.z);      
        Vector3 pos = SnapCamera.transform.localPosition;
        //Validate which cam has been using
        if (SnapCamera.name == "SnapShot_Camara_1")
        {
            SnapCamera.transform.localPosition = new Vector3(
                 cara_contenedor.transform.localScale.x / 2,
                 cara_contenedor.transform.localScale.y / 2,
                 -cara_contenedor.transform.localScale.x - 1);
        }
         else if(SnapCamera.name=="SnapShot_Camara_6")
        {

            SnapCamera.transform.localPosition = new Vector3(
                -mayor-1,
                 cara_contenedor.transform.localScale.y / 2,
                 cara_contenedor.transform.localScale.z / 2);
        }
         else if (SnapCamera.name == "SnapShot_Camara_2")
        {
            SnapCamera.transform.localPosition = new Vector3(
                mayor + 3,
                 cara_contenedor.transform.localScale.y / 2,
                 cara_contenedor.transform.localScale.z /2);
        }
        else  if (SnapCamera.name == "SnapShot_Camara_3")
        {
            SnapCamera.transform.localPosition = new Vector3(
                   cara_contenedor.transform.localScale.x / 2,
                   mayor+2,
                   cara_contenedor.transform.localScale.z /2);
        }
        else  if (SnapCamera.name == "SnapShot_Camara_4")
        {
            SnapCamera.transform.localPosition = new Vector3(
                    cara_contenedor.transform.localScale.x / 2,
                    cara_contenedor.transform.localScale.y / 2,
                    cara_contenedor.transform.localScale.x+ cara_contenedor.transform.localScale.z+ 1); 
        }
         else if (SnapCamera.name == "SnapShot_Camara_5")
        {
            SnapCamera.fieldOfView =  70.244f * Mathf.Pow(mayor, -0.55f) ;
            
            SnapCamera.transform.eulerAngles = new Vector3(30, 213.41f*Mathf.Pow(mayor, 0.08f),0);
           
            SnapCamera.transform.localPosition = new Vector3(SnapCamera.transform.localPosition.x ,(0.4167f*mayor+0.45f), SnapCamera.transform.localPosition.z );
        }
        if (SnapCamera.gameObject.activeInHierarchy)
        {
            Texture2D snapshot = new Texture2D(resWidth, resHeigth, TextureFormat.RGB24, false);
            SnapCamera.Render();
            RenderTexture.active = SnapCamera.targetTexture;
            snapshot.ReadPixels(new Rect(0,0,resWidth,resHeigth),0,0);
            byte[] bytes = snapshot.EncodeToPNG();
            string file = "";
            if (SnapCamera.name == "SnapShot_Camara_5") {  file = SnapShotName(); } else {  file = SnapShotName1(); }
            System.IO.File.WriteAllBytes(file, bytes);
        }
        SnapCamera.gameObject.SetActive(false);
    }
    //Photos of the groups
    string SnapShotName()
    {
        return string.Format("{0}/StreamingAssets/SnapShots/snap_{1}x{2}_{3}_{4}.png",
            Application.dataPath,
            resWidth,
            resHeigth, SnapCamera.name,
            group,
            System.DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss"));
    }
    //
    string SnapShotName1()
    {
        return string.Format("{0}/StreamingAssets/SnapShots/snap_{1}x{2}_{3}.png",
            Application.dataPath,
            resWidth,
            resHeigth, SnapCamera.name,
            System.DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss"));
    }


}
