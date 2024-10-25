using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Feedbacks;
using UnityEngine.UI;
using TMPro;
//using UnityEngine.Rendering.VirtualTexturing;

[RequireComponent(typeof(CharacterController))]
public class FPSController : MonoBehaviour
{
    public PlayerHealthShader playerHealthShader;

    public Camera playerCamera;
    public float walkSpeed = 6.0f;
    public float jumpPower = 8.0f;
    public float gravity = 10.0f;

    public float lookSpeed = 2.0f;
    public float lookXLimit = 45.0f;

    Vector3 moveDirection = Vector3.zero;
    float rotationX = 0f;

    public bool canMove = true;

    CharacterController characterController;

    public GameObject bulletPrefab;
    public Transform firePoint; 
    public float bulletSpeed = 100f;
    private bool isShootingInProgress = false;
    public Animator slide;

    public Texture2D crosshairImage;
    public float crosshairScale = 1;
    public float crosshairScaleMin = 0.5f;
    public float crosshairScaleMax = 1.25f;
    public float crosshairSpeed = 1f;

    // Sprinting
    public float runSpeed = 12.0f;
    public float sprintStamina = 1f; // The player's sprint stamina
    public float sprintRegenRate = 0.05f; // The rate at which stamina regenerates
    public float sprintRegenDelay = 2f; // The delay before stamina starts regenerating
    private bool isSprinting = false; // Whether the player is currently sprinting

    //ADS variables
    public float normalFOV = 60f;  // Field of View when not ADS
    public float aimingFOV = 30f;  // Field of View when ADS
    public float aimSpeed = 8f;    // The speed of changing between FOVs

    //FEEL//
    public MoreMountains.Feedbacks.MMFeedbacks landingFeedbacks;
    private bool wasInAir = false;

    //Sound
    public AudioSource jumpSound;
    //public AudioSource landSound;
    public AudioSource GunSound;
    //Random Footstep Sound
    [SerializeField] private AudioClip[] footstepSounds;
    [SerializeField] private AudioSource audioSourceFS;
    [SerializeField] private float minSpeedFS = 0.1f;
    [SerializeField] private float maxSpeedFS = 10f;
    [SerializeField] private float stepInterval = 4f;
    [SerializeField] private float speedFS;
    [SerializeField] private float sprintMultiplier = 2f;
    private float lastFootstepTime;
    private AudioClip lastFootstepClip;

    //Reloading
    public int maxAmmo = 15;  // The maximum number of bullets the player can hold
    private int currentAmmo;   // The current number of bullets the player has
    private bool isReloading = false; // Whether the player is currently reloading
    public float reloadTime = 2f;     // The time it takes to reload
    public TextMeshProUGUI AmmoCountText;      // The UI Text element that displays the bullet count

    public Animator weaponAnimator;   // The animator for the weapon used for reloading animations

    //Time Slow
    public int timeSlowAmmo = 3; // Amount of slow time ammo 
    private bool isTimeSlowActive = false; // Whether time slow is currently active
    public TextMeshProUGUI timeSlowAmmoText;

    //Muzzle Flash
    public GameObject muzzleFlashPrefab;
    public GameObject smokePrefab;

    //Recoil
    public Recoil recoil;

    //KeyCount
    public int keyCount = 0;

    public GameObject CurrentlyHeldObject { get; set; }

    public void IncrementKeyCount()
    {
        keyCount++;
        // Update keycount on UI
        Debug.Log("Key Count: " + keyCount);
    }

    // Start is called before the first frame update
    void Start()
    {
        characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        // don't play the landing feedbacks at start
        landingFeedbacks.StopFeedbacks();

        currentAmmo = maxAmmo;
        // Update the ammo text TEXT MESH PRO
        AmmoCountText.text = currentAmmo.ToString();

        // Initialize the last footstep time
        lastFootstepTime = Time.time;
    }

    void FixedUpdate()
    {
        // Handle firing
        if (Input.GetButtonDown("Fire1") && canMove && CurrentlyHeldObject == null)
        {
            Shoot();
        }
    }

    private float GetInterval(float speedFS)
    {
        if (isSprinting)
        {
            return (stepInterval / speedFS) / sprintMultiplier;
        }
        else
        {
            return stepInterval / speedFS;
        }
    }

