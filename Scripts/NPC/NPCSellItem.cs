using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NPCSellItem : MonoBehaviour
{
    public ItemIconDisplay icon;
    public int Price;
    public int ItemID;
    public TextMeshProUGUI ButtonText;
    // Start is called before the first frame update
    public void SetTrade(TradeItem trade)
    {
        ItemID = trade.item.ID;
        Price = trade.TradeValue;
        icon.SetItemStatus(trade.item.uiDisplay, 1, ItemID);
        ButtonText.text = Price.ToString();
    }
    public void Sell()
    {
        if (GameDataHolder.getInstance().player.playerInventory.HasItem(ItemID, 1))
        {
            InventoryObject inventory = GameDataHolder.getInstance().player.playerInventory;
            inventory.RemoveItem(ItemID, 1);
            inventory.AddMoney(Price);
            MessageSystem.GetInstance().NewTipMessage("交易成功");
        }
        else
        {
            MessageSystem.GetInstance().NewWarningMessage("你没有所需物品");
        }
    }
}
