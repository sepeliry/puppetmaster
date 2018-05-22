using UnityEngine;
using System.Collections;
using AddComponent;
using UnityEngine.UI;

public class ListItemPuppetItem : ListItemBase
{
    [SerializeField] private Image img;
    [SerializeField] private AspectRatioFitter aspectRatioFitter;

    public void SetImage(Sprite sprite)
    {
        aspectRatioFitter.aspectRatio = (float)sprite.texture.width / (float)sprite.texture.height;
        this.img.sprite = sprite;
    }

    public void Select(bool selected)
    {
        // TODO change some border/background when is selected?
    }

    /* Using to spawn item for dragging? */
    public void LongPressed()
    {
        Debug.Log("Long pressed. Item index: " + Index);
    }
}
