using Microsoft.Win32.SafeHandles;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour, IDamage, IStunnable
{
    [Header("----- Components -----")]
    [SerializeField] CharacterController controller;
    [SerializeField] Renderer model;
    [SerializeField] LayerMask ignoreMask;

    [Header("----- Stats -----")]
    [SerializeField][Range(1, 10)] int health;
    [SerializeField][Range(1, 5)] int speed;
    [SerializeField][Range(1, 5)] int crouchSpeed;
    [SerializeField][Range(2, 5)] int sprintMod;
    [SerializeField][Range(1, 3)] int jumpMax;
    [SerializeField][Range(5, 20)] int jumpSpeed;
    [SerializeField][Range(15, 40)] int gravity;
    [SerializeField][Range(1, 10)] float stamina;
    [SerializeField][Range(1, 10)] float runDelay;
    [SerializeField][Range(1, 10)] float staminaUsage;
    [SerializeField][Range(1, 10)] float staminaRegen;

    [Header("----- Gun Stats -----")]
    //[SerializeField] List<gunStats> gunList = new List<gunStats>();
    //[SerializeField] GameObject gunModel;
    [SerializeField] Transform shootPos;
    [SerializeField] GunStats shotType;

    //[SerializeField] GameObject bullet;
    //[SerializeField] float shootRate;
    //[SerializeField] int maxAmmo;
    //[SerializeField] float reloadTime;

    [Header("----- Throwable Object Stats -----")]
    [SerializeField] Transform throwPos;
    [SerializeField] GameObject throwableObject;
    [SerializeField] float throwableCooldown;

    [Header("----- Player Sounds -----")]
    [SerializeField] AudioSource aud;
    [SerializeField] AudioClip[] audJump;
    [SerializeField][Range(0, 1)] float audJumpVel;
    [SerializeField] AudioClip[] audDmg;
    [SerializeField][Range(0, 1)] float audDmgVel;
    [SerializeField] AudioClip[] audStep;
    [SerializeField][Range(0, 1)] float audStepVel;
    [SerializeField] AudioClip[] audShot;
    [SerializeField][Range(0, 1)] float audShotVel;
    [SerializeField] AudioClip[] audReload;
    [SerializeField][Range(0, 1)] float audReloadVel;
    [SerializeField] AudioClip[] audClick;
    [SerializeField][Range(0, 1)] float audClickVel;

    Vector3 moveDirection, playerVelocity;
    int jumpCount, healthOriginal, speedOriginal, gunListPos, sprintSpeed, currentAmmo;
    float throwableCooldownTimer, staminaMax, staminaPercentage, totalDelay, reloadCooldownTimer, crouchOriginal, crouchHeight;
    bool isShooting, isSprinting, isPlayingStep, thrownObject, isCrouching, hasRan, isReloading;

    Coroutine reloading;

    void Start()
    {
        if (GameManager.instance.isStartScreen)
            return;

        throwableCooldownTimer = throwableCooldown;
        reloadCooldownTimer = shotType.reloadTime;
        healthOriginal = health;
        speedOriginal = speed;
        crouchOriginal = controller.height;
        crouchHeight = crouchOriginal / 2;
        sprintSpeed = speed * sprintMod;
        staminaMax = stamina;
        currentAmmo = shotType.maxAmmo;

        updatePlayerUI();
        updateStaminaUI();
    }

    void Update()
    {
        if (GameManager.instance.isStartScreen)
            return;

        if (!GameManager.instance.GetPauseState())
        {
            movement();
            checkCooldowns();
        }
        staminaPercentage = stamina / staminaMax;
        Sprint();
    }

    void movement()
    {
        if (controller.isGrounded)
        {
            if (moveDirection.magnitude > 0.3f && !isPlayingStep)
            {
                StartCoroutine(playStep());
            }
            jumpCount = 0;
            playerVelocity = Vector3.zero;
        }

        moveDirection = transform.right * Input.GetAxis("Horizontal") +
                        transform.forward * Input.GetAxis("Vertical");

        controller.Move(moveDirection * speed * Time.deltaTime);

        jump();
        crouch();

        controller.Move(playerVelocity * Time.deltaTime);
        playerVelocity.y -= gravity * Time.deltaTime;

        if ((controller.collisionFlags & CollisionFlags.Above) != 0)
        {
            playerVelocity = Vector3.zero;
        }

        if (!GameManager.instance.GetPauseState())
        {
            if (Input.GetButton("Fire1") && !isShooting && !isReloading)
            {
                StartCoroutine(shoot());
            }
            if (Input.GetButton("Throwable") && !thrownObject)
            {
                StartCoroutine(throwThrowable());
            }
        }
    }

    void jump()
    {
        if (Input.GetButton("Jump") && jumpCount < jumpMax)
        {
            jumpCount++;
            playerVelocity.y = jumpSpeed;
            playSound(audJump[Random.Range(0, audJump.Length)], audJumpVel);
        }
    }

    void crouch()
    {
        if (Input.GetButtonDown("Crouch") && !isSprinting)
        {
            isCrouching = !isCrouching;
            speed = isCrouching ? crouchSpeed : speedOriginal;
            controller.height = isCrouching ? crouchHeight : crouchOriginal;
        }
    }
    public void updateStaminaUI()
    {
        GameManager.instance.playerStaminaBar.fillAmount = stamina / staminaMax;
        GameManager.instance.playerStaminaBack.color = hasRan ? new Color(0.5f, 0.5f, 0.5f, 0.375f) : new Color(0, 0, 0, 0.375f);
    }
    public void updateAmmoUI()
    {
        GameManager.instance.GetAmmoAmountImage().fillAmount = (float)currentAmmo / shotType.maxAmmo;
        GameManager.instance.updateAmmoAmount(currentAmmo.ToString() + " / " + shotType.maxAmmo.ToString());
    }
    public void playSound(AudioClip sound, float vol = 0.15f)
    {
        aud.PlayOneShot(sound, vol);
    }

    void Sprint()
    {
        if (!isCrouching)
        {
            if (Input.GetButtonDown("Sprint") && !hasRan && stamina > 0)
            {
                speed = sprintSpeed;
                isSprinting = true;
            }

            if (Input.GetButtonUp("Sprint") || stamina <= 0)
            {
                speed = speedOriginal;
                isSprinting = false;
                StartCoroutine(SprintDelay());
            }
        }
        
        if (Input.GetButton("Sprint") && isSprinting)
        {
            stamina -= staminaUsage * Time.deltaTime;
            updateStaminaUI();
        }

        if (!isSprinting && stamina < staminaMax)
        {
            stamina += staminaRegen * Time.deltaTime;
            stamina = stamina > staminaMax ? staminaMax : stamina;
            updateStaminaUI();
        }
    }

    IEnumerator SprintDelay()
    {
        hasRan = true;
        totalDelay = runDelay * (1 - staminaPercentage);

        yield return new WaitForSeconds(totalDelay);

        hasRan = false;
    }

    //IEnumerator NoStamina()
    //{
    //    speed = speedOriginal;
    //    totalDelay = runDelay*2;
    //    //hadRan = true;

    //    yield return new WaitForSeconds(totalDelay);

    //    hasRan = false;
    //}

    void checkCooldowns()
    {
        if (thrownObject)
        {
            throwableCooldownTimer -= Time.deltaTime;
            GameManager.instance.GetGrenadeCooldownImage().fillAmount = throwableCooldownTimer / throwableCooldown;
        }
        if (isReloading)
        {
            reloadCooldownTimer -= Time.deltaTime;
            GameManager.instance.GetReloadCooldownImage().fillAmount = reloadCooldownTimer / shotType.reloadTime;
        }
    }
    IEnumerator reloadAmmo()
    {
        isReloading = true;
        playSound(audClick[Random.Range(0, audClick.Length)], audReloadVel);

        yield return new WaitForSeconds(shotType.reloadTime); //reload takes 4x longer (need field)

        playSound(audReload[Random.Range(0, audReload.Length)], audReloadVel);
        isReloading = false;

        currentAmmo = shotType.maxAmmo;
        reloadCooldownTimer = shotType.reloadTime;

        updateAmmoUI();
    }
    IEnumerator shoot()
    {
        isShooting = true;

        Instantiate(shotType.bullet, shootPos.position, Camera.main.transform.rotation);
        --currentAmmo;
        updateAmmoUI();
        playSound(audShot[Random.Range(0, audShot.Length)], audShotVel);

        if (currentAmmo <= 0)
            reloading = StartCoroutine(reloadAmmo());
        
        yield return new WaitForSeconds(shotType.shootRate);

        isShooting = false;
    }
    IEnumerator throwThrowable()
    {
        thrownObject = true;

        playSound(audClick[Random.Range(0, audClick.Length)], audReloadVel);
        GameObject projectileGrenade = Instantiate(throwableObject, throwPos.position, transform.rotation);
        projectileGrenade.GetComponent<Rigidbody>().velocity = playerVelocity;

        yield return new WaitForSeconds(throwableCooldown);

        thrownObject = false;
        throwableCooldownTimer = throwableCooldown;
    }

    public void takeDamage(int amount)
    {
        health -= amount;

        updatePlayerUI();
        StartCoroutine(flashScreenDamage());
        playSound(audDmg[Random.Range(0, audDmg.Length)], audDmgVel);

        if (health <= 0)
        {
            GameManager.instance.youLose();
        }
    }
    
    public void stunObject(int stunTime)
    {
        StartCoroutine(stun(stunTime));
    }

    IEnumerator stun(int stunTime)
    {
        GameManager.instance.playerStunScreen.SetActive(true);

        yield return new WaitForSeconds(stunTime);

        GameManager.instance.playerStunScreen.SetActive(false);
    }

    IEnumerator flashScreenDamage()
    {
        GameManager.instance.playerDamageScreen.SetActive(true);

        yield return new WaitForSeconds(0.1f);

        GameManager.instance.playerDamageScreen.SetActive(false);
    }
    public void updatePlayerUI()
    {
        GameManager.instance.playerHPBar.fillAmount = (float)health / healthOriginal;
    }
    public void changeThrowable(GameObject throwable)
    {
        throwableObject = throwable;
    }
    public void changeBullet(GunStats newShotType)
    {
        currentAmmo = 0;
        shotType = newShotType;

        if (reloading != null)
            StopCoroutine(reloading);
        
        reloadCooldownTimer = shotType.reloadTime;
        GameManager.instance.GetReloadCooldownImage().fillAmount = reloadCooldownTimer / shotType.reloadTime;
        reloading = StartCoroutine(reloadAmmo());
    }

    /*
    public void getGunStats(gunStats gun)
    {
        gunList.Add(gun);
        gunListPos = gunList.Count - 1;

        shootDamage = gun.shootDMG;
        shootDist = gun.shootDist;
        shootRate = gun.shootRate;

        gunModel.GetComponent<MeshFilter>().sharedMesh = gun.model.GetComponent<MeshFilter>().sharedMesh;
        gunModel.GetComponent<MeshRenderer>().sharedMaterial = gun.model.GetComponent<MeshRenderer>().sharedMaterial;
    }

    void selectGun()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0 && gunListPos < gunList.Count - 1)
        {
            gunListPos++;
            changeGun();
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0 && gunListPos > 0)
        {
            gunListPos--;
            changeGun();
        }
    }

    void changeGun()
    {
        shootDamage = gunList[gunListPos].shootDMG;
        shootDist = gunList[gunListPos].shootDist;
        shootRate = gunList[gunListPos].shootRate;

        gunModel.GetComponent<MeshFilter>().sharedMesh = gunList[gunListPos].model.GetComponent<MeshFilter>().sharedMesh;
        gunModel.GetComponent<MeshRenderer>().sharedMaterial = gunList[gunListPos].model.GetComponent<MeshRenderer>().sharedMaterial;
    }
    */

    IEnumerator playStep()
    {
        isPlayingStep = true;

        playSound(audStep[Random.Range(0, audStep.Length)], audStepVel);

        if (!isSprinting)
            yield return new WaitForSeconds(0.5f);
        else
            yield return new WaitForSeconds(0.3f);

        isPlayingStep = false;
    }

}