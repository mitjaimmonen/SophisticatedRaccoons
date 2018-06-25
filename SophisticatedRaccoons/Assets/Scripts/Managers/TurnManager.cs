using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour {

    static Dictionary<string, List<TacticsMove>> units = new Dictionary<string, List<TacticsMove>>();
    static Queue<string> turnKey = new Queue<string>();
    static TacticsMove[] turnTeam;

    static void InitTeamTurnQueue()
    {
        List<TacticsMove> teamList = units[turnKey.Peek()];

        turnTeam = new TacticsMove[teamList.Count];
        int i = 0;

        foreach (TacticsMove unit in teamList)
        {
            turnTeam[i] = unit;
            i++;
        }

        //StartTurn();
    }

    private void Update()
    {
        if (turnTeam == null)
        {
            InitTeamTurnQueue();
        }
    }

    static void StartTurn()
    {
        if (turnTeam[0]!= null)
        {
           
        }
    }

}
