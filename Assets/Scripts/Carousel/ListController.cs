using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using AddComponent;
using UnityEngine.UI;
using System.Linq;

public class ListController : MonoBehaviour
{
    [SerializeField] private List list;

    [SerializeField] private ListItemBase listItem;

    // TODO: this should be done in the code later when loading items dynamically
    [SerializeField] private CarouselItemBlock theBlock;
    //

    private int selectedIndex;

    private ListItemPuppetItem selectedItem;

    private List<CarouselItemBlock> blocks = new List<CarouselItemBlock>();

    //private DataController dataController;

    void Awake()
    {
        // dataController = FindObjectOfType<DataController>();
    }

    void Start()
    {
        //var listClothes = dataController.GetClothesByType("Pants");
        var listClothes = DataController.Instance.GetClothesByType("Pants");
        //var listClothes = dataController.GetClothesByType("Shirts");
        //var listClothes = dataController.GetClothesByType("Hats");
        //var listClothes = dataController.GetClothesByType("Shoes");
        //var listClothes = dataController.GetClothesByType("Overalls");
        //var listClothes = dataController.GetClothesByType("Accesories");


        foreach (var cloth in listClothes)
        {
            CarouselItemBlock clone = Instantiate(theBlock, transform.position, transform.rotation);
            clone.onItemUpdated = HandleOnItemUpdated;

            StartCoroutine(DataController.Instance.LoadClothSprite(cloth, clone));
            //clone.SetSprite(sprite);
            blocks.Add(clone);
            Debug.Log("Cloth added: "+cloth.Name);
        }

        list.onItemLoaded = HandleOnItemLoadedHandler;     // called when an item is recycled
        list.onItemSelected = HandleOnItemSelectedHandler; // called when the item is selected
        
        list.Create(blocks.Count, listItem); // create the list with a number and a prefab
        list.gameObject.SetActive(true);
    }

    void HandleOnItemUpdated()
    {
        Debug.Log("HandleOnItemUpdated: ");
        list.Create(blocks.Count, listItem); // create the list with a number and a prefab
    }

    void HandleOnItemSelectedHandler(ListItemBase item) // reference to the selected list item
    {
        if (selectedItem != null)
        {
            selectedItem.Select(false);
        }

        selectedItem = (ListItemPuppetItem)item;
        selectedItem.Select(true);

        selectedIndex = selectedItem.Index;

#if UNITY_EDITOR || DEVELOPMENT_BUILD
        Debug.Log("Selected block's index | " + item.Index);
#endif
    }

    void HandleOnItemLoadedHandler(ListItemBase item) // reference to the loaded list item
    {
        if (item == (ListItemPuppetItem)selectedItem)
        {
            selectedItem.Select(selectedIndex == selectedItem.Index);
        }

        ListItemPuppetItem puppetItem = (ListItemPuppetItem)item;
        puppetItem.SetImage(blocks[item.Index].GetSprite());
        Debug.Log("Image was set. Index: " + item.Index);
    }
}