using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class GlobalLanguage : MonoBehaviour
{
    public static string currentLanguage = "Español";
    public Toggle toggleEs;
    public Toggle toggleEn;
    public bool MenuScene;
    private string languagePath;
    // Start is called before the first frame update
    private void Awake()
    {
       // DontDestroyOnLoad(transform.gameObject);
    }

    private void Start()
    {
        languagePath = string.Format("{0}/StreamingAssets/{1}",
                    UnityEngine.Application.dataPath, "language.txt");
        currentLanguage = System.IO.File.ReadAllText(languagePath);
        changeLanguage(currentLanguage);
    }
    public string returnLanguage()
    {
        return (currentLanguage);
    }
     public void changeLanguage(string language1)
    {
        print(language1);
        currentLanguage = language1;
        NotificationCenter.DefaultCenter().PostNotification(this,"ChangeLanguage_");
        if (MenuScene)
        {
            if (language1 == "Español")
            {
                toggleEs.isOn = true;
                File.Delete(languagePath);
                using (StreamWriter writer = new StreamWriter(languagePath, true))
                {
                    writer.Write("Español");
                }
            }
            else if (language1 == "English")
            {
                toggleEn.isOn = true;
                File.Delete(languagePath);
                using (StreamWriter writer = new StreamWriter(languagePath, true))
                {
                    writer.Write("English");
                }
            }
        }
       
    }
}
