using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Furnace Recipe Database", menuName = "Crafting/Furnace/Database")]
public class FurnaceRecipeDatabase : ScriptableObject
{
    public FurnaceRecipe[] recipes;
}
