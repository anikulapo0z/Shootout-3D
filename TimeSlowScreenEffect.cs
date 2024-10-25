using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class TimeSlowScreenEffect : MonoBehaviour
{
    public RaycastPlayer raycastPlayer;
    public Volume volume;
    private ColorAdjustments colorAdjustments;

    void Start()
    {
        if (volume.profile.TryGet<ColorAdjustments>(out var colorAdjust))
        {
            colorAdjustments = colorAdjust;
        }
    }

    void Update()
    {
        if (raycastPlayer.isTimeSlowActive)
        {
            colorAdjustments.saturation.value = -100;
        }
        else
        {
            colorAdjustments.saturation.value = 0;
        }
    }
}
