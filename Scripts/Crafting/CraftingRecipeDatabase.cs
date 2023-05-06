
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Crafting Recipe Database", menuName = "Crafting/Craft/Database")]
public class CraftingRecipeDatabase : ScriptableObject
{
    public CraftingRecipe[] recipes;
}
