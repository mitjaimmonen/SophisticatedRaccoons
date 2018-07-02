using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XInputDotNetPure;
public class PlayerHolder : MonoBehaviour {

	public bool playerOne;
	public Material playerMaterial;
	public GameObject selectionIndicator;
	List<PlayerMove> characters = new List<PlayerMove>();

	public bool isOwnTurn;
	bool playerSelected = false;
	bool showSelection = true;
    public bool selectPhase = true;
	int characterIndex = 0;
	float lastInputTime = 0;

	void Awake()
	{
		if (isOwnTurn)
			StartTurn();

		PlayerMove[] playerMove = GetComponentsInChildren<PlayerMove>();
		foreach(var p in playerMove)
		{
			if (!characters.Contains(p))
			{
				var mat = p.GetComponent<Renderer>();
				mat.material = playerMaterial;
				characters.Add(p);

			}
		}
	}

	void Update()
	{
		if (isOwnTurn)
		{
			if (selectionIndicator)
			{
				Vector3 pos = characters[characterIndex].transform.position;
				pos.y += 1.5f;
				selectionIndicator.transform.position = pos;
				selectionIndicator.transform.Rotate(new Vector3(0,1f,0));
			}
                       
		}
	}

	public void StartTurn()
	{
		playerSelected = false;
		selectionIndicator.SetActive(true);
	}

    public void DoTurn()
    {
        //select phase
        //move/push phase
        //turn phase
    }

	public void EndTurn()
	{
		selectionIndicator.SetActive(false);
        //do end things
	}

	public void HandleInput(PlayerGamepadData gamepadData)
	{
		//Gets called by input handler if this player's turn
		PlayerSelectionInputs(gamepadData);

		if (playerSelected && lastInputTime+0.1f < Time.time)
			characters[characterIndex].HandleGamepadInput(gamepadData);
	}

	void PlayerSelectionInputs(PlayerGamepadData gamepadData)
	{
		if (!playerSelected)
		{
			if ((gamepadData.prevState.ThumbSticks.Left.X < 0.2f && gamepadData.state.ThumbSticks.Left.X >= 0.2f) ||
				(gamepadData.prevState.Buttons.LeftShoulder == ButtonState.Released && gamepadData.state.Buttons.LeftShoulder == ButtonState.Pressed))
			{
				//Right pressed this frame
				characterIndex++;
				if (characterIndex > characters.Count-1)
					characterIndex = 0;
			}
			if ((gamepadData.prevState.ThumbSticks.Left.X > -0.2f && gamepadData.state.ThumbSticks.Left.X <= -0.2f) ||
				(gamepadData.prevState.Buttons.RightShoulder == ButtonState.Released && gamepadData.state.Buttons.RightShoulder == ButtonState.Pressed))
			{
				//Left pressed this frame
				characterIndex--;
				if (characterIndex < 0)
					characterIndex = characters.Count-1;
			}

			if (gamepadData.prevState.Buttons.A == ButtonState.Released && gamepadData.state.Buttons.A == ButtonState.Pressed)
			{
				lastInputTime = Time.time;
				characters[characterIndex].active = true;
				playerSelected = true;
			}

		}
		// else
		// {
		// 	if (gamepadData.prevState.Buttons.B == ButtonState.Released && gamepadData.state.Buttons.B == ButtonState.Pressed)
		// 	{
		// 		playerSelected = false;
		// 	}
		// }

	}
   
}
