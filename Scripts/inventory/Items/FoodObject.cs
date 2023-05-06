using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "New Food Object", menuName = "Inventory/Items/Food")]
public class FoodObject : ItemObject
{
    public int Energy;
    public void Awake()
    {
        type = ItemType.FOOD;
    }

}
