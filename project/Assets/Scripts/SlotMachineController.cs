using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityAtoms.BaseAtoms;
using System.Linq;
using UnityEngine.UI;
public class SlotMachineController : MonoBehaviour
{
    const int nCols = 5;
    const int nRows = 4;
    List<string> inventory;
    List<GameObject>[] colContents;
    Transform[] colContainers;
    float slotWidth;
    float slotHeight;
    float spinSpeed = 50f;
    float firstSlotY;
    float lastSlotY;
    CityComponent currentCity;
    [SerializeField] Vector2 showPos;
    [SerializeField] Vector2 hidePos;
    [SerializeField] IntVariable gameState;
    [SerializeField] Transform dividers;
    [SerializeField] Button spinButton;
    /*
     * Slot machine algorithm description
     * 
     * Slot machine of nRows rows by nCols columns
     * Total of items in inventory is n
     * 
     * 1. Load inventory from city
     * 2. Add nRows*nCols - n empty objects to fill the machine
     * 3. Ensure n % nCols == 0 if it isn't add empties until it is
     * 4. Shuffle inventory
     * 5. Load Gameobjects into the columns and rows
     * 6. Initiate spin animation
     * 7. When spin animation is going to stop, add winning combination so it shows as it ends
     */
    public void Start()
    {
        colContents = new List<GameObject>[nCols];

        colContainers = new Transform[nCols];
        if (dividers.childCount != nCols)
            Debug.LogError("Slot machine number of rows gameobjects and nRows mismatch, fix in editor");

        for (int i = 0; i < nCols; i++)
        {
            colContainers[i] = dividers.GetChild(i);
        }


    }
    public void OnStatChanged(IntPair state)
    {
        //If we are entering spin state show machine
        if (state.Item1 == (int)GameState.SPIN)
        {
            LeanTween.moveLocal(gameObject, showPos, 1f).setDelay(2f).setEaseOutBack();
        }
        //If we are leaving spin state hide machine
        if (state.Item2 == (int)GameState.SPIN)
        {
            LeanTween.moveLocal(gameObject, hidePos, 1f).setEaseInBack();
        }
    }
    public void OnCityChanged(GameObject cityObject)
    {
        RectTransform rectTransformParent = colContainers[0].GetComponent<RectTransform>();
        slotHeight = rectTransformParent.rect.height;
        slotWidth = rectTransformParent.rect.width;

        //Delete old items
        for (int i = 0; i < nCols; i++)
        {
            if(colContents[i]!= null)
                for (int j = 0; j < colContents[i].Count; j++)
                    Destroy(colContents[i][j]);
    
            colContents[i] = new List<GameObject>();
        }
        currentCity = cityObject.GetComponent<CityComponent>();


        inventory = new List<string>(currentCity.cityGoods);
        UpdateRowContents();
    }
    public void UpdateRowContents()
    {
        //Add empty simbols until we fill the machine
        if (nCols * nRows > inventory.Count)
        {
            inventory.AddRange(Enumerable.Repeat("empty", nCols * nRows - inventory.Count));
        }
        //Ensure all cols will have the same number of items
        if (inventory.Count % nCols != 0)
        {
            inventory.AddRange(Enumerable.Repeat("empty", nCols - inventory.Count % nCols));
        }


        Shuffle(inventory);
        RectTransform rectTransform = null;
        for (int i = 0; i < nCols; i++)
        {
            for (int j = 0; j < inventory.Count / nCols; j++)
            {
                GameObject instance = GameData.Instance.GetGoodGameObjectInstance(inventory[i * (inventory.Count / nCols) + j]);
                colContents[i].Add(instance);

                rectTransform = colContents[i][j].GetComponent<RectTransform>();
                rectTransform.SetParent(colContainers[i]);

                rectTransform.anchorMin = new Vector2(0.5f, 1f);
                rectTransform.pivot = new Vector2(0.5f, 0.5f);
                rectTransform.anchorMax = new Vector2(0.5f, 1f);
                rectTransform.localScale = Vector3.one * 0.45f;
                // TODO: change spacing to automatic
                rectTransform.anchoredPosition = new Vector3(0, -30 - j * slotWidth * 0.65f, 0);
                //rectTransform.localPosition = new Vector3(0, (j-1)*slotWidth*0.6f - 0.45f*slotHeight, 0);
            }
        }
        firstSlotY = rectTransform.localPosition.y;
        lastSlotY = -0.5f * slotHeight - 0.5f * slotWidth;

        Debug.Log("Slot machine updated");
        spinButton.interactable = true;
    }
    public void Spin()
    {
        spinButton.interactable = false;
        /*
        float rowDelay = 0.25f;
        for(int i=0; i<nRows; i++)
        { 
            foreach (GameObject g in rowContents[i])
            {
                SpinGood(g, rowDelay * i);
            }
        }*/
        ComputeRewards();
    }
    void ComputeRewards()
    {
        List<GameObject> rewards = new List<GameObject>();

        for (int i = 0; i < nCols; i++)
        {
            for (int j = 0; j < nRows; j++)
            {
                GameObject selectedGoodObject = colContents[i][j];
                Good selectedGood = selectedGoodObject.GetComponent<GoodComponent>().good;
                if(selectedGood.name != "empty")
                {
                    // Check adjacent goods
                    bool foundAdjacent = false;
                    int[,] positionsToCheck = { { i + 1, j }, { i + 1, j + 1 }, { i, j + 1 }, { i + 1, j - 1 } };
                    for (int k = 0; k < positionsToCheck.GetLength(0); k++)
                    {
                        if (positionsToCheck[k, 0] < nCols &&
                            positionsToCheck[k, 0] >= 0 &&
                            positionsToCheck[k, 1] < nRows &&
                            positionsToCheck[k, 1] >= 0)
                            foundAdjacent |= CheckAdjacentGood(rewards, selectedGood, colContents[positionsToCheck[k, 0]][positionsToCheck[k, 1]]);
                    }

                    // If it had one adjacent of the same type we add this one as reward as well
                    if (foundAdjacent && !rewards.Contains(selectedGoodObject))
                        rewards.Add(selectedGoodObject);
                }
            }
        }

        foreach (GameObject g in rewards)
        {
            Good good = g.GetComponent<GoodComponent>().good;
            // Animate all rewards and add them
            LeanTween.rotateAroundLocal(g, new Vector3(0, 0, 1), 360, 0.5f).setOnComplete(() =>
                GameData.Instance.AddPlayerGood(good.name, 1));

            //Remove rewards from city inventory
            if (!currentCity.cityGoods.Remove(good.name))
                Debug.LogError("Awarded item to player that was not in city inventory");
        }
        LeanTween.delayedCall(2f, () => gameState.Value = (int)GameState.MOVEMENT);


    }
    bool CheckAdjacentGood(List<GameObject> rewards, Good selected, GameObject otherGameObject)
    {
        Good other = otherGameObject.GetComponent<GoodComponent>().good;
        if (selected.name == other.name && !rewards.Contains(otherGameObject))
        {
            rewards.Add(otherGameObject);
            return true;
        }
        return false;
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
