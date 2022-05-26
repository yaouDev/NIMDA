using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CallbackSystem;

public class AI_Controller : MonoBehaviour {

    [SerializeField] private float acceleration = 13.5f, maxSpeed = 13.5f, critRange, allowedTargetDiscrepancy, turnSpeed = 100f;
    [SerializeField] private LayerMask targetMask;
    [SerializeField] private bool drawPath = false, isBoss = false, isStopped = true;

    private EnemyHealth enemyHealth;
    private Vector3 activeTarget, destination;
    private List<Vector3> currentPath;
    private Collider col;
    private SphereCollider avoidTrigger;
    private Rigidbody rBody;
    private int currentPathIndex = 0;
    private BehaviorTree behaviorTree;
    private CallbackSystem.PlayerHealth[] targets;
    private bool targetInSight = false, updatingPath = false;

    // Getters and setters below
    public bool TargetInSight {
        get { return targetInSight; }
    }

    public float Acceleration {
        get { return acceleration; }
        set {
            if (value < 0) acceleration = 0;
            else acceleration = value;
        }
    }

    public float MaxSpeed {
        get { return maxSpeed; }
        set {
            if (value < 0) maxSpeed = 0;
            else maxSpeed = value;
        }
    }

    public Vector3 ClosestPlayer {
        get {
            CallbackSystem.PlayerHealth closestTarget = Vector3.Distance(targets[0].transform.position, transform.position) >
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
            if (currentPath != null) {
                if (value > currentPath.Count) currentPathIndex = currentPath.Count - 1;
                else if (value < 0) currentPathIndex = 0;
                else currentPathIndex = value;
                return;
            }
            currentPathIndex = 0;
        }
    }

    public bool TargetReachable {
        get {
            if (DynamicGraph.Instance.IsModuleLoaded(DynamicGraph.Instance.GetModulePosFromWorldPos(activeTarget)) &&
            !DynamicGraph.Instance.AllNeighborsBlocked(activeTarget)) return true;
            return false;
        }
    }

