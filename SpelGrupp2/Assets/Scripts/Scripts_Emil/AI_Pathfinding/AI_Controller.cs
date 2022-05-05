using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_Controller : MonoBehaviour {

    [SerializeField] private float speed, timeBetweenPathUpdates, maxCritRange, minCritRange, allowedTargetDiscrepancy;
    [SerializeField] private bool drawPath = false;
    private EnemyHealth enemyHealth;
    private Vector3 desiredTarget, targetBlocked, activeTarget;
    private LineRenderer lineRenderer;
    private List<Vector3> currentPath;
    private Collider col;
    private SphereCollider otherEnemyTrigger;
    private Rigidbody rBody;
    private int currentPathIndex = 0;
    private bool updatingPath = false;
    private BehaviorTree behaviorTree;
    private bool isStopped = true;
    GameObject[] targets;
    private Vector3 destination = Vector3.zero;

    // Start is called before the first frame update
    void Start() {

        targets = GameObject.FindGameObjectsWithTag("Player");
        activeTarget = targets[0].transform.position;
        col = GetComponent<Collider>();
        behaviorTree = GetComponent<BehaviorTree>();
        otherEnemyTrigger = GetComponentInChildren<SphereCollider>();
        rBody = GetComponent<Rigidbody>();
        Physics.IgnoreLayerCollision(12, 12);
        enemyHealth = GetComponent<EnemyHealth>();
        lineRenderer = GetComponent<LineRenderer>();
    }




    void Update() {
        UpdateTarget();
        behaviorTree.Update();
        if(!updatingPath) StartCoroutine(UpdatePathInterval());
        if (!DynamicGraph.Instance.IsModuleLoaded(DynamicGraph.Instance.GetModulePosFromWorldPos(Position))) {
            Destroy(gameObject);
        }
        // This code is for debugging purposes only, shows current calculated path
        if (drawPath && currentPath != null && currentPath.Count != 0) {
            Vector3 prevPos = currentPath[0];
            foreach (Vector3 pos in currentPath) {
                if (pos != prevPos)
                    Debug.DrawLine(prevPos, pos, Color.blue);
                prevPos = pos;
            }
        }
    }

    public IEnumerator UpdatePath() {
        updatingPath = true;
        PathfinderManager.Instance.RequestPath(this, Position, activeTarget);
        yield return new WaitForSeconds(timeBetweenPathUpdates);
        updatingPath = false;
    }

    public IEnumerator UpdatePathInterval() {
        updatingPath = true;
        PathfinderManager.Instance.RequestPath(this, Position, activeTarget);
        yield return new WaitForSeconds(5f);
        updatingPath = false;
    }

    // getters and setters below
    public bool TargetInSight {
        get {
            RaycastHit hit;
            Physics.BoxCast(Position, transform.lossyScale, (activeTarget - Position).normalized, out hit, transform.rotation, Mathf.Infinity);
            if (hit.collider != null) {
                if (hit.collider.tag == "Player") {
                    return true;
                }
            }
            return false;
        }
    }
    public Vector3 ClosestPlayer {
        get {
            GameObject closestTarget = Vector3.Distance(targets[0].transform.position, transform.position) >
            Vector3.Distance(targets[1].transform.position, transform.position) ? closestTarget = targets[1] : targets[0];
            return closestTarget.transform.position;
        }
    }

    public Vector3 Destination {
        get { return destination; }
        set { destination = value; }
    }

    public int CurrentPathIndex {
        get { return currentPathIndex; }
        set {
            if (value > currentPath.Count) currentPathIndex = currentPath.Count - 1;
            else if (value < 0) currentPathIndex = 0;
            else currentPathIndex = value;
        }
    }

    public EnemyHealth Health { get { return enemyHealth; } }

    public Vector3 CurrentTarget {
        get { return activeTarget; }
    }

    public List<Vector3> CurrentPath {
        get { return currentPath; }
        set { currentPath = value; }
    }

    public Vector3 Position {
        get { return transform.position; }
    }

    public bool IsStopped {
        get { return isStopped; }
        set { isStopped = value; }
    }

    public float DistanceFromTarget {
        get { return Vector3.Distance(activeTarget, Position); }
    }

    public Vector3 Velocity {
        get { return rBody.velocity; }
    }

    public LineRenderer LineRenderer {
        get { return lineRenderer; }
    }

    public Vector3 CurrentPathNode {
        get { return currentPath[currentPathIndex]; }
    }

    public Rigidbody Rigidbody {
        get { return rBody; }
    }

    public bool IsPathRequestAllowed() {
        bool nullCond = currentPath != null && currentPath.Count != 0;
        bool indexCond = nullCond && currentPath.Count - currentPathIndex <= 5;
        bool discrepancyCond = nullCond && currentPath[currentPath.Count - 1] != activeTarget &&
        Vector3.Distance(currentPath[currentPath.Count - 1], activeTarget) >= allowedTargetDiscrepancy;
        float distToTarget = Vector3.Distance(transform.position, activeTarget);
        bool criticalRangeCond = distToTarget < minCritRange;
        bool distCond = distToTarget > minCritRange && isStopped;
        return (currentPath == null || distCond || discrepancyCond || (criticalRangeCond && !discrepancyCond) || indexCond) && !updatingPath;
    }



    private void FixedUpdate() {
        if (!isStopped) {
            AdjustForLatePathUpdate();
            Move();
        }
        Debug.DrawLine(Position, Position + Rigidbody.velocity, Color.red);
    }

    // causes FPS to tank with many enemies. Needs a better solution
    /*     private void OnTriggerStay(Collider other) {
            if (other != null && other.tag == "Enemy" && other.transform.parent.gameObject != gameObject) {
                Vector3 directionOfOtherEnemy = (other.transform.position - Position).normalized;
                Vector3 valueToTest = transform.position;
                if (rBody.velocity.magnitude > 0.05f) valueToTest = rBody.velocity.normalized;
                float dot = Vector3.Dot(valueToTest, -directionOfOtherEnemy);
                if ((dot >= 0) || valueToTest == transform.position) {
                    Vector3 forceToAdd = Vector3.zero;
                    if (isStopped) {
                        //forceToAdd = -directionOfOtherEnemy.normalized * speed * 0.33f;
                        forceToAdd = Vector3.Lerp(-directionOfOtherEnemy, activeTarget, 0.9f).normalized * speed * 0.15f;
                    } else forceToAdd = Vector3.Lerp(rBody.velocity.normalized, -directionOfOtherEnemy, 0.85f).normalized * speed * 0.05f;
                    forceToAdd.y = 0;
                    // I have no idea why this suddenly made the force so explosive
                    rBody.AddForce(forceToAdd, ForceMode.Force);
                }
            }
        } */

    private void AdjustForLatePathUpdate() {
        if (currentPath != null && currentPathIndex == 0 && currentPath.Count != 0) {
            bool distanceCond = Vector3.Distance(CurrentPathNode, activeTarget) > Vector3.Distance(Position, activeTarget);
            while (currentPathIndex < currentPath.Count - 2 &&
            Vector3.Distance(CurrentPathNode, activeTarget) > Vector3.Distance(Position, activeTarget) &&
            Vector3.Dot(currentPath[currentPathIndex + 1] - CurrentPathNode.normalized, (activeTarget - CurrentPathNode).normalized) >= 0)
                currentPathIndex++;
        }
    }


    // Something weird going on with movement jank atm
    private void Move() {
        if (currentPath != null && currentPath.Count != 0) {
            if (Vector3.Distance(Position, CurrentPathNode) > 0.5f || (currentPathIndex == currentPath.Count - 1 && Vector3.Distance(Position, CurrentPathNode) > 2f)) {
                int indexesToLerp = 4;
                if (currentPath.Count - 1 - currentPathIndex < 4) indexesToLerp = currentPath.Count - 1 - currentPathIndex;
                Vector3 lerpForceToAdd = (Vector3.Lerp(CurrentPathNode, currentPath[currentPathIndex + indexesToLerp], 0.5f) - Position).normalized * speed;
                lerpForceToAdd.y = 0;
                Vector3 forceTadd = lerpForceToAdd;
                if (currentPathIndex != currentPath.Count - 1 && rBody.velocity.magnitude < 0.5f) forceTadd = (CurrentPathNode - Position).normalized * speed * 5;
                Rigidbody.AddForce(forceTadd, ForceMode.Force);
                if (currentPathIndex != currentPath.Count - 1 && Vector3.Distance(currentPath[currentPathIndex + 1], Position) < Vector3.Distance(CurrentPathNode, Position)) currentPathIndex++;

            } else if (currentPathIndex < currentPath.Count - 2) {
                currentPathIndex++;
            }
        }
    }

    public void UpdateTarget() {
        if (destination == Vector3.zero) desiredTarget = ClosestPlayer;
        else desiredTarget = Destination;
        activeTarget = desiredTarget;
        if (DynamicGraph.Instance.GetBlockedNode(desiredTarget).Length > 0) {
            targetBlocked = DynamicGraph.Instance.GetClosestNodeNotBlocked(desiredTarget, Position);
            activeTarget = targetBlocked;
        }
    }
}
