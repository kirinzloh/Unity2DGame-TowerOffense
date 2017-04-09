using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour {
    // This is to show the range of the tower
    private SpriteRenderer rangeSpriteRenderer;

    public int towerId;
    public string towerName;
    public int price;
    public int upgradeCost;   // set to 0 to disable upgrade.
    public float range;       // Radius
    public float damage;
    public float delay;       // Number of sec per attack
    public string attackType; // ??? Included temporarily first. May not be used.

    private List<Monster> inRange;

    // Use this for initialization
    void Start () {
        Transform child = transform.GetChild(0);
        child.localScale = new Vector3(range*2,range*2,1);
        rangeSpriteRenderer = child.GetComponent<SpriteRenderer>();
        GetComponent<CircleCollider2D>().radius = range;
        inRange = new List<Monster>();
    }

    // Update is called once per frame
    void Update() {
        //Attack();
    }

    /*
    private void Attack() {
        if (!canAttack) {
            attackTimer += Time.deltaTime;
            if (attackTimer >= attackSpeed) {
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

    private void Shoot() {
        Projectile projectile = Object.FindObjectOfType<PlayMap>().projectilePool.GetProjectile(projectileType).GetComponent<Projectile>();
        projectile.transform.position = transform.position;
        projectile.Initialize(this);
    }*/

    public void OnTriggerEnter2D(Collider2D other) {
        if (other.tag == "Monster") {
            inRange.Add(other.GetComponent<Monster>());
            //monsters.Enqueue(other.GetComponent<Monster>());
        }
    }

    public void OnTriggerExit2D(Collider2D other) {
        if (other.tag == "Monster") {
            inRange.Remove(other.GetComponent<Monster>());
            //target = null;
        }
    }

    /*
    void OnMouseDown()
    {
        TriggerRangeDisplay();
        //Object.FindObjectOfType<PlayMap>().DisplayUpgradePanel(towerType);
    }*/

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

    public void TriggerRangeDisplay()
    {
        rangeSpriteRenderer.enabled = !rangeSpriteRenderer.enabled;
    }
}
