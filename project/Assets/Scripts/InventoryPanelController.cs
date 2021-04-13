using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryPanelController : MonoBehaviour
{
    Dictionary<string, GoodPanelController> goodPanels;
    [SerializeField] GameObject goodPanelPrefab;
    // Start is called before the first frame update
    void Start()
    {
        goodPanels = new Dictionary<string, GoodPanelController>();
        GameData.Instance.OnInventoryChanged += UpdatePanels;
    }
    void UpdatePanels(System.Tuple<string,int> updatedValue)
    {
        if (!goodPanels.ContainsKey(updatedValue.Item1))
        {
            GameObject instance = Instantiate(goodPanelPrefab, transform);
            goodPanels.Add(updatedValue.Item1, instance.GetComponent<GoodPanelController>());
            goodPanels[updatedValue.Item1].UpdateIcon(GameData.Instance.GetGoodGameObject(updatedValue.Item1));
        }
        goodPanels[updatedValue.Item1].UpdateAmount(updatedValue.Item2);
    }
}
