using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeDamage : MonoBehaviour
{
    [SerializeField] int damageAmount;
    [SerializeField] int destoryTime;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(explode());
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator explode()
    {
        yield return new WaitForSeconds(destoryTime);

        Collider[] colliders = Physics.OverlapSphere(transform.position, GetComponent<SphereCollider>().radius * transform.lossyScale.x);

        foreach (var other in colliders)
        {
            IDamage dmg = other.gameObject.GetComponent<IDamage>();
            Debug.Log("Is Trigger: " + other.isTrigger + "\tDamage: " + (dmg!=null).ToString() + "\t Collider: " + other);

            if (!other.isTrigger && dmg != null && other is CapsuleCollider)
            {
                //Debug.Log(other.name);
                dmg.takeDamage(damageAmount);
            }
        }
        Destroy(gameObject);
    }
}
