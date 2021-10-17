using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class LodingScene : MonoBehaviour
{
    [SerializeField]
    //--------------------------
    //VARIABLES
    //--------------------------
    private Image progressbar;
    public int numscene;
    // Start is called before the first frame update
    void Start()
    {
        //Start async operational
        StartCoroutine(LoadAsyncOperation());

    }

    IEnumerator LoadAsyncOperation()
    {
        //create async operational
        AsyncOperation level = SceneManager.LoadSceneAsync(numscene);
       
        while(level.progress<1)
        {
            //Taker the progress bar fill
            progressbar.fillAmount = level.progress;
            yield return new WaitForEndOfFrame();
        }

       
    }
}
