using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_Controller : MonoBehaviour {

    [SerializeField] private float speed, timeBetweenPathUpdates, distanceFromTargetToStop;
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
    bool started = false;
    public AI_State[] states;
    private AI_StateMachine stateMachine;
    public bool IsStopped = true;

    // Start is called before the first frame update
    void Start() {
        col = GetComponent<Collider>();
        otherEnemyTrigger = GetComponent<SphereCollider>();
        pathfinder = PathfinderManager.instance;
        pathfindingGrid = SimpleGraph.instance; //GameObject.Find("PathfindingPlane").GetComponent<SimpleGraph>();
        rBody = GetComponent<Rigidbody>();
        Physics.IgnoreLayerCollision(12, 12);
        enemyAttack = GetComponent<EnemyAttack>();
        stateMachine = new AI_StateMachine(this, states);
        StartCoroutine(updatePath());
    }

    // this code is for debugging purposes only, shows current calculated path
    void Update() {
        if (currentPath != null && currentPath.Count != 0) {
            Vector3 prevPos = currentPath[0];
            foreach (Vector3 pos in currentPath) {
                if (pos != prevPos)
                    Debug.DrawLine(prevPos, pos, Color.blue);
                prevPos = pos;
            }
        }

    }

    public bool targetInSight() {
        RaycastHit hit;
        Physics.BoxCast(transform.position, transform.lossyScale, (activeTarget - transform.position).normalized, out hit, transform.rotation, Mathf.Infinity);
        if (hit.collider != null) {
            if (hit.collider.tag == "Player") {
                return true;
            }
        }
        return false;
    }

    public IEnumerator updatePath() {
        while (true) {
            pathfinder.requestPath(this, transform.position, activeTarget);
            currentPathIndex = 0;
            yield return new WaitForSeconds(timeBetweenPathUpdates);
        }
    }

    public Vector3 getCurrentTarget() {
        return activeTarget;
    }

    public void setPath(List<Vector3> newPath) {
        currentPath = newPath;
        currentPathIndex = 0;
    }

    public List<Vector3> getCurrentPath() {
        return currentPath;
    }

    public Vector3 getPosition() {
        return transform.position;
    }

    public void addForce(Vector3 force) {
        rBody.AddForce(force, ForceMode.Force);
    }

    public float getAttackRange() {
        return enemyAttack.getAttackRange();
    }

    public float getDistanceFromTarget() {
        return Vector3.Distance(activeTarget, transform.position);
    }

    public Vector3 getVelocity() {
        return rBody.velocity;
    }

    public int getCurrentPathIndex() { return currentPathIndex; }
    public void advanceCurrentPathIndex() { currentPathIndex++; }



    private void FixedUpdate() {
        if (!IsStopped) move();
    }

    private void move() {
        Collider[] cols = Physics.OverlapSphere(transform.position, otherEnemyTrigger.radius);
        foreach (Collider c in cols) {
            if (c.tag == "Enemy" && c.gameObject != gameObject) {
                if (Vector3.Dot(c.GetComponent<Rigidbody>().velocity.normalized, rBody.velocity.normalized) >= -0.75f) {
                    rBody.AddForce((transform.position - c.transform.position).normalized * speed);
                }
            }
        }
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


    public void updateTarget() {
        desiredTarget = enemyAttack.getCurrentTarget();
        activeTarget = desiredTarget;
        if (pathfindingGrid.getBlockedNode(desiredTarget).Length > 0) {
            targetBlocked = pathfindingGrid.getClosestBoxNotBlocked(desiredTarget, transform.position);
            activeTarget = targetBlocked;
        }
    }
}
