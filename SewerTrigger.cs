using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SewerTrigger : MonoBehaviour
{
    private GameManager gameManager;
    public float delay = 5f;
    public GameObject controlsText; // Reference to your UI Text element
    public RaycastPlayer player;

    public GameObject hands;

    private void Start()
    {
        gameManager = GameManager.instance;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            StopAllCoroutines();
            ResumeMovement();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            gameManager.TogglePlayerMovement(false); // stop player movement
            controlsText.gameObject.SetActive(true); // display the controls text
            hands.gameObject.SetActive(false);
            StartCoroutine(ResumePlayerMovementAfterDelay(delay)); // start the coroutine
        }
    }

    IEnumerator ResumePlayerMovementAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        ResumeMovement();
    }
    private void ResumeMovement()
    {
        gameManager.TogglePlayerMovement(true); // allow player movement
        if (player != null)
        {
            player.canShoot = true;
        }
        controlsText.gameObject.SetActive(false); // hide the controls text
        hands.gameObject.SetActive(true);
    }
}
