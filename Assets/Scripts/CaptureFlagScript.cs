using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaptureFlagScript : MonoBehaviour
{
    [SerializeField] string teamTag = "Player";

    // Start is called before the first frame update
    void Start()
    {
        GameManager.instance.updateFlagGoal(1); //Create for every flag generated
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(teamTag))
        {

            // Changing to be better...
            GameManager.instance.updateFlagGoal(-1);
            GameManager.instance.hasFlagUI.SetActive(true);

            Destroy(gameObject);


            //GameManager.instance.setHasFlagState(true);
            //Destroy(gameObject);
        }
    }

}
