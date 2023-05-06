using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;

public class ItemIconDisplay : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private static LTDescr delay;
    public Image Icon;
    public TextMeshProUGUI Text;
    public int ItemID = -1;
    public void SetItemStatus(Sprite sprite,int number,int ItemID)
    {
        SetIcon(sprite);
        SetNumber(number);
        this.ItemID = ItemID;
        Debug.Log("Set to " + ItemID);
    }
    public void SetIcon(Sprite sprite)
    {
        Icon.sprite = sprite;
        Icon.color = new Color(1, 1, 1, 1);
    }
    public void SetNumber(int number)
    {
        if (number <= 1)
            Text.text = "";
        else
            Text.text = number.ToString("n0");
    }
    public void SetTextColor(Color color)
    {
        Text.color = color;
    }
    public void SetEmpty()
    {
        Icon.sprite = null;
        Icon.color = new Color(1, 1, 1, 0);
        Text.text = "";
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (ItemID >= 0)
        {
            delay = LeanTween.delayedCall(0.5f, () =>
            {
                ItemTipSystem tipSystem = ItemTipSystem.getInstance();
                tipSystem.itemTip.SetTitle(GameDataHolder.getInstance().itemDatabase.GetItem[ItemID].name);
                tipSystem.itemTip.SetDiscription(GameDataHolder.getInstance().itemDatabase.GetItem[ItemID].description);
                tipSystem.Show();
            });
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (ItemID >= 0)
        {
            LeanTween.cancel(delay.uniqueId);
            ItemTipSystem.getInstance().Hide();
        }
    }
    public void OnDestroy()
    {
        if (ItemID >= 0)
        {
            if(delay != null)
                LeanTween.cancel(delay.uniqueId);
        }
    }
}
