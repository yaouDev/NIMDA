using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_Controller : MonoBehaviour {

    [SerializeField] private float speed, timeBetweenPathUpdates, distanceFromTargetToStop, maxCritRange, minCritRange, allowedTargetDiscrepancy;
    private EnemyAttack enemyAttack;
    private EnemyHealth enemyHealth;
    private Vector3 desiredTarget, targetBlocked, activeTarget;
    private PathfinderManager pathfinder;
    private SimpleGraph pathfindingGrid;

    private List<Vector3> currentPath;
    private Collider col;
    private SphereCollider otherEnemyTrigger;
    private Rigidbody rBody;
    private int currentPathIndex = 0;
    private bool updatingPath = false;
    private BehaviorTree behaviorTree;
    public bool IsStopped = true;

    // Start is called before the first frame update
    void Start() {
        col = GetComponent<Collider>();
        behaviorTree = GetComponent<BehaviorTree>();
        otherEnemyTrigger = GetComponentInChildren<SphereCollider>();
        pathfinder = PathfinderManager.instance;
        pathfindingGrid = SimpleGraph.instance;
        rBody = GetComponent<Rigidbody>();
        Physics.IgnoreLayerCollision(12, 12);
        enemyAttack = GetComponent<EnemyAttack>();
        enemyHealth = GetComponent<EnemyHealth>();
    }

    
    void Update() {
        UpdateTarget();
        behaviorTree.Update();
        // This code is for debugging purposes only, shows current calculated path
/*         if (currentPath != null && currentPath.Count != 0) {
            Vector3 prevPos = currentPath[0];
            foreach (Vector3 pos in currentPath) {
                if (pos != prevPos)
                    Debug.DrawLine(prevPos, pos, Color.blue);
                prevPos = pos;
            }
        } */
    }

    public bool TargetInSight() {
        RaycastHit hit;
        Physics.BoxCast(transform.position, transform.lossyScale, (activeTarget - transform.position).normalized, out hit, transform.rotation, Mathf.Infinity);
        if (hit.collider != null) {
            if (hit.collider.tag == "Player") {
                return true;
            }
        }
        return false;
    }

    public IEnumerator UpdatePath() {
        updatingPath = true;
        pathfinder.RequestPath(this, transform.position, activeTarget);
        yield return new WaitForSeconds(timeBetweenPathUpdates);
        updatingPath = false;
    }

    public void ResetPathIndex() { currentPathIndex = 0; }

    public EnemyAttack GetEnemyAttack() { return enemyAttack; }
    public EnemyHealth GetEnemyHealth() { return enemyHealth; }

    public Vector3 GetCurrentTarget() {
        return activeTarget;
    }

    public void SetPath(List<Vector3> newPath) {
        currentPath = newPath;
        currentPathIndex = 0;
    }

    public List<Vector3> GetCurrentPath() {
        return currentPath;
    }

    public Vector3 GetPosition() {
        return transform.position;
    }


    public float GetAttackRange() {
        return enemyAttack.GetAttackRange();
    }

    public float GetDistanceFromTarget() {
        return Vector3.Distance(activeTarget, transform.position);
    }

    public Vector3 getVelocity() {
        return rBody.velocity;
    }

    public bool IsPathRequestAllowed() {
        bool nullCond = currentPath != null && currentPath.Count != 0;
        bool indexCond = nullCond && currentPath.Count - currentPathIndex <= 5;
        bool discrepancyCond = nullCond && currentPath[currentPath.Count - 1] != activeTarget &&
        Vector3.Distance(currentPath[currentPath.Count - 1], activeTarget) >= allowedTargetDiscrepancy;
        float distToTarget = Vector3.Distance(transform.position, activeTarget);
        bool criticalRangeCond = nullCond && distToTarget <= maxCritRange && distToTarget > minCritRange;
        return (currentPath == null || discrepancyCond || criticalRangeCond || indexCond) && !updatingPath;
    }

    private void FixedUpdate() {
        AvoidOtherEnemies();
        if (!IsStopped) {
            AdjustForLatePathUpdate();
            Move();
        }
    }

    private void AvoidOtherEnemies() {
        Collider[] colliders = Physics.OverlapSphere(transform.position, otherEnemyTrigger.radius);
        foreach (Collider collider in colliders) {
            if (collider.tag == "Enemy" && collider.transform.parent.gameObject != gameObject) {
                Vector3 directionOfOtherEnemy = (collider.transform.position - transform.position).normalized;
                Vector3 valueToTest = transform.position;
                if (rBody.velocity.magnitude > 0.05f) valueToTest = rBody.velocity.normalized;
                float dot = Vector3.Dot(valueToTest, -directionOfOtherEnemy);
                if ((dot >= -0.2f && dot <= 0.5f) || valueToTest == transform.position) {
                    float localSpeeed = speed;
                    if (valueToTest != transform.position) localSpeeed = speed * 0.33f;
                    Vector3 forceToAdd = -directionOfOtherEnemy.normalized * localSpeeed;
                    forceToAdd.y = 0;
                    rBody.AddForce(forceToAdd, ForceMode.Force);
                }
            }
        }
    }

    private void AdjustForLatePathUpdate() {
        if (currentPath != null && currentPathIndex == 0 && currentPath.Count != 0) {
            bool distanceCond = Vector3.Distance(currentPath[currentPathIndex], activeTarget) > Vector3.Distance(transform.position, activeTarget);
            while (currentPathIndex < currentPath.Count - 2 &&
            Vector3.Distance(currentPath[currentPathIndex], activeTarget) > Vector3.Distance(transform.position, activeTarget) &&
            Vector3.Dot(currentPath[currentPathIndex + 1] - (currentPath[currentPathIndex]).normalized, (activeTarget - currentPath[currentPathIndex]).normalized) >= 0)
                currentPathIndex++;
        }
    }


    private void Move() {
        if (currentPath != null && currentPath.Count != 0 && Vector3.Distance(transform.position, activeTarget) > distanceFromTargetToStop) {
            if (Vector3.Distance(transform.position, currentPath[currentPathIndex]) > 0.5f || (currentPathIndex == currentPath.Count - 1 && Vector3.Distance(transform.position, currentPath[currentPathIndex]) > 2f)) {
                int indexesToLerp = 4;
                if (currentPath.Count - 1 - currentPathIndex < 4) indexesToLerp = currentPath.Count - 1 - currentPathIndex;
                Vector3 lerpForceToAdd = (Vector3.Lerp(currentPath[currentPathIndex], currentPath[currentPathIndex + indexesToLerp], 0.5F) - transform.position).normalized * speed;
                Vector3 forceTadd = lerpForceToAdd;
                if (currentPathIndex != currentPath.Count - 1 && rBody.velocity.magnitude < 1) forceTadd = (currentPath[currentPathIndex] - transform.position).normalized * speed * 5;
                rBody.AddForce(forceTadd, ForceMode.Force);
                if (currentPathIndex != currentPath.Count - 1 && Vector3.Distance(currentPath[currentPathIndex + 1], transform.position) < Vector3.Distance(currentPath[currentPathIndex], transform.position)) currentPathIndex++;
            } else if (currentPathIndex < currentPath.Count - 2) {
                currentPathIndex++;
            }
        }
    }

    public void UpdateTarget() {
        desiredTarget = enemyAttack.GetCurrentTarget();
        activeTarget = desiredTarget;
        if (pathfindingGrid.GetBlockedNode(desiredTarget).Length > 0) {
            targetBlocked = pathfindingGrid.GetClosestNodeNotBlocked(desiredTarget, transform.position);
            activeTarget = targetBlocked;
        }
    }
}
