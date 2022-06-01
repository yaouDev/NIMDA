using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AIBehavior/Behavior/EnemyPulseAttack")]
public class EnemyPulseAttackAreaNode : Node
{
    [SerializeField] private float attackCoolDown = 5.0f;
    [SerializeField] private float damage = 50.0f;
    [SerializeField] private LayerMask whatAreTargets;
    [SerializeField] private float explosionRange = 4f;
    [SerializeField] private float explosionForce = 50f;
    [SerializeField] private Material flashMaterial;
    [SerializeField] private float duration = 0.2f;
    [SerializeField] private Material originalMaterial;

    private bool isAttacking = true;
    private bool soundStarted;
    private float end;
    private MeshRenderer meshRenderer;


    IDamageable damageable;

    Collider[] colliders;

    public override NodeState Evaluate()
    {
        if (meshRenderer == null)
        {
            meshRenderer = agent.GetComponent<MeshRenderer>();
        }

        if (isAttacking && agent.TargetInSight)
        {
            agent.IsStopped = true;
            isAttacking = false;
            agent.StartCoroutine(AttackDelay());
            agent.StartCoroutine(FlashRoutine());
            NodeState = NodeState.RUNNING;
            return NodeState;
        }

        NodeState = NodeState.FAILURE;
        return NodeState;

    }
    public IEnumerator AttackDelay()
    {
        end = Time.realtimeSinceStartup + attackCoolDown;
        soundStarted = false;

        while (end > Time.realtimeSinceStartup)
        {
            if (end - Time.realtimeSinceStartup <= 0.3f && !soundStarted)
            {
                soundStarted = true;
                AudioController.instance.PlayOneShotAttatched(AudioController.instance.enemySound.explosion, agent.gameObject);
            }
            yield return null;
        }
        Attack();
        isAttacking = true;

    }

    private IEnumerator FlashRoutine()
    {
        while (true)
        {

            meshRenderer.material = flashMaterial;
            yield return new WaitForSeconds(duration);
            meshRenderer.material = originalMaterial;
            yield return new WaitForSeconds(duration);

        }

    }


    void Attack()
    {
        // schreenshake
        CallbackSystem.CameraShakeEvent shakeEvent = new CallbackSystem.CameraShakeEvent();
        shakeEvent.affectsPlayerOne = true;
        shakeEvent.affectsPlayerTwo = true;
        shakeEvent.magnitude = .4f;
        CallbackSystem.EventSystem.Current.FireEvent(shakeEvent);

        //Particklesystem
        Instantiate(AIData.Instance.ExplosionParticles, agent.Position, Quaternion.identity);
        CheckForPlayers();

    }

    private void CheckForPlayers()
    {
        colliders = Physics.OverlapSphere(agent.Position, explosionRange, whatAreTargets);
        foreach (Collider coll in colliders)
        {
            if (coll.CompareTag("Player") || coll.CompareTag("BreakableObject") || coll.CompareTag("Enemy"))
            {
                damageable = coll.transform.GetComponent<IDamageable>();

                if (damageable != null)
                {
                    //damage
                    damageable.TakeDamage(damage);

                    //ExplosionForce - check with Will if we will remove
                    Rigidbody rbTemp = coll.GetComponent<Rigidbody>();
                    if (rbTemp != null)
                    {
                        rbTemp.AddExplosionForce(explosionForce, agent.Position, explosionRange);
                    }

                    agent.Health.DieNoLoot();
                }

            }
        }
    }
}
