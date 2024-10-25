using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHeadBop : MonoBehaviour
{
    //head bop variables
    public float bobbingSpeed = 12.5f;
    public float bobbingAmount = 0.115f;
    public GameObject playerCamera;
    public GameObject cameraRig;

    //make a subtle head bop when the player walks around the map
    void Update()
    {
        float waveslice = 0.0f;
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        //if the player is moving, make the camera bob
        if (Mathf.Abs(horizontal) == 0 && Mathf.Abs(vertical) == 0)
        {
            //if the player is not moving, set the camera to the default position with a very subtle head bop animation
            cameraRig.transform.localPosition = new Vector3(0f, 0.01f, 0f);
            
        }
        else
        {
            //if the player is moving, make the camera bob
            waveslice = Mathf.Sin(Time.time * bobbingSpeed);
            if (waveslice != 0)
            {
                //if the player is moving, make the camera bob
                float translateChange = waveslice * bobbingAmount;
                float totalAxes = Mathf.Abs(horizontal) + Mathf.Abs(vertical);
                totalAxes = Mathf.Clamp(totalAxes, 0.0f, 1.0f);
                translateChange = totalAxes * translateChange;

                //move the camera rig
                cameraRig.transform.localPosition = new Vector3(0f, translateChange, 0f);
            }
            else
            {
                //if the player is not moving, set the camera to the default position
                cameraRig.transform.localPosition = Vector3.zero;
            }
        }

        // reduce the head bop amount when the player is aiming down sights
        if (Input.GetButton("Fire2"))
        {
            bobbingAmount = 0.05f;
        }
        else
        {
            bobbingAmount = 0.115f;
        }

        // increase the head bop amount when the player is sprinting
        if (Input.GetKey(KeyCode.LeftShift))
        {
            bobbingAmount = 0.15f;
        }
        else
        {
            bobbingAmount = 0.115f;
        }

        // turn off the head bop when the player is jumping or in the air
        if (!GetComponent<CharacterController>().isGrounded)
        {
            bobbingAmount = 0f;
        }
        
    }


}
