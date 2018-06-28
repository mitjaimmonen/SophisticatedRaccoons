using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XInputDotNetPure;

public class PlayerMove : TacticsMove
{

    public bool active = true;
    public bool turnPhase = false;
    public bool beingPushed = false;
    public Vector3 target;
    public Vector3 direction;
    public bool skipMove = false;

    private void Start()
    {
        Init();

        GameObject aHolder = Instantiate(arrowHolderPrefab, transform.position, Quaternion.identity);
        arrowHolder = aHolder.AddComponent<ArrowHolder>();
        arrowHolder.owner = this;
        ToggleArrows(false);

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
        else if (!beingPushed && active)
        {
            Move();
        }

        else
        {
            GetPushed();
        }

    }

    public void HandleGamepadInput(PlayerGamepadData gamepadData)
    {
        if (!active || moving)
            return;
        
        var thumbstick = gamepadData.state.ThumbSticks;
         if (thumbstick.Left.X > 0.2f || thumbstick.Left.X < -0.2f ||
            thumbstick.Left.Y > 0.2f || thumbstick.Left.Y < -0.2f)
        {

            if (!turnPhase)
            {
                //check camera state
                Tile t = new Tile();

                if (thumbstick.Left.X < -0.2f)
                {
                    t = currentTile.adjacencyDict["Left"];
                }
                if (thumbstick.Left.X > 0.2f)
                {
                    t = currentTile.adjacencyDict["Right"];
                }
                if ( thumbstick.Left.Y > 0.2f)
                {
                    t = currentTile.adjacencyDict["Up"];
                }
                if (thumbstick.Left.Y < -0.2f)
                {
                    t = currentTile.adjacencyDict["Down"];
                }

                if (t == currentSelectedTile)
                {
                    //nothing
                }

                else
                {
                    currentSelectedTile = t;
                }
            }

            else
            {
                Arrow a = new Arrow();

                if (thumbstick.Left.X < -0.2f)
                {

                    a = arrowHolder.GetArrow("Left");
                }
                if (thumbstick.Left.X > 0.2f)
                {
                    a = arrowHolder.GetArrow("Right");

                }
                if (thumbstick.Left.Y > 0.2f)
                {
                    a = arrowHolder.GetArrow("Up");

                }
                if (thumbstick.Left.Y < -0.2f)
                {
                    a = arrowHolder.GetArrow("Down");

                }

                if (a == arrowHolder.currentArrow)
                {
                    //nothing
                }

                else
                {
                    arrowHolder.ClearSelectedArrows();
                    arrowHolder.currentArrow = null;
                    arrowHolder.currentArrow = a;
                    a.selected = true;

                }

            }
        }

        if (gamepadData.state.Buttons.A == ButtonState.Pressed)
        {
            if (!turnPhase)
            {
                if (currentSelectedTile)
                {
                    if (currentSelectedTile.walkable)
                    {
                        MoveToTile(currentSelectedTile);
                    }
                    else if (currentSelectedTile.pushable)
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
                    if (!skipMove)
                    {
                        skipMove = !skipMove;
                    }
                    else
                    {
                        skipMove = !skipMove;
                        turnPhase = !turnPhase;
                        Debug.Log("Setting arrows active");
                        ToggleArrows(true);
                    }
                }
            }

            else
            {
                TurnTo(arrowHolder.currentArrow);
                arrowHolder.currentArrow = null;
                turnPhase = !turnPhase;
            }
        }

        if (gamepadData.state.Buttons.B == ButtonState.Pressed)
        {
            if (currentSelectedTile)
            {
                currentSelectedTile = null;
            }
        }
    }


    void KeyBoardControl()
    {
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.DownArrow))
        {

            if (!turnPhase)
            {
                //check camera state
                Tile t = new Tile();

                if (Input.GetKey(KeyCode.LeftArrow))
                {
                    t = currentTile.adjacencyDict["Left"];
                }
                if (Input.GetKey(KeyCode.RightArrow))
                {
                    t = currentTile.adjacencyDict["Right"];
                }
                if (Input.GetKey(KeyCode.UpArrow))
                {
                    t = currentTile.adjacencyDict["Up"];
                }
                if (Input.GetKey(KeyCode.DownArrow))
                {
                    t = currentTile.adjacencyDict["Down"];
                }

                if (t == currentSelectedTile)
                {
                    //nothing
                }

                else
                {
                    currentSelectedTile = t;
                }
            }

            else
            {
                Arrow a = new Arrow();

                if (Input.GetKey(KeyCode.LeftArrow))
                {

                    a = arrowHolder.GetArrow("Left");
                }
                if (Input.GetKey(KeyCode.RightArrow))
                {
                    a = arrowHolder.GetArrow("Right");

                }
                if (Input.GetKey(KeyCode.UpArrow))
                {
                    a = arrowHolder.GetArrow("Up");

                }
                if (Input.GetKey(KeyCode.DownArrow))
                {
                    a = arrowHolder.GetArrow("Down");

                }

                if (a == arrowHolder.currentArrow)
                {
                    //nothing
                }

                else
                {
                    arrowHolder.ClearSelectedArrows();
                    arrowHolder.currentArrow = null;
                    arrowHolder.currentArrow = a;
                    a.selected = true;

                }

            }
        }

        if (Input.GetKey(KeyCode.Space))
        {
            if (!turnPhase)
            {
                if (currentSelectedTile)
                {
                    Debug.Log("it came here??");
                    if (currentSelectedTile.walkable)
                    {
                        MoveToTile(currentSelectedTile);
                    }
                    else if (currentSelectedTile.pushable)
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
                    if (!skipMove)
                    {
                        skipMove = !skipMove;
                    }
                    else
                    {
                        skipMove = !skipMove;
                        turnPhase = !turnPhase;
                        Debug.Log("Setting arrows active");
                        ToggleArrows(true);
                    }
                }
            }

            else
            {
                TurnTo(arrowHolder.currentArrow);
                arrowHolder.currentArrow = null;
                turnPhase = !turnPhase;
            }
        }

        if (Input.GetKey(KeyCode.Z))
        {
            if (currentSelectedTile)
            {
                currentSelectedTile = null;
            }
        }
    }



    void CheckControl()
    {
        if (Input.anyKeyDown)
        {
            KeyBoardControl();
        }


        //if (Input.GetMouseButtonUp(0))
        //{
        //    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        //    RaycastHit hit;
        //    if (Physics.Raycast(ray, out hit))
        //    {
        //        if (hit.collider.tag == "Tile")
        //        {
        //            Tile t = hit.collider.GetComponent<Tile>();

        //            if (t.selectable)
        //            {
        //                if (t == currentSelectedTile)
        //                {
        //                    if (!turnPhase)
        //                    {
        //                        if (t.walkable)
        //                        {
        //                            MoveToTile(t);
        //                        }
        //                        else if (t.pushable)
        //                        {
        //                            TryToPush();
        //                        }
        //                        else
        //                        {
        //                            //nothing
        //                        }

        //                        turnPhase = !turnPhase;
        //                    }
        //                    else
        //                    {
        //                        TurnTo(t);
        //                        turnPhase = !turnPhase;
        //                    }

        //                }
        //                else
        //                    currentSelectedTile = t;
        //            }
        //        }
        //    }
        //}
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

    void TurnTo(Arrow turnDir)
    {
        Vector3 lookAt = turnDir.gameObject.transform.position;
        lookAt.y = transform.position.y;
        transform.LookAt(lookAt);
        Debug.Log("Setting arrows active");
        ToggleArrows(false);
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
