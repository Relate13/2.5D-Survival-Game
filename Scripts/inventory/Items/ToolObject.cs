using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "New Tool Object", menuName = "Inventory/Items/Tool")]
public class ToolObject : ItemObject
{
    public int Damage;
    public void Awake()
    {
        type = ItemType.TOOL;
    }

}
