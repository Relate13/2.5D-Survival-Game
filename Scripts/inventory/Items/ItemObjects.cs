using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType {TOOL,FOOD,MATERIAL,DEFAULT }
public abstract class ItemObject : ScriptableObject
{
    public int ID;
    public Sprite uiDisplay;
    [TextArea(15, 20)]
    public string description;
    public ItemType type;
    public GameObject ItemEntity;
}
[System.Serializable]
public class Item
{
    //public string Name;
    public int ID;
    public Item()
    {
        //Name = "";
        ID = -1;
    }
    public Item(int id)
    {
        //Name = "";
        ID = id;
    }
    public Item(ItemObject item)
    {
        //Name = item.name;
        ID = item.ID;
    }
    public int Use()
    {
        return 0;
    }
}

