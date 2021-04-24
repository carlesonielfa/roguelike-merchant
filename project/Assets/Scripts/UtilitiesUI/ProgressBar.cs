using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityAtoms.BaseAtoms;
public class ProgressBar : MonoBehaviour
{
    [SerializeField] RectTransform maskTransform;
    [SerializeField] IntVariable maxValue;
    float maxWidth;

    void Start()
    {
        maxWidth = GetComponent<RectTransform>().rect.width;
    }
    public void OnValueChanged(IntPair values)
    {
        float step = maxWidth / maxValue.Value;

        LeanTween.value((maxValue.Value - values.Item2) * step, (maxValue.Value - values.Item1) * step, 1f).
            setOnUpdate((float v) => maskTransform.sizeDelta = new Vector2(v, maskTransform.sizeDelta.y)).
            setEaseOutCubic();
    }
}
