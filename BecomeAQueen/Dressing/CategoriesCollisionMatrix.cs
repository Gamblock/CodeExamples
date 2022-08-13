using System;
using System.Collections.Generic;
using ComfortGames.CharacterCustomization;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;

[Serializable]
public class CategoryCell
{
    public OutfitCategoryScriptableObject categoryFirst;
    public OutfitCategoryScriptableObject categorySecond;
    public bool shouldCollide;

    public CategoryCell(OutfitCategoryScriptableObject category1, OutfitCategoryScriptableObject category2, bool b)
    {
        categoryFirst = category1;
        categorySecond = category2;
        shouldCollide = b;
    }
}

[CreateAssetMenu(fileName = "CollisionMatrixSO", menuName = "_Game/CollisionMatrixSO", order = 0)]
public class CategoriesCollisionMatrix : SerializedScriptableObject
{
    private List<OutfitCategoryScriptableObject> categories;
    
    public ClothesGroupConfigSO clothesContfigSO;
    //[HideInInspector] public List<(ClothesCategory, ClothesCategory)> collisionList = new List<(ClothesCategory, ClothesCategory)>();
    [HideInInspector] public List<ClothesCategory> collisionListFirst = new List<ClothesCategory>();
    [HideInInspector] public List<ClothesCategory> collisionListSecond = new List<ClothesCategory>();
    
    [TableMatrix(DrawElementMethod = "DrawCell", RowHeight = 50)]
    public CategoryCell[,] matrixDrawer;
    public static CategoryCell DrawCell(Rect rect, CategoryCell value)
    {
        GUIStyle labelStyle = new GUIStyle(GUI.skin.label);
        GUIStyle toggleStyle = new GUIStyle(GUI.skin.toggle);
        labelStyle.alignment = TextAnchor.UpperCenter;
        if (value.categoryFirst != null)
        {
            GUI.Label(rect, value.categoryFirst.categoryName, labelStyle);
        }
        labelStyle.alignment = TextAnchor.MiddleCenter;
        if (value.categoryFirst != null)
        {
            GUI.Label(rect, value.categorySecond.categoryName, labelStyle);
        }
        toggleStyle.alignment = TextAnchor.LowerCenter;
        var toggle = GUI.Toggle(rect, value.shouldCollide, "");
        value.shouldCollide = toggle;
        return value;
    }
    
    [Button(ButtonSizes.Medium)]
    public void Repopulate()
    {
        matrixDrawer = new CategoryCell[categories.Count, categories.Count];
        categories = GetCategoryScriptableObjects(clothesContfigSO.categoriesPath);
        for (var i = 0; i < categories.Count; i++)
        {
            for (var j = 0; j < categories.Count; j++)
            {
                if (j >= i)
                {
                    matrixDrawer[i, j] = new CategoryCell(null, null, false);
                }
                else
                {
                    matrixDrawer[i,j] = new CategoryCell(categories[i], categories[j], false);

                }
            }
        }
    }

    [Button(ButtonSizes.Medium)]
    public void SaveMatrix()
    {
        categories = GetCategoryScriptableObjects(clothesContfigSO.categoriesPath);
        collisionListFirst = new List<ClothesCategory>();
        collisionListSecond = new List<ClothesCategory>();
        for (int i = 0; i <categories.Count; i++)
        for (int j = 0; j < categories.Count; j++)
        {
            if (matrixDrawer[i,j].categoryFirst != null 
                && matrixDrawer[i,j].categorySecond != null
                && matrixDrawer[i,j].shouldCollide)
            {
                collisionListFirst.Add(matrixDrawer[i, j].categoryFirst.clothesCategory);
                collisionListSecond.Add(matrixDrawer[i, j].categorySecond.clothesCategory);
            }
        }
    }
    

    private List<OutfitCategoryScriptableObject> GetCategoryScriptableObjects(string path)
    {
        return new List<OutfitCategoryScriptableObject>(Resources.LoadAll<OutfitCategoryScriptableObject>(path));
    }
}