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
        currentTurn.Value = 0;
    }

    public void OnStatChanged(IntPair state)
    {
        //If previous state was spin or movement we advance one turn
        if(state.Item2 == (int)GameState.SPIN || state.Item2 == (int)GameState.MOVEMENT)
        {
            currentTurn.Value += 1;
        }
        if(currentTurn.Value == 0)
        {
            gameState.Value = (int)GameState.PAY;
        }
    }
}
