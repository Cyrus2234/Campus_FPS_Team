using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class enemyAi : MonoBehaviour, IDamage
{
    [SerializeField] Renderer model;
    [SerializeField] NavMeshAgent agent;

    [SerializeField] Transform shootPos;
    [SerializeField] Transform headPos;

    [SerializeField] int HP;
    [SerializeField] int faceTargetSpeed;
    [SerializeField] int fov;


    [SerializeField] GameObject bullet;
    [SerializeField] float shootRate;

    Vector3 playerDir;
    Vector3 teamDir;

    private Animator animator;

    bool playerInRange;
    bool teamInRange;
    bool isShooting;

    Color colorOrig;

    float angleToPlayer;
    float angleToTeam;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        colorOrig = model.material.color;
        GameManager.instance.updateGameGoal(1);
    }

    // Update is called once per frame
    void Update()
    {   if (playerInRange && canSeePlayer())
        {

        }
        if (teamInRange && canSeeTeam())
        {

        }
    }

    bool canSeePlayer()
    {

        playerDir = GameManager.instance.GetPlayer().transform.position - headPos.position;
        angleToPlayer = Vector3.Angle(playerDir, transform.forward);

        Debug.DrawRay(headPos.position, playerDir);
        RaycastHit hit;

        if(Physics.Raycast(headPos.position, playerDir, out hit))
        {
            if (hit.collider.CompareTag("Player") && angleToPlayer <= fov)
            {

                agent.SetDestination(GameManager.instance.GetPlayer().transform.position);

                if (agent.remainingDistance < agent.stoppingDistance)
                {
                    faceTarget();
                }

                if (!isShooting)
                {
                    StartCoroutine(shoot());
                }
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

        return false;
    }

    void faceTarget()
    {
        Quaternion rot = Quaternion.LookRotation( new Vector3 (playerDir.x, 0, playerDir.z));
        transform.rotation = Quaternion.Lerp(transform.rotation, rot , Time.deltaTime * faceTargetSpeed);
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
            animator.SetBool("IsMoving", true);
        }
        if (other.CompareTag("Team"))
        {
            teamInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            animator.SetBool("IsMoving", false);
        }
        if (other.CompareTag("Team"))
        {
            teamInRange= false;
        }
    }

    public void takeDamage(int amount)
    {
       HP -= amount;
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
