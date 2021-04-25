using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityAtoms.BaseAtoms;
using TMPro;
public class ProgressBar : MonoBehaviour
{
    [SerializeField] bool vertical;
    [SerializeField] GameObject segmentPrefab;
    [SerializeField] RectTransform maskTransform;
    [SerializeField] IntVariable maxValue;
    [SerializeField] TextMeshProUGUI textMaxValue;
    [SerializeField] TextMeshProUGUI textCurrentValue;

    float maxSize;
    GameObject[] segmentations;
    void Start()
    {
        if (vertical)
            maxSize = GetComponent<RectTransform>().rect.height;
        else
            maxSize = GetComponent<RectTransform>().rect.width;

        maskTransform.GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(GetComponent<RectTransform>().rect.width, GetComponent<RectTransform>().rect.height);

    }
    public void OnValueChanged(IntPair values)
    {
        float step = maxSize / maxValue.Value;

        LeanTween.value(values.Item2 * step, values.Item1 * step, 1f).
            setOnUpdate((float v) => UpdateSize(v)).
            setEaseOutCubic();
    }
    public void UpdateSize(float value)
    {
        if (vertical)
            maskTransform.sizeDelta = new Vector2(maskTransform.sizeDelta.x, value);
        else
            maskTransform.sizeDelta = new Vector2(value, maskTransform.sizeDelta.y);
    }
    public void UpdateSegmentations(int maxValue)
    {
        if (maxSize == 0)
            Start();
        if (segmentations != null)
        {
            foreach (GameObject g in segmentations)
            {
                Destroy(g);
            }
        }

        if (maxValue > 1)
        {
            segmentations = new GameObject[maxValue - 1];
            float step = maxSize / maxValue;
            for (int i = 0; i < maxValue - 1; i++)
            {
                segmentations[i] = Instantiate(segmentPrefab, gameObject.transform);
                if (vertical)
                    segmentations[i].GetComponent<RectTransform>().localPosition = new Vector3(0, step * (i + 1), 0);
                else
                    segmentations[i].GetComponent<RectTransform>().anchoredPosition = new Vector3(step * (i + 1), 0, 0);
            }
        }
    }
    public void UpdateTextCurrentValue(IntPair values)
    {
        LeanTween.value(values.Item2, values.Item1, 0.5f).
            setOnUpdate((float val) => textCurrentValue.text = ((int)val).ToString());
    }
    public void UpdateTextMaxValue(int value)
    {
        textMaxValue.text = value.ToString();
    }
}
