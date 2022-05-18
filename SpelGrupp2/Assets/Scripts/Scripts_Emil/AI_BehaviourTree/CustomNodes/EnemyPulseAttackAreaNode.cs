using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AIBehavior/Behavior/EnemyPulseAttack")]
public class EnemyPulseAttackAreaNode : Node {
    [SerializeField] private float attackCoolDown = 3.0f;
    [SerializeField] private float damage = 50.0f;
    [SerializeField] private LayerMask whatAreTargets;
    [SerializeField] private float explosionRange = 4f;
    [SerializeField] private float explosionForce = 50f;
    [SerializeField] private Material flashMaterial;
    [SerializeField] private float duration = 2f;
    private MeshRenderer meshRenderer;
    private Material originalMaterial;
    //private Coroutine flashRoutine; 


    private bool isAttacking = true;

    IDamageable damageable;

    Collider[] colliders;

    public override NodeState Evaluate() {

        meshRenderer = agent.GetComponent<MeshRenderer>();
        originalMaterial = meshRenderer.material;

        if (isAttacking && agent.TargetInSight) {
            agent.IsStopped = true;
            isAttacking = false;
            agent.StartCoroutine(AttackDelay());
            NodeState = NodeState.RUNNING;
            return NodeState;
        }

        NodeState = NodeState.FAILURE;
        return NodeState;

    }
    public IEnumerator AttackDelay() {
        agent.StartCoroutine(FlashRoutine());
        yield return new WaitForSeconds(attackCoolDown);
        Attack();
        //agent.StartCoroutine(AnimateLineRenderer());
        isAttacking = true;

    }

    private IEnumerator FlashRoutine()
    {
        meshRenderer.material = flashMaterial;
        yield return new WaitForSeconds(duration);
        meshRenderer.material = originalMaterial;
        agent.StopCoroutine(FlashRoutine());
    }


    void Attack() {
        //Particklesystem
        Instantiate(AIData.Instance.PulseAttackParticles, agent.Position, Quaternion.identity);
        CheckForPlayers();

    }

    private void CheckForPlayers() {
        colliders = Physics.OverlapSphere(agent.Position, explosionRange, whatAreTargets);
        Debug.Log("PulseAttack");
        foreach (Collider coll in colliders) {
            if (coll.CompareTag("Player") || coll.CompareTag("BreakableObject") || coll.CompareTag("Enemy")) {
                damageable = coll.transform.GetComponent<IDamageable>();

                if (damageable != null) {
                    //damage
                    damageable.TakeDamage(damage);

                    //ExplosionForce
                    Rigidbody rbTemp = coll.GetComponent<Rigidbody>();
                    if (rbTemp != null) {
                        rbTemp.AddExplosionForce(explosionForce, agent.Position, explosionRange);
                    }

                    //Destroy gameobject
                    agent.Health.DieNoLoot();
                }

            }
        }
    }
}
