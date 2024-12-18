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
    [SerializeField][Range(2, 5)] int sprintMod;
    [SerializeField][Range(1, 3)] int jumpMax;
    [SerializeField][Range(5, 20)] int jumpSpeed;
    [SerializeField][Range(15, 40)] int gravity;

    [Header("----- Gun Stats -----")]
//    [SerializeField] List<gunStats> gunList = new List<gunStats>();
//    [SerializeField] GameObject gunModel;
    [SerializeField] Transform shootPos;
    [SerializeField] GameObject bullet;
    [SerializeField] float shootRate;

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

    Vector3 moveDirection, playerVelocity;

    int jumpCount, healthOriginal, gunListPos;

    float throwableCooldownTimer;

    bool isShooting, isSprinting, isPlayingStep, thrownObject;

    void Start()
    {
        throwableCooldownTimer = throwableCooldown;
        healthOriginal = health;
        updatePlayerUI();
    }

    void Update()
    {
        if (!GameManager.instance.GetPauseState())
        {
            movement();
            checkCooldowns();
        }
        sprint();
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

        controller.Move(playerVelocity * Time.deltaTime);
        playerVelocity.y -= gravity * Time.deltaTime;

        if ((controller.collisionFlags & CollisionFlags.Above) != 0)
        {
            playerVelocity = Vector3.zero;
        }

        if (!GameManager.instance.GetPauseState())
        {
            if (Input.GetButton("Fire1") && !isShooting)
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
            aud.PlayOneShot(audJump[Random.Range(0, audJump.Length)], audJumpVel);
        }
    }

    void sprint()
    {
        if (Input.GetButtonDown("Sprint"))
        {
            speed *= sprintMod;
            isSprinting = true;
        }
        else if (Input.GetButtonUp("Sprint"))
        {
            speed /= sprintMod;
            isSprinting = false;
        }
    }
    void checkCooldowns()
    {
        if (thrownObject)
        {
            throwableCooldownTimer -= Time.deltaTime;
            GameManager.instance.GetGrenadeCooldownImage().fillAmount = throwableCooldownTimer / throwableCooldown;
        }
    }
    IEnumerator shoot()
    {
        isShooting = true;

        Instantiate(bullet, shootPos.position, transform.rotation);

        //aud.PlayOneShot(gunList[gunListPos].shootSound[Random.Range(0, gunList[gunListPos].shootSound.Length)], gunList[gunListPos].shootSoundVol);

        yield return new WaitForSeconds(shootRate);

        isShooting = false;
    }

    IEnumerator throwThrowable()
    {
        thrownObject = true;

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
        aud.PlayOneShot(audDmg[Random.Range(0, audDmg.Length)], audDmgVel);

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

        aud.PlayOneShot(audStep[Random.Range(0, audStep.Length)], audStepVel);

        if (!isSprinting)
            yield return new WaitForSeconds(0.5f);
        else
            yield return new WaitForSeconds(0.3f);

        isPlayingStep = false;
    }

}