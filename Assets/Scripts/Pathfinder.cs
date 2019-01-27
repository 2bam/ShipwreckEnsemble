using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Pathfinder {

	public static List<INode> BFS(INode source, INode dest) {
		//initialization
		var visited = new HashSet<INode>();// add contains remove
		var parent = new Dictionary<INode, INode>();


		parent[source] = null;


		Queue<INode> nodeQueue = new Queue<INode>();
		nodeQueue.Enqueue(source);

		INode current = null;

		while(nodeQueue.Count != 0) {
			//visit current top of the queue.
			current = nodeQueue.Dequeue();

			if(current == dest) break;




			//add it's neighbors to the queue.
			foreach(var node in current.neighbors)
				if(!visited.Contains(node)) {
					visited.Add(node);
					nodeQueue.Enqueue(node);
					parent[node] = current;
				}
		}


		//build path.
		List<INode> path = new List<INode>();

		while(current != source) {
			path.Add(current);
			current = parent[current];
		}
		path.Add(source);
		path.Reverse();

		return path;
	}

}




