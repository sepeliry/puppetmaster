using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class CarouselItemBlock : MonoBehaviour
{
    [SerializeField] private Image img;

    public delegate void OnItemUpdated();

    public OnItemUpdated onItemUpdated;

    private void Awake()
    {
        Assert.IsNotNull(img, "Img was not bound to CarouselItemBlock");
    }

    public CarouselItemBlock ReturnThis()
    {
        return this;
    }

    public Sprite GetSprite()
    {
        return img.sprite;
    }

    public void SetSprite(Sprite sprite)
    {
        img.sprite = sprite;
        onItemUpdated();
    }

}
