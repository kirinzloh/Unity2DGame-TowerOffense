using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OnClickTile : MonoBehaviour {
    public static bool startPicked;
    public static bool endPicked;
    public string gameState;
    public GameObject tile;
    public Text instructionsText;
    private bool isSel;
    private bool startTile;

    void Start()
    {
        instructionsText.text = "Pick your starting point";
        isSel = false;
    }

    void Update()
    {   
        if (tile.tag.Equals("StartTile"))
        {   
            if (startPicked == false)
            {
                tile.GetComponent<Renderer>().material.color = new Color(1,0.843f,0,1);
                startTile = false;
            }
            else if (startPicked == true && startTile == false)
            {
                tile.GetComponent<Renderer>().material.color = new Color(1, 1, 1, 1);
            }
        }
    }
    void OnMouseDown()
    {
        if (tile.tag.Equals("StartTile"))
        {
            if (isSel == false && startPicked == false)
            {
                tile.GetComponent<Renderer>().material.color = new Color(1, 0, 0, 1);
                startPicked = true;
                isSel = true;
                startTile = true;
                instructionsText.text = "Create route for the monsters \n attacking you";
            }
            else if (isSel == true && startPicked == true) 
            {
                tile.GetComponent<Renderer>().material.color = new Color(1, 1, 1, 1);
                startPicked = false;
                isSel = false;
                instructionsText.text = "Pick your starting point";
            }
        }
        else if(tile.tag.Equals("EndTile"))
        {
            if (isSel == false && endPicked == false)
            {
                tile.GetComponent<Renderer>().material.color = new Color(1, 0, 0, 1);
                endPicked = true;
                isSel = true;
                instructionsText.text = "You have finished creating your route!\n Are you ready?";
            }
            else if (isSel == true && endPicked == true)
            {
                tile.GetComponent<Renderer>().material.color = new Color(1, 1, 1, 1);
                endPicked = false;
                isSel = false;
                instructionsText.text = "Create route for the monsters \n attacking you";
            }
        }
        else if(tile.tag.Equals("Tile"))
        {   
            if (isSel == false)
            {
                tile.GetComponent<Renderer>().material.color = new Color(140, 120, 250, 255);
                isSel = true;
            }
            else
            {
                tile.GetComponent<Renderer>().material.color = new Color(1, 1, 1, 1);
                isSel = false;
            }
        }
    }

}
