using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;

[RequireComponent(typeof(AStar))]
public class ProceduralWorldGeneration : MonoBehaviour
{
    [SerializeField] private bool animate;
    private const uint N = 8;
    private const uint S = 4;
    private const uint E = 2;
    private const uint W = 1;

    private static readonly Vector2Int[] directions =
        new Vector2Int[]
        {
            Vector2Int.up,
            Vector2Int.down,
            Vector2Int.right,
            Vector2Int.left
        };

    private Dictionary<Vector2Int, uint> walls = new Dictionary<Vector2Int, uint>();
    private uint[,] graph;
    private System.Random random;
    private static ProceduralWorldGeneration instance;
    private Quaternion tileRotation = Quaternion.Euler(90, 0, 0);

    [SerializeField] private Vector2Int worldSize;
    [SerializeField] private Vector2Int startPos;
    [SerializeField] private Vector2Int goalPos;
    [SerializeField] [Range(0.0f, 1.0f)] private float eraseFraction = .1f;
    [SerializeField] [Range(0, 10000)] private int worldSeed;
    [SerializeField] [Range(0, 10)] private int deadEndMaxDepth = 1;
    [SerializeField] [Range(0, 10)] private int deadEndMinDepth = 1;

    [SerializeField] private GameObject[] tileTypes;

    [SerializeField] private Camera cam;

    private GameObject[,] tiles;

    private SpriteRenderer[,] sr;

    //[SerializeField] private GameObject currentIndicator;
    [SerializeField] private GameObject pathIndicator;
    [SerializeField] private Vector2Int[] criticalPoints;
    private AStar aStar;
    public List<Vector2Int> pointsOfInterest = new List<Vector2Int>();

    private void Awake()
    {
        aStar = GetComponent<AStar>();
        tiles = new GameObject[worldSize.x, worldSize.y];
        sr = new SpriteRenderer[worldSize.x, worldSize.y];
        if (instance != null) Destroy(this.transform.gameObject);
        instance ??= this;

        walls.Add(Vector2Int.up, N);
        walls.Add(Vector2Int.down, S);
        walls.Add(Vector2Int.right, E);
        walls.Add(Vector2Int.left, W);

        random = new System.Random(worldSeed);
        graph = new uint[worldSize.x, worldSize.y];
        // StartCoroutine( 
        MakeMaze();
        // );
        TearDownWalls();
        Path();
        //ShowMaze();
    }

    private void MakeMaze()
    {
        int slowAnimate = 0;
        Stack<Vector2Int> backTracker = new Stack<Vector2Int>();
        HashSet<Vector2Int> unvisited = new HashSet<Vector2Int>();

        MakeAllTilesUnvisited(unvisited);

        Vector2Int current = startPos;
        unvisited.Remove(current);

        while (unvisited.Count > 0)
        {
            List<Vector2Int> neighbors = GetUnvisitedNeighbours(current, unvisited);

            if (neighbors.Count > 0)
            {
                Vector2Int next = neighbors[random.Next(neighbors.Count)];
                backTracker.Push(current);

                Vector2Int dir = next - current;

                graph[current.x, current.y] -= walls[dir];
                graph[next.x, next.y] -= walls[-dir];

                //InstantiateTile(current);
                //InstantiateTile(next);

                current = next;
                unvisited.Remove(current);
            }
            else if (backTracker.Count > 0)
            {
                current = backTracker.Pop();
            }

            // animation
            //currentIndicator.transform.position = new Vector3( current.x, .1f, current.y);
            //MoveTrail(currentIndicator.transform.position);

            // if (false && animate && slowAnimate++ % 5 == 0)
            // 	yield return null;
        }

        // StartCoroutine(TrailMover());
        // StartCoroutine(
        // TearDownWalls();
        // );
        // StartCoroutine(FadeTiles());
    }

