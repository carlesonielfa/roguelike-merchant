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
    public void OnNewCity(GameObject city)
    {
        GameData.Instance.AddPlayerGood("gem", 1);
        GameData.Instance.AddPlayerGood("strawberry", 1);
        
        if (gameState.Value != (int)GameState.MOVEMENT)
        {
            Debug.LogError("Illegal Action: A new city was selected when GameState wasn't movement");
        }
        gameState.Value = (int)GameState.MOVING;

        //Get close to city
        Vector3 w = city.transform.position - transform.position;
        Vector3 q = transform.position + w - 0.9f * (w/w.magnitude);
        LeanTween.move(gameObject, q, 1f).
            setEase(LeanTweenType.easeInOutQuad).
            setOnComplete(()=> gameState.Value = (int)GameState.SELL);

    }
}
