using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RecipeRequireItem
{
    public ItemData itemData;
    public int amount;
}

[CreateAssetMenu(menuName = "ScriptableObject/Recipe Model")]
public class RecipeModel : ScriptableObject
{
    public RecipeRequireItem[] requireItems;

    public ItemData createdItem;
    public int createdAmount;
}
