using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NPCBuyItem : MonoBehaviour
{
    public ItemIconDisplay icon;
    public TextMeshProUGUI ButtonText;
    public int Price;
    public int ItemID;
    // Start is called before the first frame update
    public void SetTrade(TradeItem trade)
    {
        ItemID = trade.item.ID;
        Price = trade.TradeValue;
        icon.SetItemStatus(trade.item.uiDisplay, 1, ItemID);
        ButtonText.text = Price.ToString();
    }
    public void Buy()
    {
        InventoryObject inventory = GameDataHolder.getInstance().player.playerInventory;
        if (inventory.TryBuy(Price))
        {
            if (inventory.AddItem(ItemID, 1))
            {
                MessageSystem.GetInstance().NewTipMessage("交易成功"); 
            }
            else
                MessageSystem.GetInstance().NewWarningMessage("物品栏已满");
        }
        else
        {
            MessageSystem.GetInstance().NewWarningMessage("没有足够的货币");
        }
    }
}
