using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour {
    // This is to show the range of the tower
    private SpriteRenderer rangeSpriteRenderer;

    private string towerType;

    
    // Use this for initialization
    void Start () {
        rangeSpriteRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
        this.name = towerType;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnMouseDown()
    {
        TriggerRangeDisplay();
        PlayMap.Instance.DisplayUpgradePanel(towerType);
    }

    public void TriggerRangeDisplay()
    {
        rangeSpriteRenderer.enabled = !rangeSpriteRenderer.enabled;
    }


}
