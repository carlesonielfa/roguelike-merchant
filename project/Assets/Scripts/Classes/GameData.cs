using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VectorGraphics;
using System.IO;
using System.Linq;

[System.Serializable]
public class GameData
{
    static string dataJSONPath = "Data/data";

    static string goodsIconsPath = "Icons/Goods/";
    public Good[] goods;
    private Dictionary<string, System.Tuple<Good, GameObject>> goodsDict;

    //TODO: rework this area, we dont need this much memory
    static string citiesIconsPath = "Icons/Cities/";
    public City[] cities;
    private Dictionary<string, System.Tuple<City, GameObject>> citiesDict;
    private GameObject cityPrefab;

    private Dictionary<string, int> playerInventory;

    public delegate void InventoryChangeEvent(System.Tuple<string, int> updatedValue);
    public event InventoryChangeEvent OnInventoryChanged; 

    //Singleton pattern
    private static GameData instance;
    private GameData(){
        playerInventory = new Dictionary<string, int>();
    }
    public static GameData Instance
    {
        get
        {
            if (instance == null)
                LoadDataFromJSON();
            return instance;
        }
    }
    public void AddPlayerGood(string goodName, int amount)
    {
        if (!playerInventory.ContainsKey(goodName))
        {
            playerInventory.Add(goodName, amount);
        }
        else
        {
            playerInventory[goodName] += amount;
        }
        OnInventoryChanged?.Invoke(new System.Tuple<string,int>(goodName, playerInventory[goodName]));
    }
    public void RemovePlayerGood(string goodName, int amount)
    {
        int goodQuantity = 0;
        if (playerInventory.ContainsKey(goodName))
        {
            playerInventory[goodName] -= amount;
            goodQuantity = playerInventory[goodName];
            if (goodQuantity < 0)
                Debug.LogWarning("Player good amount went negative!");
            if (goodQuantity <= 0)
            {
                playerInventory.Remove(goodName);
                goodQuantity = 0;
            }
        }
        else
        {
            Debug.LogError("Tried to remove a good the player doesn't have!");
        }
        OnInventoryChanged?.Invoke(new System.Tuple<string, int>(goodName, goodQuantity));
    }
    
    private static void LoadDataFromJSON()
    {
        TextAsset json = Resources.Load<TextAsset>(dataJSONPath);
        string jsonString = json.text;
        instance = JsonUtility.FromJson<GameData>(jsonString);
        if(instance.goods.Length == 0 || instance.cities.Length == 0)
        {
            Debug.LogError("No data was loaded");
            instance = null;
        }
        else
        {
            LoadGoods();
            LoadCities();
            instance.cityPrefab = Resources.Load<GameObject>("Prefabs/City");
        }

    }
    private static void LoadCities()
    {
        instance.citiesDict = new Dictionary<string, System.Tuple<City, GameObject>>();
        foreach (City item in instance.cities)
        {
            GameObject g = Resources.Load<GameObject>(citiesIconsPath + item.name);
            if (g == null)
                Debug.LogWarning("City " + item.name + " not found in resources, skipping");
            else
            {
                System.Tuple<City, GameObject> tuple = new System.Tuple<City, GameObject>(item, g);
                instance.citiesDict.Add(item.name, tuple);
            }
        }
        Debug.Log("Loaded " + instance.citiesDict.Count + "/" + instance.cities.Length + " cities");
    }
    private static void LoadGoods()
    {
        instance.goodsDict = new Dictionary<string, System.Tuple<Good, GameObject>>();
        foreach (Good item in instance.goods)
        {
            GameObject g = Resources.Load<GameObject>(goodsIconsPath + item.name);
            if (g == null)
                Debug.LogWarning("Good " + item.name + " not found in resources, skipping");
            else
            {
                System.Tuple<Good, GameObject> tuple = new System.Tuple<Good, GameObject>(item, g);
                instance.goodsDict.Add(item.name, tuple);
            }
        }
        Debug.Log("Loaded " + instance.goodsDict.Count + "/" + instance.goods.Length + " goods");
    }
    public GameObject GetGoodGameObjectInstance(string name)
    {
        //Must set parent after
        var t = goodsDict[name];
        GameObject g = t.Item2;
        GameObject gameObjectInstance = GameObject.Instantiate(g);
        gameObjectInstance.AddComponent<GoodComponent>();
        gameObjectInstance.GetComponent<GoodComponent>().good = t.Item1;
        return gameObjectInstance;
    }
    public GameObject GetGoodGameObjectPrefab(string name)
    {
        return goodsDict[name].Item2;
    }
    public GameObject GetRandomCity()
    {
        //Could be optimized using yield
        var values = citiesDict.Values.ToArray();
        var t = values[Random.Range(0, values.Length)];
        GameObject g = t.Item2;
        GameObject gameObjectInstance = GameObject.Instantiate(cityPrefab);
        CityComponent cityComponent = gameObjectInstance.GetComponent<CityComponent>();
        cityComponent.SetImage(g.GetComponent<SpriteRenderer>().sprite);
        cityComponent.city = new City(t.Item1);
        return gameObjectInstance;
    }
    public IEnumerable<string> RandomGoods(string cityType)
    {
        //System.Random rand = new System.Random();
        //string[] list = goodsDict.Keys.ToArray();
        //int size = list.Length;
        //while (true)
        //{
        //    yield return list[rand.Next(size)];
        //}
        IEnumerable<System.Tuple<Good,GameObject>> goodsCity = goodsDict.Values.SkipWhile(g => !g.Item1.spawns_in.Contains("all") && !g.Item1.spawns_in.Contains(cityType));
        while (true)
        {
            yield return goodsCity.RandomElementByWeight(e => e.Item1.probability).Item1.name;
        }
    }
}
