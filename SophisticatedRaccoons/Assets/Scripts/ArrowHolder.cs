using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowHolder : MonoBehaviour {

    public Dictionary<string, Arrow> directionArrows;
    public Arrow currentArrow = null;
    public PlayerMove owner;
    public bool haveSelected = false;

    public ArrowHolder(PlayerMove parent)
    {

    }

    private void Awake()
    {
        directionArrows = new Dictionary<string, Arrow>();
        FetchArrows();
    }

    private void LateUpdate()
    {
        transform.position = owner.gameObject.transform.position;
    }

    private void FetchArrows()
    {
        Arrow[] arrows = GetComponentsInChildren<Arrow>();

        foreach (Arrow a in arrows)
        {
        
            directionArrows.Add(a.direction, a);          
      
        }
    }

    public Arrow GetArrow(string direction)
    {   
        return directionArrows[direction];
    }

     public void ClearSelectedArrows()
    {
        foreach (KeyValuePair<string,Arrow> a in directionArrows)
        {
            a.Value.selected = false;
        }
    }

    public void OnDisable()
    {
        if (haveSelected)
        ClearSelectedArrows();
    }

    public void SelectArrow(string direction)
    {

    }

    public Arrow SelectedArrow()
    {      
        foreach (KeyValuePair<string, Arrow> a in directionArrows)
        {
            if (a.Value.selected)
            {
                return a.Value;
            }            
        }

        return null;
    }

    public void ToggleArrow(bool toggle)
    {       
        if (toggle)
        {
            foreach (KeyValuePair<string, Arrow> a in directionArrows)
            {
                a.Value.gameObject.GetComponent<Renderer>().enabled = true;
            }
        }
        else
        {
            foreach (KeyValuePair<string, Arrow> a in directionArrows)
            {              
                a.Value.gameObject.GetComponent<Renderer>().enabled = false;
            }
        }
    }


}
