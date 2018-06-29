using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public enum GameState
{
    menu = 0,
    game = 1
}

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

    public GameState gamestate = GameState.menu;
    public MainMenuController menuControl;
    public InputHandler inputHandler;


    void Awake()
    {
        if (GameObject.FindGameObjectsWithTag("GameMaster").Length > 1)
            Destroy(this.gameObject);

        DontDestroyOnLoad(this.gameObject);

        SceneManager.sceneLoaded += OnLevelLoaded;

        Instantiate();

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
    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnLevelLoaded;

    }

    private void Update()
    {
        if (!_instance)
        {
            _instance = this;
            Debug.Log("instance was null");
        }
        if (gamestate == GameState.game)
        {
            // if treasure is out
                //end game

        }
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



    void OnLevelLoaded(Scene scene, LoadSceneMode mode)
    {
        if (SceneManager.GetActiveScene().buildIndex == 0)
            gamestate = GameState.menu;
        else
            gamestate = GameState.game;
            
        Instantiate();
    }


    void Instantiate()
    {
        _instance = this;
        inputHandler = GetComponent<InputHandler>();
        if (gamestate == GameState.menu)
        {
            GameObject menu = GameObject.Find("Main Menu");
            if (menu)
                menuControl = menu.GetComponent<MainMenuController>();

        }
        if (gamestate == GameState.game)
        {
            var holders = GameObject.FindGameObjectsWithTag("PlayerHolder");
            if (holders.Length == 2)
            {
                for(int i = 0; i < 2; i++)
                {
                    var h = holders[i].GetComponent<PlayerHolder>();
                    if (h.playerOne)
                        inputHandler.playerHolders[0] = h;
                    else if (!h.playerOne)
                        inputHandler.playerHolders[1] = h;
                }
            }
            else
                Debug.LogWarning("No two player holders found in level");
        }
    }
}
