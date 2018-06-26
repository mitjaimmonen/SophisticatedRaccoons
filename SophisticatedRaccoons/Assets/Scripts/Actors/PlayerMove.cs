using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : TacticsMove
{

    public bool active = true;
    public bool turnPhase = false;
    public bool beingPushed = false;
    public Vector3 target;
    public Vector3 direction;

    private void Start()
    {
        Init();
    }

    private void Update()
    {
        //if selected & not moving
        if (!moving && active)
        {
            FindSelectableTiles();
            CheckControl(); //mouse for now
            if (currentSelectedTile)
            {
                currentSelectedTile.target = true;
            }

        }
        else if (!beingPushed)
        {
            Move();
        }

        else
        {
            GetPushed();
        }

    }

    void CheckControl()
    {
        if (Input.GetMouseButtonUp(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.tag == "Tile")
                {
                    Tile t = hit.collider.GetComponent<Tile>();

                    if (t.selectable)
                    {
                        if (t == currentSelectedTile)
                        {
                            if (!turnPhase)
                            {
                                if (t.walkable)
                                {
                                    MoveToTile(t);
                                }
                                else if (t.pushable)
                                {
                                    TryToPush();
                                }
                                else
                                {
                                    //nothing
                                }

                                turnPhase = !turnPhase;
                            }
                            else
                            {
                                TurnTo(t);
                                turnPhase = !turnPhase;
                            }

                        }
                        else
                            currentSelectedTile = t;
                    }
                }
            }
        }
    }

    void TryToPush()
    {
        //if looking at thing
        //{
        List<Tile> pushableTiles = new List<Tile>();
        Tile thisTile = currentTile;
        bool done = false;

        GetCurrentTile();



        while (!done)
        {
            Tile temp = thisTile.ReturnTile(transform.forward);

            if (temp)
            {
                if (temp.pushable)
                {
                    pushableTiles.Add(temp);
                    thisTile = temp;
                }
                else
                {
                    done = true;
                }
            }
            else
            {
                done = true;
            }
        }

        if (pushableTiles.Count > 0)
        {
            if (CalculatePush(pushableTiles))
            {
                foreach (Tile t in pushableTiles)
                {
                    GameObject temp = t.thingOnTopOfIt;
                    if (temp.GetComponent<Booty>())
                    {
                        temp.GetComponent<Booty>().BePushed(transform.forward);
                    }
                    else
                    {
                        temp.GetComponent<PlayerMove>().BePushed(transform.forward);
                    }
                }
                MoveToTile(pushableTiles[0]);
            }
            else
            {
                //can't push
            }
        }


        //}
    }

    bool CalculatePush(List<Tile> list)
    {
        float strenght = 1;
        float enemyCost = 0;
        float bootyCost = 0;
        foreach (Tile t in list)
        {
            GameObject pushee = t.thingOnTopOfIt;
            if (pushee.tag == "Booty")
            {
                bootyCost -= 1;
            }
            else
            {
                if (pushee.transform.forward == transform.forward)
                {
                    strenght++;
                }
                else if (pushee.transform.forward == -transform.forward)
                {
                    enemyCost -= 1;
                }
            }
        }

        if (strenght + enemyCost > 0 && strenght + bootyCost >= 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    void TurnTo(Tile turnDir)
    {
        Vector3 lookAt = turnDir.gameObject.transform.position;
        lookAt.y = transform.position.y;
        transform.LookAt(lookAt);
    }

    public void BePushed(Vector3 _direction)
    {
        direction = _direction;
        target = transform.position + direction;
        beingPushed = true;
    }

    public void GetPushed()
    {
        if (Vector3.Distance(transform.position, target) >= 0.05f)
        {
            transform.position += (moveSpeed * direction) * Time.deltaTime;
        }
        else
        {
            transform.position = target;
            beingPushed = false;
        }
    }

}
