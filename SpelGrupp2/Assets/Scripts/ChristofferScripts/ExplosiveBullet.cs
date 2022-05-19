using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using CallbackSystem;
using UnityEngine;

public class ExplosiveBullet : MonoBehaviour, IPoolable {
    //Assignables
    [SerializeField] private Rigidbody rigidBody;
    //[SerializeField] private GameObject explosion;
    [SerializeField] private LayerMask whatIsTarget;
    //[SerializeField] private GameObject[] players;

    //Stats
    [SerializeField] private float bounciness;
    [SerializeField] private bool useGravity;

    //Damage
    [SerializeField] private int explosionDamage;
    [SerializeField] private float explosionRange;
    [SerializeField] private float explosionForce;


    //LifeTime
    [SerializeField] private int maxCollisions = 2;
    [SerializeField] private float maxLifeTime = 4.0f;
    private float currentLifeTime;
    [SerializeField] private bool explodeOnTouch = true;

    [SerializeField] private float damage = .1f;
    [SerializeField] private TrailRenderer trailRenderer;

    private int colissions;

    PhysicMaterial physicsMat;

    private void Start() {
        Setup();
    }

    private void Update() {
        //When to Explode
        if (colissions > maxCollisions) {
            Explode();
        }
        //Count down lifetime
        currentLifeTime -= Time.deltaTime;
        if (currentLifeTime <= 0) {
            Explode();
        }

    }
    private void Explode() {
        //Add delay to destroy
        Invoke("Delay", 0.05f);
    }

    private void Delay() {
        trailRenderer.enabled = false;
        ObjectPool.Instance.ReturnToPool("SimpleBullet", gameObject);
    }

    private void OnTriggerEnter(Collider other) {
        //Count up colissions
        colissions++;
        //Explode if bullet hits enemy directly
        if (other.gameObject.CompareTag("Player") && explodeOnTouch) {
            Explode();
            other.gameObject.GetComponent<PlayerHealth>().TakeDamage(damage);
        }
    }

    private void Setup() {
        //Create a new Ohysics material
        physicsMat = new PhysicMaterial();
        physicsMat.bounciness = bounciness;
        physicsMat.frictionCombine = PhysicMaterialCombine.Minimum;
        physicsMat.bounceCombine = PhysicMaterialCombine.Maximum;

        //Assign material to collider
        GetComponent<SphereCollider>().material = physicsMat;

        //Set Gravity
        rigidBody.useGravity = useGravity;
        currentLifeTime = maxLifeTime;
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRange);
    }

    public void OnSpawn() {
        currentLifeTime = maxLifeTime;
        colissions = 0;
        rigidBody.velocity = Vector3.zero;
        trailRenderer.enabled = true;
        trailRenderer.Clear();
    }
}
