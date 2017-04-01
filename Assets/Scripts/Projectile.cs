using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {
    private Monster target;
    private Tower towerType;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        AttackTarget();
	}

    public void Initialize(Tower towerType)
    {
        this.target = towerType.GetTarget;
        this.towerType = towerType;
    }
    private void AttackTarget()
    {
        if (target != null && target.isActive)
        {
            transform.position = Vector3.MoveTowards(transform.position, target.transform.position, Time.deltaTime * towerType.projectileSpeed);
        }
        else if (!target.isActive)
        {
            GameplayManager.Instance.Pool.ReleaseObject(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Monster")
        {
            if (target.gameObject == other.gameObject)
            {
                target.TakeDamage(towerType.TowerDamage);
            }
            GameplayManager.Instance.Pool.ReleaseObject(gameObject);
        }
    }
}
