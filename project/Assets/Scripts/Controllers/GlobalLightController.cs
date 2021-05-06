using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityAtoms.BaseAtoms;
using UnityEngine.Experimental.Rendering.Universal;
public class GlobalLightController : MonoBehaviour
{
    [SerializeField] float maxBrightness = 1;
    [SerializeField] float minBrightness = 0.25f;
    [SerializeField] IntVariable totalTurns;
    Light2D light;
    // Start is called before the first frame update
    void Awake()
    {
        light = GetComponent<Light2D>();
    }
    public void OnCurrentTurnsChanged(IntPair values)
    {

        LeanTween.value(values.Item2, values.Item1, 0.5f).setOnUpdate((float v) => {
            light.intensity = maxBrightness - (maxBrightness- minBrightness) *( v / totalTurns.Value);
        });
    }
}
