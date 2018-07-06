using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using XInputDotNetPure;

public class PauseMenu : MonoBehaviour {


	[Header("Menu Panel")]
	public GameObject menuPanel;
	public Text playerNumberText;
	public GameObject resumeButton;
	public GameObject newGameButton;
	public GameObject menuButton;
	public GameObject QuitButton;

	[Header("Confirm Panel")]
	public GameObject confirmationPanel;
	public Text confirmationText;

	[Header("States for debugging")]
	public bool isGameOver = false;
	public bool isPaused = false;
	public bool confirmationPending = false;
	public bool confirmationPositive = false;


	int lastPlayerIndex = 0;

	// Use this for initialization
	void Start () {
		menuPanel.SetActive(false);
		confirmationPanel.SetActive(false);
		newGameButton.SetActive(false);
		GameMaster.Instance.pauseMenu = this;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (menuPanel.activeSelf && !confirmationPanel.activeSelf)
		{
			if (EventSystem.current.currentSelectedGameObject == null)
				EventSystem.current.SetSelectedGameObject(resumeButton);
		}

	}

	public bool TogglePaused(int playerIndex)
	{
		isPaused = !isPaused;
		menuPanel.SetActive (isPaused);
        if (!GameMaster.Instance.IsGameOver)
            playerNumberText.text = "Player" + (playerIndex+1);
		lastPlayerIndex = playerIndex;
		EventSystem.current.SetSelectedGameObject(null);
		if (isPaused)
			EventSystem.current.SetSelectedGameObject(resumeButton);

		GameMaster.Instance.isPaused = isPaused;
			
		return isPaused;
	}

	public bool SetPaused(bool state, int playerIndex)
	{
		isPaused = state;
		menuPanel.SetActive(state);
		lastPlayerIndex = playerIndex;
        if (!GameMaster.Instance.IsGameOver)
		    playerNumberText.text = "Player" + (playerIndex+1);

		EventSystem.current.SetSelectedGameObject(null);
		if (state)
		{
			EventSystem.current.SetSelectedGameObject(isGameOver ? newGameButton : resumeButton);
		}

		GameMaster.Instance.isPaused = state;
			
		return isPaused;
	}

	public void HandleInput(PlayerGamepadData gamepadData)
	{
		AxisEventData a = new AxisEventData(EventSystem.current);
		bool moveInput = false;
		bool submitInput = false;

		if (!confirmationPending)
		{
			if (gamepadData.state.ThumbSticks.Left.Y < -0.2f && gamepadData.prevState.ThumbSticks.Left.Y >= -0.2f)
			{
				a.moveDir = MoveDirection.Down;
				moveInput = true;
			}
			if (gamepadData.state.ThumbSticks.Left.Y > 0.2f && gamepadData.prevState.ThumbSticks.Left.Y <= 0.2f)
			{
				a.moveDir = MoveDirection.Up;
				moveInput = true;
			}
			if (moveInput)
				ExecuteEvents.Execute(EventSystem.current.currentSelectedGameObject, a ,ExecuteEvents.moveHandler);


			if (gamepadData.state.Buttons.A == ButtonState.Pressed && gamepadData.prevState.Buttons.A == ButtonState.Released)
			{
				submitInput = true;
			}


			if (submitInput)
				ExecuteEvents.Execute(EventSystem.current.currentSelectedGameObject, a ,ExecuteEvents.submitHandler);
		}
		else
		{
			if (gamepadData.state.Buttons.A == ButtonState.Pressed && gamepadData.prevState.Buttons.A == ButtonState.Released)
			{
				confirmationPending = false;
				confirmationPositive = true;
			}
			if (gamepadData.state.Buttons.B == ButtonState.Pressed && gamepadData.prevState.Buttons.B == ButtonState.Released)
			{
				confirmationPending = false;
				confirmationPositive = false;
			}
		}



	}


	public void ResumeGame()
	{
		SetPaused(false, lastPlayerIndex);
	}


	public void ToMainMenu(string sceneName)
	{
		if (!isGameOver && !confirmationPending)
		{
			confirmationPending = true;
			StartCoroutine(ConfirmToScene(sceneName));
		}
		else
			LoadScene(sceneName);
	}

	IEnumerator ConfirmToScene(string sceneName)
	{
		confirmationText.text = "Surrender?";
		confirmationPanel.SetActive(true);
		while (confirmationPending)
			yield return null;
		
		if (confirmationPositive)
			EndGame(sceneName);
		else
			confirmationPanel.SetActive(false);
		
		yield break;
	}

	void LoadScene(string sceneName)
	{
		SceneManager.LoadScene(sceneName);
	}

	void EndGame(string sceneName) // Calls when surrender
	{
		isGameOver = true;
		GameMaster.Instance.playerIndex = lastPlayerIndex == 0 ? 1 : 0;
		GameMaster.Instance.IsGameOver = true;

		newGameButton.SetActive(true);
		resumeButton.SetActive(false);
		confirmationPanel.SetActive(false);
		menuPanel.SetActive(false);
		
		StartCoroutine(LoadSceneWithDelay(sceneName, 2f));
	}
	public void GameOver() // GameMaster Calls when game finishes
	{
		if (!isGameOver)
		{
			isGameOver = true;
			newGameButton.SetActive(true);
			resumeButton.SetActive(false);
            playerNumberText.text = "End Menu";
			StartCoroutine(SetPauseWithDelay(true, 2f));
		}
	}

	IEnumerator LoadSceneWithDelay(string sceneName, float delay)
	{
		yield return new WaitForSeconds(delay);
		SceneManager.LoadScene(sceneName);
	}
	IEnumerator SetPauseWithDelay(bool state, float delay)
	{
		yield return new WaitForSeconds(delay);
		SetPaused(state, lastPlayerIndex);
	}

	public void QuitGame() //Call from quit button
	{
		confirmationPending = true;
		StartCoroutine(ConfirmQuit());
		Application.Quit();
	}
	IEnumerator ConfirmQuit()
	{
		confirmationText.text = "Quit to desktop?";
		confirmationPanel.SetActive(true);
		while (confirmationPending)
			yield return null;
		
		if (confirmationPositive)
			Application.Quit();
		else
			confirmationPanel.SetActive(false);
		
		yield break;
	}
	
}
