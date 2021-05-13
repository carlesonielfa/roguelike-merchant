using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityAtoms.BaseAtoms;

public enum GameState { START, MOVEMENT, MOVING, SELL, SPIN, PAY, GAMEOVER, GETITEM}
public class GameStateController : MonoBehaviour
{

    [SerializeField] int initialGold;
    [SerializeField] int initialTurns;
    [SerializeField] int initialDebt;
    [SerializeField] IntVariable gameState;
    [SerializeField] IntVariable gold;
    [SerializeField] IntVariable debt;
    [SerializeField] IntVariable totalTurns;
    [SerializeField] IntVariable currentTurn;

    bool firstAction = true;
    // Start is called before the first frame update
    private void Awake()
    {
        Physics.autoSimulation = false;
    }
    void Start()
    {
        gold.Value = initialGold;
        debt.Value = initialDebt;
        totalTurns.Value = initialTurns;
    }

    public void OnStatChanged(IntPair state)
    {
        if (firstAction && state.Item2 == (int)GameState.MOVING)
        {
            firstAction = false;
            currentTurn.Value = 0;
            gameState.Value = 1;
        }
        else if (!firstAction)
        {
            //If previous state was spin or movement we advance one turn
            if (state.Item2 == (int)GameState.SPIN || state.Item2 == (int)GameState.MOVEMENT)
            {
                currentTurn.Value += 1;
            }
            if (currentTurn.Value == 0)
            {
                gameState.Value = (int)GameState.PAY;
            }
        }


    }

    public void OnCurrentCityChanged(GameObjectPair cityVar)
    {
        if (cityVar.Item2 != null)
        {
            SetCityReachable(cityVar.Item2.GetComponent<CityComponent>(), false);
        }
        SetCityReachable(cityVar.Item1.GetComponent<CityComponent>(), true);
    }
    private void SetCityReachable(CityComponent city, bool value)
    {
        city.Reachable = value;
        foreach (CityComponent other in city.connectedCities)
        {
            other.Reachable = value;
        }
    }
}
