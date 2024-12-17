using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class enemyAi : MonoBehaviour, IDamage
{
    [SerializeField] Renderer model;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Animator anim;

    [SerializeField] Transform shootPos;
    [SerializeField] Transform headPos;

    [SerializeField] int HP;
    [SerializeField] int faceTargetSpeed;
    [SerializeField] int fov;
    [SerializeField] int roamDis;
    [SerializeField] int roamTimer;
    [SerializeField] int aniSpeedTans;




    [SerializeField] GameObject bullet;
    [SerializeField] float shootRate;

    Vector3 playerDir;
    Vector3 startingPos;

    bool playerInRange;
    bool isShooting;
    bool isRoaming;

    Color colorOrig;

    float angleToPlayer;
    float stoppingDisOrg;

    Coroutine co;

    // Start is called before the first frame update
    void Start()
    {
        colorOrig = model.material.color;
        GameManager.instance.updateGameGoal(1);
        startingPos = transform.position;
        stoppingDisOrg = agent.stoppingDistance;
    }

    // Update is called once per frame
    void Update()
    {
        float agentSpeed = agent.velocity.normalized.magnitude;
        float animSpeed = anim.GetFloat("Speed");

        anim.SetFloat("Speed", Mathf.MoveTowards(animSpeed, agentSpeed, Time.deltaTime * aniSpeedTans));

        if (playerInRange && !canSeePlayer())
        {
            if (!isRoaming && agent.remainingDistance < 0.01f)
            {
                co = StartCoroutine(roam());
            }
        }
        else if (!playerInRange)
        {
            if (!isRoaming && agent.remainingDistance < 0.01f)
            {
                co = StartCoroutine(roam());
            }
        }
    }

    IEnumerator roam()
    {
        isRoaming = true;
        yield return new WaitForSeconds(roamTimer);

        agent.stoppingDistance = 0;

        Vector3 randomPos = Random.insideUnitSphere * roamDis;
        randomPos += startingPos;

        NavMeshHit hit;
        NavMesh.SamplePosition(randomPos, out hit, roamDis, 1);
        agent.SetDestination(hit.position);

        isRoaming = false;

    }

    bool canSeePlayer()
    {

        playerDir = GameManager.instance.player.transform.position - headPos.position;
        angleToPlayer = Vector3.Angle(playerDir, transform.forward);


        Debug.DrawRay(headPos.position, playerDir);
        RaycastHit hit;

        if (Physics.Raycast(headPos.position, playerDir, out hit))
        {
            if (hit.collider.CompareTag("Player") && angleToPlayer <= fov)
            {

                agent.SetDestination(GameManager.instance.player.transform.position);

                if (agent.remainingDistance < agent.stoppingDistance)
                {
                    faceTarget();
                }

                if (!isShooting)
                {
                    StartCoroutine(shoot());
                }
                agent.stoppingDistance = stoppingDisOrg;

                return true;
            }
        }
        agent.stoppingDistance = 0;
        return false;
    }

    void faceTarget()
    {
        Quaternion rot = Quaternion.LookRotation(new Vector3(playerDir.x, 0, playerDir.z));
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * faceTargetSpeed);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            agent.stoppingDistance = 0;
        }
    }



    public void takeDamage(int amount)
    {
        HP -= amount;
        agent.SetDestination(GameManager.instance.player.transform.position);

        if (co != null)
        StopCoroutine(co);

        isRoaming = false;
        StartCoroutine(flashRed());
        if (HP <= 0)
        {
            // I'm Dead
            GameManager.instance.updateGameGoal(-1);
            Destroy(gameObject);
        }
    }

    IEnumerator shoot()
    {
        isShooting = true;
        anim.SetTrigger("Shoot");
        Instantiate(bullet, shootPos.position, transform.rotation);
        yield return new WaitForSeconds(shootRate);
        isShooting = false;
    }

    IEnumerator flashRed()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = colorOrig;
    }


}
