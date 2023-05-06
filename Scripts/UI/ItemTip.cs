using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
[ExecuteInEditMode()]
public class ItemTip : MonoBehaviour
{
    public RectTransform rectTransform;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI discriptionText;
    public LayoutElement layOutElement;
    public int wrapLimit;
    public void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }
    public void SetTitle(string title)
    {
        titleText.text = title;
    }
    public void SetDiscription(string discription)
    {
        discriptionText.text = discription;
    }
    void Update()
    {
        AjustSize();
        FollowMouse();
    }
    public void AjustSize()
    {
        if (titleText.text.Length > wrapLimit || discriptionText.text.Length > wrapLimit)
        {
            layOutElement.enabled = true;
        }
        else layOutElement.enabled = false;
    }
    public void FollowMouse()
    {
        Vector2 pos = Input.mousePosition;
        Vector2 pivot = new Vector2(pos.x / Screen.width, pos.y / Screen.height);
        rectTransform.pivot = pivot;
        rectTransform.position = pos;
    }
}
