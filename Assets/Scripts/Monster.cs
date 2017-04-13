using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour {

    public ViewMap playMap;

    // Monster data
    [SerializeField]
	private float speed;
    public int damage;
    public int hp;
	public int monsterId;
	public int price;

    // Pathfinding data
	private Stack<ViewTile> path;
	private ViewTile startPath;
	public Coord gridPosition;
	private Vector3 destination;

	// Use this for initialization
	void Start () {
        this.hp = 10;
        SetPath(playMap.getPath());
	}

	// Update is called once per frame
	void Update () {
		Move();
        if (hp <= 0)
        {
            Destroy(this.gameObject); 
        }
	}

	void OnMouseDown()
	{
        //Object.FindObjectOfType<PlayMap>().DisplayUpgradePanel(monsterType);
	}

	private void Move()
	{
		transform.position = Vector2.MoveTowards (transform.position, destination, speed*Time.deltaTime);

		if (transform.position == destination) {
			if (path != null && path.Count > 0) {
				gridPosition = path.Peek ().tileData.coord; 
				destination = path.Pop ().transform.position;
			}
			else {
				playMap.GameState.hp -= this.damage;
				transform.position = startPath.transform.position;
				gridPosition = startPath.tileData.coord;
				Destroy (this.gameObject);
			}
		}
	}

	void SetPath(List<ViewTile> pathList)
	{
        Debug.Log(pathList);
		if (pathList != null) {
			List<ViewTile> reversedList = new List<ViewTile> (pathList);
			reversedList.Reverse ();
			this.path = new Stack<ViewTile> (reversedList);
			this.gridPosition = path.Peek().tileData.coord;
			this.startPath = path.Peek ();
			this.destination = path.Pop().transform.position;
		}

	}

    public void TakeDamage(int damage)
    {
        hp -= damage;
    }


}
