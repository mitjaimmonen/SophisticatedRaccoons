using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{

    public bool walkable = true;
    public bool pushable = false;
    public bool current = false;
    public bool target = false;
    public bool selectable = false;
    Vector3 playerDirection;

    public List<Tile> adjacencyList = new List<Tile>();

    //Needed BFS (breadth first search)
    public bool visited = false;
    public Tile parent = null;
    public int distance = 0;




    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (current)
        {
            GetComponent<Renderer>().material.color = Color.magenta;
        }
        else if (target)
        {
            GetComponent<Renderer>().material.color = Color.green;

        }
        else if (selectable)
        {
            GetComponent<Renderer>().material.color = Color.yellow;

        }
        else if (pushable)
        {
            GetComponent<Renderer>().material.color = Color.red;
        }
        else
        {
            GetComponent<Renderer>().material.color = Color.gray;
        }
    }

    public void Reset()
    {
        adjacencyList.Clear();

        current = false;
        target = false;
        selectable = false;
        pushable = false;
        playerDirection = Vector3.zero;

        visited = false;
        parent = null;
        distance = 0;
    }

    public void FindNeighbors()
    {
        Reset();
       

        CheckTile(Vector3.forward);
        CheckTile(-Vector3.forward);
        CheckTile(Vector3.right);
        CheckTile(-Vector3.right);

    }

    public Vector3 CheckPlayerDirection()
    {
        //return player's forward
        return Vector3.zero;
    }

    public void CheckTile(Vector3 direction)
    {
        Vector3 halfExtents = new Vector3(0.25f, 0.25f, 0.25f);
        Collider[] colliders = Physics.OverlapBox(transform.position + direction, halfExtents);

        foreach (Collider item in colliders)
        {
            Tile tile = item.GetComponent<Tile>();
            if (tile != null)
            {
                RaycastHit hit;

                if (Physics.Raycast(tile.transform.position, Vector3.up, out hit, 1))
                {
                    if (hit.transform.gameObject.tag == "Player" && hit.transform.gameObject.transform.forward == -CheckPlayerDirection())
                    {
                        //check if can push, if other thing isn't looking at you
                        tile.pushable = false;
                        tile.walkable = false;
                    }

                    else
                    {
                        tile.pushable = true;
                    }
                }
                if (tile.walkable)
                adjacencyList.Add(tile);
            }
        }
    }
}
