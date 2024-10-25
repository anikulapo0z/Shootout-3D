using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using DG.Tweening;

[RequireComponent(typeof(StudioEventEmitter))]

public class TimeSlow : MonoBehaviour
{
    //public float timeSlow = 0.5f;
    //public float timeSlowDuration = 5f;

    //access the player
    public GameObject player;
    private RaycastPlayer playerController; // The Character movement and shooting code of the player
    private float originalAimSpeed; // To store the original aim speed

    private StudioEventEmitter emitter;

    // Start is called before the first frame update 
    void Start()
    {
        playerController = player.GetComponent<RaycastPlayer>();
        originalAimSpeed = playerController.aimSpeed;
        emitter = AudioManager.instance.InitializeEventEmitter(FMODEvents.instance.timeSlowPickupIDLESound, this.gameObject);
        emitter.Play();

        float floatDuration = 2f; // Change this to control the speed of floating
        float startHeight = transform.position.y;
        float endHeight = startHeight + 0.25f; // Change this to control the height of floating

        Sequence mySequence = DOTween.Sequence();
        mySequence.Append(transform.DOMoveY(endHeight, floatDuration).SetEase(Ease.InOutSine));
        mySequence.Append(transform.DOMoveY(startHeight, floatDuration).SetEase(Ease.InOutSine));
        mySequence.SetLoops(-1); // This will make the sequence loop indefinitely
    }

    // slow down the player's aim speed
    private void SlowDownAimSpeed()
    {
        playerController.aimSpeed = 2f;
    }

    // Update is called once per frame
    void Update()
    {
        float rotationSpeed = 45f; // Change this to control the speed of rotation
        transform.DORotate(new Vector3(0, rotationSpeed, 0), 1, RotateMode.LocalAxisAdd).SetEase(Ease.Linear);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            playerController.IncrementTimeSlowAmmo();
            emitter.Stop();
            
            Destroy(gameObject);
        }
    }
}
