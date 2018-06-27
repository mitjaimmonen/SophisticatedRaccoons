using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public enum GameState
{
    menu = 0,
    game = 1
}

public class StateHandler : MonoBehaviour
{

    #region Singleton
    private static StateHandler _instance;
    public static StateHandler Instance
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


    void Awake()
    {
        if (GameObject.FindGameObjectsWithTag("GameMaster").Length > 1)
        {
            Destroy(this.gameObject);
        }

        _instance = this;

        DontDestroyOnLoad(this.gameObject);
        SceneManager.sceneLoaded += OnLevelLoaded;

        Instantiate();

    }

    private void Update()
    {
        if (gamestate == GameState.game)
        {
            // if treasure is out
                //end game

        }
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnLevelLoaded;

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
        if (gamestate == GameState.menu)
        {
            GameObject menu = GameObject.Find("Main Menu");
            if (menu)
                menuControl = menu.GetComponent<MainMenuController>();

        }
    }
}
