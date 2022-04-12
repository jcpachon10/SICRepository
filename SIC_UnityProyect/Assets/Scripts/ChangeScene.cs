using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class ChangeScene : MonoBehaviour
{
    public static string language;
    private bool fullsize=false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void BtnNewScene(int scene)
    {
        SceneManager.LoadScene(scene);
    }
    public void BtnNewCube()
    {

        SceneManager.LoadScene(2);   
    }
    public void BtnNewMultipleMenu()
    {

        SceneManager.LoadScene(10);
    }
    public void BtnNewMultiple()
    {

        SceneManager.LoadScene(10);
    }
    public void BtnNewIrregularMenu()
    {
        SceneManager.LoadScene(7);
    }
    public void BtnNewIrregularSKP()
    {
        SceneManager.LoadScene(7);
    }
    public void BtnNewIrregularSPP()
    {
        SceneManager.LoadScene(7);
    }
    public void returMainMenu()
    {
        SceneManager.LoadScene(1);
    }
    public void resize()
    {
        if (!fullsize)
        {
            Screen.fullScreen = true;
            fullsize = true;
        }
        else
        {
            Screen.fullScreen = false;
            fullsize = false;
        }
    }
}
