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
    public bool isTurning;
    public bool slerpMoving = false;

    Stack<Tile> path = new Stack<Tile>();
    public Tile currentTile;

    public Vector3 startPos;
    public Vector3 startForward;
    Vector3 velocity = new Vector3();
    Vector3 heading = new Vector3();
    public ArrowHolder arrowHolder;
    public GameObject arrowHolderPrefab;
    public bool keyReleased = true;
    public bool autoTurnAfterMove = false;




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

    public IEnumerator SlerpMove(Vector3 target)
    {
        float time = Time.time;
        float t;
        float smoothLerp;
        float lerpTime = 0.5f;
        slerpMoving = true;
        Vector3 startPos = transform.position;
        Vector3 endPos = target;
        CalculateHeading(target);
        Debug.Log("Position is: " + transform.position + " target is: " + target);

        while (time + lerpTime > Time.time)
        {
            t = (Time.time - time) / lerpTime;
            smoothLerp = t * t * (3f - 2f * t);
            transform.position = Vector3.Lerp(startPos, endPos, smoothLerp);
            Debug.Log("looping...");
            yield return null;
        }
        transform.position = target;
        slerpMoving = false;
        yield break;
    }

    public void Move()
    {
        if (path.Count > 0)
        {
            Tile t = path.Peek();
            Vector3 target = t.transform.position;

            //calculate unit's position on top of the target tile
            target.y = gameObject.transform.position.y;

            //StartCoroutine(SlerpMove(target));
            //path.Pop();

            if (Vector3.Distance(transform.position, target) >= 0.1f)
            {
                CalculateHeading(target);
                //SetHorizontalVelocity();

                //  transform.forward = heading;
            if (!isTurning)
                StartCoroutine(SlerpTurn(target));
            if (!slerpMoving)
                StartCoroutine(SlerpMove(target));
                //transform.position += velocity * Time.deltaTime;
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
                if (autoTurnAfterMove)
                {
                    if (arrowHolder.SelectedArrow())
                    {
                        StartCoroutine(SlerpTurn(arrowHolder.SelectedArrow().gameObject.transform.position));
                    }
                }

                ToggleArrows(true);
            }
            else
            {
                Deactivate(true);
            }
        }

    }

    public IEnumerator SlerpTurn(Vector3 lookAt)
    {
        isTurning = true;
        float elapsedTime = 0;
        float time = 0.3f;
        lookAt.y = transform.position.y;
        Vector3 relativePos = lookAt - transform.position;
        Quaternion targetRotation = Quaternion.LookRotation(relativePos);
        Quaternion startRotation = transform.rotation;

        while (elapsedTime < time)
        {
            elapsedTime += Time.deltaTime;

            transform.rotation = Quaternion.Slerp(startRotation, targetRotation, (elapsedTime / time));

            yield return new WaitForEndOfFrame();
        }
        isTurning = false;
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

        foreach (GameObject tile in tiles)
        {
            Debug.Log("got here!!");
            Tile temp = tile.GetComponent<Tile>();
            if (!temp.isSpawn)
            {
                temp.Reset();
            }
        }

        if (endTurn)
        {
            GameMaster.Instance.EndTurn();
        }
    }
}
