using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;
[RequireComponent(typeof(Image))]
public class TapButtom : MonoBehaviour, IPointerEnterHandler,IPointerClickHandler,IPointerExitHandler
{
    public TabGroup tabGroup;
    public Image blackground;
    public UnityEvent onTabSelect;
    public UnityEvent onTabDeselect;
    

    public void OnPointerClick(PointerEventData eventData)
    {
        tabGroup.OnTabSelected(this);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        tabGroup.OnTabEnter(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        tabGroup.OnTabExit(this);
    }

    // Start is called before the first frame update
    void Start()
    {
        blackground = GetComponent<Image>();
        tabGroup.Suscribe(this);
    }

    public void select()
    {
        if(onTabSelect!= null)
        {
            onTabSelect.Invoke();
        }
    }
    public void Deselect()
    {
        if (onTabDeselect != null)
        {
            onTabDeselect.Invoke();
        }
    }
    // Update is called once per frame
     void Update()
    {
        
    }
}
