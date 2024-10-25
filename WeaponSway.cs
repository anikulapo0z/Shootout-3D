using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSway : MonoBehaviour
{
    /*
    [Header("Sway Settings")]
    [SerializeField] private float smooth;
    [SerializeField] private float swayMultiplier;

    private void Update()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * swayMultiplier;
        float mouseY = Input.GetAxisRaw("Mouse Y") * swayMultiplier;

        //calculate the target rotation
        Quaternion rotatioX = Quaternion.AngleAxis(-mouseY, Vector3.right);
        Quaternion rotatioY = Quaternion.AngleAxis(mouseX, Vector3.up);

        Quaternion targetRotation = rotatioX * rotatioY;

        //rotate the camera
        transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, Time.deltaTime * smooth * Time.deltaTime);
    }
    */

    // weapon sway variables
    public float swayAmount;
    public float maxSwayAmount;
    public float smoothSwayAmount;
    public float minSwayAmount;
    public float smoothSwayValue;
    public float smoothSwayValueMultiplier;
    public float sprintSwayMultiplier;

    public float swayPositionMultiplier = 100f;

    public void Start()
    {
        /* initialize the weapon sway variables
        swayAmount = 0.02f;
        maxSwayAmount = 0.06f;
        smoothSwayAmount = 6.0f;
        minSwayAmount = 0.02f;
        smoothSwayValue = 4.0f;
        smoothSwayValueMultiplier = 0.02f;
        sprintSwayMultiplier = 2.0f;
        */
    }

    public void Update()
    {
        // calculate the target rotation
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        float swayX = Mathf.Clamp(-mouseX * swayAmount, -maxSwayAmount, maxSwayAmount);
        float swayY = Mathf.Clamp(-mouseY * swayAmount, -maxSwayAmount, maxSwayAmount);

        Vector3 swayPosition = new Vector3(swayX, swayY, 0);

        // rotate the camera
        transform.localPosition = Vector3.Lerp(transform.localPosition, swayPosition * swayPositionMultiplier, Time.deltaTime * smoothSwayAmount);

        // reset the sway amount
        if (Input.GetKey(KeyCode.LeftShift))
        {
            swayAmount = Mathf.Lerp(swayAmount, minSwayAmount, Time.deltaTime * smoothSwayValue * sprintSwayMultiplier);
        }
        else
        {
            swayAmount = Mathf.Lerp(swayAmount, minSwayAmount, Time.deltaTime * smoothSwayValue);
        }

        // increase the sway amount
        if (Input.GetButton("Fire1"))
        {
            IncreaseSwayAmount(1.0f);
        }
        else
        {
            DecreaseSwayAmount(1.0f);
        }

        // reset the sway amount
        if (Input.GetButtonDown("Fire2"))
        {
            ResetSwayAmount();
            maxSwayAmount = 0.05f;
        }
        if (Input.GetButtonUp("Fire2"))
        {
            ResetSwayAmount();
            maxSwayAmount = 0.4f;
        }

    }

    public void IncreaseSwayAmount(float value)
    {
        swayAmount += value * smoothSwayValueMultiplier;
    }

    public void DecreaseSwayAmount(float value)
    {
        swayAmount -= value * smoothSwayValueMultiplier;
    }

    public void ResetSwayAmount()
    {
        swayAmount = minSwayAmount;
    }


}
