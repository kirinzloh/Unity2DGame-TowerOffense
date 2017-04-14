using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public SpriteRenderer spriteR;
    public Monster target;
    public Vector2 source;
    public ProjectileData projData;
    private float interval;
    
    // Use this for initialization
    void Start()
    {
        transform.position = source;
        interval = projData.hitTime - projData.startTime;
        if (interval <= 0) {
            hitTarget();
        }
    }

    // Update is called once per frame
    void Update() {
        if (target == null) { // Target disappeared.
            Release();
            return;
        }

        int elapsedtime = GameManager.instance.getTime() - projData.startTime;
        float progress = elapsedtime / interval;
        transform.position = Vector2.Lerp(source, target.transform.position, progress);
        if (progress >= 1) {
            hitTarget();
        }
    }

    private void hitTarget() {
        foreach (Collider2D inrange in Physics2D.OverlapCircleAll(target.transform.position, projData.splashRadius, 1<<2)) {
            if (inrange.CompareTag("Monster")) {
                Monster monster = inrange.GetComponent<Monster>();
                monster.TakeDamage(projData.damage);
                if (projData.stunTime > 0) {
                    monster.inflictStun(projData.stunTime);
                }
                if (projData.slowTime > 0) {
                    monster.inflictSlow(projData.slowTime);
                }
            }
        }
        Release();
    }
    
    private void Release()
    {
        GameManager.instance.projectilePool.ReleaseProjectile(this);
    }
}
