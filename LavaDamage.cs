using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LavaDamage : MonoBehaviour
{
    public Health playerHealth;
    public float damagePerSecond;
    private float damageAccumulator = 0f; // Accumulates fractional damage

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && playerHealth != null)
        {
            damageAccumulator += damagePerSecond * Time.deltaTime;
            if (damageAccumulator >= 1f)
            {
                playerHealth.ApplyDamage(Mathf.Floor(damageAccumulator));
                damageAccumulator -= Mathf.Floor(damageAccumulator);
            }
        }
    }
}
