using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosiveBullet : MonoBehaviour
{
    //Assignables
    public Rigidbody rigidBody;
    public GameObject explosion;
    public LayerMask whatIsTarget;

    //Stats
    [SerializeField] private float bounciness;
    [SerializeField] private bool useGravity;

    //Damage
    [SerializeField] private int explosionDamage;
    [SerializeField] private float explosionRange;
    [SerializeField] private float explosionForce;


    //LifeTime
    [SerializeField] private int maxCollisions;
    [SerializeField] private float maxLifeTime;
    [SerializeField] private bool explodeOnTouch = true;

    private int colissions;

    PhysicMaterial physicsMat;

    private void Start()
    {
        Setup();
    }

    private void Update()
    {
        //When to Explode
        if (colissions > maxCollisions)
        {
            Explode();
        }
        //Count down lifetime
        maxLifeTime -= Time.deltaTime;
        if(maxLifeTime <= 0)
        {
            Explode();
        }

    }
    private void Explode()
    {
        //Intantiate Explosion
        if(explosion != null)
        {
            Instantiate(explosion, transform.position, Quaternion.identity);
        }
        //Check for Enemies
        Collider[] targets = Physics.OverlapSphere(transform.position, explosionRange, whatIsTarget);
        for (int i = 0; i < targets.Length; i++)
        {
            //Get component of player
            
            //targets[i].GetComponent<PlayerHealth>().TakeDamage(explosionDamage); //sätt en damage parameter i TakeDamage
            if (targets[i].GetComponent<Rigidbody>())
            {
                targets[i].GetComponent<Rigidbody>().AddExplosionForce(explosionForce, transform.position, explosionRange);
            }
        }

        //Add delay to destroy
        Invoke("Delay", 0.05f);
    }

    private void Delay()
    {
        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        //Count up colissions
        colissions++;

        //Explode if bullet hits enemy directly
        if(collision.collider.CompareTag("Player") && explodeOnTouch)
        {
            Explode();
        }
    }

    private void Setup()
    {
        //Create a new Ohysics material
        physicsMat = new PhysicMaterial();
        physicsMat.bounciness = bounciness;
        physicsMat.frictionCombine = PhysicMaterialCombine.Minimum;
        physicsMat.bounceCombine = PhysicMaterialCombine.Maximum;

        //Assign material to collider
        GetComponent<SphereCollider>().material = physicsMat;

        //Set Gravity
        rigidBody.useGravity = useGravity;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRange);
    }
}
