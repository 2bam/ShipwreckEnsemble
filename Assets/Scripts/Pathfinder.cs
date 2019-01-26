using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Pathfinder
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


  public List<INode> bfs(INode source, INode dest)
    {
        //initialization
        var visited = new HashSet<INode>();// add contains remove
        var parent = new Dictionary<INode, INode>();
        Queue<INode> nodeQueue = new Queue<INode>();
        nodeQueue.Enqueue(source);

        INode current;

        while (nodeQueue.Count != 0)
        {
            //visit current top of the queue.
            current = nodeQueue.Dequeue();

            if (current == dest) break;

            if (!visited.contains(current))
                visited.add(current);

            //add it's neighbors to the queue.
            foreach (var node in current.neighbors)
                if (!visited.contains(node))
                {
                    nodeQueue.add(node);
                    parent[node] = current;
                }
        }


        //build path.
        List<INode> path = new List<INode>();

        while (current != source)
        {
            path.add(current);
            currrent = parent[current];
        }
        path.add(source);

        return path;
    }

}




