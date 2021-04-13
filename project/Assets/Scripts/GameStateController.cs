using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityAtoms.BaseAtoms;

public enum GameState { START, MOVEMENT, MOVING, SELL, SPIN, PAY, GAMEOVER, GETITEM}
public class GameStateController : MonoBehaviour
{
    [SerializeField] IntVariable gameState;
    public InputControls controls;
    // Start is called before the first frame update
    private void Awake()
    {
        Physics.autoSimulation = false;
        controls = new InputControls();
    }
    private void OnEnable()
    {
        controls.Enable();
    }
    void Start()
    {
        //GameData.LoadDataFromJSON();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
