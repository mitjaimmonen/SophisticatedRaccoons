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
    TacticsCamera tacticsCamera;
    public MainMenuController menuControl;
    public GamepadStateHandler gamepadStateHandler;
    public InputHandler inputHandler;
    public PlayerHolder[] players;
    public int playerIndex;
    public PauseMenu pauseMenu;
    public HudHandler hudHandler;
    [FMODUnity.EventRef] public string startSound;
    bool startSoundPlayed = false;
    bool isGameOver = false;
    public bool isPaused = false;

    public bool IsGameOver
    {
        get {return isGameOver;}
        set 
        {
            if (isGameOver != value)
            {
                isGameOver = value;
                GameOver();
            }
        }
    }

    void GameOver()
    {
        // TODO: Hud text to show who won
        hudHandler.GameOver(playerIndex);
        pauseMenu.GameOver();
    }

    void Awake()
    {
        if (GameObject.FindGameObjectsWithTag("GameMaster").Length > 1)
            Destroy(this.gameObject);

        DontDestroyOnLoad(this.gameObject);

        SceneManager.sceneLoaded += OnLevelLoaded;

        Instantiate();

        playerIndex = 0;


    }
    private void Start()
    {

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
            //nothing
        }
    }

    public void EntryModeToggle(bool state)
    {
        entryMode = state;
        players[playerIndex].selectPhase = state;
        players[playerIndex].playerSelected = !state;
    }

    public void EndTurn()
    {
        StartCoroutine(WaitForEnd());
        //switch player
    }

    IEnumerator WaitForEnd()
    {
        yield return new WaitForSeconds(0.5f);

        if (!isGameOver)
        {

            players[playerIndex].isOwnTurn = false;
            players[playerIndex].EndTurn();

            Debug.Log("turn ended, is now player " + (playerIndex + 1));

            if (playerIndex == 1)
            {
                playerIndex = 0;
                inputHandler.playerOneTurn = true;
                tacticsCamera.ChangePlayer(true);
            }
            else
            {
                playerIndex = 1;
                inputHandler.playerOneTurn = false;
                tacticsCamera.ChangePlayer(false);
            }

            StartTurn();
        }
        else
        {
            EndGame();
        }
    }

    private void StartTurn()
    {
        if (!isGameOver)
        {
            players[playerIndex].isOwnTurn = true;
            entryMode = true;
            players[playerIndex].StartTurn();
            hudHandler.SetPlayerTurn("Player" + (playerIndex+1));
        }
    }

    void EndGame()
    {
        Debug.Log("The Game may not continue as it is over!!!!");
        hudHandler.GameOver(playerIndex);
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
        gamepadStateHandler = GetComponent<GamepadStateHandler>();

        if (gamestate == GameState.menu)
        {
            GameObject menu = GameObject.Find("Main Menu");
            if (menu)
                menuControl = menu.GetComponent<MainMenuController>();
            Reset();

        }
        if (gamestate == GameState.game)
        {
            tacticsCamera = Camera.main.GetComponentInParent<TacticsCamera>();
            var holders = GameObject.FindGameObjectsWithTag("PlayerHolder");
            if (holders.Length == 2)
            {
                for (int i = 0; i < 2; i++)
                {
                    var h = holders[i].GetComponent<PlayerHolder>();
                    if (h.playerOne)
                    {
                        inputHandler.playerHolders[0] = h;
                        players[0] = h;
                    }
                    else if (!h.playerOne)
                    {
                        inputHandler.playerHolders[1] = h;
                        players[1] = h;
                    }
                }
            }
            else
                Debug.LogWarning("No two player holders found in level");

            if (!startSoundPlayed)
            {
                startSoundPlayed = true;
                Debug.Log("Play sound");
                FMODUnity.RuntimeManager.PlayOneShot(startSound, Camera.main.transform.position);

            }
            if (!pauseMenu)
                pauseMenu = GameObject.Find("Menu").GetComponent<PauseMenu>();
            if (!hudHandler)
                hudHandler = GameObject.Find("Hud").GetComponent<HudHandler>();

            StartTurn();
        }
    }

    void Reset()
    {
        startSoundPlayed = false;
        isGameOver = false;
        isPaused = false;

        inputHandler.Reset();
        gamepadStateHandler.Reset();
        menuControl.Reset();
    }
}
