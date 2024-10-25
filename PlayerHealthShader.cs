using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealthShader : MonoBehaviour
{
    public Material playerMaterial; // Reference to the player's material
    public Material playerLeftMaterial; // Reference to the player's left hand material
    public Health playerHealth; // Reference to the Health script of the player
    public Material TimeMaterial; // Reference to the time material


    void Start()
    {
        // Initialize shader's _Health with player's current health normalized to 0-1 range
        playerMaterial.SetFloat("_Health", playerHealth.currentHealth / playerHealth.maxHealth);
        playerLeftMaterial.SetFloat("_Health", playerHealth.currentHealth / playerHealth.maxHealth);
        // Set the time shader to 0
        TimeMaterial.SetFloat("_Time_SWITCH", 0);

        //Object.GetComponent<MeshRenderer> ().material = Material1;
    }

    void Update()
    {
        // Update shader's _Health with player's current health normalized to 0-1 range
        playerMaterial.SetFloat("_Health", playerHealth.currentHealth / playerHealth.maxHealth);
        playerLeftMaterial.SetFloat("_Health", playerHealth.currentHealth / playerHealth.maxHealth);
        // If the player activates time slow, set the time shader to 1 for 5 seconds
                        
    }

    public void ActivateTimeShader()
    {
        TimeMaterial.SetFloat("_Time_SWITCH", 1);
    }

    public void ResetTimeShader()
    {
        TimeMaterial.SetFloat("_Time_SWITCH", 0);
    }
}
