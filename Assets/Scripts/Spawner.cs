using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] GameObject objectToSpawn;
    [SerializeField] int numberToSpawn;
    [SerializeField] int spawnInterval;
    [SerializeField] int spawnPerInterval = 1;
    [SerializeField] Transform[] spawnPositions;

    int spawnCount;
    bool startSpawning;
    bool isSpawning;



    // Start is called before the first frame update
    void Start()
    {
        GameManager.instance.updateGameGoal(numberToSpawn);
    }

    // Update is called once per frame
    void Update()
    {
        if((startSpawning) && (spawnCount < numberToSpawn) && (!isSpawning))
        {
            StartCoroutine(Spawn());
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        { 
            startSpawning = true;
        }
    }

    IEnumerator Spawn()
    { 
        isSpawning = true;


        for (int i = 0; i < spawnPerInterval; ++i)
        {
            int SpawnPositionChosen = Random.Range(0, spawnPositions.Length);
            Instantiate(objectToSpawn, spawnPositions[SpawnPositionChosen].position, spawnPositions[SpawnPositionChosen].rotation);
            spawnCount++;
        }
        
        yield return new WaitForSeconds(spawnInterval);
        isSpawning = false;
    }
}
