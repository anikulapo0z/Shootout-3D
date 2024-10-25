using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Aiming : MonoBehaviour
{
    public GameObject GunHolder; // The empty GameObject
    public GameObject hands; // The gun GameObject

    // Update is called once per frame
    void Update()
    {
        Animator handAnimator = hands.GetComponentInChildren<Animator>();


        if (Input.GetMouseButtonDown(1))
        {
            handAnimator.SetBool("IsAiming", true);
            
        }

        if (Input.GetMouseButtonUp(1))
        {
            handAnimator.SetBool("IsAiming", false);
            
        }

        
        // Make the GunHolder follow the camera's rotation
        GunHolder.transform.rotation = Camera.main.transform.rotation;
    }
}
