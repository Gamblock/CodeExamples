using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class DressingUpGridLayoutHelper
{
    public int paddingLeft;
    public int paddingRight;
    public int paddingTop;
    public int paddingBottom;
    [Space(4f)]
    public Vector2 cellSize;
    public Vector2 spacing;
    [Space(4f)]
    public GameObject textOverlayToActivate;
    public GameObject textOverlayToDeactivate;
    [Space(4f)] 
    public Vector2 imageSize;

    public void ApplySettingForGridLayoutGroup(GridLayoutGroup gridLayoutGroup)
    {
        gridLayoutGroup.padding = new RectOffset(paddingLeft, paddingRight, paddingTop, paddingBottom);
        gridLayoutGroup.cellSize = cellSize;
        gridLayoutGroup.spacing = spacing;
        textOverlayToActivate.SetActive(true);
        textOverlayToDeactivate.SetActive(false);
    }
}
