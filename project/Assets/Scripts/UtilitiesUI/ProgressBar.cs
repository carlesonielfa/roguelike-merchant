using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityAtoms.BaseAtoms;
using TMPro;
public class ProgressBar : MonoBehaviour
{
    [SerializeField] bool vertical;
    [SerializeField] RectTransform maskTransform;
    [SerializeField] IntVariable maxValue;
    [SerializeField] TextMeshProUGUI textMaxValue;
    [SerializeField] TextMeshProUGUI textCurrentValue;

    float maxSize;

    void Start()
    {
        if(vertical)
            maxSize = GetComponent<RectTransform>().rect.height;
        else
            maxSize = GetComponent<RectTransform>().rect.width;
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
