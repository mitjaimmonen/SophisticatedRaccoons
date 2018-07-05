using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XInputDotNetPure;

public class PlayerMove : TacticsMove
{


    public bool turnPhase = false;

    public bool beingPushed = false;
    public bool settingPhase = true;


    public Vector3 target;
    public Vector3 direction;
    public bool skipMove = false;

    PlayerHolder daddy;

    float lastInputTime = 0;

    private void Start()
    {
        Init();

        startPos = transform.position;
        startForward = transform.forward;
        GameObject aHolder = Instantiate(arrowHolderPrefab, transform.position, Quaternion.identity);
        daddy = GetComponentInParent<PlayerHolder>();
        arrowHolder = aHolder.AddComponent<ArrowHolder>();
        arrowHolder.owner = this;

        ToggleArrows(false);
    }

    private void Update()
    {
        if (active)
        {
            if (!GameMaster.Instance.entryMode)
            {
                //if selected & not moving
                if (!moving)
                {
                    FindSelectableTiles();
                    if (currentSelectedTile)
                    {
                        currentSelectedTile.target = true;

                    }

                    if (!turnPhase)
                    {
                        GameMaster.Instance.MovePhaseInstructions(currentSelectedTile, inBoard, currentTile.isCorner);

                        if (currentSelectedTile && currentSelectedTile.thingOnTopOfIt)
                        {
                            GameMaster.Instance.PushInstructions(CouldPush());
                        }
                        else
                        {
                            GameMaster.Instance.MovePhaseInstructions(currentSelectedTile, inBoard, currentTile.isCorner);
                        }


                    }
                    else
                    {
                        GameMaster.Instance.TurnPhaseInstructions(arrowHolder.currentArrow);
                        GameMaster.Instance.ToggleExitBoard(false);
                    }
                }
                else if (!beingPushed)
                {
                    GameMaster.Instance.TurnOffInstructions();
                    Move();
                }
            }
        }

        if (beingPushed)
        {
            GetPushed();
        }

    }

