using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using CallbackSystem;
using UnityEngine;

public class BossBullet : MonoBehaviour
{
    //Assignables
    [SerializeField] private LayerMask whatIsTarget;


    //Damage
    [SerializeField] private int explosionDamage = 20;
    [SerializeField] private float explosionRange = 3.0f;
    [SerializeField] private float explosionForce = 20.0f;
    [SerializeField] private float shootForce = 20.0f;

    //LifeTime
    [SerializeField] private int maxCollisions = 5;
    [SerializeField] private float maxLifeTime = 2;
    [SerializeField] private bool explodeOnTouch = true;

    [SerializeField] private float damage = .1f;

    private GameObject currentBulletOne;
    private GameObject currentBulletTwo;
    private GameObject currentBulletThree;
    private GameObject currentBulletFour;



    private void Update()
    {
        //Count down lifetime
        maxLifeTime -= Time.deltaTime;
        if (maxLifeTime <= 0)
        {
            Explode();
        }

    }
    private void Explode()
    {
        currentBulletOne = Instantiate(AIData.Instance.SmallBullet, transform.position, Quaternion.identity);
        currentBulletTwo = Instantiate(AIData.Instance.SmallBullet, transform.position, Quaternion.identity);
        currentBulletThree = Instantiate(AIData.Instance.SmallBullet, transform.position, Quaternion.identity);
        currentBulletFour = Instantiate(AIData.Instance.SmallBullet, transform.position, Quaternion.identity);
        //AddForce to bullets
        currentBulletOne.GetComponent<Rigidbody>().AddForce(Vector3.forward * shootForce, ForceMode.Impulse);
        currentBulletTwo.GetComponent<Rigidbody>().AddForce(Vector3.right * shootForce, ForceMode.Impulse);
        currentBulletThree.GetComponent<Rigidbody>().AddForce(Vector3.left * shootForce, ForceMode.Impulse);
        currentBulletFour.GetComponent<Rigidbody>().AddForce(Vector3.back * shootForce, ForceMode.Impulse);


        //Add delay to destroy
        Invoke("Delay", 0.05f);
    }

    private void Delay()
    {
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        //Explode if bullet hits Player directly
        if (other.gameObject.CompareTag("Player") && explodeOnTouch)
        {
            Explode();
            other.gameObject.GetComponent<PlayerHealth>().TakeDamage(damage);
        }
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRange);
    }
}
