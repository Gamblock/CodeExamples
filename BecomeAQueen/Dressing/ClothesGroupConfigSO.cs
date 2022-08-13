using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ClothesGroupConfigSO", menuName = "_Game/ClothesGroupConfigSO", order = 0)]
public class ClothesGroupConfigSO : ScriptableObject
{
    public List<string> outfitsPaths;
    public List<string> makeUpsPaths;
    public List<string> hairColorsPaths;
    public string categoriesPath = "Dressing/Default/Categories";
    public CategoriesCollisionMatrix collisionMatrix;
}
