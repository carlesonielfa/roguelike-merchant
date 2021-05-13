using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityAtoms.BaseAtoms;
public class SellButtonController : MonoBehaviour
{
    [SerializeField] GoodPanelController goodPanel;
    [SerializeField] IntVariable gold;
    int price;
    public void Start()
    {
        price = GameData.Instance.GetGoodPrice(goodPanel.goodName);
        GetComponentInChildren<TextMeshProUGUI>().text = price.ToString();
    }
    public void OnClick()
    {
        gold.Value += price;
        GameData.Instance.RemovePlayerGood(goodPanel.goodName, 1);
    }
}
