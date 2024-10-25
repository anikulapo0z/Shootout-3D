using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DoorScript : MonoBehaviour
{
    public GameObject doorPivot;
    public bool doorOpen = false;
    public float openAngle = 90.0f;  // The angle to which the door should open
    public float closeDelay = 1.0f;  // The delay before the door begins to close

    private Coroutine closeCoroutine;

    public void OpenDoor()
    {
        if (!doorOpen)
        {
            // Rotate the door
            doorPivot.transform.localRotation = Quaternion.Euler(openAngle, 0, 0);
            doorOpen = true;

            // If a close coroutine is already running, stop it
            if (closeCoroutine != null)
            {
                StopCoroutine(closeCoroutine);
            }

            // Start a new coroutine to close the door after a delay
            closeCoroutine = StartCoroutine(CloseDoorAfterDelay(closeDelay));
        }
    }

    private IEnumerator CloseDoorAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        // Close the door
        doorPivot.transform.localRotation = Quaternion.identity;
        doorOpen = false;
    }
}
