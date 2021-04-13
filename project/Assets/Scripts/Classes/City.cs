using System.Collections;
using System.Collections.Generic;
using System.Linq;
public class City
{
    const int MAX_GOODS = 30;
    List<string> cityGoods = new List<string>(MAX_GOODS);
    public City()
    {
        Restock();
        //
    }

    public List<string> CityGoods { get => cityGoods;}

    public void Restock()
    {
        //TODO change to not truly random
        foreach(string good in GameData.Instance.RandomGoods().Take(MAX_GOODS - CityGoods.Count))
        {
            CityGoods.Add(good);
        }
    }
}
