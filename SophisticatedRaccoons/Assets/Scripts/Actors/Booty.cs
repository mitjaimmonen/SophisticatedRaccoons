using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Booty : MonoBehaviour
{
    public bool beingPushed = false;
    public float moveSpeed = 2;
    public Vector3 target;
    public Vector3 direction;
    public GameObject pusher;

    private void Update()
    {
        if (beingPushed)
        {
            Move();
        }
    }

    public void BePushed(Vector3 _direction)
    {
        GameObject _pusher = GetTargetTile(gameObject).GetComponent<Tile>().ReturnTile(-_direction).thingOnTopOfIt;
        if (_pusher.GetComponent<PlayerMove>())
        {
            pusher = _pusher;
        }
        else if (_pusher.GetComponent<Booty>())
        {
            pusher = _pusher.GetComponent<Booty>().pusher;
        }

        direction = _direction;
        target = transform.position + direction;
        beingPushed = true;
    }

    public void Move()
    {
        if (Vector3.Distance(transform.position, target) >= 0.05f)
        {          
            transform.position += (moveSpeed * direction) * Time.deltaTime;
        }
        else
        {
            transform.position = target;
            beingPushed = false;
            CheckBoardLimits();
        }
    }

     void CheckBoardLimits()
    {
        //get current tile
        if (GetTargetTile(gameObject).isSpawn)
        {
            PlayerHolder winnerHolder = pusher.GetComponentInParent<PlayerHolder>();
            int winner = 0;
            if (winnerHolder.playerOne)
            {
                winner = 0;
            }
            else
            {
                winner = 1;
            }
            GameMaster.Instance.winnerIndex = winner;
            GameMaster.Instance.IsGameOver = true;
            Debug.Log("Game Ended! Player " +(winner + 1) + " won!");
        }
        //is spawn tile, game ends
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
}
