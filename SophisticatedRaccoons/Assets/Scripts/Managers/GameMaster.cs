using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMaster : MonoBehaviour
{

    //List<Tile> entryTiles = new List<Tile>();
    public bool entryMode = true;
    private bool displayingSelected;

    #region Singleton
    private static GameMaster _instance;
    public static GameMaster Instance
    {
        get
        {
            if (_instance == null)
            {
                return null;
            }

            return _instance;
        }
    }
    #endregion

    void Awake()
    {
        if (GameObject.FindGameObjectsWithTag("GameMaster").Length > 1)
        {
            Destroy(this.gameObject);
        }

        _instance = this;

        DontDestroyOnLoad(this.gameObject);

    }

    private void Start()
    {
        //List<Tile> temp = new List<Tile>();

        //foreach(GameObject T in GameObject.FindGameObjectsWithTag("Tile"))
        //{
        //    Tile t = T.GetComponent<Tile>();
        //    if (t.isSpawn)
        //    {
        //        entryTiles.Add(t);
        //    }
        //}
    }
    private void LateUpdate()
    {
        if (entryMode)
        {
            //foreach (Tile entry in entryTiles)
            //{
            //    Debug.Log("got here");
            //    entry.selectable = entryMode;
            //}
        }
    }

    //public List<Tile> GetEntryTiles()
    //{
        //return entryTiles;
    //}

    public void EntryModeToggle(bool state)
    {
        entryMode = state;
    }
}
