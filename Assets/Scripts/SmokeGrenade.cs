using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmokeGrenade : MonoBehaviour
{
    [SerializeField] Rigidbody rigidBody;
    [SerializeField] ParticleSystem smoke;
    [SerializeField] AudioClip[] smokeSound;

    [SerializeField][Range(1, 5)] int smokeTime;
    [SerializeField][Range(5, 15)] int speed;
    [SerializeField][Range(5, 10)] int destroyTime;
    [SerializeField][Range(1, 10)] int fallRate;

    // Start is called before the first frame update
    void Start()
    {
        rigidBody.velocity = transform.forward * speed;
        StartCoroutine(explode());
    }

    void OnCollisionEnter(Collision collision)
    {
        rigidBody.velocity += Vector3.Reflect(rigidBody.velocity, collision.relativeVelocity.normalized) / (int)(speed * fallRate * .1);
    }

    IEnumerator explode()
    {

        yield return new WaitForSeconds(destroyTime);

        Destroy(gameObject);
        Instantiate(smoke, transform.position, Quaternion.identity);
        GameManager.instance.playerScript.playSound(smokeSound[Random.Range(0, smokeSound.Length)]);
    }
}
