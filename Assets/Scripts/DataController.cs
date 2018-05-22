using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using System;


// Streaming Assets folder always contains file named asset_resource_data.json
// This file contains collection name and path
// 
//
// Each collection is located at own folder and in this folder is always collection.json file
// ex. /basic/collection.json
//
//

public class DataController : Singleton<DataController> {

    private string gameDataFileName = "asset_resource_data.json";
    public GameData gameData;
    public CollectionData currentCollection;
    private Sprite TestSprite;
    private bool isLoaded = false;
    object lockObject = new object();

    public bool IsLoaded()
    {
        return isLoaded;
    }


    void OnEnable()
    {
        Debug.Log("DataController got enabled");
        StartCoroutine(StartLoading());
    }

    IEnumerator LoadGameData()
    {
        string filePath = Path.Combine(Application.streamingAssetsPath + "/", gameDataFileName);
        Debug.Log("UNITY: (Start LoadGameData) " + filePath);

        string dataAsJson = "";
        yield return StartCoroutine(LoadJsonData(filePath, result => dataAsJson = result));

        GameData loadedData = JsonUtility.FromJson<GameData>(dataAsJson);
        gameData = loadedData;

        Debug.Log("UNITY: (End LoadGameData) ");

    }

    IEnumerator StartLoading()
    {
        yield return StartCoroutine(LoadGameData());
        yield return StartCoroutine(LoadCollection("basic"));
    }

    IEnumerator LoadJsonData(string filePath, Action<string> dataAsJson)
    {
        if (filePath.Contains("://") || filePath.Contains(":///"))
        {
            Debug.Log("UNITY: (Start LoadJsonData) " + filePath);
            WWW www = new WWW(filePath);
            //while (!www.isDone)
            //{ }
            yield return www;
            dataAsJson(www.text);
            Debug.Log("UNITY: (End LoadJsonData) " + www.text);
        }
        else
        {
            dataAsJson(File.ReadAllText(filePath));
            yield return null;
        }
    }

    IEnumerator LoadByteData(string filePath, Action<byte[]> bytes)
    {
        if (filePath.Contains("://") || filePath.Contains(":///"))
        {
            Debug.Log("UNITY: (Start LoadByteData) " + filePath);
            WWW www = new WWW(filePath);
            //while (!www.isDone)
            //{ }
            yield return www;
            bytes(www.bytes);
            Debug.Log("UNITY: (End LoadByteData) " + www.bytes.Length);
        }
        else
        {
            bytes(File.ReadAllBytes(filePath));
            yield return null;
        }
    }

    IEnumerator LoadTexture(string filePath, Action<Texture2D> texture)
    {
        Texture2D tex = new Texture2D(2,2);
        if (filePath.Contains("://") || filePath.Contains(":///"))
        {
            Debug.Log("UNITY: (Start LoadTexture) " + filePath);
            WWW www = new WWW(filePath);
            //while (!www.isDone)
            //{ }
            yield return www;
            www.LoadImageIntoTexture(tex);
            texture(tex);
            Debug.Log("UNITY: (End LoadTexture) ");
        }
        else
        {
            var bytes = File.ReadAllBytes(filePath);
            tex.LoadImage(bytes); //..this will auto-resize the texture dimensions.
            texture(tex);
            yield return null;
        }
    }

    IEnumerator LoadCollection(string collectionName)
    {
        Debug.Log("UNITY: (Start LoadCollection) ");

        foreach (AssetResourceData asset_resource in gameData.AssetResources)
        {
            if (asset_resource.Name == collectionName)
            {
                string filePath = Application.streamingAssetsPath + asset_resource.Path + "collection.json";

                string dataAsJson = "";
                yield return StartCoroutine(LoadJsonData(filePath, result => dataAsJson = result));

                CollectionData loadedData = JsonUtility.FromJson<CollectionData>(dataAsJson);
                currentCollection = loadedData;
            }
        }
        isLoaded = true;
        Debug.Log("UNITY: (End LoadCollection) ");


    }

