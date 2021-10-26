using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TabGroup : MonoBehaviour
{
    public List<TapButtom> tabButtom;
    public Sprite tabIdlde;
    public Sprite tabHover;
    public Sprite tabActive;
    public TapButtom selectedTab;
    public List<GameObject> objectsToSwap;

    public void Suscribe( TapButtom buttom)
    {
        if(tabButtom==null)
        {
            tabButtom = new List<TapButtom>();
        }
        tabButtom.Add(buttom);
    }
    // Start is called before the first frame update
    public void OnTabEnter(TapButtom buttom)
    {
        ResetTabs();
        if (selectedTab == null || buttom != selectedTab)
        {

            buttom.blackground.sprite = tabHover;
        }
    }
    public void OnTabExit(TapButtom buttom)
    {
        ResetTabs();
    }
    public void OnTabSelected(TapButtom buttom)
    {
        if(selectedTab!= null)
        {
            selectedTab.Deselect();
        }
        selectedTab = buttom;

        selectedTab.select();

        ResetTabs();
       buttom.blackground.sprite = tabActive;
        int index = buttom.transform.GetSiblingIndex();
        for(int i=0; i < objectsToSwap.Count; i++)
        {
            if (i == index)
            {
                if (i == 3)
                {
                    objectsToSwap[0].SetActive(true);
                    objectsToSwap[3].SetActive(true);
                }
                else
                {
                 objectsToSwap[i].SetActive(true);
                }
               
            }
            else
            {
                objectsToSwap[i].SetActive(false);
            }
            }
       

    }
    public void ResetTabs( )
    {
        foreach(TapButtom buttom in tabButtom)
        {
            if(selectedTab!=null && buttom== selectedTab) { continue; }
            buttom.blackground.sprite = tabIdlde;
        }
    }
}
