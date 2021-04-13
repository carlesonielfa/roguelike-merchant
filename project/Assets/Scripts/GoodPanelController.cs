using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VectorGraphics;
using TMPro;

public class GoodPanelController : MonoBehaviour
{
    TextMeshProUGUI textComponent;
    SVGImage image;
    public void OnEnable()
    {
        textComponent = GetComponentInChildren<TextMeshProUGUI>();
        image = GetComponent<SVGImage>();
    }
    public void UpdateIcon(GameObject goodGameObject)
    {
        image.sprite = goodGameObject.GetComponent<SVGImage>().sprite;
        transform.localScale = new Vector3();
        LeanTween.scale(gameObject,Vector3.one, 0.25f);
    }
    public void UpdateAmount(int amount)
    {
        textComponent.text = "x"+amount;
    }
}