    public void HandleGamepadInput(PlayerGamepadData gamepadData)
    {
        if (!active || moving)
            return;

        var thumbstick = gamepadData.state.ThumbSticks;
        if (!GameMaster.Instance.entryMode)
        {
            if (thumbstick.Left.X > 0.2f || thumbstick.Left.X < -0.2f ||
                thumbstick.Left.Y > 0.2f || thumbstick.Left.Y < -0.2f)
            {
                if (!turnPhase)
                {
                    //check camera state
                    Tile t = new Tile();

                    if (GameMaster.Instance.playerIndex == 0)
                    {
                        if (thumbstick.Left.X < -0.2f)
                        {
                            if (currentTile.adjacencyDict.ContainsKey("Left"))
                                t = currentTile.adjacencyDict["Left"];
                        }
                        if (thumbstick.Left.X > 0.2f)
                        {
                            if (currentTile.adjacencyDict.ContainsKey("Right"))
                                t = currentTile.adjacencyDict["Right"];
                        }
                        if (thumbstick.Left.Y > 0.2f)
                        {
                            if (currentTile.adjacencyDict.ContainsKey("Up"))
                                t = currentTile.adjacencyDict["Up"];
                        }
                        if (thumbstick.Left.Y < -0.2f)
                        {
                            if (currentTile.adjacencyDict.ContainsKey("Down"))
                                t = currentTile.adjacencyDict["Down"];
                        }

                    }
                    else
                    {
                        Debug.Log("got here!!");
                        if (thumbstick.Left.X < -0.2f)
                        {
                            if (currentTile.adjacencyDict.ContainsKey("Right"))
                                t = currentTile.adjacencyDict["Right"];
                        }
                        if (thumbstick.Left.X > 0.2f)
                        {
                            if (currentTile.adjacencyDict.ContainsKey("Left"))
                                t = currentTile.adjacencyDict["Left"];
                        }
                        if (thumbstick.Left.Y > 0.2f)
                        {
                            if (currentTile.adjacencyDict.ContainsKey("Down"))
                                t = currentTile.adjacencyDict["Down"];
                        }
                        if (thumbstick.Left.Y < -0.2f)
                        {
                            if (currentTile.adjacencyDict.ContainsKey("Up"))
                                t = currentTile.adjacencyDict["Up"];
                        }

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

                    if (GameMaster.Instance.playerIndex == 0)
                    {
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

                    }
                    else
                    {
                        if (thumbstick.Left.X < -0.2f)
                        {
                            a = arrowHolder.GetArrow("Right");
                        }
                        if (thumbstick.Left.X > 0.2f)
                        {
                            a = arrowHolder.GetArrow("Left");
                        }
                        if (thumbstick.Left.Y > 0.2f)
                        {
                            a = arrowHolder.GetArrow("Down");
                        }
                        if (thumbstick.Left.Y < -0.2f)
                        {
                            a = arrowHolder.GetArrow("Up");
                        }
                    }

                    if (a == arrowHolder.currentArrow)
                    {
                        //nothing
                    }

                    else
                    {
                        arrowHolder.currentArrow = null;
                        arrowHolder.currentArrow = a;
                        arrowHolder.ClearSelectedArrows();
                        arrowHolder.currentArrow.selected = true;
                    }
                }
            }

            if (gamepadData.state.Buttons.A == ButtonState.Pressed && gamepadData.prevState.Buttons.A == ButtonState.Released)
            {
                if (!turnPhase)
                {
                    if (currentSelectedTile)
                    {
                        if (currentSelectedTile.walkable)
                        {
                            MoveToTile(currentSelectedTile);
                            turnPhase = true;


                        }
                        else if (currentSelectedTile.pushable)
                        {
                            TryToPush();

                        }
                        else
                        {
                            //nothing
                        }

                    }
                }

                else
                {
                    if (arrowHolder.currentArrow)
                    {
                        TurnTo(arrowHolder.currentArrow);
                        if (!inBoard)
                        {
                            inBoard = true;
                        }
                        Deactivate(true);

                    }
                }
                Debug.Log("Pressed A");
                lastInputTime = Time.time;
            }

            if (gamepadData.state.Buttons.Y == ButtonState.Pressed && gamepadData.prevState.Buttons.Y == ButtonState.Released)
            {
                if (inBoard)
                {
                    turnPhase = true;
                    ToggleArrows(true);
                }
            }

            if (gamepadData.state.Buttons.X == ButtonState.Pressed && gamepadData.prevState.Buttons.X == ButtonState.Released)
            {
                if (inBoard && canCancel)
                {
                    GetCurrentTile();
                    if (currentTile.isCorner)
                    {
                        GetOutOfBoard();
                        Deactivate(true);
                    }
                }

            }
            if (gamepadData.state.Buttons.B == ButtonState.Pressed)
            {
                if (!turnPhase)
                {

                    if (currentSelectedTile)
                    {
                        currentSelectedTile = null;
                    }
                    else
                    {
                        if (canCancel)
                        {
                            GameMaster.Instance.EntryModeToggle(true);
                            if (!inBoard)
                            {
                                transform.position = startPos;
                                transform.forward = startForward;
                            }
                            Deactivate(false);
                        }
                    }

                }

                else
                {
                    if (arrowHolder.currentArrow)
                    {
                        arrowHolder.currentArrow.selected = false;
                        arrowHolder.currentArrow = null;
                    }
                }
            }
        }

        else // Is entry mode
        {
            //check camera state
            if (GameMaster.Instance.playerIndex == 0)
            {
                if (gamepadData.state.ThumbSticks.Left.X < -0.2f && gamepadData.prevState.ThumbSticks.Left.X >= -0.2f)
                {
                    if (currentTile.spawnAdjacencyDict.ContainsKey("Left"))
                    {
                        Debug.Log("Moving from " + currentTile.gameObject + " to " + currentTile.spawnAdjacencyDict["Left"].gameObject);
                        currentTile.current = false;
                        currentTile = currentTile.spawnAdjacencyDict["Left"];
                        currentTile.current = true;
                    }
                }
                if (gamepadData.state.ThumbSticks.Left.X > 0.2f && gamepadData.prevState.ThumbSticks.Left.X <= 0.2f)
                {
                    if (currentTile.spawnAdjacencyDict.ContainsKey("Right"))
                    {
                        Debug.Log("Moving from " + currentTile.gameObject + " to " + currentTile.spawnAdjacencyDict["Right"].gameObject);
                        currentTile.current = false;
                        currentTile = currentTile.spawnAdjacencyDict["Right"];
                        currentTile.current = true;
                    }
                }
                if (gamepadData.state.ThumbSticks.Left.Y > 0.2f && gamepadData.prevState.ThumbSticks.Left.Y <= 0.2f)
                {
                    if (currentTile.spawnAdjacencyDict.ContainsKey("Up"))
                    {
                        Debug.Log("Moving from " + currentTile.gameObject + " to " + currentTile.spawnAdjacencyDict["Up"].gameObject);
                        currentTile.current = false;
                        currentTile = currentTile.spawnAdjacencyDict["Up"];
                        currentTile.current = true;
                    }
                }
                if (gamepadData.state.ThumbSticks.Left.Y < -0.2f && gamepadData.prevState.ThumbSticks.Left.Y >= -0.2f)
                {
                    if (currentTile.spawnAdjacencyDict.ContainsKey("Down"))
                    {
                        Debug.Log("Moving from " + currentTile.gameObject + " to " + currentTile.spawnAdjacencyDict["Down"].gameObject);
                        currentTile.current = false;
                        currentTile = currentTile.spawnAdjacencyDict["Down"];
                        currentTile.current = true;
                    }
                }
            }

            else
            {
                if (gamepadData.state.ThumbSticks.Left.X < -0.2f && gamepadData.prevState.ThumbSticks.Left.X >= -0.2f)
                {
                    if (currentTile.spawnAdjacencyDict.ContainsKey("Right"))
                    {
                        currentTile.current = false;
                        currentTile = currentTile.spawnAdjacencyDict["Right"];
                        currentTile.current = true;
                    }
                }
                if (gamepadData.state.ThumbSticks.Left.X > 0.2f && gamepadData.prevState.ThumbSticks.Left.X <= 0.2f)
                {
                    if (currentTile.spawnAdjacencyDict.ContainsKey("Left"))
                    {
                        currentTile.current = false;
                        currentTile = currentTile.spawnAdjacencyDict["Left"];
                        currentTile.current = true;
                    }
                }
                if (gamepadData.state.ThumbSticks.Left.Y > 0.2f && gamepadData.prevState.ThumbSticks.Left.Y <= 0.2f)
                {
                    if (currentTile.spawnAdjacencyDict.ContainsKey("Down"))
                    {
                        currentTile.current = false;
                        currentTile = currentTile.spawnAdjacencyDict["Down"];
                        currentTile.current = true;
                    }
                }
                if (gamepadData.state.ThumbSticks.Left.Y < -0.2f && gamepadData.prevState.ThumbSticks.Left.Y >= -0.2f)
                {
                    if (currentTile.spawnAdjacencyDict.ContainsKey("Up"))
                    {
                        currentTile.current = false;
                        currentTile = currentTile.spawnAdjacencyDict["Up"];
                        currentTile.current = true;
                    }
                }
            }


            if (gamepadData.state.Buttons.A == ButtonState.Pressed && gamepadData.prevState.Buttons.A == ButtonState.Released && lastInputTime + 0.1f < Time.time)
            {
                //Doesnt work??
                // MoveToTile(currentTile);

                //This hack works :))))
                transform.position = new Vector3(currentTile.transform.position.x, currentTile.transform.position.y + 1f, currentTile.transform.position.z);
                Debug.Log("Entry mode finished");
                GameMaster.Instance.EntryModeToggle(false);
                currentTile.FindNeighbors();
                Vector3 temp = currentTile.adjacencyList[0].transform.position;
                temp.y = transform.position.y;
                transform.LookAt(temp);
            }

            if (gamepadData.state.Buttons.B == ButtonState.Pressed)
            {
                if (active && canCancel)
                {
                    Deactivate(false);
                    GameMaster.Instance.EntryModeToggle(true);
                }
            }
        }
    }

    /*
    void KeyBoardControl()
    {
        if (!GameMaster.Instance.entryMode)
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
                            // ToggleArrows(true);
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

        else
        {
            //check camera state


            if (Input.GetKey(KeyCode.LeftArrow))
            {
                if (currentTile.spawnAdjacencyDict.ContainsKey("Left"))
                {
                    currentTile.current = false;
                    currentTile = currentTile.spawnAdjacencyDict["Left"];
                    currentTile.current = true;
                }
            }
            if (Input.GetKey(KeyCode.RightArrow))
            {
                if (currentTile.spawnAdjacencyDict.ContainsKey("Right"))
                {
                    currentTile.current = false;
                    currentTile = currentTile.spawnAdjacencyDict["Right"];
                    currentTile.current = true;
                }
            }
            if (Input.GetKey(KeyCode.UpArrow))
            {
                if (currentTile.spawnAdjacencyDict.ContainsKey("Up"))
                {
                    currentTile.current = false;
                    currentTile = currentTile.spawnAdjacencyDict["Up"];
                    currentTile.current = true;
                }
            }
            if (Input.GetKey(KeyCode.DownArrow))
            {
                if (currentTile.spawnAdjacencyDict.ContainsKey("Down"))
                {

                    currentTile.current = false;
                    currentTile = currentTile.spawnAdjacencyDict["Down"];
                    currentTile.current = true;
                }
            }

            else
            {


            }
        }
    }
    */

    void CheckControl()
    {
        //if (Input.anyKeyDown)
        //{
        //    KeyBoardControl();
        //}


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

    bool CouldPush()
    {
        return true;
    }

    void TryToPush()
    {
        List<Tile> pushableTiles = new List<Tile>();
        Tile thisTile = currentTile;
        bool done = false;
        GetCurrentTile();
        Tile temp = thisTile.ReturnTile(transform.forward);

        if (temp == currentSelectedTile)
        {
            while (!done)
            {
                temp = thisTile.ReturnTile(transform.forward);

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
                        GameObject obstacle = t.thingOnTopOfIt;
                        if (obstacle.GetComponent<Booty>())
                        {
                            obstacle.GetComponent<Booty>().BePushed(transform.forward);
                        }
                        else
                        {
                            obstacle.GetComponent<PlayerMove>().BePushed(transform.forward);
                        }
                    }
                    hasPushed = true;
                    MoveToTile(pushableTiles[0]);
                }
                else
                {
                    GameMaster.Instance.FailPush();
                }
            }
        }


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
        StartCoroutine(SlerpTurn(lookAt));
        ToggleArrows(false);
        turnPhase = false;

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
            CheckBoardLimit();
        }
    }

    void CheckBoardLimit()
    {
        GetCurrentTile();
        if (currentTile.isSpawn)
        {
            GetOutOfBoard();
        }
    }

    void GetOutOfBoard()
    {
        inBoard = false;
        transform.position = startPos;
        transform.forward = startForward;
        currentTile = null;

    }

    public void Activate()
    {
        active = true;
        turnPhase = false;
        hasPushed = false;
        ToggleArrows(false);
        canCancel = true;
        if (!inBoard && GameMaster.Instance.entryMode)
        {
            FindEntryTiles();
        }
        else
        {
            GameMaster.Instance.EntryModeToggle(false);
        }
    }

}
