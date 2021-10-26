using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Change the language of the game
public class Language : MonoBehaviour
{
    //VARIABLES
    public string español;
    public string English;
    public bool isButton;
    public Toggle toggleEs;
    public Toggle toggleEn;
    public GlobalLanguage global;
    void Start()
    {
        NotificationCenter.DefaultCenter().AddObserver(this,"ChangeLanguage_");
        ChangeLanguage_();
    }
    private void Awake()
    {
        global = GameObject.Find("LanguageManager").GetComponent<GlobalLanguage>();
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    public void ChangeLanguage_()
    {
        if (global.returnLanguage() == "Español") { 
            
            if(isButton)
            {
                GetComponentInChildren<Text>().text = español;
            }
            else
            {
                GetComponent<Text>().text = español;
            }
        }
        if (global.returnLanguage() == "English") {
            if (isButton)
            {
                GetComponentInChildren<Text>().text = English;

            }
            else
            {
                GetComponent<Text>().text = English;
            }
        }
    }
}
