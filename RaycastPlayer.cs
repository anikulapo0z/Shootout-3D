using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Feedbacks;
using UnityEngine.UI;
using TMPro;
//using UnityEngine.Rendering.VirtualTexturing;
using FMOD.Studio;
//using static UnityEditor.Searcher.SearcherWindow.Alignment;
using FMODUnity;

public class RaycastPlayer : MonoBehaviour
{
    public PlayerHealthShader playerHealthShader;

    public Camera playerCamera;
    public float walkSpeed = 6.0f;
    public float currentSpeed;
    public float jumpPower = 8.0f;
    public float gravity = 10.0f;

    public float lookSpeed = 2.0f;
    public float lookXLimit = 45.0f;

    Vector3 moveDirection = Vector3.zero;
    float rotationX = 0f;

    public bool canMove = true;
    public bool canShoot = false;
    public bool isGrounded;

    public int damage = 10;

    CharacterController characterController;

    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletSpeed = 100f;
    private bool isShootingInProgress = false;
    public Animator slide;

    // Crosshair
    public bool IsMoving { get; private set; }
    public bool IsSprinting { get; private set; }
    public bool IsJumping { get; private set; }
    public bool IsShooting { get; private set; }

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

    //audio
    public EventInstance playerFootsteps;
 
    //Reloading
    public int maxAmmo = 15;  // The maximum number of bullets the player can hold
    private int currentAmmo;   // The current number of bullets the player has
    private bool isReloading = false; // Whether the player is currently reloading
    public float reloadTime = 1f;     // The time it takes to reload
    public TextMeshProUGUI AmmoCountText;      // The UI Text element that displays the bullet count

    public Animator weaponAnimator;   // The animator for the weapon used for reloading animations

    //Time Slow
    public int timeSlowAmmo = 3; // Amount of slow time ammo 
    public bool isTimeSlowActive = false; // Whether time slow is currently active
    public TextMeshProUGUI timeSlowAmmoText;

    public float slowTimeFOV = 120f;  // Field of View in slowed time
    public float fovChangeSpeed = 16f;    // The speed of changing between FOVs

    //Muzzle Flash
    public GameObject muzzleFlashPrefab;
    public GameObject smokePrefab;

    //Recoil
    public Recoil recoil;

    //KeyCount
    public int keyCount = 0;

    public AudioManager audioManager;

    //Boss Variables
    //public BossAttack BossHealth;

    public PlatformMovement platformMovement;

    public GameObject CurrentlyHeldObject { get; set; }

    public void IncrementKeyCount()
    {
        keyCount++;
        // Update keycount on UI
        Debug.Log("Key Count: " + keyCount);
    }

    private void Awake()
    {
        audioManager = AudioManager.instance;
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
        
        AmmoCountText.text = currentAmmo.ToString();
        timeSlowAmmoText.text = timeSlowAmmo.ToString();

        playerFootsteps = AudioManager.instance.CreateEventInstance(FMODEvents.instance.playerFootsteps);

        isGrounded = characterController.isGrounded;

        canShoot = false;
    }

    void FixedUpdate()
    {
        // if the player pressed enter then reload the scene
        if (Input.GetKeyDown(KeyCode.CapsLock))
        {
            // Reload the scene
            GameManager.instance.ReloadScene();
        }

        // if the player pressed tab then enable shooting
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            canShoot = true;
        }

        IsMoving = Input.GetAxis("Vertical") != 0 || Input.GetAxis("Horizontal") != 0;
        IsSprinting = Input.GetKey(KeyCode.LeftShift) && IsMoving && canMove;
        IsJumping = Input.GetButton("Jump") && canMove && characterController.isGrounded;
        IsShooting = Input.GetButtonDown("Fire1") && canMove && CurrentlyHeldObject == null;

