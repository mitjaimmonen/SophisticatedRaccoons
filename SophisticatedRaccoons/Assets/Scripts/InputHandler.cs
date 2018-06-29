using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XInputDotNetPure;

public class InputHandler : MonoBehaviour {

	// List<Player> players = new List<Player>();
	MainMenuController menuControl;
	GamepadStateHandler gamepad;
	GameMaster gameMaster; 

	public PlayerHolder[] playerHolders = new PlayerHolder[2];

	public bool inputDebug = false;
	public bool playerOneTurn = true;
	bool isPaused = false;
	int pausedController = 0;

	void Awake()
	{
		gamepad = GetComponent<GamepadStateHandler>();
		gameMaster = GetComponent<GameMaster>();
	}
	public PlayerGamepadData HandleInput(PlayerGamepadData gamepadData)
	{
		if (gameMaster.gamestate == GameState.menu)
		{
			gamepadData = HandleMenuInputs(gamepadData);
		}
		if (gameMaster.gamestate == GameState.game)
		{
			if ((gamepadData.characterIndex == 0 && playerOneTurn) || (gamepadData.characterIndex == 1 && !playerOneTurn) || (inputDebug && gamepadData.gamepadPlayerIndex == PlayerIndex.One))
			{
				//Only checks current player's inputs
				if (!inputDebug)
					playerHolders[gamepadData.characterIndex].HandleInput(gamepadData);
				else
					playerHolders[0].HandleInput(gamepadData);

			}
			if (!isPaused || (gamepadData.characterIndex == pausedController && isPaused))
			{
				//Allow any control when not paused. Allow only one paused controller if paused
				PauseInputs(gamepadData);
			}

		}

		return gamepadData;
	}


	void PlayerInputs(PlayerGamepadData gamepadData)
	{

		if (gamepadData.prevState.Buttons.Y == ButtonState.Released && gamepadData.state.Buttons.Y == ButtonState.Pressed)
		{
			//Y pressed this frame
		}
		if (gamepadData.prevState.Buttons.A == ButtonState.Released && gamepadData.state.Buttons.A == ButtonState.Pressed)
		{
			Debug.Log("pressed A, characterindex: " + gamepadData.characterIndex + ", playerone: " + playerOneTurn);
			//A pressed this frame
		}


		//DIRECTIONS
		//**********
		if (gamepadData.prevState.ThumbSticks.Left.X < 0.2f && gamepadData.state.ThumbSticks.Left.X >= 0.2f)
		{
			//Right pressed this frame
		}
		if (gamepadData.prevState.ThumbSticks.Left.X > -0.2f && gamepadData.state.ThumbSticks.Left.X <= -0.2f)
		{
			//Left pressed this frame
		}
		if (gamepadData.prevState.ThumbSticks.Left.Y < 0.2f && gamepadData.state.ThumbSticks.Left.Y >= 0.2f)
		{
			//Up pressed this frame
		}
		if (gamepadData.prevState.ThumbSticks.Left.Y > -0.2f && gamepadData.state.ThumbSticks.Left.Y <= -0.2f)
		{
			//Down pressed this frame
		}
		//**********
		//END OF DIRECTIONS
	}

	void PauseInputs (PlayerGamepadData gamepadData)
	{
		if (gamepadData.state.Buttons.Start == ButtonState.Pressed && gamepadData.prevState.Buttons.Start == ButtonState.Released)
		{
			isPaused = !isPaused;
			pausedController = gamepadData.characterIndex;
		}
		if (isPaused)
		{
			//rest of inputs
		}
	}
	PlayerGamepadData HandleMenuInputs(PlayerGamepadData gamepadData)
	{
		//Handle player (gamepad) activation - check if Y-button was pressed in this gamepad in this frame
		if (gamepadData.prevState.Buttons.Y == ButtonState.Released && gamepadData.state.Buttons.Y == ButtonState.Pressed)
		{
			//Invert active bool

			gamepadData.active = !gamepadData.active;

			if (gamepadData.active)
			{
				bool firstJoined = false, secondJoined = false;
				for(int j = 0; j < gamepad.playerGamepadData.Length; j++)
				{
					if (gamepad.playerGamepadData[j].characterIndex == 0)
						firstJoined = true;
					if (gamepad.playerGamepadData[j].characterIndex == 1)
						secondJoined = true;

				}
				
				if (firstJoined && secondJoined)
				{
					gamepadData.active = false;
					return gamepadData;
				}

				gamepadData.characterIndex = firstJoined ? 1 : 0;			
				gameMaster.menuControl.AddPlayer(gamepadData.characterIndex);
			}
			else
			{
				gameMaster.menuControl.RemovePlayer(gamepadData.characterIndex);
				gamepadData.characterIndex = -1;
			}
		}

		if (gamepadData.active && gamepadData.prevState.Buttons.A == ButtonState.Released && gamepadData.state.Buttons.A == ButtonState.Pressed)
		{
			gameMaster.menuControl.ToggleReady(gamepadData.characterIndex);
		}

		return gamepadData;
	}
}
