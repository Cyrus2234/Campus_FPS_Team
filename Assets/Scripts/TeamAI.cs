using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TeamAI : MonoBehaviour, IDamage
{
    [SerializeField] Renderer model;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Animator anim;

    [SerializeField] Transform shootPos;
    [SerializeField] Transform headPos;

    [SerializeField] int HP;
    [SerializeField] int faceTargetSpeed;
    [SerializeField] int fov;
    [SerializeField] float roamDist;
    [SerializeField] int roamTimer;
    [SerializeField] int aniSpeedTans;

    [SerializeField] GameObject bullet;
    [SerializeField] float shootRate;

    Vector3 enemyDir;
    Vector3 startingPos;

    private Animator animator;

    bool enemyInRange;
    bool isShooting;
    bool isRoaming;

    Color colorOrig;

    float angleToEnemy;
    float stoppingDistOrig;

    Coroutine co;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        //colorOrig = model.material.color;
        startingPos = transform.position;
        stoppingDistOrig = agent.stoppingDistance;
    }

    // Update is called once per frame
    void Update()
    {
        float agentSpeed = agent.velocity.normalized.magnitude;
        float animSpeed = anim.GetFloat("Speed");

        anim.SetFloat("Speed", Mathf.MoveTowards(animSpeed, agentSpeed, Time.deltaTime * aniSpeedTans));

        if (enemyInRange && canSeeEnemy()) 
        {
            if (!isRoaming && agent.remainingDistance < 0.01f)
                co = StartCoroutine(roam());
        }
        else if (!enemyInRange)
        {
            if (!isRoaming && agent.remainingDistance < 0.01f)
                co = StartCoroutine(roam());
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            enemyInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            enemyInRange = false;
        }
    }

    bool canSeeEnemy()
    {
        foreach (GameObject enemyMember in GameManager.instance.enemy)
        {
            if (enemyMember == null) continue;

            RaycastHit hit;

            enemyDir = enemyMember.transform.position - headPos.position;
            angleToEnemy = Vector3.Angle(enemyDir, transform.forward);

            Debug.DrawRay(headPos.position, enemyDir, Color.blue);

            if (Physics.Raycast(headPos.position, enemyDir, out hit))
            {
                if (hit.collider.CompareTag("Enemy") && angleToEnemy <= fov)
                {

                    agent.SetDestination(enemyMember.transform.position);

                    if (agent.remainingDistance < agent.stoppingDistance)
                    {
                        faceTargetTeam();
                    }

                    if (!isShooting && agent.remainingDistance < agent.stoppingDistance)
                    {
                        StartCoroutine(shoot());
                    }
                    agent.stoppingDistance = stoppingDistOrig;
                    return true;
                }
            }

        }
        agent.stoppingDistance = 0;
        return false;
    }

    void faceTargetTeam()
    {
        Quaternion rot = Quaternion.LookRotation(new Vector3(enemyDir.x, 0, enemyDir.z));
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * faceTargetSpeed);
    }


    public void takeDamage(int amount)
    {
        HP -= amount;
        //StartCoroutine(flashRed());
        if (HP <= 0)
        {
            
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

    IEnumerator roam()
    {
        isRoaming = true;
        yield return new WaitForSeconds(roamTimer);

        agent.stoppingDistance = 0;

        Vector3 randomPos = UnityEngine.Random.insideUnitSphere * roamDist;
        randomPos += startingPos;

        Vector3 playerPos = GameManager.instance.GetPlayer().transform.position / roamDist;
        playerPos += startingPos;

        NavMeshHit hit;
        NavMesh.SamplePosition(playerPos, out hit, roamDist, 1);
        agent.SetDestination(hit.position);


        isRoaming = false;
    }

    //IEnumerator flashRed()
    //{
    //    model.material.color = Color.red;
    //    yield return new WaitForSeconds(0.1f);
    //    model.material.color = colorOrig;
    //}

}
