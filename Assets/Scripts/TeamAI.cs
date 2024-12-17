using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TeamAI : MonoBehaviour, IDamage
{
    [SerializeField] NavMeshAgent agent;

    [SerializeField] int HP;

    bool enemyInRange;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (enemyInRange) 
        {

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

    public void takeDamage(int amount)
    {
        HP -= amount;
        //StartCoroutine(flashRed());
        if (HP <= 0)
        {
            //GameManager.instance.updateGameGoal(-1);
            Destroy(gameObject);
        }
    }
}
