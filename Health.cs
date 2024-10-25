using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using FMODUnity;

public class Health : MonoBehaviour
{

    public float maxHealth = 100f;
    public float currentHealth;
    public ParticleSystem explosionPrefab;
    public ParticleSystem smokePrefab;

    public VingetteDamage vingetteDamage;

    public bool isPlayer = false; // Add this

    public AudioManager audioManager; // Make sure to assign this in the Unity Editor


    void Start()
    {
        currentHealth = maxHealth;
        if (tag == "Player") // If the GameObject's tag is "Player", then it is a player
        {
            isPlayer = true;
        }
    }

    public void ApplyDamage(float amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            Die();
        }

        else
        {
            // Play the player hurt sound only if this is the player
            if (isPlayer)
            {
                audioManager.PlayOneShot(FMODEvents.instance.playerHurt, transform.position);
                vingetteDamage.TakeDamageVignette();
            }
        }
    }

    //after the object is destroyed play the explosion particle effect for 1 second
    void Die()
    {
        // Instantiate the explosion at the current position
        if (explosionPrefab != null)
        {
            var explosion = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            Destroy(explosion.gameObject, 1f);
        }

        if (smokePrefab != null)
        {
            var smoke = Instantiate(smokePrefab, transform.position, Quaternion.identity);
            Destroy(smoke.gameObject, 3f);
        }

        // Handle death here
        if (GetComponent<RaycastPlayer>() != null) // Check if this is the player
        {
            // If it's the player, restart the scene
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        else // Otherwise, it's an enemy
        {
            EnemyAI enemyAI = GetComponent<EnemyAI>();
            if (enemyAI != null)
            {
                enemyAI.EnemyKilled();
            }

            // Play the enemy death sound
            AudioManager.instance.PlayOneShot(FMODEvents.instance.enemyDeathSound, transform.position);
            // Destroy the enemy
            Destroy(gameObject);
        }
    }

}
