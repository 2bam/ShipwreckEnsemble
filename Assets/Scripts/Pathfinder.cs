using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class Pathfinder {

    //static bool FilterNodes(INode node)
    //{
    //    return true;
    //}


	//NOTE: Always returns at least a list of one (it's source)
    public static List<INode> BFS(INode source, INode dest, Func<INode, bool> filterNeighbors) {

		//initialization
		var visited = new HashSet<INode>();// add contains remove
		var parent = new Dictionary<INode, INode>();
        var distance = new Dictionary<INode, float>();
        parent[source] = null;
        List<INode> nodeList = new List<INode>();
        nodeList.Add(source);
        distance[source] = 0.0f;
		INode current = null;

		while(nodeList.Count != 0) {
            //Calculate distances.
            INode minNode = nodeList[0];
            float minDistance = distance[minNode];
            foreach (var node in nodeList)
                if (distance.ContainsKey(node) && distance[node] < minDistance)
                    minNode = node;
            current = minNode;

            //Remove swapping with last element.
            int lastIndex = nodeList.Count - 1;
            int midIndex = nodeList.FindIndex(ind => ind.Equals(minNode));
            INode temp = nodeList[lastIndex];
            nodeList[midIndex] = nodeList[lastIndex];
			nodeList.Remove(nodeList[lastIndex]);

			if(current == dest) break;


            //Visit and add it's neighbors to the queue.
            foreach (var node in current.neighbors)
				if(!visited.Contains(node) && filterNeighbors(node)) {
					visited.Add(node);
                    distance[node] = distance[current] + (current.position - node.position).sqrMagnitude;
                    nodeList.Add(node);
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

        //return source.position.
		return path;
	}

}




