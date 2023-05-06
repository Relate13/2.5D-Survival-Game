using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemTipSystem : MonoBehaviour
{
    // Start is called before the first frame update
    public static ItemTipSystem instance;
    public static ItemTipSystem getInstance() { return instance; }
    public ItemTip itemTip;
    public ItemTipSystem()
    {
        instance = this;
    }
    public void Start()
    {
        Hide();
    }
    public void Show()
    {
        itemTip.FollowMouse();
        itemTip.AjustSize();
        itemTip.gameObject.SetActive(true);
    }
    public void Hide()
    {
        itemTip.gameObject.SetActive(false);
    }
}
