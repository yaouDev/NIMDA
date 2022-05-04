using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStar : MonoBehaviour {
	
	private const uint N = 8;
	private const uint S = 4;
	private const uint E = 2;
	private const uint W = 1;
	private readonly uint[] walls = new[] { N, S, E, W };
	private static readonly Vector2Int[] directions = 
			new Vector2Int[] {
					Vector2Int.up, 
					Vector2Int.down, 
					Vector2Int.right, 
					Vector2Int.left
			};

	private Dictionary<Vector2Int, Node> nodes = new Dictionary<Vector2Int, Node>();
	private class Node {
		public Node(Vector2Int pos, Vector2Int goal) {
			this.Pos = pos;
			Node.goal = goal;
		}
		
		public Node(Vector2Int pos, Vector2Int goal, int g) {
			this.Pos = pos;
			Node.goal = goal;
			this.G = g;
		}

		private static Vector2Int goal;
		public Vector2Int Pos;
		public int H => Mathf.Abs(Pos.x - goal.x) + Mathf.Abs(Pos.y - goal.y);
		public int G = int.MaxValue;
		public int F => Mathf.Min(H + G, Int32.MaxValue); 
		public Vector2Int CameFrom;
	}

	

	private List<Vector2Int> ReconstructPath(Node current) {
		List<Vector2Int> path = new List<Vector2Int>();
		int debug = 0;
		while (current.CameFrom != current.Pos) {
			path.Add(current.Pos);
			current = nodes[current.CameFrom];
			if (debug++ > 1000) break;
		}

		return path;
	}
	
	public List<Vector2Int> Path(Vector2Int start, Vector2Int goal, uint[,] graph) {
		
		List<Node> openSet = new List<Node>();
		nodes = new Dictionary<Vector2Int, Node>();
		Node startNode = new Node(start, goal, 0);
		startNode.CameFrom = start;
		openSet.Add(startNode);
		nodes.Add(startNode.Pos, startNode);
		
		while (openSet.Count > 0) {

			// TODO [Patrik] make openSet a min-heap/priority queue
			// find lowest F cost Node in openSet
			Node current = null;
			int lowestFCost = 10000;
			for (int node = 0; node < openSet.Count; node++) {

				if (openSet[node].F < lowestFCost) {
					lowestFCost = openSet[node].F;
					current = openSet[node];
				}
			}
		
			if (current.Pos == goal) {
		
				List<Vector2Int> completePath = ReconstructPath(current);
				completePath.Reverse();
				return completePath;
			}
			
			openSet.Remove(current);
			List<Node> neighbors = GetNeighbors(current.Pos, graph, goal);

			for (int neighbor = 0; neighbor < neighbors.Count; neighbor++) {

				int tentativeG = current.G + 1;
				
				if (tentativeG < neighbors[neighbor].G) {
					
					neighbors[neighbor].CameFrom = current.Pos;
					neighbors[neighbor].G = tentativeG;
					
					if (!openSet.Contains(neighbors[neighbor])) {
						openSet.Add(neighbors[neighbor]);
					}
				}
			}
		}

		List<Vector2Int> path = new List<Vector2Int>();
		
		return path;
	}
	
	private List<Node> GetNeighbors(Vector2Int current, uint[,] graph, Vector2Int goal) {
		List<Node> neighbors = new List<Node>();
		
		for (int dir = 0; dir < walls.Length; dir++) {

			Vector2Int neighbor = current + directions[dir];
			
			if (neighbor.x >= 0 &&
			    neighbor.y >= 0 &&
			    neighbor.x < graph.GetLength(0) &&
			    neighbor.y < graph.GetLength(1) && 
				(graph[current.x, current.y] & walls[dir]) == 0) {

				if (nodes.ContainsKey(current + directions[dir])) {
					
					neighbors.Add(nodes[current + directions[dir]]);
				} else {
					
					Node node = new Node(current + directions[dir], goal);
					node.CameFrom = current;
					// TODO add more known variables to node
					nodes.Add(current + directions[dir], node);
					neighbors.Add(node);
				}
			}
		}
		
		return neighbors;
	}
}