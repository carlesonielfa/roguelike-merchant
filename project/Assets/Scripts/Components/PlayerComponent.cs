using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityAtoms.BaseAtoms;
public class PlayerComponent : MonoBehaviour
{
    [SerializeField] IntVariable gameState;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    public void OnNewCity(GameObjectPair city)
    {        
        if (gameState.Value != (int)GameState.MOVEMENT)
        {
            Debug.LogError("Illegal Action: A new city was selected when GameState wasn't movement");
        }
        gameState.Value = (int)GameState.MOVING;

        //Get close to city
        Vector3 w = city.Item1.transform.position - transform.position;
        Vector3 q = transform.position + w - 0.9f * (w/w.magnitude);

        LeanTween.move(gameObject, q, 1f).
            setEase(LeanTweenType.easeInOutQuad).
            setOnComplete(()=> gameState.Value = city.Item2 != null ? (int)GameState.SELL: (int)GameState.MOVEMENT);

    }
}
