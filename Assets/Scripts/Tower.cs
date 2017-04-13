using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour {
    // This is to show the range of the tower
    private SpriteRenderer rangeSpriteRenderer;

    // Tower data
    public int towerId;
    public string towerName;
    public int price;
    public int upgradeCost;   // set to 0 to disable upgrade.
    public int damage;
    public float range;       // Radius
    public float delay;       // Number of sec per attack
    public string attackType; // ??? Included temporarily first. May not be used.
    public float projectileSpeed;
    public string projectileType;

    // Data for shooting projectiles
    private Queue<Monster> monsters;
    private Monster target;
    private bool canAttack;
    private float attackTimer;

    // Use this for initialization
    void Start () {
        Transform child = transform.GetChild(0);
        child.localScale = new Vector3(range*2,range*2,1);
        rangeSpriteRenderer = child.GetComponent<SpriteRenderer>();
        GetComponent<CircleCollider2D>().radius = range;
        monsters = new Queue<Monster>();
        canAttack = true;
    }

    // Update is called once per frame
    void Update() {
        Attack();
    }

    private void Attack() {
        if (!canAttack) {
            attackTimer += Time.deltaTime;
            if (attackTimer >= delay) {
                canAttack = true;
                attackTimer = 0;
            }
        }
        if (target == null && monsters.Count > 0)
        {
            target = monsters.Dequeue();
        }
        if (target != null)
        {
            if (canAttack == true)
            {
                Shoot();
                canAttack = false;
            }
        }
    }

    private void Shoot() {
        Projectile projectile = Object.FindObjectOfType<PlayMap>().projectilePool.GetProjectile(projectileType).GetComponent<Projectile>();
        projectile.transform.position = transform.position;
        projectile.Initialize(this);
    }

    public void OnTriggerEnter2D(Collider2D other) {
        if (other.tag == "Monster") {
            monsters.Enqueue(other.GetComponent<Monster>());
        }
    }

    public void OnTriggerExit2D(Collider2D other) {
        if (other.tag == "Monster") {
            target = null;
        }
    }

    public void ShowRange() {
        if (rangeSpriteRenderer != null) {
            rangeSpriteRenderer.enabled = true;
        }
    }

    public void HideRange() {
        if (rangeSpriteRenderer != null) {
            rangeSpriteRenderer.enabled = false;
        }
    }

    public Monster GetTarget {
        get { return target; }
    }

}
