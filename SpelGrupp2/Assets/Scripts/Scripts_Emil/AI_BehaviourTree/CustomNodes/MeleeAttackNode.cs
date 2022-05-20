using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AIBehavior/Behavior/MeleeAttack")]
public class MeleeAttackNode : Node {
    [SerializeField] private float attackCoolDown = 1.0f;
    [SerializeField] private float damage = 5.0f;
    [SerializeField] private LayerMask whatAreTargets;

    private bool isAttacking = true;

    IDamageable damageable;

    Collider[] colliders;

    public override NodeState Evaluate() {

        if (isAttacking && agent.TargetInSight) {//CheckIfCoverIsValid() == false) {
            //agent.IsStopped = true;
            isAttacking = false;
            agent.StartCoroutine(AttackDelay());
            NodeState = NodeState.RUNNING;
            return NodeState;
        }

        NodeState = NodeState.FAILURE;
        return NodeState;

    }
    public IEnumerator AttackDelay() {
        yield return new WaitForSeconds(attackCoolDown);
        Attack();
        //agent.StartCoroutine(AnimateLineRenderer());
        isAttacking = true;

    }


    void Attack() {
        CheckForPlayers();
        if (AIData.Instance.EnemyMuzzleflash != null) {
            Instantiate(AIData.Instance.EnemyMuzzleflash, agent.Health.FirePoint, Quaternion.identity);
        }
    }

    private void CheckForPlayers() {
        //Check with a overlapsphere what colliders are in the area
        colliders = Physics.OverlapSphere(agent.Position, 1.5f, whatAreTargets);
        foreach (Collider coll in colliders) {
            if (coll.CompareTag("Player") || coll.CompareTag("BreakableObject")) {
                damageable = coll.transform.GetComponent<IDamageable>();

                if (damageable != null) {
                    damageable.TakeDamage(damage);

                }

            }
        }
    }
}
