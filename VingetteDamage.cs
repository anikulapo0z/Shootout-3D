using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class VingetteDamage : MonoBehaviour
{
    public Volume vignetteVolume;
    private Vignette vignette;

    // Start is called before the first frame update
    void Start()
    {
        // Check if the volume and vignette components exist
        if (vignetteVolume != null && vignetteVolume.profile.TryGet(out Vignette vignetteComponent))
        {
            vignette = vignetteComponent;
        }
        else
        {
            Debug.LogError("Vignette component not found on the volume or volume not assigned.");
        }
    }


    public void TakeDamageVignette()
    {
        StartCoroutine(ChangeVignetteOverTime());
    }

    IEnumerator ChangeVignetteOverTime()
    {
        // Set the vignette intensity to 1 ad smoothness to 0.5 when this is called over 1 second
        float duration = 3f;
        float start = 0.25f;
        float end = 0.5f;

        float time = 0;
        while (time < duration)
        {
            time += Time.deltaTime;
            float pingPongTime = Mathf.PingPong(time, 1);
            vignette.intensity.value = Mathf.Lerp(start, end, pingPongTime);
            yield return null;
        }
        // Gradually fade the vignette back to its original intensity
        float resetDuration = 1f; // Adjust this value to make the fade faster or slower
        float resetTime = 0;
        while (resetTime < resetDuration)
        {
            resetTime += Time.deltaTime;
            vignette.intensity.value = Mathf.Lerp(end, start, resetTime / resetDuration);
            yield return null;
        }

        // Reset to original values
        vignette.intensity.value = start;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
