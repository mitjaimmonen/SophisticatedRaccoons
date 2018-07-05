using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HudHandler : MonoBehaviour {

	public Text playerTurnText;
	public Text winText;

    public Text aText, bText, yText, xText, cantPushText;

	void Start ()
	{
		playerTurnText.gameObject.SetActive(true);
		winText.gameObject.SetActive(false);
		GameMaster.Instance.hudHandler = this;
        cantPushText.enabled = false;

     }

     public void Reset()
     {
         winText.gameObject.SetActive(false);
         cantPushText.enabled = false;
         playerTurnText.gameObject.SetActive(true);
     }

	public void SetPlayerTurn(string playerName)
	{
		playerTurnText.text = playerName + " turn";
	}

	public void GameOver(int winnerIndex)
	{
		winText.gameObject.SetActive(true);
		playerTurnText.gameObject.SetActive(false);
		winText.text = "Player " + (winnerIndex+1) + " won!";
	}

    public void SetInstructions(string button, string instruction)
    {
        if (button == "a" || button == "A")
        {
            aText.text = instruction;
        }

        if (button == "b" || button == "B")
        {
            bText.text = instruction;
        }

        if (button == "x" || button == "X")
        {
            xText.text = instruction;
        }

        if (button == "y" || button == "Y")
        {
            yText.text = instruction;
        }
    }

    public void ToggleInstructions(string button, bool state)
    {
        if (button == "a" || button == "A")
        {
            aText.enabled = state;
        }

        if (button == "b" || button == "B")
        {           
            bText.enabled = state;
        }

        if (button == "x" || button == "X")
        {
            xText.enabled = state;
        }

        if (button == "y" || button == "Y")
        {
            yText.enabled = state;
        }
    }

    public void CallFailPush()
    {
        StartCoroutine(FailPush());
    }

    public IEnumerator FailPush()
    {
        float timer = 0;
        Debug.Log("CANT PUSH???");
        cantPushText.enabled = true;
     

        while (timer < 0.4f)
        {
            timer += Time.deltaTime;
            yield return null;
        }
        
        cantPushText.enabled = false; 
      
    }

}
