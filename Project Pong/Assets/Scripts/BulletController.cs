using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    [Header("Bullet")]
    public float speed;
    public Rigidbody myRigidbody;
    public float bulletLife = 2f;
    public bool isRocket;
    public GameObject explosionEffect;
    private ParticleSystem rocketTrail;

    // Start is called before the first frame update
    void Start()
    {
        rocketTrail = GetComponentInChildren<ParticleSystem>();
        
    }

    // Update is called once per frame
    void Update()
    {
        BulletShootStraight();

        bulletLife -= Time.deltaTime;
        if(bulletLife < 0)
        {
            Destroy(gameObject);
        }
    }

    void BulletShootStraight()
    {
        myRigidbody.velocity = transform.forward * speed;
    }

    void OnTriggerEnter(Collider other)
    {
        if(isRocket)
        {
            Instantiate(explosionEffect, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }
}
