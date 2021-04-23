using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityAtoms.BaseAtoms;
using UnityEngine.InputSystem;
using System.Linq;
public class CityComponent : MonoBehaviour
{
    public GameObjectVariable currentCity;
    public IntVariable gameState;
    const int MAX_GOODS = 30;
    public readonly List<string> cityGoods = new List<string>(MAX_GOODS);
    public readonly List<CityComponent> connectedCities = new List<CityComponent>();
    public void OnEnable()
    {
        Restock();
    }
    public void OnClickCity()
    {
        if(gameState.Value == (int)GameState.MOVEMENT)
        {
            if(currentCity.Value == gameObject)
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
    

    public void Restock()
    {
        //TODO change to not truly random
        foreach (string good in GameData.Instance.RandomGoods().Take((MAX_GOODS - cityGoods.Count)/2))
        {
            cityGoods.Add(good);
            cityGoods.Add(good);
        }
    }
    public Vector2 GetPosition()
    {
        return new Vector2(transform.position.x, transform.position.y);
    }
}