        UpdateSound();
        //check if update sound is working

    }

    // Update is called once per frame
    void Update()
    {
        // Handle firing
        if (Input.GetButtonDown("Fire1") && canMove && canShoot)
        {
            Shoot();
        }

        float speedFS = characterController.velocity.magnitude;
        
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

        currentSpeed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;

        if (Input.GetKey(KeyCode.LeftShift) && canMove && (Input.GetAxis("Vertical") != 0 || Input.GetAxis("Horizontal") != 0))
        {
            if (sprintStamina > 50 && !isSprinting)
            {
                isSprinting = true;
                sprintStamina -= 50f * Time.deltaTime; // Decrease stamina while sprinting
                //Debug.Log("Sprinting, Stamina: " + sprintStamina);
            }
            else if (sprintStamina <= 50)
            {
                isSprinting = false;
                StartCoroutine(RegenerateStamina()); // Start regenerating stamina when it's depleted
                //Debug.Log("Stamina too low to sprint: " + sprintStamina);
            }
        }
        else
        {
            isSprinting = false;
            StartCoroutine(RegenerateStamina()); // Start regenerating stamina when shift is released
        }



        //Jumping
        PlatformMovement platformMovement = null;
        if (characterController.isGrounded)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, -Vector3.up, out hit))
            {
                platformMovement = hit.transform.GetComponent<PlatformMovement>();
            }
        }

        if (Input.GetButton("Jump") && canMove && characterController.isGrounded &&
        (platformMovement == null || !platformMovement.canMove))
        {
            moveDirection.y = jumpPower;
            isGrounded = false;
            //play the jump sound
            AudioManager.instance.PlayOneShot(FMODEvents.instance.playerJump, this.transform.position);
        }
        else
        {
            moveDirection.y = movementDirectionY;
            isGrounded = characterController.isGrounded;
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
            timeSlowAmmoText.text = timeSlowAmmo.ToString();
            isTimeSlowActive = true;

            // Activate time shader
            playerHealthShader.ActivateTimeShader();

            // Reset time shader after 5 seconds
            StartCoroutine(ResetTimeShaderAfterSeconds(5f));
        }

        if (isTimeSlowActive)
        {
            playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, slowTimeFOV, Time.deltaTime * fovChangeSpeed);
        }
        else
        {
            playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, normalFOV, Time.deltaTime * fovChangeSpeed);
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
            //Debug.Log("Regenerating Stamina: " + sprintStamina);
            sprintStamina += sprintRegenRate * Time.deltaTime; // Increase stamina over time
            yield return null; // Wait for the next frame
        }
        isSprinting = false; // Allow sprinting again once stamina is fully regenerated
    }

    void Shoot()
    {
        // Check if player is currently holding an object before shooting
        if (canShoot && CurrentlyHeldObject == null && !isReloading && !isShootingInProgress)
        {

            weaponAnimator.SetTrigger("ShootAlt");

            //if the shooting animation is playing debug log to check 
            Debug.Log("Shooting");
            isShootingInProgress = true;

            if (currentAmmo > 0)
            {
                currentAmmo--; // Decrease the current ammo by 1
            }
            

            // Instantiate the bullet and get its Rigidbody component.
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, playerCamera.transform.rotation);
            Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();

            Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
            RaycastHit hit;

            // Fire the ray for a distance of 1000 units (or whatever your max distance should be)
            if (Physics.Raycast(ray, out hit, 5000f))
            {
                // If the ray hit a collider, apply damage or other effects to this game object
                var health = hit.collider.gameObject.GetComponent<Health>();

                if (health != null)
                {
                    health.ApplyDamage(damage);
                }

                var bossHealth = hit.collider.gameObject.GetComponentInParent<BossAttack>();

                // If the ray hit the boss, apply damage ousing boss attack take damage function
                if (hit.collider.gameObject.CompareTag("Boss"))
                {
                    bossHealth.TakeDamage(damage);
                }

            }

            //Spawn muzzle flash at the fire point position, with the same rotation as the fire point, and it should follow the fire point transform
            GameObject muzzleFlash = Instantiate(muzzleFlashPrefab, firePoint.position, firePoint.rotation, firePoint);
            // Spawn smoke at the bullet's position, with the same rotation as the bullet
            GameObject smoke = Instantiate(smokePrefab, bullet.transform.position, bullet.transform.rotation, bullet.transform);
            // play the shoot sound
            AudioManager.instance.PlayOneShot(FMODEvents.instance.playerShoot, this.transform.position);

            // Turn the muzzle flash off after 0.5 seconds
            Destroy(muzzleFlash, 0.5f);

            // ADD RECOIL
            recoil.recoil();

            if (bulletRb != null)
            {
                // Fire the bullet in the direction the camera is facing.
                bulletRb.velocity = playerCamera.transform.forward * bulletSpeed;
                Destroy(bullet, 10.0f);
            }

            StartCoroutine(ResetShootAnimation());
            //weaponAnimator.ResetTrigger("ShootAlt");
        }

        // If the player is currently reloading, don't play the shooting animation
        else
        {
            weaponAnimator.SetBool("Shoot", false);
        }

        //play the gunslide animation from the animation controller in gunslide and reset it

        slide.SetTrigger("ShootAlt");

    }

    
    IEnumerator ResetShootAnimation()
    {
        yield return new WaitForSeconds(0.00001f); // Adjust the delay as needed
        weaponAnimator.SetBool("Shoot", false);
        slide.SetBool("Shoot", false);

        // Reset the shooting flag to false
        isShootingInProgress = false;
    }
    

    public void ResetTimeSlow()
    {
        isTimeSlowActive = false;
        timeSlowAmmoText.text = timeSlowAmmo.ToString(); // And this line
    }

    public void IncrementTimeSlowAmmo()
    {
        timeSlowAmmo++;
        timeSlowAmmoText.text = timeSlowAmmo.ToString(); // Add this line
        AudioManager.instance.PlayOneShot(FMODEvents.instance.timeSlowPickupSound, this.transform.position);
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

    private void UpdateSound()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // Play fmod footstep sounds if the player is moving and the footstep sound is not already playing and the player is grounded
        if (horizontal != 0 || vertical != 0 && isGrounded)
        {
            var attributes = RuntimeUtils.To3DAttributes(transform.position);
            playerFootsteps.set3DAttributes(attributes);

            Debug.Log("Player moving and grounded");
            PLAYBACK_STATE playbackState;
            playerFootsteps.getPlaybackState(out playbackState);
            if (playbackState.Equals(PLAYBACK_STATE.STOPPED))
            {
                playerFootsteps.start();
                //Debug.Log("Playing footstep sound");

            }
        }
        else
        {
            playerFootsteps.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            //Debug.Log("Footstep sound stopped");
        }

        // if time slow is active decrease the pitch of all sounds
        if (isTimeSlowActive)
        {
            foreach (EventInstance soundEvent in audioManager.eventInstances)
            {
                soundEvent.setParameterByName("Pitch", 0f);
            }

            foreach (StudioEventEmitter emitter in audioManager.studioEventEmitters)
            {
                emitter.EventInstance.setParameterByName("Pitch", 0f);
            }
        }
        else
        {
            foreach (EventInstance soundEvent in audioManager.eventInstances)
            {
                soundEvent.setParameterByName("Pitch", 2f);
            }

            foreach (StudioEventEmitter emitter in audioManager.studioEventEmitters)
            {
                emitter.EventInstance.setParameterByName("Pitch", 2f);
            }
        }
    }

}
