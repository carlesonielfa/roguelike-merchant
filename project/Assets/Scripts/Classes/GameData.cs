using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;

[System.Serializable]
public class GameData
{
    static string goodsJSONPath = Application.dataPath + "/Data/data.json";
    static string goodsIconsPath = "Icons/Goods/";
    public Good[] goods;
    private Dictionary<string, System.Tuple<Good, GameObject>> goodsDict;

    //Singleton pattern
    private static GameData instance;

    public static GameData Instance
    {
        get
        {
            if (instance == null)
                LoadDataFromJSON();
            return instance;
        }
    }

    
    private static void LoadDataFromJSON()
    {
        string jsonString = File.ReadAllText(goodsJSONPath);
        instance = JsonUtility.FromJson<GameData>(jsonString);
        if(instance.goods.Length == 0)
        {
            Debug.LogError("No goods were loaded");
            instance = null;
        }
        else
        {
            
            instance.goodsDict = new Dictionary<string, System.Tuple<Good, GameObject>>();
            //TODO: This could be threaded
            foreach(Good item in instance.goods)
            {
                GameObject g = Resources.Load<GameObject>(goodsIconsPath + item.name);
                if (g == null)
                    Debug.LogWarning("Good " + item.name + " not found in resources, skipping");
                else
                {
                    System.Tuple<Good, GameObject> tuple = new System.Tuple<Good, GameObject>(item,g);
                    instance.goodsDict.Add(item.name, tuple);
                }
            }
            Debug.Log("Loaded " + instance.goodsDict.Count + "/" + instance.goods.Length + " goods");
        }
        
    }
    public GameObject GetGoodGameObject(string name)
    {
        //Must set parent after
        var t = goodsDict[name];
        GameObject g = t.Item2;
        GameObject gameObjectInstance = GameObject.Instantiate(g);
        gameObjectInstance.AddComponent<GoodComponent>();
        gameObjectInstance.GetComponent<GoodComponent>().good = t.Item1;
        return gameObjectInstance;
    }
    public IEnumerable<string> RandomGoods()
    {
        System.Random rand = new System.Random();
        string[] list = goodsDict.Keys.ToArray();
        int size = list.Length;
        while (true)
        {
            yield return list[rand.Next(size)];
        }
    }
}
