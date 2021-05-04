using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityArrow : MonoBehaviour
{
    int tweenId = 0;
    private void OnEnable()
    {
        if (tweenId == 0)
            tweenId = LeanTween.moveLocalY(gameObject, 1.2f, 0.5f).setEaseOutCubic().setLoopPingPong().id;
        else
            LeanTween.resume(tweenId);
            
    }
    private void OnDisable()
    {
        LeanTween.pause(tweenId);
    }
}
