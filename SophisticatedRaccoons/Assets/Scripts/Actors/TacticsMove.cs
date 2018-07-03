using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TacticsMove : MonoBehaviour
{

    List<Tile> selectableTiles = new List<Tile>();

    GameObject[] tiles;
    public Tile currentSelectedTile = null;
    public bool active = true;
    public bool canCancel = true;
    public bool inBoard;
    public int move = 1;
    public float moveSpeed = 2;
    public bool moving = false;
    public bool hasPushed;

    Stack<Tile> path = new Stack<Tile>();
    public Tile currentTile;

    public Vector3 startPos;
    Vector3 velocity = new Vector3();
    Vector3 heading = new Vector3();
    public ArrowHolder arrowHolder;
    public GameObject arrowHolderPrefab;
    public bool keyReleased = true;




    protected void Init()
    {
        //cache all the tiles
        tiles = GameObject.FindGameObjectsWithTag("Tile");

    }

    public void GetCurrentTile()
    {
        currentTile = GetTargetTile(gameObject);
        currentTile.current = true;
    }

    public Tile GetTargetTile(GameObject target)
    {
        RaycastHit hit;
        Tile tile = null;

        if (Physics.Raycast(target.transform.position, -Vector3.up, out hit, 1))
        {
            tile = hit.collider.GetComponent<Tile>();
        }

        return tile;
    }

    public void ComputeAdjacencyLists()
    {
        foreach (GameObject tile in tiles)
        {
            Tile t = tile.GetComponent<Tile>();
            t.FindNeighbors();
        }
    }

    public void FindSelectableTiles()
    {
        ComputeAdjacencyLists();
        GetCurrentTile();

        Queue<Tile> process = new Queue<Tile>();
        process.Enqueue(currentTile);
        currentTile.visited = true;
        //currentTile.parent = ?? leave as null

        while (process.Count > 0)
        {
            Tile t = process.Dequeue();

            selectableTiles.Add(t);
            t.selectable = true;

            if (t.distance < move)
            {
                foreach (Tile tile in t.adjacencyList)
                {
                    if (!tile.visited)
                    {
                        tile.parent = t;
                        tile.visited = true;
                        tile.distance = 1 + t.distance;
                        process.Enqueue(tile);
                    }
                }
            }
        }
    }

    public void FindEntryTiles()
    {
        selectableTiles.Clear();

        foreach (GameObject t in tiles)
        {
            Tile temp = t.GetComponent<Tile>();

            if (temp.isSpawn)
            {
                selectableTiles.Add(temp);
                temp.selectable = true;
            }
        }

        if (currentTile == null)
        {
            currentTile = selectableTiles[0].GetComponent<Tile>();
            selectableTiles[0].GetComponent<Tile>().current = true;
        }

        foreach (Tile t in selectableTiles)
        {
            t.FindNeighbors();
        }

    }

    public void MoveToTile(Tile tile)
    {
        currentSelectedTile = null;
        path.Clear();
        tile.target = true;
        moving = true;

        Tile next = tile;
        while (next != null)
        {
            path.Push(next);
            next = next.parent;
        }
    }

    public void Move()
    {
        if (path.Count > 0)
        {
            Tile t = path.Peek();
            Vector3 target = t.transform.position;

            //calculate unit's position on top of the target tile
            target.y = gameObject.transform.position.y;

            if (Vector3.Distance(transform.position, target) >= 0.1f)
            {
                CalculateHeading(target);
                SetHorizontalVelocity();

                transform.forward = heading;
                transform.position += velocity * Time.deltaTime;
            }
            else
            {
                transform.position = target;
                path.Pop();
            }
        }

        else
        {
            RemoveSelectableTiles();
            moving = false;
            canCancel = false;
            inBoard = true;
            if (!hasPushed)
            {
                ToggleArrows(true);
            }
            else
            {               
                Deactivate(true);          
            }
        }

    }

    protected void RemoveSelectableTiles()
    {
        if (currentTile != null)
        {
            currentTile.current = false;
            currentTile = null;
        }

        foreach (Tile tile in selectableTiles)
        {
            tile.Reset();
        }

        selectableTiles.Clear();
    }

    void CalculateHeading(Vector3 target)
    {
        heading = target - transform.position;
        heading.Normalize();
    }

    void SetHorizontalVelocity()
    {
        velocity = heading * moveSpeed;
    }

    public void ToggleArrows(bool toggle)
    {
        arrowHolder.ToggleArrow(toggle);
    }

    public void Deactivate(bool endTurn)
    {
        active = false;   
             
        if (endTurn)
        GameMaster.Instance.EndTurn();
    }
}
