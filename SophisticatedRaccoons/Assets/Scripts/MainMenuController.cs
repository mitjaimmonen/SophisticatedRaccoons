using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Events;

public class MainMenuController : MonoBehaviour
{
    public Text[] joinTexts = new Text[2];
    public Text[] cancelTexts = new Text[2];
    public GameObject[] aIcons = new GameObject[2];
    public GameObject[] bIcons = new GameObject[2];
    public Text countdownText;
    public string LevelToLoad = "SampleScene";
    [HideInInspector] public bool[] ready = new bool[2];



    public int amountJoined = 0;
    public int countdownTime = 3;
    int amountReady = 0;
    bool isCountdown = false;
    public bool joiningPhase = false;


    public void StartJoining()
    {
        joiningPhase = true;
        GameMaster.Instance.menuCamera.ToJoinScreen();
    }
    public void BackToTitle()
    {
        joiningPhase = false;
        GameMaster.Instance.menuCamera.ToTitleScreen();
    }


    // Use this for initialization
    void Awake()
    {
        Reset();

    }

    public void Reset()
    {
        for (int i = 0; i < ready.Length; i++)
        {
            ready[i] = false;
            cancelTexts[i].gameObject.SetActive(false);
            cancelTexts[i].color = Color.white;
            joinTexts[i].color = Color.white;
            bIcons[i].SetActive(false);
            joinTexts[i].text = "Join";
        }

        amountJoined = 0;
        amountReady = 0;
        isCountdown = false;
    }

    void Update()
    {
        if (amountReady == 2 && amountJoined == 2 && !isCountdown)
        {
            isCountdown = true;
            StartCoroutine(StartCountdown());
        }
        if (isCountdown && amountJoined != amountReady)
            isCountdown = false;
    }

    public void LoadLevel(string levelName)
    {
        SceneManager.LoadScene(levelName);
    }

    IEnumerator StartCountdown()
    {
        countdownText.gameObject.SetActive(true);
        int countdown = countdownTime;

        while (isCountdown && countdown >= 0)
        {
            countdownText.text = countdown.ToString();
            yield return new WaitForSeconds(1);
            countdown--;
            if (countdown < 0)
                isCountdown = false;
        }
        if (countdown < 0)
        {
            LoadLevel(LevelToLoad);
        }
        countdownText.gameObject.SetActive(false);
        yield break;
    }

    public bool ToggleReady(int playerNumber)
    {
        if (!ready[playerNumber])
        {
            joinTexts[playerNumber].text = "Ready!";
            joinTexts[playerNumber].color = Color.green;
            ready[playerNumber] = true;
            amountReady++;
        }
        else
        {
            joinTexts[playerNumber].color = Color.white;
            joinTexts[playerNumber].text = "Ready?";

            cancelTexts[playerNumber].gameObject.SetActive(false);
            bIcons[playerNumber].SetActive(false);

            ready[playerNumber] = false;
            amountReady--;
        }
        return ready[playerNumber];
    }
    public bool ToggleReady(int playerNumber, bool state)
    {
        if (state)
        {
            if (!ready[playerNumber]) //Add to ready only if toggle true
                amountReady++;

            joinTexts[playerNumber].color = Color.green;
            joinTexts[playerNumber].text = "Ready!";

            ready[playerNumber] = state;
        }
        else
        {
            if (ready[playerNumber]) //Remove from ready only if toggle true
                amountReady--;

            joinTexts[playerNumber].color = Color.white;
            joinTexts[playerNumber].text = "Ready?";

            ready[playerNumber] = state;
        }
        return ready[playerNumber];

    }

    public void AddPlayer(int playerNumber)
    {
        cancelTexts[playerNumber].gameObject.SetActive(true);
        bIcons[playerNumber].SetActive(true);

        ToggleReady(playerNumber, false);
        joinTexts[playerNumber].color = Color.white;
        joinTexts[playerNumber].text = "Ready?";
        amountJoined++;
    }
    public void RemovePlayer(int playerNumber)
    {
        cancelTexts[playerNumber].gameObject.SetActive(false);
        bIcons[playerNumber].SetActive(false);

        ToggleReady(playerNumber, false);
        joinTexts[playerNumber].color = Color.white;
        joinTexts[playerNumber].text = "Join";

        amountJoined--;
        Debug.Log("RemovePlayer: " + playerNumber + ", amount joined: " +amountJoined);
    }
}
