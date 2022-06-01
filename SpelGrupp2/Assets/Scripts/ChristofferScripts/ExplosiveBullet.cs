using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using CallbackSystem;
using UnityEngine;

public class ExplosiveBullet : MonoBehaviour, IPoolable {
    //Assignables
    [SerializeField] private Rigidbody rigidBody;
    [SerializeField] private LayerMask whatIsTarget;

    //Stats
    [SerializeField] private float bounciness;
    [SerializeField] private bool useGravity;

    //Damage
    [SerializeField] private float explosionRange;

    //LifeTime
    [SerializeField] private float maxLifeTime = 100f;
    private float currentLifeTime;
    [SerializeField] private bool explodeOnTouch = true;

    [SerializeField] private float damage = .1f;
    [SerializeField] private TrailRenderer trailRenderer;

    PhysicMaterial physicsMat;

    private void Start() {
        Setup();
    }

    private void Update() {
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
        //ObjectPool.Instance.ReturnToPool("SimpleBullet", gameObject);
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other) {

        //Explode if bullet hits enemy directly
        if (other.gameObject.CompareTag("Player") && explodeOnTouch) {
            Explode();
            other.gameObject.GetComponent<PlayerHealth>().TakeDamage(damage);
        }
    }

    private void Setup() { //-check wit Will 
        //Create a new Physics material
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

    public void OnSpawn() {
        currentLifeTime = maxLifeTime;
        rigidBody.velocity = Vector3.zero;
        trailRenderer.enabled = true;
        trailRenderer.Clear();
    }
}
