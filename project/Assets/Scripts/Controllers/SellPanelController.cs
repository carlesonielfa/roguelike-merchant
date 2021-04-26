using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityAtoms.BaseAtoms;
public class SellPanelController : MonoBehaviour
{
    [SerializeField] Vector3 hidePos;
    [SerializeField] Vector3 showPos;
    public void OnStateChanged(IntPair state)
    {
        //If we are entering spin state show machine
        if (state.Item1 == (int)GameState.SELL)
        {
            LeanTween.moveLocal(gameObject, showPos, 2f).setEaseOutCubic();
        }
        //If we are leaving spin state hide machine
        if (state.Item2 == (int)GameState.SELL)
        {
            LeanTween.moveLocal(gameObject, hidePos, 2f).setEaseInCubic();
        }
    }
}