    IEnumerator WaitLoadingFinish()
    {
        Debug.Log("UNITY: (Start WaitLoadingFinish) " + isLoaded.ToString());
        yield return new WaitForSeconds(0.1f);
        Debug.Log("UNITY: (End WaitLoadingFinish) " + isLoaded.ToString());
    }

    public List<CollectionItemData> GetClothesByType(string clothType)
    {
        Debug.Log("UNITY: (Start GetClothesByType) ");

        List<CollectionItemData> clothes = new List<CollectionItemData>();

        foreach (CollectionItemData item in currentCollection.Items)
        {
            if (item.Type == clothType)
            {
                clothes.Add(item);
            }
        }

        Debug.Log("UNITY: (End GetClothesByType) ");

        return clothes;
    }

    public IEnumerator LoadClothSprite(CollectionItemData cloth, CarouselItemBlock block)
    {
        string filePath = Application.streamingAssetsPath + currentCollection.Path + cloth.Id + ".png";

        // Load a PNG or JPG image from disk to a Texture2D, assign this texture to a new sprite and return its reference
        Sprite newSprite = new Sprite();

        float PixelsPerUnit = 100.0f;
        Texture2D spriteTexture = new Texture2D(2, 2);
        byte[] fileData = null;

        Debug.Log("UNITY: (Start LoadClothSprite) ");

        yield return StartCoroutine(LoadTexture(filePath, result => spriteTexture = result));
        
        newSprite = Sprite.Create(spriteTexture, new Rect(0, 0, spriteTexture.width, spriteTexture.height), new Vector2(0, 0), PixelsPerUnit);
        newSprite.name = "Loaded sprite";
        block.SetSprite(newSprite);

        Debug.Log("UNITY: (End LoadClothSprite) ");

    }

