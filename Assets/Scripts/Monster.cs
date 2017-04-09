using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour {

	[SerializeField]
	private float speed;
	public PlayMap playMap;

	private string monsterType;
	private Stack<PlayTile> path;
	public Coord gridPosition;
	private Vector3 destination;

	// Use this for initialization
	void Start () {
		this.name = monsterType;
		SetPath(playMap.path);
	}

	// Update is called once per frame
	void Update () {
		Move();
	}

	void OnMouseDown()
	{
		PlayMap.Instance.DisplayUpgradePanel(monsterType);
	}

	private void Move()
	{
		transform.position = Vector2.MoveTowards (transform.position, destination, speed*Time.deltaTime);

		if (transform.position == destination) {
			if (path != null && path.Count > 0) {
				gridPosition = path.Peek().tileData.coord; 
				destination = path.Pop().transform.position;
			}
		}
	}

	void SetPath(List<PlayTile> pathList)
	{
		Debug.Log(pathList);
		if (pathList != null) {
			this.path = new Stack<PlayTile> (pathList);
			this.gridPosition = path.Peek().tileData.coord;
			this.destination = path.Pop().transform.position;
		}

	}
}