    public bool TargetStationary {
        get { return ClosestPlayer.x != Destination.x && ClosestPlayer.z != Destination.z; }
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

    public float DistanceFromTarget { get { return Vector3.Distance(activeTarget, Position); } }

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

    private bool PathRequestAllowed {
        get {
            bool nullCond = currentPath != null && currentPath.Count != 0;
            bool indexCond = nullCond && currentPath.Count - currentPathIndex <= 5;
            bool discrepancyCond = nullCond && !TargetStationary &&
            Vector3.Distance(currentPath[currentPath.Count - 1], activeTarget) >= allowedTargetDiscrepancy;
            float distToTarget = Vector3.Distance(Position, activeTarget);
            bool criticalRangeCond = distToTarget < critRange && !TargetStationary;
            bool distCond = distToTarget > critRange && isStopped;
            return ((currentPath == null || currentPath.Count == 0) || distCond || discrepancyCond || (criticalRangeCond && discrepancyCond) || indexCond) && !updatingPath;
        }
    }

    void Start() {

        // Getting components
        targets = new CallbackSystem.PlayerHealth[2];
        GameObject[] tmp = GameObject.FindGameObjectsWithTag("Player");
        for (int i = 0; i < tmp.Length; i++) targets[i] = tmp[i].GetComponent<CallbackSystem.PlayerHealth>();
        col = GetComponent<Collider>();
        enemyHealth = GetComponent<EnemyHealth>();
        behaviorTree = GetComponent<BehaviorTree>();
        avoidTrigger = GetComponentInChildren<SphereCollider>();
        rBody = GetComponent<Rigidbody>();

        activeTarget = targets[0].transform.position;
        Destination = ClosestPlayer;

        Physics.IgnoreLayerCollision(12, 12);

        EventSystem.Current.RegisterListener<SafeRoomEvent>(OnPlayerEnterSafeRoom);
        EventSystem.Current.RegisterListener<ModuleDeSpawnEvent>(OnModuleUnload);
    }

    private void FixedUpdate() {
        MoveFromBlock();
        if (!isStopped) {
            Move();
        }
    }

    IEnumerator updatePath() {
        updatingPath = true;
        UpdateTarget();
        PathfinderManager.Instance.RequestPath(this, Position, CurrentTarget);
        yield return new WaitForSeconds(0.5f);
        updatingPath = false;
    }

    void Update() {

        UpdateTarget();
        UpdateTargetInSight();
        UpdateRotation();
        behaviorTree.Update();

        if (TargetReachable && PathRequestAllowed) StartCoroutine(updatePath());

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

    private void UpdateRotation() {
        //Find Closest Player
        Vector3 relativePos = ClosestPlayer - Position;

        // Rotate the Enemy towards the player
        Quaternion rotation = Quaternion.LookRotation(relativePos, Vector3.up);
        transform.rotation = Quaternion.RotateTowards(transform.rotation,
                                                            rotation, Time.deltaTime * turnSpeed);
        transform.rotation = new Quaternion(0, transform.rotation.y, 0, transform.rotation.w);
    }

    private void UpdatePath() {
        UpdateTarget();
        PathfinderManager.Instance.RequestPath(this, Position, activeTarget);
    }

    private void UpdateTargetInSight() {
        RaycastHit hit;
        Vector3 dir = (ClosestPlayer - Position).normalized;
        Physics.Raycast(Position, dir, out hit, Mathf.Infinity, targetMask);

        if (hit.collider != null) {
            if (hit.collider.tag == "Player") {
                targetInSight = true;
                return;
            }
        }
        targetInSight = false;
    }

    public void UpdateTarget() {
        activeTarget = Destination;
        activeTarget = DynamicGraph.Instance.GetClosestNode(activeTarget);
    }

    public void ResetAgent() {
        CurrentPath = null;
        IsStopped = false;
        behaviorTree.ResetTree();
        updatingPath = false;
    }

    void MoveFromBlock() {
        Vector3 forceToAdd = Vector3.zero;
        if (currentPath != null && currentPathIndex != currentPath.Count - 1) {
            forceToAdd = CalculateAvoidForce(currentPathIndex, 8f);
            Rigidbody.AddForce(forceToAdd, ForceMode.Force);
        }
    }

    Vector3 CalculateAvoidForce(int index, float forceMultiplier) {
        bool velocityCond = Rigidbody.velocity.magnitude < 0.05f;
        if (isBoss) {
            velocityCond = Rigidbody.velocity.magnitude < 0.01f;
        }
        Vector3 force = Vector3.zero;

        if (velocityCond && !isStopped && currentPath != null && CurrentPath.Count > 0 && DistanceFromTarget > 2f) {

            Vector3 currentNode = currentPath[index];
            Vector3 nextNode = currentPath[index + 1];

            Vector3[] pathNeighbors = DynamicGraph.Instance.GetPossibleNeighbors(currentNode);
            bool[] blockedNeighbors = new bool[pathNeighbors.Length];

            bool sideBlocked = false;

            for (int i = 0; i < pathNeighbors.Length; i++) {
                if (DynamicGraph.Instance.IsNodeBlocked(pathNeighbors[i])) {
                    blockedNeighbors[i] = true;
                }
                if (Vector3.Dot((pathNeighbors[i] - currentNode).normalized, (nextNode - currentNode).normalized) == 0) sideBlocked = true;
            }

            bool anyNodeBlocked = false;
            Vector3 dirToMoveBack = Vector3.zero;
            for (int i = 0; i < pathNeighbors.Length; i++) {
                if (blockedNeighbors[i]) {
                    anyNodeBlocked = true;
                    float dot = Vector3.Dot((pathNeighbors[i] - currentNode).normalized, (nextNode - currentNode).normalized);

                    if (dirToMoveBack == Vector3.zero) dirToMoveBack = (currentNode - pathNeighbors[i]).normalized;
                    else {
                        float lerpVal = 0;
                        if (dot < 0.5f || (dot >= 0.5f && !sideBlocked)) lerpVal = 0.5f;
                        else if (dot >= 0.5f && sideBlocked) lerpVal = 0.3f;
                        dirToMoveBack = Vector3.Lerp(dirToMoveBack, (currentNode - pathNeighbors[i]).normalized, lerpVal);
                    }
                    dirToMoveBack.y = 0;
                    dirToMoveBack = dirToMoveBack.normalized;
                }
                // the enemy is stuck on a collider, like a wall
                if (anyNodeBlocked) {
                    force = dirToMoveBack * 15f * forceMultiplier;
                }
            }
        }
        return force;
    }

    private void Move() {
        if (currentPath != null && currentPath.Count != 0) {

            bool onPathCond = Vector3.Distance(Position, CurrentPathNode) > 0.1f;
            bool endOfPathCond = (currentPathIndex == currentPath.Count - 1 && Vector3.Distance(Position, CurrentPathNode) > 0.1f);

            if (onPathCond || endOfPathCond) {
                int indexesToLerp = 4;
                if (currentPath.Count - 1 - currentPathIndex < 4) indexesToLerp = currentPath.Count - 1 - currentPathIndex;

                Vector3 lerpForceToAdd = (Vector3.Lerp(CurrentPathNode, currentPath[currentPathIndex + indexesToLerp], 0.5f) - Position).normalized * acceleration;
                lerpForceToAdd.y = 0;
                Vector3 forceToAdd = lerpForceToAdd;

                if (currentPathIndex <= currentPath.Count - 4) forceToAdd += CalculateAvoidForce(currentPathIndex + 2, 13f);

                Rigidbody.AddForce(forceToAdd, ForceMode.Force);
                if (currentPathIndex != currentPath.Count - 1 && Vector3.Distance(NextPathNode, Position) < Vector3.Distance(CurrentPathNode, Position)) currentPathIndex++;

            } else if (currentPathIndex < currentPath.Count - 2) {
                currentPathIndex++;
            }
        }
        // ensure enemies stay at their max speed
        Rigidbody.velocity = Vector3.ClampMagnitude(Rigidbody.velocity, maxSpeed);
    }

    // Callback functions below

    public void OnPlayerEnterSafeRoom(CallbackSystem.SafeRoomEvent safeRoomEvent) {
        if (!isBoss) {
            Health.DieNoLoot();
        }
    }

    private void OnDestroy() {
        try {
            EventSystem.Current.UnregisterListener<SafeRoomEvent>(OnPlayerEnterSafeRoom);
            EventSystem.Current.UnregisterListener<ModuleDeSpawnEvent>(OnModuleUnload);
        } catch (System.Exception) {
            // only so it doesn't spam nullreference on exit playmode
        }
    }

    private void OnModuleUnload(ModuleDeSpawnEvent deSpawnEvent) {
        Vector2Int modulePos = DynamicGraph.Instance.GetModulePosFromWorldPos(Position);
        if (deSpawnEvent.Position == modulePos) Health.DieNoLoot();
    }

    // causes FPS to tank with many enemies, sometimes. Needs a better solution. moves enemies away form each other.
    private void OnTriggerStay(Collider other) {
        if (other != null && other.tag == "Enemy") {
            Vector3 offset = Vector3.zero;
            if (other.transform.position == Position) offset.x += 0.1f;
            Vector3 directionOfOtherEnemy = ((other.transform.position + offset) - Position).normalized;
            Vector3 valueToTest = transform.position;
            if (rBody.velocity.magnitude > 0.05f) valueToTest = rBody.velocity.normalized;
            float dot = Vector3.Dot(valueToTest, -directionOfOtherEnemy);
            if ((dot >= 0) || valueToTest == transform.position) {
                Vector3 forceToAdd = -directionOfOtherEnemy * acceleration;
                float multiplier = 0.15f;
                if (!isStopped) multiplier = 0.05f;
                forceToAdd.y = 0;
                rBody.AddForce(forceToAdd * multiplier, ForceMode.Force);
                //Rigidbody.velocity = Vector3.ClampMagnitude(Rigidbody.velocity, maxSpeed);
            }
        }
    }
}
