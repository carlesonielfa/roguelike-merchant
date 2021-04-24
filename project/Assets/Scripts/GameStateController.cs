using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityAtoms.BaseAtoms;

public enum GameState { START, MOVEMENT, MOVING, SELL, SPIN, PAY, GAMEOVER, GETITEM}
public class GameStateController : MonoBehaviour
{

    [SerializeField] int initialGold;
    [SerializeField] int initialTurns;
    [SerializeField] IntVariable gameState;
    [SerializeField] IntVariable gold;
    [SerializeField] IntVariable totalTurns;
    [SerializeField] IntVariable remainingTurns;
    
    // Start is called before the first frame update
    private void Awake()
    {
        Physics.autoSimulation = false;
    }
    void Start()
    {
        gold.Value = initialGold;
        totalTurns.Value = initialTurns;
        remainingTurns.Value = initialTurns;
    }

    public void OnStatChanged(IntPair state)
    {
        //If previous state was spin or movement we advance one turn
        if(state.Item2 == (int)GameState.SPIN || state.Item2 == (int)GameState.MOVEMENT)
        {
            remainingTurns.Value -= 1;
        }
        if(remainingTurns.Value == 0)
        {
            gameState.Value = (int)GameState.PAY;
        }
    }
}
