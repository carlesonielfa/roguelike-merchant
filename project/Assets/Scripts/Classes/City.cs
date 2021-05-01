using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class City
{
    public string name;
    public float probability;
    const int MAX_GOODS = 30;
    public readonly List<string> cityGoods = new List<string>(MAX_GOODS);

    public City()
    {

    }
    public City(City other)
    {
        this.name = other.name;
        this.probability = other.probability;
        this.Restock();
    }
    public void Restock()
    {
        //TODO change to not truly random
        foreach (string good in GameData.Instance.RandomGoods(name).Take((MAX_GOODS - cityGoods.Count) / 2))
        {
            cityGoods.Add(good);
            cityGoods.Add(good);
        }
    }
}