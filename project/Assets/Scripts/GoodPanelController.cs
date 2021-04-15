using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VectorGraphics;
using TMPro;

public class GoodPanelController : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI textComponent;
    [SerializeField] SVGImage image;
    public void OnEnable()
    {
        if(textComponent==null)
            textComponent = GetComponentInChildren<TextMeshProUGUI>();
        if(image==null)
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
        if (amount == 0)
        {
            LeanTween.scale(gameObject, Vector3.zero, 0.25f).destroyOnComplete = true;
        }
        textComponent.text = "x"+amount;
    }
}
