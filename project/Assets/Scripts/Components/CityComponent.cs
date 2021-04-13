using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityAtoms.BaseAtoms;
using UnityEngine.InputSystem;
public class CityComponent : MonoBehaviour
{
    public GameObjectVariable currentCity;
    public IntVariable gameState;
    public City city;
    public void OnEnable()
    {
        city = new City();
    }
    public void OnClickCity()
    {
        if(gameState.Value == (int)GameState.MOVEMENT)
        {
            currentCity.Value = gameObject;
            Debug.Log("Clicked city : "+transform.position);
        }        
    }
}
