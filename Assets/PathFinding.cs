using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFinding : MonoBehaviour {

    public Grid grid;

    public Transform dotStart, dotTarget;

    //Find a path between dots\\
    void Update()
    {
        FindPath(dotStart.position, dotTarget.position);
    }

    void FindPath(Vector3 startPos, Vector3 targetPos)
    {
        //Get the position of the start node and the target node\\
        Node startNode = grid.NodeFromWorldPoint(startPos);
        Node targetNode = grid.NodeFromWorldPoint(targetPos);

        //Create a list for nodes that are open and could be used and a closed list to add the nodes that are bein used\\
        List<Node> openSet = new List<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();
        // Add the first node\\
        openSet.Add(startNode);

        //Add and remove nodes from the sets depending on their cost\\
        while(openSet.Count > 0)
        {
            Node currentNode = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
            {
                if(openSet[i].fCost < currentNode.fCost || openSet[i].fCost == currentNode.fCost && openSet[i].hCost< currentNode.hCost)
                {
                    currentNode = openSet[i];
                }
            }

            openSet.Remove(currentNode);
            closedSet.Add(currentNode);
            //when arriving at he target node retrace the path\\
            if(currentNode == targetNode)
            {
                RetracePath(startNode, targetNode);
                return;
            }
            //if the neighbour is not walkable or in the close set ignore it\\
            foreach(Node neighbour in grid.GetNeighbours(currentNode))
            {
                if(!neighbour.walkable || closedSet.Contains(neighbour))
                {
                    continue;
                }

                bool isOverlaping = false;
                //ignore the other paths\\
                foreach (List<Node> otherpath in grid.paths)
                {
                    if(otherpath.Contains(neighbour))
                    {
                        isOverlaping = true;
                    }
                }

                if(isOverlaping == true)
                {
                    continue;
                }


                int NewMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);
                if(NewMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                {
                    neighbour.gCost = NewMovementCostToNeighbour;
                    neighbour.hCost = GetDistance(neighbour, targetNode);
                    neighbour.parent = currentNode;

                    if (!openSet.Contains(neighbour))
                        openSet.Add(neighbour);
                }
            }
        }

    }
    //Reverse the path when added to the list of paths\\
    void RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        path.Reverse();

        grid.paths.Add(path);
    }
    //Get the distance between nodes\\
    int GetDistance(Node nodeA, Node nodeB)
    {
        int distX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int distY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

        if (distX > distY)
            return 14 * distY + 10 * (distX - distY);
        return 14 * distX + 10 * (distY - distX);



    }
}
