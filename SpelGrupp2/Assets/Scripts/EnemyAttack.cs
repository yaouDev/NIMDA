using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Identifierar n�rmsta spelare
 * Pausar & l�ser sikte f-tid innan skott
 * skjuter
 * pausar f-tid innan loop b�rjar om
 * */
public class EnemyAttack : MonoBehaviour {
    GameObject[] targets;
    private GameObject CurrentTarget;
    private float dist;
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private float attackRange;
    private bool isShooting = true;
    [SerializeField] private float turnSpeed = 10.0f;

    void Awake() {
        targets = GameObject.FindGameObjectsWithTag("Player");
        CurrentTarget = targets[0];
    }

    void Update() {
        GameObject closestTarget = Vector3.Distance(targets[0].transform.position, transform.position) > Vector3.Distance(targets[1].transform.position, transform.position) ? closestTarget = targets[1] : targets[0];
        dist = Vector3.Distance(transform.position, closestTarget.transform.position);
        CurrentTarget = closestTarget;
        if (dist <= attackRange) {
            Vector3 relativePos = closestTarget.transform.position - transform.position;

            // the second argument, upwards, defaults to Vector3.up
            Quaternion rotation = Quaternion.LookRotation(relativePos, Vector3.up);

            transform.rotation = Quaternion.RotateTowards(transform.rotation,
                rotation,
                Time.deltaTime * turnSpeed);

            if (isShooting)
            {
                isShooting = false;
                StartCoroutine(AttackDelay());
            }
        }
    }

    public bool IsShooting() {
        return isShooting;
    }

    public void SetShooting(bool shootingStatus) { isShooting = shootingStatus; }

    public Vector3 GetCurrentTarget() {
        return CurrentTarget.transform.position;
    }

    public float GetAttackRange() {
        return attackRange;
    }

    public IEnumerator AttackDelay() {
        yield return new WaitForSeconds(1f);
        Attack();
        StartCoroutine(AnimateLineRenderer());
        isShooting = true;

        yield return new WaitForSeconds(3f);
    }


    void Attack() {
        Physics.Raycast(transform.position + transform.forward + Vector3.up, transform.forward, out RaycastHit hitInfo, 30.0f);
        if (hitInfo.collider != null) {
            PlayerController player = hitInfo.transform.GetComponent<PlayerController>();
            //player.TakeDamage();
        }
    }

    private IEnumerator AnimateLineRenderer() {
        Vector3[] positions = { transform.position + Vector3.up, transform.position + Vector3.up + transform.forward * 30.0f };
        lineRenderer.SetPositions(positions);
        float t = 0.0f;
        while (t < 1.0f) {
            float e = Mathf.Lerp(Ease.EaseOutQuint(t), Ease.EaseOutBounce(t), t);
            float lineWidth = Mathf.Lerp(.5f, .0f, e);
            lineRenderer.startWidth = lineWidth;
            lineRenderer.endWidth = lineWidth;
            Color color = Color.Lerp(Color.white, Color.blue, Ease.EaseInQuart(t));
            lineRenderer.startColor = color;
            lineRenderer.endColor = color;
            t += Time.deltaTime * 3.0f;
            yield return null;
        }

        lineRenderer.startWidth = 0.0f;
        lineRenderer.endWidth = 0.0f;
    }
}
