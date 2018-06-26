using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XInputDotNetPure;

public class InputHandler : MonoBehaviour {

	// List<Player> players = new List<Player>();
	MainMenuController menuControl;
	GamepadStateHandler gamepad;
	StateHandler stateHandler; 

	void Awake()
	{
		gamepad = GetComponent<GamepadStateHandler>();
		stateHandler = GetComponent<StateHandler>();
	}
	public PlayerGamepadData HandleInput(PlayerGamepadData gamepadData)
	{
		if (stateHandler.gamestate == GameState.menu)
		{
			gamepadData = HandleMenuInputs(gamepadData);
		}
		if (stateHandler.gamestate == GameState.game)
		{
			// if (gamepadData.characterIndex == 0 && playerOne turn)
			{
				//Handle p1 input
			}
			// if (gamepadData.characterIndex == 1 && !playerOne turn)
			{
				//Handle p2 input

			}
		}

		return gamepadData;
	}

	public PlayerGamepadData HandleFixedInput(PlayerGamepadData gamepadData)
	{
		if (stateHandler.gamestate == GameState.game)
		{
			// if (gamepadData.characterIndex != -1)
				// players[gamepadData.characterIndex].HandleFixedInput(gamepadData.state, gamepadData.prevState);
		}
		return gamepadData;
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
				stateHandler.menuControl.AddPlayer(gamepadData.characterIndex);
			}
			else
			{
				stateHandler.menuControl.RemovePlayer(gamepadData.characterIndex);
				gamepadData.characterIndex = -1;
			}
		}

		if (gamepadData.active && gamepadData.prevState.Buttons.A == ButtonState.Released && gamepadData.state.Buttons.A == ButtonState.Pressed)
		{
			stateHandler.menuControl.ToggleReady(gamepadData.characterIndex);
		}

		return gamepadData;
	}
}
