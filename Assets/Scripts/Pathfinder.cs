using System;
using UnityEngine;
using System.Collections.Generic;

public class Node : IComparable<Node>
{
    public Vector3 Position { get; private set; }
    public List<Node> Neighbors { get; private set; }
    public float FCost { get; set; }
    public float GCost { get; set; }
    public float HCost { get; set; }
    public Node Parent { get; set; }

    public Transform floor;

    public Node(Vector3 position)
    {
        Position = position;
        Neighbors = new List<Node>();
    }

    public void AddNeighbor(Node neighbor)
    {
        Neighbors.Add(neighbor);
    }

    public int CompareTo(Node other)
    {
        if (HCost > other.HCost)
            return 1;
        if (HCost < other.HCost)
            return -1;
        return 0;
    }
}

public class Pathfinder : MonoBehaviour
{
    public GameObject floor;
    public float nodeDistanceThreshold = 4.0f;
    public int moveCost = 1;

    private List<Node> nodes = new List<Node>();

    void Start()
    {
        GenerateGrid();
    }

    private void GenerateGrid()
    {
        foreach (Transform child in floor.transform)
        {
            Vector3 nodePosition = child.position;
            Node newNode = new Node(nodePosition);
            newNode.floor = child;
            nodes.Add(newNode);
        }

        ConnectNeighbors();
    }

    private void ConnectNeighbors()
    {
        for (int i = 0; i < nodes.Count; i++)
        {
            for (int j = i + 1; j < nodes.Count; j++)
            {
                float distance = Vector3.Distance(nodes[i].Position, nodes[j].Position);
                if (distance <= nodeDistanceThreshold)
                {
                    nodes[i].AddNeighbor(nodes[j]);
                    nodes[j].AddNeighbor(nodes[i]);
                }
            }
        }
    }

    public List<Vector3> PathFind(Vector3 start, Vector3 end)
    {
        Node startNode = FindClosestNode(start);
        Node endNode = FindClosestNode(end);

        Debug.Log(startNode.Position + "StartNode");
        Debug.Log(endNode.Position + "StartNode");

        if (startNode == null || endNode == null)
        {
            Debug.LogError("Invalid start or end node.");
            return null;
        }

        List<Node> openList = new List<Node>();
        List<Node> closedList = new List<Node>();

        startNode.GCost = 0.0f;
        startNode.HCost = CalculateDistanceCost(startNode, endNode);
        startNode.FCost = startNode.GCost + startNode.HCost;

        openList.Add(startNode);

        while (openList.Count > 0)
        {
            Node currentNode = GetLowestFCostNode(openList);
            openList.Remove(currentNode);
            closedList.Add(currentNode);

            if (currentNode == endNode)
            {
                Debug.Log("Got the Path");
                return CalculatePath(currentNode);
            }

            foreach (Node neighbor in currentNode.Neighbors)
            {
                if (closedList.Contains(neighbor))
                    continue;

                float tentativeGCost = currentNode.GCost + CalculateDistanceCost(currentNode, neighbor);
                if (!openList.Contains(neighbor) || tentativeGCost < neighbor.GCost)
                {
                    neighbor.Parent = currentNode;
                    neighbor.GCost = tentativeGCost;
                    neighbor.HCost = CalculateDistanceCost(neighbor, endNode);
                    neighbor.FCost = neighbor.GCost + neighbor.HCost;

                    if (!openList.Contains(neighbor))
                    {
                        openList.Add(neighbor);
                    }
                }
            }
        }

        Debug.LogError("Path not found.");

        return null;
    }

    private Node FindClosestNode(Vector3 position)
    {
        float minDistance = float.MaxValue;
        Node closestNode = null;

        foreach (Node node in nodes)
        {
            float distance = Vector3.Distance(position, node.Position);
            if (distance < minDistance)
            {
                minDistance = distance;
                closestNode = node;
            }
        }

        return closestNode;
    }

    private float CalculateDistanceCost(Node a, Node b)
    {
        return Vector3.Distance(a.Position, b.Position) * moveCost;
    }

    private List<Vector3> CalculatePath(Node endNode)
    {
        List<Vector3> path = new List<Vector3>();
        Node currentNode = endNode;
        HashSet<Node> visitedNodes = new HashSet<Node>();
        int maxIterations = nodes.Count * 2;

        while (currentNode != null)
        {
            if (visitedNodes.Contains(currentNode) || maxIterations <= 0)
            { 
                path.Reverse();
                return path;
            }

            visitedNodes.Add(currentNode); 
            path.Add(currentNode.Position);

            if (currentNode.Parent == null)
            {
                Debug.LogWarning("Parent node is null");
                break;
            }

            currentNode = currentNode.Parent;
            maxIterations--;
        }

        path.Reverse();

        foreach (Vector3 nodePosition in path)
        {
            Debug.Log("Path node: " + nodePosition);
        }
        return path;
    }

    private Node GetLowestFCostNode(List<Node> openList)
    {
        Node lowestFCostNode = openList[0];
        foreach (Node node in openList)
        {
            if (node.FCost < lowestFCostNode.FCost)
            {
                lowestFCostNode = node;
            }
        }
        return lowestFCostNode;
    }
}
