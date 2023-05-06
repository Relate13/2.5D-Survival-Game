using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecipeDisplay : MonoBehaviour
{
    public static readonly int INPUT_COUNT = 4;
    public CraftingRecipe recipe;
    public GameObject[] Slot;
    public Color COLOR0;
    public Color COLOR1;

    public void SetRecipe(CraftingRecipe newRecipe)
    {
        recipe = newRecipe;
        UpdateDisplay();
    }
    public void UpdateDisplay()
    {
        InventoryObject inventory = GameDataHolder.getInstance().player.playerInventory;
        if (recipe == null)
        {
            foreach (var slot in Slot)
                slot.SetActive(false);
            return;
        }
        Sprite sprite;
        int number, i, itemID;
        for (i = 0; i < recipe.inputs.Count; i++)
        {
            itemID = recipe.inputs[i].item.ID;
            Slot[i].SetActive(true);
            sprite = GameDataHolder.getInstance().itemDatabase.GetItem[itemID].uiDisplay;
            number = recipe.inputs[i].amount;
            Slot[i].GetComponent<ItemIconDisplay>().SetItemStatus(sprite, number, itemID);
            if (!inventory.HasItem(itemID, number))
            {
                Slot[i].GetComponent<ItemIconDisplay>().SetTextColor(COLOR0);
            }
            else
            {
                Slot[i].GetComponent<ItemIconDisplay>().SetTextColor(COLOR1);
            }
        }
        while (i < INPUT_COUNT)
        {
            Slot[i].SetActive(false);
            ++i;
        }
        sprite = GameDataHolder.getInstance().itemDatabase.GetItem[recipe.output.item.ID].uiDisplay;
        number = recipe.output.amount;
        Slot[4].GetComponent<ItemIconDisplay>().SetItemStatus(sprite, number, recipe.output.item.ID);
    }
    public void Craft()
    {
        Player player = GameDataHolder.getInstance().player;
        bool hasEnoughItem = true;
        foreach (var inputItem in recipe.inputs)
        {
            if (!player.playerInventory.HasItem(inputItem.item.ID, inputItem.amount))
            {
                hasEnoughItem = false;
                break;
            }
        }
        if (hasEnoughItem)
        {
            if(player.playerInventory.AddItem(recipe.output.item.ID, recipe.output.amount))
            {
                foreach(var inputItem in recipe.inputs)
                {
                    player.playerInventory.RemoveItem(inputItem.item.ID, inputItem.amount);
                }
                SoundManager.GetInstance().PlaySound(GameDataHolder.getInstance().CraftSucceed);
                Debug.Log("Craft Option Succeed");
            }
            else MessageSystem.GetInstance().NewWarningMessage("物品栏已满");
        }
        else MessageSystem.GetInstance().NewErrorMessage("缺少合成所需物品");
    }
}