    private void TearDownWalls()
    {
        // procedurally remove a number of the map walls
        for (int i = 0; i < worldSize.x * worldSize.y * eraseFraction; i++)
        {
            int x = random.Next(1, worldSize.x - 2);
            int y = random.Next(1, worldSize.y - 2);
            Vector2Int current = new Vector2Int(x, y);

            Vector2Int direction = directions[random.Next(directions.Length)];

            // if there is a wall in current direction
            if ((graph[current.x, current.y] & walls[direction]) != 0)
            {
                Vector2Int neighbor = current + direction;

                // tear down walls
                graph[current.x, current.y] -= walls[direction];
                graph[neighbor.x, neighbor.y] -= walls[-direction];

                //InstantiateTile(current);
                //InstantiateTile(neighbor);

                //currentIndicator.transform.position = new Vector3( current.x, .1f, current.y);
                // if (false && animate)
                // 	yield return null;
            }
        }

        // StartCoroutine(
        // Path();
        // );
    }

    private void Path()
    {
        HashSet<Vector2Int> allCriticalPathCoordinates = new HashSet<Vector2Int>();
        Vector2Int[][] connections = new[]
        {
            new Vector2Int[] {criticalPoints[0], criticalPoints[1]},
            new Vector2Int[] {criticalPoints[0], criticalPoints[2]},
            new Vector2Int[] {criticalPoints[1], criticalPoints[3]},
            new Vector2Int[] {criticalPoints[2], criticalPoints[4]},
            new Vector2Int[] {criticalPoints[3], criticalPoints[5]},
            new Vector2Int[] {criticalPoints[4], criticalPoints[5]},
        };
        Color[] colors = new[] {Color.blue, Color.red, Color.green, Color.cyan, Color.red, Color.magenta, Color.blue};

        // animation
        for (int paths = 0; paths < connections.Length; paths++)
        {
            Vector2Int start = connections[paths][0];
            Vector2Int goal = connections[paths][1];
            List<Vector2Int> path = aStar.Path(start, goal, graph);

            for (int node = 0; node < path.Count; node++)
            {
                allCriticalPathCoordinates.Add(path[node]);
                Vector3 current = new Vector3(path[node].x, .1f, path[node].y);
                //currentIndicator.transform.position = current;
                //SpriteRenderer sr = Instantiate(pathIndicator, current + Vector3.down, tileRotation).GetComponent<SpriteRenderer>();
                //sr.color = colors[paths % colors.Length];
                //MoveTrail(currentIndicator.transform.position + Vector3.up);
                //if (false && animate)
                //yield return null;
            }
        } // end animation

        List<List<Vector2Int>> criticalPaths = new List<List<Vector2Int>>();

        for (int p = 0; p < connections.Length; p++)
        {
            criticalPaths.Add(aStar.Path(connections[p][0], connections[p][1], graph));
        }

        //StartCoroutine(
        PutUpWallsAroundCriticalPath(allCriticalPathCoordinates);
        //);
    }

