using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RecipeSlot
{
    public ItemObject item;
    public int amount;
}
[CreateAssetMenu(fileName = "Furnace Recipe", menuName = "Crafting/Furnace/Furnace Recipe")]
public class FurnaceRecipe : ScriptableObject
{
    public List<RecipeSlot> inputs;
    public RecipeSlot output;
    public float burningTime;
    public int GetRecipeItemIndex() { return inputs[0].item.ID; }
}
