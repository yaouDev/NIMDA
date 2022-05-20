using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AIBehavior/Behavior/PulseAttack")]
public class PulseAreaAttackNode : Node {
    [SerializeField] private float attackRange;
    [SerializeField] private float attackCoolDown = 3.0f;
    [SerializeField] private float damage = 10.0f;
    [SerializeField] private LayerMask whatAreTargets;

    private bool isAttacking = true;

    IDamageable damageable;

    Collider[] colliders;

    RaycastHit checkCover;


    public override NodeState Evaluate() {

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
        yield return new WaitForSeconds(attackCoolDown);
        Attack();
        //agent.StartCoroutine(AnimateLineRenderer());
        isAttacking = true;

    }


    void Attack() {
        CallbackSystem.CameraShakeEvent shakeEvent = new CallbackSystem.CameraShakeEvent();
        shakeEvent.affectsPlayerOne = true;
        shakeEvent.affectsPlayerTwo = true;
        shakeEvent.magnitude = .4f;
        CallbackSystem.EventSystem.Current.FireEvent(shakeEvent);

        Instantiate(AIData.Instance.PulseAttackParticles, agent.Position, Quaternion.identity);
        CheckForPlayers();
    }

    private void CheckForPlayers() {
        colliders = Physics.OverlapSphere(agent.Position, 5f, whatAreTargets);
        Debug.Log("PulseAttack");
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