    public void PutUpWallsAroundCriticalPath(HashSet<Vector2Int> criticalPathCoordinates)
    {
        for (int circle = 0; circle < 3; circle++)
        {
            float revolution = Mathf.PI * 2;
            float radians = 0;
            while (radians < revolution)
            {
                radians += .001f;

                int[] radius = new int[] {15, 27, 35};

                Vector2Int vec2 = goalPos +
                                  new Vector2Int((int) (Mathf.Cos(radians) * radius[circle]),
                                      (int) (Mathf.Sin(radians) * radius[circle]));

                // if (false && 
                //     animate &&
                //     last != vec2 &&
                //     vec2.x < worldSize.x &&
                //     vec2.y < worldSize.y &&
                //     vec2.x > 0 &&
                //     vec2.y > 0)
                //  yield return null;

                if (!criticalPathCoordinates.Contains(vec2))
                {
                    if (vec2.x < worldSize.x &&
                        vec2.y < worldSize.y &&
                        vec2.x >= 0 &&
                        vec2.y >= 0)
                    {
                        // upper part of circle

                        if (radians < revolution * .5f)
                        {
                            graph[vec2.x, vec2.y] = (W | N);

                            // North
                            if (vec2.y + 1 < worldSize.y)
                            {
                                graph[vec2.x, vec2.y + 1] |= S;
                                //InstantiateTile(new Vector2Int(vec2.x, vec2.y + 1));
                            }

                            // south
                            if (vec2.y - 1 > 0 && (graph[vec2.x, vec2.y - 1] & N) != 0)
                            {
                                graph[vec2.x, vec2.y - 1] -= N;
                                //InstantiateTile(new Vector2Int(vec2.x, vec2.y - 1));
                            }
                        }
                        else // lower part of circle
                        {
                            graph[vec2.x, vec2.y] = (W | S);

                            // South
                            if (vec2.y - 1 >= 0)
                            {
                                graph[vec2.x, vec2.y - 1] |= N;
                                //InstantiateTile(new Vector2Int(vec2.x, vec2.y - 1));
                            }

                            // North
                            if (vec2.y + 1 < worldSize.y && (graph[vec2.x, vec2.y + 1] & S) != 0)
                            {
                                graph[vec2.x, vec2.y + 1] -= S;
                                //InstantiateTile(new Vector2Int(vec2.x, vec2.y + 1));    
                            }
                        }
                        //InstantiateTile(vec2);

                        // West
                        if (vec2.x - 1 > 0)
                        {
                            graph[vec2.x - 1, vec2.y] |= E;
                            //InstantiateTile(new Vector2Int(vec2.x - 1, vec2.y));
                        }

                        // East
                        if (vec2.x + 1 < worldSize.x)
                        {
                            // if wall to W, remove wall
                            if ((graph[vec2.x + 1, vec2.y] & W) != 0)
                                graph[vec2.x + 1, vec2.y] -= W;
                            //InstantiateTile(new Vector2Int(vec2.x + 1, vec2.y));
                        }

                        //currentIndicator.transform.position = new Vector3(vec2.x, .1f, vec2.y);
                        //MoveTrail(currentIndicator.transform.position);
                    }
                }
                else if (!pointsOfInterest.Contains(vec2))
                {
                    pointsOfInterest.Add(vec2);
                }
            }
        }

        for (int i = 0; i < pointsOfInterest.Count; i++)
        {
            SpriteRenderer sr =
                Instantiate(tileTypes[15], 
                    new Vector3(pointsOfInterest[i].x, 0, pointsOfInterest[i].y), 
                    tileRotation,
                    this.transform).GetComponent<SpriteRenderer>();
            sr.color = Color.black;
        }

        CheckDeadEnds(criticalPathCoordinates);

        for (int y = 0; y < worldSize.y; y++)
        {
            for (int x = 0; x < worldSize.x; x++)
            {
                Vector2Int pos = new Vector2Int(x, y);
                InstantiateTile(pos);
            }
        }
    }

    [SerializeField] private GameObject deadEnd;

    private void CheckDeadEnds(HashSet<Vector2Int> criticalPathCoordinates)
    {
        // make sure border coordinates have "walls" to the North and South
        for (int x = 0; x < worldSize.x; x++)
        {
            graph[x, 0] |= S;
            graph[x, worldSize.y - 1] |= N;
        }

        // make sure border coordinates have "walls" to the East and West
        for (int y = 0; y < worldSize.y; y++)
        {
            graph[0, y] |= W;
            graph[worldSize.x - 1, y] |= E;
        }

        for (int y = 0; y < worldSize.y; y++)
        {
            for (int x = 0; x < worldSize.x; x++)
            {
                // only one entrance
                if (!criticalPathCoordinates.Contains(new Vector2Int(x, y)) &&
                    (graph[x, y] == (N | S | E) ||
                     graph[x, y] == (N | S | W) ||
                     graph[x, y] == (N | E | W) ||
                     graph[x, y] == (S | E | W)))
                {
                    SealOffDeadEnds(new Vector2Int(x, y));
                }
            }
        }
    }