    // Update is called once per frame
    void Update()
    {
        float speedFS = characterController.velocity.magnitude;
        if (speedFS > minSpeedFS && speedFS < maxSpeedFS)
        {
            if (Time.time > lastFootstepTime + GetInterval(speedFS))
            {
                PlayFootstepSound();
            }

            // Play the footstep sound with its multiplier if the player is sprinting



        }
        
        if (currentAmmo < 1) StartCoroutine(Reload());

        // Update the ammo text TEXT MESH PRO
        AmmoCountText.text = currentAmmo.ToString();

        //handle reloading
        if (Input.GetKeyDown(KeyCode.R)) // The R key is used for reloading
        {
            StartCoroutine(Reload());
        }

        // Handle movement
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        //Left Shift to run
        float curSpeedX = canMove ? (Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed) : 0;
        float movementDirectionY = moveDirection.y;
        moveDirection = (forward * Input.GetAxis("Vertical")) + (right * Input.GetAxis("Horizontal"));
        moveDirection = moveDirection.normalized * curSpeedX;
        if (Input.GetKey(KeyCode.LeftShift) && canMove && (Input.GetAxis("Vertical") != 0 || Input.GetAxis("Horizontal") != 0))
        {
            if (sprintStamina > 50 && !isSprinting)
            {
                isSprinting = true;
                sprintStamina -= 50f * Time.deltaTime; // Decrease stamina while sprinting
                Debug.Log("Sprinting, Stamina: " + sprintStamina);
            }
            else if (sprintStamina <= 50)
            {
                isSprinting = false;
                StartCoroutine(RegenerateStamina()); // Start regenerating stamina when it's depleted
                Debug.Log("Stamina too low to sprint: " + sprintStamina);
            }
        }
        else
        {
            isSprinting = false;
            StartCoroutine(RegenerateStamina()); // Start regenerating stamina when shift is released
        }

        

        //Jumping
        if (Input.GetButton("Jump") && canMove && characterController.isGrounded)
        {
            moveDirection.y = jumpPower;
            // Play the jump sound simple
            jumpSound.Play();
        }
        else
        {
            moveDirection.y = movementDirectionY;
        }
        if (!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }

        characterController.Move(moveDirection * Time.deltaTime);
        if (canMove)
        {
            rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
        }

        //Handle Time Slow
        if (Input.GetKeyDown(KeyCode.T) && timeSlowAmmo > 0 && !isTimeSlowActive)
        {

            GameManager.instance.SlowTime(0.5f, 5f);
            timeSlowAmmo--;
            timeSlowAmmoText.text = "Time Slow Ammo: " + timeSlowAmmo.ToString();
            isTimeSlowActive = true;

            // Activate time shader
            playerHealthShader.ActivateTimeShader();

            // Reset time shader after 5 seconds
            StartCoroutine(ResetTimeShaderAfterSeconds(5f));
        }

        IEnumerator ResetTimeShaderAfterSeconds(float seconds)
        {
            yield return new WaitForSeconds(seconds);
            playerHealthShader.ResetTimeShader();
        }

        // Handle Aiming Down Sights
        if (Input.GetMouseButton(1)) // Right mouse button
        {
            weaponAnimator.SetBool("IsAiming", true);
            playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, aimingFOV, Time.deltaTime * aimSpeed);
            // slowd down the look speed when aiming
            lookSpeed = 1f;
        }
        else
        {
            weaponAnimator.SetBool("IsAiming", false);
            playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, normalFOV, Time.deltaTime * aimSpeed);
        }

        // Handle custom cursor
        float finalCrosshairScale = crosshairScale;
        finalCrosshairScale += Input.GetAxis("Mouse ScrollWheel") * crosshairSpeed;
        finalCrosshairScale = Mathf.Clamp(finalCrosshairScale, crosshairScaleMin, crosshairScaleMax);
        crosshairScale = finalCrosshairScale;

        //Handle Camera Shake
        if (wasInAir && characterController.isGrounded)
        {
            wasInAir = false;
            //landSound.Play();
            // Play the landing feedbacks
            landingFeedbacks.PlayFeedbacks();
        }
        else if (!characterController.isGrounded)
        {
            // If the character is not grounded, they are in the air
            wasInAir = true;
        }
    }
    IEnumerator RegenerateStamina()
    {
        yield return new WaitForSeconds(sprintRegenDelay); // Wait for the specified delay
        while (sprintStamina < 100f) // Loop until stamina is fully regenerated
        {
            Debug.Log("Regenerating Stamina: " + sprintStamina);
            sprintStamina += sprintRegenRate * Time.deltaTime; // Increase stamina over time
            yield return null; // Wait for the next frame
        }
        isSprinting = false; // Allow sprinting again once stamina is fully regenerated
    }

    private void PlayFootstepSound()
    {
        int clipCount = footstepSounds.Length;
        List<AudioClip> availableClips = new List<AudioClip>();
        for (int i = 0; i < clipCount; i++)
        {
            if (footstepSounds[i] != lastFootstepClip)
            {
                availableClips.Add(footstepSounds[i]);
            }
        }

        //if there are no available clips, use the last one
        if (availableClips.Count == 0)
        {
            availableClips.Add(lastFootstepClip);
        }

        //otherwise, pick a random clip from the list and update the last clip
        else
        {
            int clipIndex = Random.Range(0, availableClips.Count);
            AudioClip clipToPlay = availableClips[clipIndex];
            audioSourceFS.PlayOneShot(clipToPlay);
            lastFootstepClip = clipToPlay;
        }

        lastFootstepTime = Time.time;
        
    }

    void Shoot()
    {
        // Check if player is currently holding an object before shooting
        if (CurrentlyHeldObject == null && !isReloading && !isShootingInProgress)
        {
            // play the shooting animation if the player is not currently reloading
            weaponAnimator.SetBool("Shoot", true);
            //if the shooting animation is playing debug log to check 
            Debug.Log("Shooting");
            isShootingInProgress = true;
            
            currentAmmo--; // Decrease the current ammo by 1

            // Instantiate the bullet and get its Rigidbody component.
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, playerCamera.transform.rotation);
            Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();

            //Spawn muzzle flash at the fire point position, with the same rotation as the fire point, and it should follow the fire point transform
            GameObject muzzleFlash = Instantiate(muzzleFlashPrefab, firePoint.position, firePoint.rotation, firePoint);
            // Spawn smoke at the bullet's position, with the same rotation as the bullet
            GameObject smoke = Instantiate(smokePrefab, bullet.transform.position, bullet.transform.rotation, bullet.transform);

            // Turn the smoke off after 0.5 seconds
            // Destroy(smoke, 20f);

            // Turn the muzzle flash off after 0.5 seconds
            Destroy(muzzleFlash, 0.5f);
            
            //Don't play the sound if it's already playing
            if (!GunSound.isPlaying)
            {
                GunSound.pitch = Random.Range(0.8f, 1.1f);
                GunSound.Play();
            }

            // ADD RECOIL
            recoil.recoil();
                
            if (bulletRb != null)
            {
                // Fire the bullet in the direction the camera is facing.
                bulletRb.velocity = playerCamera.transform.forward * bulletSpeed;
            }

            StartCoroutine(ResetShootAnimation());
        }

        // If the player is currently reloading, don't play the shooting animation
        else
        {
            weaponAnimator.SetBool("Shoot", false);
        }

        //play the gunslide animation from the animation controller in gunslide and reset it

        slide.SetBool("Shoot", true);

    }
    IEnumerator ResetShootAnimation()
    {
        yield return new WaitForSeconds(0.1f); // Adjust the delay as needed
        weaponAnimator.SetBool("Shoot", false);
        slide.SetBool("Shoot", false);

        // Reset the shooting flag to false
        isShootingInProgress = false;
    }

    public void ResetTimeSlow()
    {
        isTimeSlowActive = false;
        timeSlowAmmoText.text = "Time Slow Ammo: " + timeSlowAmmo.ToString(); // And this line
    }

    public void IncrementTimeSlowAmmo()
    {
        timeSlowAmmo++;
        timeSlowAmmoText.text = "Time Slow Ammo: " + timeSlowAmmo.ToString(); // Add this line
    }

    IEnumerator Reload()
    {
        // Set the reloading flag to true
        isReloading = true;

        // Play the reload animation
        weaponAnimator.SetBool("Reload", true);

        // Wait for the reload time
        yield return new WaitForSeconds(reloadTime);

        // Refill the ammo
        currentAmmo = maxAmmo;

        // Set the reloading flag to tree
        isReloading = false;

        //reset the reload animation
        weaponAnimator.SetBool("Reload", false);
    }

    // HANDLE Custom cursor
    void OnGUI()
    {
        float xMin = (Screen.width / 2) - (crosshairImage.width / 2);
        float yMin = (Screen.height / 2) - (crosshairImage.height / 2);
        GUI.DrawTexture(new Rect(xMin, yMin, crosshairImage.width, crosshairImage.height), crosshairImage);
    }

        
}
