using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AIBehavior/Behavior/EnemyPulseAttack")]
public class EnemyPulseAttackAreaNode : Node
{
    [SerializeField] private float turnSpeed = 70.0f;
    [SerializeField] private float attackCoolDown = 1.0f;
    [SerializeField] private float damage = 50.0f;
    [SerializeField] private LayerMask whatAreTargets;
    [SerializeField] private float explosionRange = 4f;
    [SerializeField] private float explosionForce = 50f;

    private bool isAttacking = true;

    IDamageable damageable;

    Collider[] colliders;

    Vector3 closestTarget;
    Vector3 relativePos;

    RaycastHit checkCover;

    Quaternion rotation;

    public override NodeState Evaluate()
    {
        //Find Closest Player
        closestTarget = agent.ClosestPlayer;
        relativePos = closestTarget - agent.transform.position;

        // Rotate the Enemy towards the player
        rotation = Quaternion.LookRotation(relativePos, Vector3.up);
        agent.transform.rotation = Quaternion.RotateTowards(agent.transform.rotation,
                                                            rotation, Time.deltaTime * turnSpeed);

        if (isAttacking && CheckIfCoverIsValid() == false)
        {
            agent.IsStopped = true;
            isAttacking = false;
            agent.StartCoroutine(AttackDelay());
            NodeState = NodeState.RUNNING;
            return NodeState;
        }

        NodeState = NodeState.FAILURE;
        return NodeState;

    }
    public IEnumerator AttackDelay()
    {
        yield return new WaitForSeconds(attackCoolDown);
        Attack();
        //agent.StartCoroutine(AnimateLineRenderer());
        isAttacking = true;

    }


    void Attack()
    {
        //Particklesystem
        Instantiate(AIData.Instance.PulseAttackParticles, agent.Position, Quaternion.identity);
        CheckForPlayers();

    }

    private void CheckForPlayers()
    {
        colliders = Physics.OverlapSphere(agent.Position, explosionRange, whatAreTargets);
        Debug.Log("PulseAttack");
        foreach (Collider coll in colliders)
        {
            if (coll.CompareTag("Player") || coll.CompareTag("BreakableObject") || coll.CompareTag("Enemy"))
            {
                damageable = coll.transform.GetComponent<IDamageable>();

                if (damageable != null)
                {
                    //damage
                    damageable.TakeDamage(damage);

                    //ExplosionForce
                    Rigidbody rbTemp = coll.GetComponent<Rigidbody>();
                    if (rbTemp != null)
                    {
                        rbTemp.AddExplosionForce(explosionForce, agent.Position, explosionRange);
                    }

                    //Destroy gameobject
                    agent.Health.DieNoLoot();
                }

            }
        }
    }



    bool CheckIfCoverIsValid()
    {
        //Casting rays towards the player. if the ray hits the player, the cover is not valid anymore.

        // Create the ray to use
        Ray ray = new Ray(agent.transform.position, closestTarget - agent.transform.position);
        //Casting a ray against the player
        if (Physics.Raycast(ray, out checkCover, 30.0f))
        {
            //Check if that collider is the player
            if (checkCover.collider.gameObject.CompareTag("Player"))
            {
                //There is no cover
                return false;
            }
        }
        return true;
    }

}
