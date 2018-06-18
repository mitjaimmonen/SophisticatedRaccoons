using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : TacticsMove
{

    public bool active = true;
    

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
        else
        {
            Move();
        }
      


        //else
        //nothing
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
                            MoveToTile(t);
                        else
                            currentSelectedTile = t;                   

                    }
                    //else if (t == currentSelectedTile)
                    //{
                    //   
                    //}
                }
            }
        }
    }


}
