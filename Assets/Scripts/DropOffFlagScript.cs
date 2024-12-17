using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropOffFlagScript : MonoBehaviour
{ 
    [SerializeField] string teamTag = "Player";

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(teamTag))
        {
            if (GameManager.instance.FlagDropOffComplete())
            {
                Destroy(gameObject); // For the pause menu to not hav a flag sticking out of it
            }

        }
    }

}
