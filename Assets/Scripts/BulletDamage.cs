using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletDamage : MonoBehaviour
{
    enum damageType {moving, stationary}
    [SerializeField] damageType type;
    [SerializeField] Rigidbody rb;

    [SerializeField] ParticleSystem splash;

    //Bullet stats
    [SerializeField] int damageAmount;
    [SerializeField] float bulletSpeed;
    [SerializeField] int destoryTime;


    // Start is called before the first frame update
    void Start()
    {
        if (type == damageType.moving)
        {
            rb.velocity = transform.forward * bulletSpeed;
            Destroy(gameObject, destoryTime);
        }
    }

    // Update is called once per frame
    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger)
            return;

        IDamage dmg = other.GetComponent<IDamage>();
        dmg?.takeDamage(damageAmount);

        if (type == damageType.moving)
            Destroy(gameObject);
        Instantiate(splash, transform.position, Quaternion.identity);
    }
}
