using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityAtoms.BaseAtoms;
using UnityEngine.EventSystems;
public class CityComponent : MonoBehaviour, IPointerClickHandler
{
    public GameObjectVariable currentCity;
    public IntVariable gameState;


    public readonly List<CityComponent> connectedCities = new List<CityComponent>();

    public City city;

    [SerializeField] GameObject arrowGameObject;
    [SerializeField] SpriteRenderer cityImage;
    bool reachable;

    public void SetImage(Sprite newImage)
    {
        cityImage.sprite = newImage;
    }
    public bool Reachable
    {
        get => reachable;
        set
        {
            reachable = value;
            arrowGameObject.SetActive(value);
            //GetComponent<Button>().interactable = value;
        }
    }
    public Vector2 GetPosition()
    {
        return new Vector2(transform.position.x, transform.position.y);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (gameState.Value == (int)GameState.MOVEMENT)
        {
            if (currentCity.Value == gameObject)
            {
                currentCity.GetEvent<GameObjectEvent>().Raise(gameObject);
                Debug.Log("Repeated City : " + transform.position);
            }
            else
            {
                currentCity.Value = gameObject;
                Debug.Log("New City : " + transform.position);
            }


        }
    }
}
