using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotMachineController : MonoBehaviour
{
    const int nRows = 5;
    List<string> inventory;
    List<GameObject>[] rowContents;

    public void Start()
    {
        rowContents = new List<GameObject>[nRows];
        for (int i=0; i<rowContents.Length; i++)
        {
            rowContents[i] = new List<GameObject>();
        }
    }
    public void OnCityChanged(GameObject cityObject)
    {
        //Delete old items
        for (int i = 0; i < nRows; i++)
            for (int j = 0; j < rowContents[i].Count; j++)
            {
                Destroy(rowContents[i][j]);
            }
        City city = cityObject.GetComponent<CityComponent>().city;


        inventory =  new List<string>(city.CityGoods);
        Shuffle(inventory);

        //Add the same amount of goods to each row
        for(int i = 0; i < nRows; i++)
            for(int j = 0; j < inventory.Count/nRows; j++)
            {
                rowContents[i].Add(GameData.Instance.GetGoodGameObject(inventory[i*nRows + j]));
            }
        Debug.Log("Slot machine updated");
    }
    public void Shuffle<T>(IList<T> ts)
    {
        var count = ts.Count;
        var last = count - 1;
        for (var i = 0; i < last; ++i)
        {
            var r = Random.Range(i, count);
            var tmp = ts[i];
            ts[i] = ts[r];
            ts[r] = tmp;
        }
    }
}
