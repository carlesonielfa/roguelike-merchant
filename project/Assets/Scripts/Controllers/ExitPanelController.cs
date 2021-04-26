using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityAtoms.BaseAtoms;
using UnityEngine.UI;
public class ExitPanelController : MonoBehaviour
{
    [SerializeField] IntVariable state;
    Image image;
    // Start is called before the first frame update
    void Awake()
    {
        image = gameObject.GetComponent<Image>();
        image.canvasRenderer.SetAlpha(0);
    }

    // Update is called once per frame
    public void OnClick()
    {
        if(state.Value == (int)GameState.SELL)
        {
            state.Value = (int)GameState.SPIN;
        }
        else if(state.Value == (int)GameState.SPIN)
        {
            state.Value = (int)GameState.MOVEMENT;
        }
    }

    public void OnGameStateChanged(IntPair state)
    {
        bool target = state.Item1 == (int)GameState.SELL || state.Item1 == (int)GameState.SPIN;

        LeanTween.value(image.canvasRenderer.GetAlpha(),  target? 1:0, 0.5f).
            setOnUpdate((float val) => image.canvasRenderer.SetAlpha(val)).
            setOnComplete(()=> image.raycastTarget = target);
    }

}
