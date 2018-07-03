using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
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

	bool gameOver = false;
	bool isPaused = false;

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
		if (gameOver)
		{
			resumeButton.SetActive(false);
			newGameButton.SetActive(true);
			menuPanel.SetActive(true);
		}
		if (menuPanel.activeSelf && !confirmationPanel.activeSelf)
		{
			if (EventSystem.current.currentSelectedGameObject == null)
				EventSystem.current.SetSelectedGameObject(resumeButton);
		}

	}

	public bool TogglePaused()
	{
		isPaused = !isPaused;
		menuPanel.SetActive (isPaused);
		return isPaused;
	}

	public void HandleInput(PlayerGamepadData gamepadData)
	{
		AxisEventData a = new AxisEventData(EventSystem.current);
		bool input = false;

		if (gamepadData.state.ThumbSticks.Left.Y < -0.2f && gamepadData.prevState.ThumbSticks.Left.Y >= -0.2f)
		{
			a.moveDir = MoveDirection.Down;
			input = true;
		}
		if (gamepadData.state.ThumbSticks.Left.Y > 0.2f && gamepadData.prevState.ThumbSticks.Left.Y <= 0.2f)
		{
			a.moveDir = MoveDirection.Up;
			input = true;
		}


		if (input)
			ExecuteEvents.Execute(EventSystem.current.currentSelectedGameObject, a ,ExecuteEvents.moveHandler);

	}
	
}
