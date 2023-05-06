using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Crafting Recipe", menuName = "Crafting/Craft/Crafting Recipe")]
public class CraftingRecipe : ScriptableObject
{
    public List<RecipeSlot> inputs;
    public RecipeSlot output;
    public float burningTime;
    public int GetRecipeItemIndex() { return inputs[0].item.ID; }
}
