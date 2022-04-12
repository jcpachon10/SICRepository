using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class Access : MonoBehaviour
{
    [Serializable]
    public class Sheet
    {
        public Line[] line;
    }
    [Serializable]
    public class Line
    {
        public string USUARIOS;
        public string CLAVES;
    }
    //VARIABLES
    public InputField userId;
    public InputField password;
    public GameObject userErrorText;
    public GameObject passwordErrorText;
    public GameObject wifiError;
    private bool correctUserBool = false;
    private bool correctPasswordBool = false;
   
    Sheet myObject;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    

    public void Request()
    {
        print("downloadinf data....");
        StartCoroutine(ObtainSheetData());
    }
    IEnumerator ObtainSheetData()
    {
        correctUserBool = false;
        correctPasswordBool = false;
        userErrorText.SetActive(false);
        passwordErrorText.SetActive(false);

        UnityWebRequest www = UnityWebRequest.Get("https://opensheet.vercel.app/1uK13TxaX5mg4pF-4npDbEEIBhP8JdbuHk-G9zxG6FQ8/Hoja+1");
        yield return www.SendWebRequest();
        if (www.isNetworkError || www.isHttpError)
        {
            wifiError.SetActive(true);
            Debug.Log("Error:" + www.error);
        }
        else
        {
            string json = www.downloadHandler.text;
           
            json = "{\"line\":" + json + '}';
            Debug.Log(json);
            myObject = JsonUtility.FromJson<Sheet>(json);
            Debug.Log(myObject.line.Length);
            for (int i = 0; i < myObject.line.Length; i++)
            {
                if(myObject.line[i].USUARIOS==userId.text)
                {
                    correctUserBool = true;
                    if (myObject.line[i].CLAVES == password.text)
                    {
                        correctPasswordBool = true;
                        SceneManager.LoadScene(2);
                    }
                }
            }
            if(!correctUserBool)
            {
                userErrorText.SetActive(true);
            }
            else if(!correctPasswordBool)
            {
                passwordErrorText.SetActive(true);
            }
        }
    }
}
