using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DoorRaycast : MonoBehaviour
{
    [SerializeField] private int rayLength = 5;
    [SerializeField] private LayerMask layerMaskInteract;
    [SerializeField] private Image crosshair = null;
    [SerializeField] private KeyCode openDoorKey = KeyCode.E;
    [SerializeField] private string excludeLayerName = null;
    private bool isCrosshairActive;
    private bool doOnce;
    private string interactableTag = "InteractableObject";

    private void Update()
    {
        RaycastHit hit;
        Vector3 fwd = transform.TransformDirection(Vector3.forward);
        int mask = 1 << LayerMask.NameToLayer(excludeLayerName) | layerMaskInteract.value; // Add your door layer here

        if (Physics.Raycast(transform.position, fwd, out hit, rayLength, mask))
        {
            if (hit.collider.CompareTag(interactableTag))
            {
                if (!doOnce)
                {
                    crosshair.color = Color.red;
                    doOnce = true;
                }

                isCrosshairActive = true;

                if (Input.GetKeyDown(openDoorKey))
                {
                    DoorScript doorScript = hit.collider.GetComponent<DoorScript>();
                    if (doorScript != null)
                    {
                        doorScript.OpenDoor();
                    }
                }
            }
        }
        else
        {
            if (isCrosshairActive)
            {
                crosshair.color = Color.white;
                isCrosshairActive = false;
                doOnce = false;
            }
        }
    }
}
