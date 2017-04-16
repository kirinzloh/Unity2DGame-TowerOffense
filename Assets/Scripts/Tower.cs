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
    public int upgradeCost;    // set to 0 to disable upgrade.
    public float _range;       // Radius
    public float _delay;       // Number of sec per attack
    public bool isSupport;     // flag if the tower is a support tower. Suport towers don't attack.
    public float supportMultiplier; // Multiplier for aura support towers.
    // Projectile stats
    public float projectileSpeed; // GameUnits/second
    public int _damage;
    public int stunTime;         // in ms
    public int slowTime;         // in ms
    public int DOTdamage;        // DOT total damage to apply.
    public int DOTduration;      // in ms
    public float splashRadius;
    public Color32 splashColor;
    public Sprite projectileSprite;

    public AudioClip shootingSound;

    // Support aura tower effects are hardcoded and static.
    public int damage {
        get {
            if (isSupport) { return _damage; }
            float d = _damage;
            foreach (Collider2D inaura in Physics2D.OverlapCircleAll(transform.position, TowerR.getById(60).range, 1 << 2)) {
                if (inaura.CompareTag("AttackAura")) {
                    d *= TowerR.getById(60).supportMultiplier;
                }
            }
            return (int)System.Math.Round(d, System.MidpointRounding.AwayFromZero);
        }
    }

    public float delay {
        get {
            if (isSupport) { return _delay; }
            float de = _delay;
            foreach (Collider2D inaura in Physics2D.OverlapCircleAll(transform.position, TowerR.getById(70).range, 1 << 2)) {
                if (inaura.CompareTag("SpeedAura")) {
                    de /= TowerR.getById(70).supportMultiplier;
                }
            }
            return de;
        }
    }

    public float range {
        get {
            if (isSupport) { return _range; }
            float r = _range;
            foreach (Collider2D inaura in Physics2D.OverlapCircleAll(transform.position, TowerR.getById(80).range, 1 << 2)) {
                if (inaura.CompareTag("RangeAura")) {
                    r *= TowerR.getById(80).supportMultiplier;
                }
            }
            return r;
        }
    }

    // Data for shooting projectiles
    public Coord coord;
    private Monster target;
    private float attackTimer;

    // Use this for initialization
    void Start () {
        Transform child = transform.GetChild(0);
        rangeSpriteRenderer = child.GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update() {
        if (isSupport) { return; }
        List<Monster> potentialTargets = new List<Monster>();

        foreach (Collider2D inrange in Physics2D.OverlapCircleAll(transform.position, range, 1 << 2)) {
            if (inrange.CompareTag("Monster")) {
                potentialTargets.Add(inrange.GetComponent<Monster>());
            }
        }

        if (!potentialTargets.Contains(target)) {
            target = null;
        }
        foreach (Monster monster in potentialTargets) {
            if (target == null || monster.pathDestIndex > target.pathDestIndex) {
                target = monster;
            }
        }

        if (attackTimer > 0) {
            attackTimer -= Time.deltaTime;
        }
        if (attackTimer <= 0 && target != null) {
            Shoot();
            attackTimer = delay;
        }
    }

    private void Shoot() {
        ProjectileData projData = new ProjectileData();
        projData.targetSerializeId = target.serializeId;
        projData.towerId = towerId;
        projData.startCoord = coord;
        float distance = Vector2.Distance(this.transform.position, target.transform.position);
        int msTime = (int) (1000 * distance / projectileSpeed);
        //Debug.Log("tower" + towerId + " | distance: " + distance + " | time: " + msTime); // DEBUG
        projData.startTime = GameManager.instance.getTime();
        projData.hitTime = projData.startTime + msTime;
        projData.damage = damage;
        projData.stunTime = stunTime;
        projData.slowTime = slowTime;
        projData.DOTdamage = DOTdamage;
        projData.DOTduration = DOTduration;
        projData.splashRadius = splashRadius;
        GameManager.instance.shootProjectile(projData);
    }

    public void ShowRange() {
        if (rangeSpriteRenderer != null) {
            rangeSpriteRenderer.transform.localScale = new Vector3(range * 2, range * 2, 1);
            rangeSpriteRenderer.enabled = true;
        }
    }

    public void HideRange() {
        if (rangeSpriteRenderer != null) {
            rangeSpriteRenderer.enabled = false;
        }
    }
}
