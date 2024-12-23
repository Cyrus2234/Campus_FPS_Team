using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunGrenadeDamage : MonoBehaviour
{
    [SerializeField] Rigidbody rb;
    [SerializeField] ParticleSystem explosion;
    [SerializeField] AudioClip[] explosionSound;

    [SerializeField][Range(1, 5)] int stunTime;
    [SerializeField][Range(5, 15)] int speed;
    [SerializeField][Range(5,10)] int destoryTime;
    [SerializeField][Range(1,10)] int fallRate;

    // Start is called before the first frame update
    void Start()
    {
        rb.velocity = transform.forward * speed;
        StartCoroutine(explode());
    }

    void OnCollisionEnter(Collision collision)
    {
        rb.velocity += Vector3.Reflect(rb.velocity, collision.relativeVelocity.normalized) / (int)(speed * fallRate * .1);
    }

    IEnumerator explode()
    {
        yield return new WaitForSeconds(destoryTime);

        Collider[] colliders = Physics.OverlapSphere(transform.position, GetComponent<SphereCollider>().radius * transform.lossyScale.x);

        foreach (var other in colliders)
        {
            IStunnable stun = other.gameObject.GetComponent<IStunnable>();

#if UNITY_EDITOR
            Debug.Log("Is Trigger: " + other.isTrigger + "\tDamage: " + (stun != null).ToString() + "\t Collider: " + other);
#endif
            if (!other.isTrigger && stun != null && ((other is CapsuleCollider && !other.CompareTag("Player")) || (other is CharacterController)))
            {
                stun.stunObject(stunTime);
            }
        }
        Destroy(gameObject);
        Instantiate(explosion, transform.position, Quaternion.identity);
        GameManager.instance.playerScript.playSound(explosionSound[Random.Range(0, explosionSound.Length)]);
    }
}
