using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityAtoms.BaseAtoms;
public class SetTextFromEvent : MonoBehaviour
{
    string baseText;
    int val;
    TextMeshProUGUI textComponent;


    // Start is called before the first frame update
    void Awake()
    {
        textComponent = GetComponent<TextMeshProUGUI>();
    }

    public void UpdateText(string newText)
    {
        if (baseText == null)
        {
            baseText = textComponent.text;
        }
        textComponent.text = baseText + newText;
    }
    public void UpdateText(int newValue)
    {
        UpdateText(newValue.ToString());
    }
    public void UpdateTextWithTween(IntPair newValues)
    {
        LeanTween.value(gameObject, newValues.Item2, newValues.Item1, 1f)
            .setOnUpdate((float val) => { UpdateText(((int)val).ToString()); });
    }
    public void UpdateText(IntPair newValues)
    {
        UpdateText(newValues.Item1.ToString());
    }
    public void UpdateText(float newValue)
    {
        UpdateText(newValue.ToString());
    }
}
