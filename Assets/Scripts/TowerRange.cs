using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerRange : MonoBehaviour
{
    // This is the projectile's type
    public string projectileType;

    // This is the target of the tower's attack
    //private Monster target;

    // This is the tower's target queue
    //private Queue<Monster> monsters = new Queue<Monster>();

    // This indicates whether the tower can attack (shoot projectile)
    private bool canAttack = true;

    // This is the timer for the tower's attack
    private float attackTimer;

    // This is the tower's attack speed (Number of secs per attack)
    public float attackSpeed;

    // This is the tower's projectile speed
    public float projectileSpeed;

    // This is the tower's damage
    public int towerDamage;

    // Use this for initialization
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Attack();
    }

    private void Attack()
    {
        if (!canAttack)
        {
            attackTimer += Time.deltaTime;
            if (attackTimer >= attackSpeed)
            {
                canAttack = true;
                attackTimer = 0;
            }
        }
        //if (target == null && monsters.Count > 0)
        //{
        //    target = monsters.Dequeue();
        //}
        //if (target != null && target.isActive)
        //{
        //    if (canAttack == true)
        //    {
        //        Shoot();
        //        canAttack = false;
        //    }
        //}
    }

    private void Shoot()
    {
        Projectile projectile = PlayMap.Instance.projectilePool.GetProjectile(projectileType).GetComponent<Projectile>();
        projectile.transform.position = transform.position;
        projectile.Initialize(this);
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Monster")
        {
            //monsters.Enqueue(other.GetComponent<Monster>());
        }
    }

    public void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Monster")
        {
            //target = null;
        }
    }

    public float GetProjectileSpeed
    {
        get { return projectileSpeed; }
    }

    //public Monster GetTarget
    //{
    //    get { return target; }
    //}

    public int TowerDamage
    {
        get
        {
            return towerDamage;
        }
    }
}
