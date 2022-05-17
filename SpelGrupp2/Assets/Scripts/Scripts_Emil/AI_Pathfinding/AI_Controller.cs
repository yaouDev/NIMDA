using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AI_Controller : MonoBehaviour {

    [SerializeField] private float speed, timeBetweenPathUpdates, critRange, allowedTargetDiscrepancy, turnSpeed = 100f;
    [SerializeField] private LayerMask targetMask;
    [SerializeField] private bool drawPath = false, isBoss = false;

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
    [SerializeField] public bool isStopped = true;
    CallbackSystem.PlayerHealth[] targets;
    private Vector3 destination = Vector3.zero;
    private bool pathRequestAllowed = true;

    // Start is called before the first frame update
    void Start() {
        targets = new CallbackSystem.PlayerHealth[2];
        GameObject[] tmp = GameObject.FindGameObjectsWithTag("Player");
        for (int i = 0; i < tmp.Length; i++) {
            targets[i] = tmp[i].GetComponent<CallbackSystem.PlayerHealth>();
        }

        activeTarget = targets[0].transform.position;
        col = GetComponent<Collider>();
        behaviorTree = GetComponent<BehaviorTree>();
        otherEnemyTrigger = GetComponentInChildren<SphereCollider>();
        rBody = GetComponent<Rigidbody>();
        Physics.IgnoreLayerCollision(12, 12);
        enemyHealth = GetComponent<EnemyHealth>();
        lineRenderer = GetComponent<LineRenderer>();
        Destination = ClosestPlayer;
        CallbackSystem.EventSystem.Current.RegisterListener<CallbackSystem.SafeRoomEvent>(OnPlayerEnterSafeRoom);
    }


    void Update() {
        UpdateTarget();
        behaviorTree.Update();
        RotateTowardPlayer();
        //if (IsPathRequestAllowed()) StartCoroutine(UpdatePath());
        if (IsPathRequestAllowed()) updatePath();
        if (!DynamicGraph.Instance.IsModuleLoaded(DynamicGraph.Instance.GetModulePosFromWorldPos(Position))) {
            Health.DieNoLoot();
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

    private void RotateTowardPlayer() {
        //Find Closest Player
        Vector3 relativePos = ClosestPlayer - Position;

        // Rotate the Enemy towards the player
        Quaternion rotation = Quaternion.LookRotation(relativePos, Vector3.up);
        transform.rotation = Quaternion.RotateTowards(transform.rotation,
                                                            rotation, Time.deltaTime * turnSpeed);
        transform.rotation = new Quaternion(0, transform.rotation.y, 0, transform.rotation.w);
    }

    private IEnumerator UpdatePath() {
        UpdateTarget();
        updatingPath = true;
        PathfinderManager.Instance.RequestPath(this, Position, activeTarget);
        yield return new WaitForSeconds(timeBetweenPathUpdates);
        updatingPath = false;
    }

    private void updatePath() {
        UpdateTarget();
        PathfinderManager.Instance.RequestPath(this, Position, activeTarget);
    }

    // getters and setters below
    public bool TargetInSight {
        get {
            RaycastHit hit;
            Vector3 dir = (ClosestPlayer - Position).normalized;
            Physics.Raycast(Position, dir, out hit, Mathf.Infinity, targetMask);

            if (hit.collider != null) {
                if (hit.collider.tag == "Player") {
                    return true;
                }
            }
            return false;
        }
    }

    public float Speed {
        get { return speed; }
        set { speed = value; }
    }

    public Vector3 ClosestPlayer {
        get {
            CallbackSystem.PlayerHealth closestTarget = Vector3.Distance(targets[0].transform.position, transform.position) >
            Vector3.Distance(targets[1].transform.position, transform.position) && targets[0].Alive ? closestTarget = targets[1] : targets[0];
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
            if (currentPath != null) {
                if (value > currentPath.Count) currentPathIndex = currentPath.Count - 1;
                else if (value < 0) currentPathIndex = 0;
                else currentPathIndex = value;
                return;
            }
            currentPathIndex = 0;
        }
    }

    public EnemyHealth Health { get { return enemyHealth; } }

    public Vector3 CurrentTarget { get { return activeTarget; } }

    public List<Vector3> CurrentPath {
        get { return currentPath; }
        set { currentPath = value; }
    }

    public Vector3 Position { get { return transform.position; } }

    public bool IsStopped {
        get { return isStopped; }
        set { isStopped = value; }
    }

    public bool PathRequestAllowed {
        get { return pathRequestAllowed && IsPathRequestAllowed(); }
        set { pathRequestAllowed = value; }
    }

    public float DistanceFromTarget { get { return Vector3.Distance(activeTarget, Position); } }

    public Vector3 Velocity { get { return rBody.velocity; } }

    public LineRenderer LineRenderer { get { return lineRenderer; } }

    public Vector3 CurrentPathNode {
        get {
            if (currentPath == null) return Vector3.zero;
            return currentPath[currentPathIndex];
        }
    }

    public Vector3 NextPathNode {
        get {
            if (currentPath == null) return Vector3.zero;
            else if (currentPathIndex == CurrentPath.Count - 1) return CurrentPathNode;
            return currentPath[currentPathIndex + 1];
        }
    }

    public Rigidbody Rigidbody { get { return rBody; } }

    private bool IsPathRequestAllowed() {
        bool nullCond = currentPath != null && currentPath.Count != 0;
        bool indexCond = nullCond && currentPath.Count - currentPathIndex <= 5;
        bool discrepancyCond = nullCond && currentPath[currentPath.Count - 1] != activeTarget &&
        Vector3.Distance(currentPath[currentPath.Count - 1], activeTarget) >= allowedTargetDiscrepancy;
        float distToTarget = Vector3.Distance(Position, activeTarget);
        bool criticalRangeCond = distToTarget < critRange && Destination == ClosestPlayer;
        bool distCond = distToTarget > critRange && isStopped;
        return ((currentPath == null || currentPath.Count == 0) || distCond || discrepancyCond || (criticalRangeCond && discrepancyCond) || indexCond);// && !updatingPath;
    }



    private void FixedUpdate() {
        MoveFromBlock();
        if (!isStopped) {
            // AdjustForLatePathUpdate();
            Move();
        }
    }

    void MoveFromBlock() {
        float jumpHeight = 0.1f;
        bool velocityCond = Rigidbody.velocity.magnitude < 0.05f;
        if (isBoss) {
            velocityCond = Rigidbody.velocity.magnitude < 0.01f;
        }

        if (velocityCond && !isStopped && currentPath != null && CurrentPath.Count > 0 && DistanceFromTarget > 2f) {

            Vector3[] pathNeighbors = DynamicGraph.Instance.GetPossibleNeighbors(CurrentPathNode);
            Vector3[] currentPosNeighbors = DynamicGraph.Instance.GetPossibleNeighbors(DynamicGraph.Instance.GetClosestNode(Position));
            bool[] blockedNeighbors = new bool[pathNeighbors.Length];

            bool posBlocked = false;
            bool sideBlocked = false;

            for (int i = 0; i < pathNeighbors.Length; i++) {
                if (DynamicGraph.Instance.IsNodeBlocked(pathNeighbors[i])) {
                    blockedNeighbors[i] = true;
                }

                if (Vector3.Dot((pathNeighbors[i] - CurrentPathNode).normalized, (NextPathNode - CurrentPathNode).normalized) == 0) sideBlocked = true;
                if (DynamicGraph.Instance.IsNodeBlocked(currentPosNeighbors[i])) {
                    posBlocked = true;
                }
            }

            if (posBlocked) {
                bool anyNodeBlocked = false;
                Vector3 dirToMoveBack = Vector3.zero;
                int lerpCount = 2;
                for (int i = 0; i < pathNeighbors.Length; i++) {
                    if (blockedNeighbors[i]) {
                        anyNodeBlocked = true;
                        float dot = Vector3.Dot((pathNeighbors[i] - CurrentPathNode).normalized, (NextPathNode - CurrentPathNode).normalized);

                        if (dirToMoveBack == Vector3.zero) dirToMoveBack = (CurrentPathNode - pathNeighbors[i]).normalized;
                        else {
                            float lerpVal = 0;
                            if (dot < 0.5f || (dot >= 0.5f && !sideBlocked)) lerpVal = 0.5f;
                            else if (dot >= 0.5f && sideBlocked) lerpVal = 0.3f;
                            dirToMoveBack = Vector3.Lerp(dirToMoveBack, (CurrentPathNode - pathNeighbors[i]).normalized, lerpVal);
                            lerpCount++;
                        }
                        dirToMoveBack.y = 0;
                        dirToMoveBack = dirToMoveBack.normalized;
                    }
                }
                // the enemy is stuck on a collider, like a wall
                if (anyNodeBlocked) {
                    Vector3 dirForce = dirToMoveBack * speed * 13f;
                    Rigidbody.AddForce(dirForce, ForceMode.Force);
                    Debug.DrawLine(Position, Position + dirForce * speed, Color.red);
                }
            }

            // the enemy is stuck between two modules
            /*          else if (Vector3.Distance(Position, currentPath[0]) > 0.5f) {
                         Rigidbody.MovePosition(new Vector3(Position.x, Position.y + jumpHeight, Position.z));
                     } */
        }
    }

    // causes FPS to tank with many enemies, sometimes. Needs a better solution. moves enemies away form each other.
    private void OnTriggerStay(Collider other) {
        if (other != null && other.tag == "Enemy") {
            Vector3 directionOfOtherEnemy = (other.transform.position - Position).normalized;
            Vector3 valueToTest = transform.position;
            if (rBody.velocity.magnitude > 0.05f) valueToTest = rBody.velocity.normalized;
            float dot = Vector3.Dot(valueToTest, -directionOfOtherEnemy);
            if ((dot >= 0) || valueToTest == transform.position) {
                Vector3 forceToAdd = -directionOfOtherEnemy * speed;
                float multiplier = 0.15f;
                if (!isStopped) multiplier = 0.05f;
                forceToAdd.y = 0;
                rBody.AddForce(forceToAdd * multiplier, ForceMode.Force);
                Rigidbody.velocity = Vector3.ClampMagnitude(Rigidbody.velocity, speed);
            }
        }
    }

    private void AdjustForLatePathUpdate() {
        if (currentPath != null && currentPathIndex == 0 && currentPath.Count != 0) {
            bool distanceCond = Vector3.Distance(CurrentPathNode, activeTarget) > Vector3.Distance(Position, activeTarget);
            while (currentPathIndex < currentPath.Count - 2 &&
            Vector3.Distance(CurrentPathNode, activeTarget) > Vector3.Distance(Position, activeTarget) &&
            Vector3.Dot(currentPath[currentPathIndex + 1] - CurrentPathNode.normalized, (activeTarget - CurrentPathNode).normalized) >= 0)
                currentPathIndex++;
        }
    }


    private void Move() {
        if (currentPath != null && currentPath.Count != 0) {
            bool onPathCond = Vector3.Distance(Position, CurrentPathNode) > 0.1f;
            bool endOfPathCond = (currentPathIndex == currentPath.Count - 1 && Vector3.Distance(Position, CurrentPathNode) > 0.1f);
            if (onPathCond || endOfPathCond) {
                int indexesToLerp = 4;
                if (currentPath.Count - 1 - currentPathIndex < 4) indexesToLerp = currentPath.Count - 1 - currentPathIndex;
                Vector3 lerpForceToAdd = (Vector3.Lerp(CurrentPathNode, currentPath[currentPathIndex + indexesToLerp], 0.5f) - Position).normalized * speed;
                lerpForceToAdd.y = 0;
                Vector3 forceTadd = lerpForceToAdd;
                //if (currentPathIndex != currentPath.Count - 1 && rBody.velocity.magnitude < 0.1f) forceTadd = (CurrentPathNode - Position).normalized * speed * 5;
                Rigidbody.AddForce(forceTadd, ForceMode.Force);
                if (currentPathIndex != currentPath.Count - 1 && Vector3.Distance(currentPath[currentPathIndex + 1], Position) < Vector3.Distance(CurrentPathNode, Position)) currentPathIndex++;

            } else if (currentPathIndex < currentPath.Count - 2) {
                currentPathIndex++;
            }
            // ensure enemies stay at their max speed
            Rigidbody.velocity = Vector3.ClampMagnitude(Rigidbody.velocity, speed);
        }
    }

    public void UpdateTarget() {
        activeTarget = Destination;
        activeTarget = DynamicGraph.Instance.GetClosestNode(activeTarget);
    }

    private void OnPlayerEnterSafeRoom(CallbackSystem.SafeRoomEvent safeRoomEvent) {
        if (!isBoss) {
            Health.DieNoLoot();
        }
    }

    private void OnDestroy() {
        CallbackSystem.EventSystem.Current.UnregisterListener<CallbackSystem.SafeRoomEvent>(OnPlayerEnterSafeRoom);
    }
}
