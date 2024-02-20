using System.Collections;
using System.Collections.Generic;
using Misc;
using TreeEditor;
using UnityEngine;

//copying a bunch of eeshaan's a star helper methods for this
//really a star and this should derive from a common base class and share node refs through a static list but I'm lazy
public class AttackScript : MonoBehaviour
{
    public static AttackScript attackScript;
    public GameObject floor;
    public float RechargeTime = 5;
    public float AreaEffectDist = 5;
    private List<Node> nodes = new List<Node>();
    private const float nodeDistanceThreshold = 4.0f;
    private bool canAttack = true;
    void Start()
    {
        GenerateGrid();
        attackScript = this;
    }

    public bool Attack(Vector3 startPosition)
    {
        if (!canAttack)
             return false;
        StartCoroutine(cd());
        Node start = FindClosestNode(startPosition);
        //openlist would get better performance with a heap but I have an llrbt I implemented for a past project easily accessible
        //whereas I don't have a heap.
        RBT<Node> openList = new RBT<Node>();
        RBT<Node> closedList = new RBT<Node>();
        openList.insert(start);
        while (openList.Count > 0)
        {
            Node cur = openList.deleteMin();
            closedList.insert(cur);
            StartCoroutine(ChangeColorForTime(cur));
            foreach (Enemy enemy in Enemies.enemyManager.enemies)
            {
                if (FindClosestNode(enemy.transform.position) == cur)
                {
                    Destroy(enemy.gameObject);
                }
            }
            foreach (Node other in cur.Neighbors)
            {
                if (closedList.Contains(other))
                    continue;
                if (openList.Contains(other))
                {
                    if (cur.HCost + 1 < other.HCost)
                    {
                        openList.delete(other);
                        other.HCost = cur.HCost + 1;
                        openList.insert(other);
                    }
                }
                else
                {
                    other.HCost = cur.HCost + 1;
                    if(other.HCost <= AreaEffectDist)
                        openList.insert(other);
                }
            }
        }
        CleanNodes();
        return true;
    }

    private IEnumerator ChangeColorForTime(Node n)
    {
        Material mat = n.floor.GetComponent<MeshRenderer>().materials[0];
        Color old = mat.color;
        mat.color = Color.blue;
        yield return new WaitForSeconds(0.5f);
        mat.color = old;
    }

    private IEnumerator cd()
    {
        canAttack = false;
        yield return new WaitForSeconds(RechargeTime);
        canAttack = true;
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

                if (Mathf.Abs(nodes[i].Position.x - nodes[j].Position.x) <= nodeDistanceThreshold &&
                    Mathf.Abs(nodes[i].Position.y - nodes[j].Position.y) <= nodeDistanceThreshold)
                {
                    foreach (Node node in nodes)
                    {
                        if (node.Position ==
                            new Vector3(nodes[i].Position.x + (nodes[i].Position.x - nodes[j].Position.x),
                                nodes[i].Position.y) || node.Position ==
                            new Vector3(nodes[i].Position.x,
                                nodes[i].Position.y+ (nodes[i].Position.y - nodes[j].Position.y)))
                        {
                            nodes[i].AddNeighbor(nodes[j]);
                            nodes[j].AddNeighbor(nodes[i]);
                            break;
                        }
                    }
                }
            }
        }
    }

    private void CleanNodes()
    {
        foreach (Node node in nodes)
        {
            node.HCost = 0;
        }
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
}
