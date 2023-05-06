using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingSystem : MonoBehaviour, Observer
{
    public CraftingRecipeDatabase OverrideRecipeDatabase;
    public CraftingRecipeDatabase OriginalRecipeDatabase;
    public GameObject recipeDisplayPrefab;
    public GameObject OptionCanvas;
    public int distanceX;
    public int distanceY;
    public List<GameObject> CraftingOptions;
    public void AcceptOverrideDatabase(CraftingRecipeDatabase craftTableDatabase)
    {
        OverrideRecipeDatabase = craftTableDatabase;
        GenerateCraftOptions();
    }
    public void ClearOverrideDatabase()
    {
        OverrideRecipeDatabase = null;
        GenerateCraftOptions();
    }
    public void GenerateCraftOptions()
    {
        foreach(var option in CraftingOptions)
        {
            Destroy(option);
        }
        CraftingRecipeDatabase CurrentDatabase;
        if (OverrideRecipeDatabase == null)
        {
            CurrentDatabase = OriginalRecipeDatabase;
        }
        else
        {
            CurrentDatabase = OverrideRecipeDatabase;
        }
        for(int i = 0; i < CurrentDatabase.recipes.Length; i++)
        {
            GameObject obj = Instantiate(recipeDisplayPrefab, Vector3.zero, Quaternion.identity, OptionCanvas.transform);
            obj.GetComponent<RectTransform>().anchoredPosition = CalPosition(i);
            obj.GetComponent<RecipeDisplay>().SetRecipe(CurrentDatabase.recipes[i]);
            CraftingOptions.Add(obj);
        }
    }
    public Vector3 CalPosition(int i)
    {
        return new Vector3(distanceX, -i * distanceY, 0);
    }
    
    public void Start()
    {
        GenerateCraftOptions();
    }
    //public void Update()
    //{
    //    GenerateCraftOptions();
    //}
    public void ObserverUpdate()
    {
        GenerateCraftOptions();
    }
}
