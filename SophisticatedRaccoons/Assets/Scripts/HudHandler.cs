using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HudHandler : MonoBehaviour {

	public Text playerTurnText;
	public Text winText;

	void Start ()
	{
		playerTurnText.gameObject.SetActive(true);
		winText.gameObject.SetActive(false);
		GameMaster.Instance.hudHandler = this;
	}

	public void SetPlayerTurn(string playerName)
	{
		playerTurnText.text = playerName + " turn";
	}

	public void GameOver(string winnerName)
	{
		winText.gameObject.SetActive(true);
		playerTurnText.gameObject.SetActive(false);
		winText.text = winnerName + " won!";
	}
}
