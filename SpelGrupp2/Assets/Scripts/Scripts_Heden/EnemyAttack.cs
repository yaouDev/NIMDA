using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    GameObject[] targets;
    private GameObject CurrentTarget;
    private float dist;
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private float attackRange;
    private IEnumerator currCor;


    void Awake()
    {
        targets = GameObject.FindGameObjectsWithTag("Player");
        CurrentTarget = targets[0];
    }

    void Update()
    {
        GameObject closestTarget = Vector3.Distance(targets[0].transform.position, transform.position) > Vector3.Distance(targets[1].transform.position, transform.position) ? closestTarget = targets[1] : targets[0];
        dist = Vector3.Distance(transform.position, closestTarget.transform.position);
        CurrentTarget = closestTarget;
        if (dist <= attackRange)
        {
            StartCoroutine(DoCheck());
        }
    }

    IEnumerator DoCheck()
    {
        ShootAtPlayer();
        Pause();
        yield return new WaitForSeconds(1f);
    } 

    void Pause()
    {
        StopCoroutine(TurnToTarget());
        StopCoroutine(AnimateLineRenderer());
    }

    void ShootAtPlayer()
    {
        StartCoroutine(TurnToTarget());
        StartCoroutine(AnimateLineRenderer());
        Attack();

    }

    void Attack()
    {
        Physics.Raycast(transform.position + transform.forward + Vector3.up, transform.forward, out RaycastHit hitInfo, 30.0f);
        Debug.Log(hitInfo.collider.transform.name);
        if (hitInfo.collider != null)
        {
            PlayerController player = hitInfo.transform.GetComponent<PlayerController>();
            //player.TakeDamage();
        }
    }

    IEnumerator TurnToTarget()
    {
        currCor = TurnToTarget();
        transform.LookAt(CurrentTarget.transform.position);
        Debug.Log("Facing target");
        yield return new WaitForSeconds(2f);
    }

    private IEnumerator AnimateLineRenderer()
    {
        currCor = AnimateLineRenderer();
        Vector3[] positions = { transform.position + Vector3.up, transform.position + Vector3.up + transform.forward * 30.0f };
        lineRenderer.SetPositions(positions);
        float t = 0.0f;
        while (t < 1.0f)
        {
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
