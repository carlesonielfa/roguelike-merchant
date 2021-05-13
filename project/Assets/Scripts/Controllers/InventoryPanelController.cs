using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryPanelController : MonoBehaviour
{
    Dictionary<string, GoodPanelController> goodPanels;
    [SerializeField] GameObject goodPanelPrefab;
    [SerializeField] Transform panelsContainer;
    [SerializeField] GameObject emptyPlaceholder;
    // Start is called before the first frame update
    void Start()
    {
        goodPanels = new Dictionary<string, GoodPanelController>();
        GameData.Instance.OnInventoryChanged += UpdatePanels;
        if (panelsContainer == null)
            panelsContainer = transform;
    }
    void UpdatePanels(System.Tuple<string, int> updatedValue)
    {
        GoodPanelController g;
        if (!goodPanels.ContainsKey(updatedValue.Item1))
        {
            g = Instantiate(goodPanelPrefab, panelsContainer).GetComponent<GoodPanelController>();
            goodPanels.Add(updatedValue.Item1, g);
            g.UpdateIcon(GameData.Instance.GetGoodGameObjectPrefab(updatedValue.Item1));
        }
        else
        {
            g = goodPanels[updatedValue.Item1];
        }

        g.UpdateAmount(updatedValue.Item2);

        if (updatedValue.Item2 == 0)
        {
            goodPanels.Remove(updatedValue.Item1);
        }

        if(emptyPlaceholder!=null)
            emptyPlaceholder.SetActive(goodPanels.Count == 0);
    }
}
