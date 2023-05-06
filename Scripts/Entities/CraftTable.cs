using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftTable : Interactable
{
    public CraftingRecipeDatabase craftTableDatabase;
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
        GameDataHolder.getInstance().inventoryDisplayer.SwitchToolBarMode(false);
        GameDataHolder.getInstance().craftingMenu.AcceptOverrideDatabase(craftTableDatabase);
        GameDataHolder.getInstance().topMenuDisplayer.ShowPanel(0);
    }

    public override void LoadData()
    {
        return;
    }

    public override void SaveData()
    {
        return;
    }
}
