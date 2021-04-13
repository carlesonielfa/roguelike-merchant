using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityAtoms.BaseAtoms;
public class SlotMachineController : MonoBehaviour
{
    const int nRows = 5;
    List<string> inventory;
    List<GameObject>[] rowContents;
    Transform[] rowContainers;
    float slotWidth;
    float slotHeight;
    float spinSpeed = 50f;
    float firstSlotY;
    float lastSlotY;

    [SerializeField] Vector2 showPos;
    [SerializeField] Vector2 hidePos;
    public void Start()
    {
        rowContents = new List<GameObject>[nRows];
        for (int i = 0; i < rowContents.Length; i++)
        {
            rowContents[i] = new List<GameObject>();
        }

        rowContainers = new Transform[nRows];
        if (gameObject.transform.GetChild(0).childCount != nRows)
            Debug.LogError("Slot machine number of rows gameobjects and nRows mismatch, fix in editor");

        for (int i = 0; i < nRows; i++)
        {
            rowContainers[i] = transform.GetChild(0).GetChild(i);
        }


    }
    public void OnStatChanged(IntPair state)
    {
        //If we are entering spin state show machine
        if(state.Item1 == (int)GameState.SPIN)
        {
            ShowSlotMachine();
        }
        //If we are leaving spin state hide machine
        if(state.Item2 == (int)GameState.SPIN)
        {
            HideSlotMachine();
        }
    }
    public void OnCityChanged(GameObject cityObject)
    {
        RectTransform rectTransformParent = rowContainers[0].GetComponent<RectTransform>();
        slotHeight = rectTransformParent.rect.height;
        slotWidth = rectTransformParent.rect.width;

        //Delete old items
        for (int i = 0; i < nRows; i++)
            for (int j = 0; j < rowContents[i].Count; j++)
            {
                Destroy(rowContents[i][j]);
            }
        City city = cityObject.GetComponent<CityComponent>().city;


        inventory = new List<string>(city.CityGoods);
        UpdateRowContents();
    }
    public void ShowSlotMachine()
    {

        LeanTween.moveLocal(gameObject,showPos,2f).setEaseOutElastic().setOnComplete(()=>Spin());
    }
    public void HideSlotMachine()
    {
        LeanTween.moveLocal(gameObject, hidePos,2f).setEaseInElastic();
    }
    public void UpdateRowContents()
    {
        Shuffle(inventory);
        RectTransform rectTransform = null;
        //Add the same amount of goods to each row
        for (int i = 0; i < nRows; i++)
        {
            for (int j = 0; j < inventory.Count / nRows; j++)
            {
                rowContents[i].Add(GameData.Instance.GetGoodGameObject(inventory[i * nRows + j]));
                rowContents[i][j].transform.SetParent(rowContainers[i]);

                rectTransform = rowContents[i][j].GetComponent<RectTransform>();

                rectTransform.anchorMin = new Vector2(0, 0f);
                rectTransform.anchorMax = new Vector2(1, 0f);
                rectTransform.localScale = new Vector3(1, 1, 1) * 0.45f;
                rectTransform.pivot = new Vector2(0.5f, 0);
                rectTransform.localPosition = new Vector3(0, (j-1)*slotWidth*0.6f - 0.45f*slotHeight, 0);
            }
        }
        firstSlotY = rectTransform.localPosition.y;
        lastSlotY = -0.5f * slotHeight - 0.5f * slotWidth;

        Debug.Log("Slot machine updated");
    }
    void Spin()
    {
        float rowDelay = 0.25f;
        for(int i=0; i<1; i++)
        { 
            foreach (GameObject g in rowContents[i])
            {
                SpinGood(g, rowDelay * i);
            }
        }

    }
    void SpinGood(GameObject g, float delay)
    {
        float time = (g.transform.localPosition.y - lastSlotY) / (spinSpeed*10);
        LeanTween.value(g.transform.localPosition.y, lastSlotY, time).
            setDelay(delay).
            setOnUpdate((float val) => ApplyMovementGood(g, val)).
            setOnComplete(()=>ResetGoodPosition(g));
    }
    void ApplyMovementGood(GameObject g, float newVal)
    {
        g.transform.localPosition = new Vector3(g.transform.localPosition.x, newVal, g.transform.localPosition.z);
    }
    void ResetGoodPosition(GameObject g)
    {
        g.transform.localPosition = new Vector3(g.transform.localPosition.x, firstSlotY, g.transform.localPosition.z);
        SpinGood(g, 0);
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
