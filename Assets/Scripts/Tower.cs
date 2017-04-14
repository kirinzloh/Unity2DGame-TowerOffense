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
    public float range;       // Radius
    public float delay;       // Number of sec per attack
    // Projectile stats
    public float projectileSpeed; // GameUnits/second
    public int damage;
    public int stunTime;         // in ms
    public int slowTime;         // in ms
    public float splashRadius;
    public Sprite projectileSprite;
    // To check

    // Data for shooting projectiles
    public Coord coord;
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
        ProjectileData projData = new ProjectileData();
        projData.targetSerializeId = target.serializeId;
        projData.towerId = towerId;
        projData.startCoord = coord;
        float distance = Vector2.Distance(this.transform.position, target.transform.position);
        int msTime = (int) (1000 * distance / projectileSpeed);
        Debug.Log("tower" + towerId + " | distance: " + distance + " | time: " + msTime); // DEBUG
        projData.startTime = GameManager.instance.getTime();
        projData.hitTime = projData.startTime + msTime;
        projData.damage = damage;
        projData.stunTime = stunTime;
        projData.slowTime = slowTime;
        projData.splashRadius = splashRadius;
        GameManager.instance.shootProjectile(projData);
    }

    public void OnTriggerEnter2D(Collider2D other) {
        if (other.tag == "Monster") {
            Debug.Log(monsters + "|" + other); // DEBUG
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
