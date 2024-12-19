using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class enemyAi : MonoBehaviour, IDamage, IStunnable
{
    [SerializeField] Renderer model;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Animator anim;

    [SerializeField] GameObject healthUI;
    [SerializeField] Image hpBar;

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
    Vector3 teamDir;
    Vector3 startingPos;

    private Animator animator;

    int originalHP;

    bool playerInRange;
    bool isShooting, isStunned, isRoaming;
    bool teamInRange;

    Color colorOrig;

    float angleToPlayer;
    float angleToTeam;
    float stoppingDisOrg;

    Coroutine co;

    // Start is called before the first frame update
    void Start()
    {
        colorOrig = model.material.color;
        updateEnemyHPBar();
        GameManager.instance.updateGameGoal(1);
        startingPos = transform.position;
        stoppingDisOrg = agent.stoppingDistance;
        originalHP = HP;
    }

    // Update is called once per frame
    void Update()
    {
        float agentSpeed = agent.velocity.normalized.magnitude;
        float animSpeed = anim.GetFloat("Speed");

        anim.SetFloat("Speed", Mathf.MoveTowards(animSpeed, agentSpeed, Time.deltaTime * aniSpeedTans));

        if (!isStunned && playerInRange && canSeePlayer())
        {
            
        }
        else if (!isStunned && teamInRange && canSeeTeam())
        {

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
        if (NavMesh.SamplePosition(randomPos, out hit, roamDis, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);

            while (Vector3.Distance(agent.transform.position, hit.position) > 15.0f)
            {
                yield return null;
            }

            agent.ResetPath();
        }
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

        
        return false;
    }

    bool canSeeTeam()
    {
        foreach (GameObject teamMember in GameManager.instance.team)
        {
            if (teamMember == null) continue;

            RaycastHit hit;

            teamDir = teamMember.transform.position - headPos.position;
            angleToTeam = Vector3.Angle(teamDir, transform.forward);

            Debug.DrawRay(headPos.position, teamDir, Color.blue);

            if (Physics.Raycast(headPos.position, teamDir, out hit))
            {
                if (hit.collider.CompareTag("Team") && angleToPlayer <= fov)
                {

                    agent.SetDestination(teamMember.transform.position);

                    if (agent.remainingDistance < agent.stoppingDistance)
                    {
                        faceTargetTeam();
                    }

                    if (!isShooting)
                    {
                        StartCoroutine(shoot());
                    }
                    return true;
                }
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

    void faceTargetTeam()
    {
        Quaternion rot = Quaternion.LookRotation(new Vector3(teamDir.x, 0, teamDir.z));
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * faceTargetSpeed);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            
            //animator.SetBool("IsMoving", true);
        }
        if (other.CompareTag("Team"))
        {
            teamInRange = true;
            //animator.SetBool("IsMoving", true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            //animator.SetBool("IsMoving", false);
        }
        if (other.CompareTag("Team"))
        {
            teamInRange= false;
            //animator.SetBool("IsMoving", false);
            agent.stoppingDistance = 0;
        }
        healthUI.SetActive(false);
    }

    public void takeDamage(int amount)
    {
        HP -= amount;

        healthUI.SetActive(true);

        if (agent.isActiveAndEnabled)
            agent.SetDestination(GameManager.instance.player.transform.position);

        updateEnemyHPBar();

        if (co != null)
            StopCoroutine(co);

        isRoaming = false;
        StartCoroutine(flashRed());

        if (HP <= 0)
        {
            GameManager.instance.updateGameGoal(-1);
            Destroy(gameObject);
        }
    }
    void updateEnemyHPBar()
    {
        hpBar.fillAmount = (float)HP / originalHP;
    }
    public void stunObject(int stunTime)
    {
        StartCoroutine(stun(stunTime));
    }

    IEnumerator stun(int stunTime)
    {
        isStunned = true;

        yield return new WaitForSeconds(stunTime);
        
        isStunned = false;
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
