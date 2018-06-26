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
    public bool occupied = false;
    public bool isCorner = false;
    public bool active = false;
    public GameObject thingOnTopOfIt = null;
    Vector3 playerDirection;

    public List<Tile> adjacencyList = new List<Tile>();
    public Dictionary<string, Tile> adjacencyDict = new Dictionary<string, Tile>();

    //Needed 4 BFS (breadth first search)
    public bool visited = false;
    public Tile parent = null;
    public int distance = 0;




    // Use this for initialization
    void Start()
    {
        Reset();
    }

    // Update is called once per frame
    void Update()
    {
        if (selectable)
        {
            GetComponent<Renderer>().enabled = true;

            if (target)
            {
                GetComponent<Renderer>().material.color = Color.green;

            }
            else if (pushable)
            {
                GetComponent<Renderer>().material.color = Color.red;
            }
            else
            {
                GetComponent<Renderer>().material.color = Color.yellow;
            }

        }

        if (current)
        {
            GetComponent<Renderer>().enabled = true;

            GetComponent<Renderer>().material.color = Color.magenta;
        }
        else if (!selectable)
        {
            GetComponent<Renderer>().enabled = false;
        }    

    }

    public void Reset()
    {
        adjacencyList.Clear();

        current = false;
        target = false;
        selectable = false;
        pushable = false;
        walkable = true;
        thingOnTopOfIt = null;

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

        CheckTile("Up",Vector3.forward);
        CheckTile("Down", -Vector3.forward);
        CheckTile("Right", Vector3.right);
        CheckTile("Left", -Vector3.right);
        CheckTop();

    }


    public Vector3 CheckPlayerDirection()
    {
        if (thingOnTopOfIt)
        {  //return player's forward
            Debug.Log("returnign vector forward");
            return thingOnTopOfIt.transform.forward;
        }
        else
        {
            return Vector3.zero;

        }
    }

    public void CheckTile(Vector3 direction)
    {
        Vector3 halfExtents = new Vector3(0.1f, 0.1f, 0.1f);
        Collider[] colliders = Physics.OverlapBox(transform.position + direction, halfExtents);

        foreach (Collider item in colliders)
        {
            Tile tile = item.GetComponent<Tile>();
            if (tile != null)
            {
                tile.CheckTop();

                if (tile.thingOnTopOfIt != null)
                {
                    tile.walkable = false;
                    //CheckOcuppiedTile(tile);
                }
                if (tile.walkable || tile.pushable)
                {

                }
                adjacencyList.Add(tile);               
            }
        }
    }

    public void CheckTile(string directionKey, Vector3 direction)
    {
        Vector3 halfExtents = new Vector3(0.1f, 0.1f, 0.1f);
        Collider[] colliders = Physics.OverlapBox(transform.position + direction, halfExtents);

        foreach (Collider item in colliders)
        {
            Tile tile = item.GetComponent<Tile>();
            if (tile != null)
            {
                tile.CheckTop();

                if (tile.thingOnTopOfIt != null)
                {
                    tile.walkable = false;
                    //CheckOcuppiedTile(tile);
                }
                if (tile.walkable || tile.pushable)
                {

                }
                adjacencyDict.Add(directionKey,tile);
            }
        }
    }


    public Tile ReturnTile(Vector3 direction)
    {
        Vector3 halfExtents = new Vector3(0.1f, 0.1f, 0.1f);
        Collider[] colliders = Physics.OverlapBox(transform.position + direction, halfExtents);

        foreach (Collider item in colliders)
        {
            Tile tile = item.GetComponent<Tile>();
            if (tile != null)
            {
                return tile;
            }
        }

        return null;
    }

    public void CheckTop()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position, Vector3.up, out hit, 3))
        {
            walkable = false;
            thingOnTopOfIt = hit.transform.gameObject;
            if (thingOnTopOfIt.tag == "Booty")
            {
                Debug.Log("found booty");
                pushable = true;
            }
            else if (thingOnTopOfIt.GetComponent<PlayerMove>())
            {
                pushable = true;
            }
        }

        else
        {
            thingOnTopOfIt = null;

        }
    }

    public void CheckIfCorner()
    {
        FindNeighbors();
        if (adjacencyList.Count <= 3)
        {
            isCorner = true;
        }
        else
        {
            isCorner = false;
        }

    }
}
