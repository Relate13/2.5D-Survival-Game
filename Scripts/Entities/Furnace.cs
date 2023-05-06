using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum FurnaceStatus {IDLE,WORKING }

public class Furnace : Interactable
{
    FurnaceStatus status = FurnaceStatus.IDLE;
    RecipeSlot FurnaceOutput;
    public FurnaceRecipeDatabase database;
    public Sprite[] texture;
    public override EntityControlUnit GenerateECU(int mode)
    {
        EntityControlUnit ecu = new EntityControlUnit();
        ecu.entityId = EntityID;
        ecu.data = "";
        return ecu;
    }

    public override void Harvest()
    {
        GameObject trophy = Instantiate(trophies[0], transform.position, Quaternion.identity);
        trophy.GetComponent<Rigidbody>().AddForce(new Vector3(0, 100, 0));
        SelfDestruct();
    }

    public override void Interact(Player player)
    {
        if (status == FurnaceStatus.IDLE)
        {
            int selectedItemiD = player.playerInventory.GetSelected();
            bool recipeFound = false;
            foreach (var recipe in database.recipes)
            {
                if (recipe.GetRecipeItemIndex() == selectedItemiD)
                {
                    recipeFound = true;
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
                        foreach (var inputItem in recipe.inputs)
                        {
                            player.playerInventory.RemoveItem(inputItem.item.ID, inputItem.amount);
                        }
                        Debug.Log("Furnace Option Succeed");
                        FurnaceBurn(recipe.burningTime, recipe.output);
                        SoundManager.GetInstance().PlaySound(GameDataHolder.getInstance().FurnaceStart);
                        break;
                    }
                    else MessageSystem.GetInstance().NewWarningMessage("没有足够的熔炼材料或燃料(熔炼至少需要5块矿物与1块燃料)");
                }
                
            }
            if (!recipeFound)
            {
                MessageSystem.GetInstance().NewWarningMessage("没有该物品对应的熔炼配方");
            }
        }
        else MessageSystem.GetInstance().NewWarningMessage("熔炉正在工作中");
    }
    public void FurnaceBurn(float duration,RecipeSlot output)
    {
        StartBurning();
        FurnaceOutput = output;
        Invoke("StopBurning", duration);
    }
    public void StartBurning()
    {
        status = FurnaceStatus.WORKING;
        GetComponent<SpriteRenderer>().sprite = texture[1];
    }
    public void StopBurning()
    {
        status = FurnaceStatus.IDLE;
        GetComponent<SpriteRenderer>().sprite = texture[0];
        if (FurnaceOutput != null)
        {
            Instantiate(FurnaceOutput.item.ItemEntity, transform.position + new Vector3(0, 0, -0.5f), Quaternion.identity);
            SoundManager.GetInstance().PlaySound(GameDataHolder.getInstance().FurnaceEnd);
            FurnaceOutput = null;
        }
    }
    public override void LoadData()
    {
        return;
        //throw new System.NotImplementedException();
    }

    public override void SaveData()
    {
        return;
        //throw new System.NotImplementedException();
    }
}
