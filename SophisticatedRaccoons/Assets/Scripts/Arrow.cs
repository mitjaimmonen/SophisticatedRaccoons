using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Arrow : MonoBehaviour
{
    [StringInList("Up", "Down", "Right", "Left")] public string direction;
    public bool selected = false;
    Transform position;

    void Awake()
    {
        
    }
    void LateUpdate()
    {
        
    }

    private void Update()
    {
        if (selected)
        {
            GetComponent<Renderer>().material.color = Color.green;
        }
        else
        {
            GetComponent<Renderer>().material.color = Color.yellow;
        }

    }

    public void OnEnable()
    {
        selected = false;
    }


}
