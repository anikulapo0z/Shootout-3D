using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BaseGun : MonoBehaviour
{
    public Camera playerCamera;

    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletSpeed = 100f;

    public AudioSource GunSound;

    //Reloading
    public int maxAmmo = 15;  // The maximum number of bullets the player can hold
    private int currentAmmo;   // The current number of bullets the player has
    private bool isReloading = false; // Whether the player is currently reloading
    public float reloadTime = 2f;     // The time it takes to reload
    public TextMeshProUGUI AmmoCountText;

    //Muzzle Flash
    public GameObject muzzleFlashPrefab;

    //Recoil
    public Recoil recoil;

    public Animator weaponAnimator;
    protected virtual void Start()
    {
        currentAmmo = maxAmmo;

        if (AmmoCountText != null)
        {
            AmmoCountText.text = currentAmmo.ToString();
        }
        else
        {
            Debug.LogWarning("AmmoCountText is not assigned in the inspector");
        }
    }

    protected virtual void Shoot()
    {
        if (currentAmmo < 1)
        {
            return;
        }
        if (!isReloading)
        {
            currentAmmo--;
            if (AmmoCountText != null)
            {
                AmmoCountText.text = currentAmmo.ToString();
            }
            else
            {
                Debug.LogWarning("AmmoCountText is not assigned in the inspector");
            }
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, playerCamera.transform.rotation);
            Rigidbody rb = bullet.GetComponent<Rigidbody>();
            rb.AddForce(firePoint.forward * bulletSpeed, ForceMode.Impulse);
            Destroy(bullet, 2f);
            if (muzzleFlashPrefab != null)
            {
                GameObject muzzleFlash = Instantiate(muzzleFlashPrefab, firePoint.position, firePoint.rotation, firePoint);
                Destroy(muzzleFlash, 0.1f);
            }
            if (!GunSound.isPlaying)
            {
                GunSound.pitch = Random.Range(0.8f, 1.1f);
                GunSound.Play();
            }

            if (rb != null)
            {
                // Fire the bullet in the direction the camera is facing.
                rb.velocity = playerCamera.transform.forward * bulletSpeed;
            }
        }
        
    }

    protected virtual void Update()
    {
        if (isReloading)
        {
            return;
        }

        if (currentAmmo < 1 && Input.GetKeyDown(KeyCode.R))
        {
            StartCoroutine(Reload());
            return;
        }

        if (Input.GetButtonDown("Fire1"))
        {
            Shoot();
        }
    }

    protected virtual IEnumerator Reload()
    {
        isReloading = true;
        weaponAnimator.SetTrigger("Reload");
        yield return new WaitForSeconds(reloadTime);
        currentAmmo = maxAmmo;
        isReloading = false;
    }
}
