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
	int pausedController = 0;

	void Awake()
	{
		gamepad = GetComponent<GamepadStateHandler>();
		gameMaster = GetComponent<GameMaster>();
	}

	public void Reset()
	{
		playerHolders = new PlayerHolder[2];
		inputDebug = false;
		playerOneTurn = true;
		pausedController = 0;
	}
	public PlayerGamepadData HandleInput(PlayerGamepadData gamepadData)
	{
		if (gameMaster.gamestate == GameState.menu)
		{
			gamepadData = HandleMenuInputs(gamepadData);
		}
		if (gameMaster.gamestate == GameState.game)
		{
			if (gamepadData.characterIndex < 0 || gamepadData.characterIndex > 1)
				return gamepadData;

			if ((gamepadData.characterIndex == 0 && playerOneTurn) || (gamepadData.characterIndex == 1 && !playerOneTurn) || (inputDebug && gamepadData.gamepadPlayerIndex == PlayerIndex.One))
			{
				if (!gameMaster.isPaused)
				{
					if (!inputDebug)
					{
						playerHolders[gamepadData.characterIndex].HandleInput(gamepadData);
					}
					else
						playerHolders[0].HandleInput(gamepadData);

				}

			}
			if (!gameMaster.isPaused || (gamepadData.characterIndex == pausedController && gameMaster.isPaused))
			{
				//Allow any control when not paused. Allow only one paused controller if paused
				if (gamepadData.state.Buttons.Start == ButtonState.Pressed && gamepadData.prevState.Buttons.Start == ButtonState.Released)
				{
					gameMaster.isPaused = gameMaster.pauseMenu.TogglePaused(gamepadData.characterIndex);
					pausedController = gamepadData.characterIndex;
				}
				if (gameMaster.isPaused)
					gameMaster.pauseMenu.HandleInput(gamepadData);
				// PauseInputs(gamepadData);
			}

		}

		return gamepadData;
	}
	PlayerGamepadData HandleMenuInputs(PlayerGamepadData gamepadData)
	{
		if (!gameMaster.menuControl.joiningPhase)
		{
			if (gamepadData.prevState.Buttons.A == ButtonState.Released && gamepadData.state.Buttons.A == ButtonState.Pressed)
			{
				gameMaster.menuControl.StartJoining();
			}
		}
		else
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

			if (gamepadData.prevState.Buttons.B == ButtonState.Released && gamepadData.state.Buttons.B == ButtonState.Pressed)
			{
				gameMaster.menuControl.BackToTitle();
				gameMaster.Reset();
			}

		}

		return gamepadData;
	}
}
