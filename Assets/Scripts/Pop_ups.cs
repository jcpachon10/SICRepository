using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;

public class Pop_ups : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    //------------------------
    //VARIABLES
    //------------------------
    //Panel
    public GameObject pop_up;
    //------------------------
    //METHODS
    //------------------------
    
    //Active pop up
    public void OnPointerEnter(PointerEventData eventData)
    {
        pop_up.SetActive(true);
    }

    //Unactive pop up
    public void OnPointerExit(PointerEventData eventData)
    {
        pop_up.SetActive(false);
    }

}