    private void SealOffDeadEnds(Vector2Int start)
    {
        if (deadEndMaxDepth < deadEndMinDepth)
            deadEndMinDepth = deadEndMaxDepth;

        HashSet<Vector2Int> seen = new HashSet<Vector2Int>();
        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        Queue<Vector2Int> deadEnds = new Queue<Vector2Int>();

        queue.Enqueue(start);
        Vector2Int lastPos;

        while (queue.Count > 0)
        {
            lastPos = queue.Dequeue();
            deadEnds.Enqueue(lastPos);
            seen.Add(lastPos);

            for (int dir = 0; dir < directions.Length; dir++)
            {
                if ((graph[lastPos.x, lastPos.y] & walls[directions[dir]]) != 0)
                    continue;
                Vector2Int next = lastPos + directions[dir];
                if (next.x < 0 ||
                    next.y < 0 ||
                    next.x >= worldSize.x ||
                    next.y >= worldSize.y)
                    continue;

                uint nextPlusFrom = (graph[next.x, next.y] | walls[-directions[dir]]);
                if (!queue.Contains(next) &&
                    !seen.Contains(next) &&
                    (nextPlusFrom == (N | S | E) ||
                     nextPlusFrom == (N | S | W) ||
                     nextPlusFrom == (N | E | W) ||
                     nextPlusFrom == (S | E | W)))
                {
                    queue.Enqueue(next);
                }
            }
        }

        int deadEndDepth = random.Next(deadEndMinDepth, deadEndMaxDepth);
        Vector2Int currDeadEnd;
        while (deadEnds.Count > deadEndDepth)
        {
            currDeadEnd = deadEnds.Dequeue();
            graph[currDeadEnd.x, currDeadEnd.y] = (N | S | E | W);

            if (!animate)
                continue;
            //Instantiate(deadEnd, new Vector3(currDeadEnd.x, -.01f, currDeadEnd.y), tileRotation, this.gameObject.transform);
            if (currDeadEnd.y > 0)
                graph[currDeadEnd.x, currDeadEnd.y - 1] |= N;
            if (currDeadEnd.y < worldSize.y - 1)
                graph[currDeadEnd.x, currDeadEnd.y + 1] |= S;
            if (currDeadEnd.x > 0)
                graph[currDeadEnd.x - 1, currDeadEnd.y] |= E;
            if (animate && currDeadEnd.x < worldSize.x - 1)
                graph[currDeadEnd.x + 1, currDeadEnd.y] |= W;

            //InstantiateTile(currDeadEnd);
            // if (currDeadEnd.y + 1 < worldSize.y) 
            // 	InstantiateTile(currDeadEnd + Vector2Int.up);
            // if (currDeadEnd.y - 1 > 0) 
            // 	InstantiateTile(currDeadEnd + Vector2Int.down);
            // if (currDeadEnd.x + 1 < worldSize.x) 
            // 	InstantiateTile(currDeadEnd + Vector2Int.right);
            // if (currDeadEnd.x - 1 > 0) 
            // 	InstantiateTile(currDeadEnd + Vector2Int.left);
        }
    }

    private void MakeAllTilesUnvisited(HashSet<Vector2Int> unvisited)
    {
        for (int y = 0; y < worldSize.y; y++)
        {
            for (int x = 0; x < worldSize.x; x++)
            {
                unvisited.Add(new Vector2Int(x, y));
                graph[x, y] = (N | S | E | W);
            }
        }
    }

    private void InstantiateTile(Vector2Int pos)
    {
        if (tiles[pos.x, pos.y] != null)
            Destroy(tiles[pos.x, pos.y]);

        uint tileType = graph[pos.x, pos.y];
        GameObject tile = Instantiate(tileTypes[tileType], new Vector3(pos.x, 0, pos.y), tileRotation, this.transform);
        tiles[pos.x, pos.y] = tile;
    }

    private SpriteRenderer indicator;

    private List<Vector2Int> GetUnvisitedNeighbours(Vector2Int currentCell, HashSet<Vector2Int> unvisited)
    {
        List<Vector2Int> neighbours = new List<Vector2Int>();

        foreach (Vector2Int dir in directions)
        {
            Vector2Int nextPos = currentCell + dir;
            if (unvisited.Contains(nextPos))
                neighbours.Add(nextPos);
        }

        return neighbours;
    }

    private void ShowMaze()
    {
        for (int y = 0; y < worldSize.y; y++)
        {
            for (int x = 0; x < worldSize.y; x++)
            {
                int tileType = (int) graph[x, y];
                GameObject tile = Instantiate(this.tileTypes[tileType], new Vector3(x, 0, y), tileRotation,
                    this.transform);
                tiles[x, y] = tile;
            }
        }
    }

    public uint[,] Get()
    {
        return graph;
    }
}