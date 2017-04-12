using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Monster target;
    private Tower towerType;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        AttackTarget();
    }

    public void Initialize(Tower towerType)
    {
        this.target = towerType.GetTarget;
        this.towerType = towerType;
    }

    private void AttackTarget()
    {
        if (target != null)
        {
            transform.position = Vector3.MoveTowards(transform.position, target.transform.position, Time.deltaTime * towerType.projectileSpeed);
        }
        else
        {
            Release();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Monster")
        {
            if (target.gameObject == other.gameObject)
            {
                target.TakeDamage(towerType.damage);
            }
            Release();
        }
    }

    private void Release()
    {
        Object.FindObjectOfType<PlayMap>().projectilePool.ReleaseProjectile(gameObject);
    }
}
