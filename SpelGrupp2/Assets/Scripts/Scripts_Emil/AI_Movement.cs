using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_Movement : MonoBehaviour
{

    [SerializeField] private float speed, timeBetweenPathUpdates;
    private float timeSinceLastUpdate;
    private EnemyAttack enemyAttack;
    private Vector3 target;
    private Vector3 desiredTarget, targetBlocked, activeTarget;
    private PathfinderManager pathfinder;
    private SimpleGraph pathfindingGrid;
    private List<Vector3> currentPath;
    private Collider col;
    private SphereCollider otherEnemyTrigger;
    private Rigidbody rBody;
    private int currentPathIndex = 0;

    // Start is called before the first frame update
    void Start()
    {
        col = GetComponent<Collider>();
        otherEnemyTrigger = GetComponent<SphereCollider>();
        pathfinder = PathfinderManager.instance;
        pathfindingGrid = GameObject.Find("PathfindingPlane").GetComponent<SimpleGraph>();
        rBody = GetComponent<Rigidbody>();
        Physics.IgnoreLayerCollision(12, 12);
        enemyAttack = GetComponent<EnemyAttack>();
    }

    // Update is called once per frame
    void Update()
    {
        timeSinceLastUpdate += Time.deltaTime;
        updateTarget();
        if (shouldUpdatePath())
        {
            currentPath = pathfinder.requestPath(transform.position, activeTarget);
            currentPathIndex = 0;
            timeSinceLastUpdate = 0;
        }
        if (currentPath.Count != 0)
        {
            Vector3 prevPos = currentPath[0];
            foreach (Vector3 pos in currentPath)
            {
                if (pos != prevPos)
                    Debug.DrawLine(prevPos, pos, Color.blue);
                prevPos = pos;
            }
        }

    }


    private void FixedUpdate()
    {
        if (currentPath != null && currentPath.Count != 0)
        {
            if (Vector3.Distance(transform.position, currentPath[currentPathIndex]) > 0.5f)
            {
                bool moveTowardTarget = true;
                /*         Collider[] cols = Physics.OverlapSphere(transform.position, otherEnemyTrigger.radius);
                        foreach (Collider c in cols) {
                            if (c.tag == "Enemy" && c.gameObject != gameObject) {
                                if (Vector3.Dot(c.GetComponent<Rigidbody>().velocity.normalized, rBody.velocity.normalized) >= -0.75f) {
                                    moveTowardTarget = false;
                                    rBody.AddForce((transform.position - c.transform.position).normalized * speed);
                                }
                            }
                        } */
                int indexesToLerp = 4;
                if (currentPath.Count - 1 - currentPathIndex < 4) indexesToLerp = currentPath.Count - 1 - currentPathIndex;
                Vector3 lerpForceToAdd = (Vector3.Lerp(currentPath[currentPathIndex], currentPath[currentPathIndex + indexesToLerp], 0.5F) - transform.position).normalized * speed;
                Vector3 forceTadd = lerpForceToAdd;
                if (currentPathIndex != currentPath.Count - 1 && rBody.velocity.magnitude < 1) forceTadd = (currentPath[currentPathIndex] - transform.position).normalized * speed * 5;
                if (moveTowardTarget) rBody.AddForce(forceTadd, ForceMode.Force);
                if (currentPathIndex != currentPath.Count - 1 && Vector3.Distance(currentPath[currentPathIndex + 1], transform.position) < Vector3.Distance(currentPath[currentPathIndex], transform.position)) currentPathIndex++;
            }
            else if (currentPathIndex < currentPath.Count - 2)
            {
                currentPathIndex++;
            }
        }
    }

    private bool shouldUpdatePath()
    {
        bool realTargetFree = activeTarget == targetBlocked && pathfindingGrid.getBlockedNode(desiredTarget).Length == 0;
        bool updateTime = timeSinceLastUpdate >= timeBetweenPathUpdates && (activeTarget != currentPath[currentPath.Count - 1] || timeSinceLastUpdate == timeBetweenPathUpdates * 5);
        return currentPath == null || currentPath.Count == 0 || isPathBlocked() || realTargetFree || updateTime;
    }

    private bool isPathBlocked()
    {
        return pathfindingGrid.getBlockedNode(activeTarget).Length != 0;
    }

    private void updateTarget()
    {
        desiredTarget = enemyAttack.getCurrentTarget().transform.position;
        activeTarget = desiredTarget;
        if (pathfindingGrid.getBlockedNode(desiredTarget).Length > 0)
        {
            targetBlocked = pathfindingGrid.getClosestBoxNotBlocked(desiredTarget, transform.position);
            activeTarget = targetBlocked;
        }
    }
}
