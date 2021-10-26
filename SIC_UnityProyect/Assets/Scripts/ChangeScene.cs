using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class ChangeScene : MonoBehaviour
{
    public static string language;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void BtnNewCube()
    {

        SceneManager.LoadScene(2);   
    }
    public void BtnNewRegular()
    {
        SceneManager.LoadScene(3);
    }
    public void returMainMenu()
    {
        SceneManager.LoadScene(1);
    }
}
