using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class ItemTipTrigger : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        ItemTipSystem.getInstance().Show();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ItemTipSystem.getInstance().Hide();
    }
}
