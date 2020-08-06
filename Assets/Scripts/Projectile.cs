using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public Vector3 direction;
    public float projectileSpeed = 10f;
    Rigidbody rigidbody;
    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        rigidbody.velocity = direction * projectileSpeed;
    }

    private void OnCollisionEnter(Collision collision)
    {
        //if (collision.other.gameObject.layer == LayerMask.NameToLayer("PlayerLYR"))
        //     return;
        Destroy(gameObject);
    }
}
