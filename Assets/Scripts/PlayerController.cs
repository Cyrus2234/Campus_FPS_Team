using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour, IDamage
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
    [SerializeField] Transform shootPos;
    [SerializeField] GameObject bullet;
    [SerializeField] float shootRate;

    [Header("----- Grenade Stats -----")]
    [SerializeField] Transform throwPos;
    [SerializeField] GameObject grenade;
    [SerializeField] float grenadeCooldown;


    Vector3 moveDirection, playerVelocity;

    int jumpCount, healthOriginal;

    float grenadeCooldownTimer;

    bool isShooting, thrownGrenade, isSprinting;

    void Start()
    {
        healthOriginal = health;
        updatePlayerUI();
    }

    void Update()
    {
        movement();
        sprint();
        checkCooldowns();
    }

    void movement()
    {
        if (controller.isGrounded)
        {
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
            if (Input.GetButton("Grenade") && !thrownGrenade)
            {
                StartCoroutine(throwGrenade());
            }
        }
    }

    void jump()
    {
        if (Input.GetButton("Jump") && jumpCount < jumpMax)
        {
            jumpCount++;
            playerVelocity.y = jumpSpeed;
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
        if (thrownGrenade)
        {
            grenadeCooldownTimer += Time.deltaTime;
            GameManager.instance.GetGrenadeCooldownImage().fillAmount = grenadeCooldownTimer / grenadeCooldown;
        }
    }
    IEnumerator shoot()
    {
        isShooting = true;

        Instantiate(bullet, shootPos.position, transform.rotation);

        yield return new WaitForSeconds(shootRate);

        isShooting = false;
    }

    IEnumerator throwGrenade()
    {
        thrownGrenade = true;

        GameObject projectileGrenade = Instantiate(grenade, throwPos.position, transform.rotation);
        projectileGrenade.GetComponent<Rigidbody>().velocity = playerVelocity;

        yield return new WaitForSeconds(grenadeCooldown);

        thrownGrenade = false;
        grenadeCooldownTimer = 0.0f;
    }

    public void takeDamage(int amount)
    {
        health -= amount;

        updatePlayerUI();
        StartCoroutine(flashScreenDamage());

        if (health <= 0)
        {
            GameManager.instance.youLose();
        }
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
    

}