    public void SaveTestGameData()
    {
        // test asset resource data file generation
        GameData testGameData = new GameData();

        AssetResourceData assetResource = new AssetResourceData();
        assetResource.Name = "basic";
        assetResource.Path = "/collections/basic/";
        testGameData.AssetResources = new AssetResourceData[3];
        testGameData.AssetResources[0] = assetResource;

        string dataAsJson = JsonUtility.ToJson(testGameData);

        string filePath = Application.streamingAssetsPath + "//" + gameDataFileName;
        File.WriteAllText(filePath, dataAsJson);


        // test collection data file generation
        CollectionData collectionData = new CollectionData();
        collectionData.Path = "/collections/basic/";
        collectionData.Author = "Fashion Guru";
        collectionData.Gender = "Male";
        collectionData.CollectionName = "Summer Breeze";
        collectionData.Items = new CollectionItemData[21];

        int index = 0;

        // pants
        collectionData.Items[index] = AddCollectionItem("d91a82db-52fa-4759-b23a-651b65d240d2", "Pants", "Army Pants", "Stripes", 2, 5, 1, 1);
        collectionData.Items[++index] = AddCollectionItem("3c231492-91d2-419f-91fe-798577b85d36", "Pants", "Green Pants", "None", 5, 1, 1, 1);
        collectionData.Items[++index] = AddCollectionItem("82ed2ccb-04de-4f1a-b676-b63581418dde", "Pants", "Lime Pants", "Arrows", 2, 1, 1, 1);
        collectionData.Items[++index] = AddCollectionItem("78829372-9d75-411e-87bb-f3895f02cda1", "Pants", "Beach shorts", "Stripes", 2, 4, 1, 1);
        collectionData.Items[++index] = AddCollectionItem("e6a5e236-5fdf-4b23-b381-a04f7995c854", "Pants", "Lovely boxers", "None", 1, 1, 1, 1);
        collectionData.Items[++index] = AddCollectionItem("8ab2fbdf-93b0-4feb-9c1d-4b677eeb6be8", "Pants", "Hawaii shorts", "None", 3, 1, 1, 1);

        // shirts
        collectionData.Items[++index] = AddCollectionItem("747f8fbc-df76-4046-ac29-1190e87c7791", "Shirts", "Skater's T-shirt", "None", 1, 1, 1, 1);
        collectionData.Items[++index] = AddCollectionItem("892aeee8-9992-4e76-a5c3-e4be5a3d5195", "Shirts", "Neutral T-shirt", "None", 5, 1, 1, 1);
        collectionData.Items[++index] = AddCollectionItem("a9294f9e-23b0-4846-85b7-1fdfb16b6325", "Shirts", "Relax Hoodie", "None", 1, 1, 1, 1);

        // hats
        collectionData.Items[++index] = AddCollectionItem("0a2b6321-36e7-4d87-9ce5-ea74fa85842c", "Hats", "Playfull cap", "Stripes", 1, 3, 1, 1);
        collectionData.Items[++index] = AddCollectionItem("5e770268-4624-42ea-ab66-5c8e7ab69b9a", "Hats", "Sailor Woolly hat", "None", 1, 3, 1, 1);

        // shoes
        collectionData.Items[++index] = AddCollectionItem("ba4610b8-7c39-4e83-b6df-ab3e193d9829", "Shoes", "Happy Sneakers", "None", 3, 3, 1, 1);
        collectionData.Items[++index] = AddCollectionItem("a056c8e1-655c-40ed-8428-26882c2948b9", "Shoes", "Mountain Style", "None", 2, 1, 1, 1);

        // overalls
        collectionData.Items[++index] = AddCollectionItem("009b0dcf-65da-4e9d-a1af-018b453da5fe", "Overalls", "Buttler suit", "None", 5, 1, 1, 1);
        collectionData.Items[++index] = AddCollectionItem("60f4f08e-d2d9-48be-aac5-fad41830eb67", "Overalls", "Working wear", "None", 1, 1, 1, 1);
        collectionData.Items[++index] = AddCollectionItem("892fcb49-fdc0-4526-8b16-9378327fcf10", "Overalls", "Lounge suit", "None", 5, 1, 1, 1);
        collectionData.Items[++index] = AddCollectionItem("27759f0c-7803-4933-9644-86b94f9d0993", "Overalls", "Night wear", "Stripes", 1, 1, 1, 1);

        // accessories
        collectionData.Items[++index] = AddCollectionItem("9a663c76-4c80-4b94-93a7-93771b0e3edd", "Accesories", "Punk bracelet", "None", 1, 1, 1, 1);
        collectionData.Items[++index] = AddCollectionItem("26943f2a-ff0e-4cf0-b54b-90eccd1a98e9", "Accesories", "Casual clock", "None", 3, 1, 1, 1);
        collectionData.Items[++index] = AddCollectionItem("56937a0f-0c4b-497a-87d6-8ba12f3ba864", "Accesories", "Stylish clock", "None", 5, 1, 1, 1);
        collectionData.Items[++index] = AddCollectionItem("68023f21-b99f-41dc-a281-0316748cbb81", "Accesories", "Retro Eyewear", "None", 3, 2, 1, 1);

        dataAsJson = JsonUtility.ToJson(collectionData);

        filePath = Application.streamingAssetsPath + collectionData.Path + "collection.json";
        File.WriteAllText(filePath, dataAsJson);

    }

    CollectionItemData AddCollectionItem(string id, string type, string name, string pattern, int formality, int sportyness, double anchorX, double anchorY)
    {
        CollectionItemData collectionItemData = new CollectionItemData();
        collectionItemData.Id = id;
        collectionItemData.Type = type;
        collectionItemData.Name = name;
        collectionItemData.Pattern = pattern;
        collectionItemData.Formality = formality;
        collectionItemData.Sportyness = sportyness;

        CollectionItemAnchorData anchor = new CollectionItemAnchorData();
        anchor.X = anchorX;
        anchor.Y = anchorY;
        collectionItemData.Anchor = anchor;

        return collectionItemData;
    }

}

