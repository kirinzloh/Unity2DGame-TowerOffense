using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectilePool {
    
    private static readonly Projectile ProjPrefab = Resources.Load<Projectile>("Projectile");
    private List<Projectile> pooledProjectiles = new List<Projectile>();
    
    public Projectile GetProjectile()
    {
        foreach (Projectile projectile in pooledProjectiles)
        {
            if (!projectile.gameObject.activeInHierarchy)
            {
                projectile.gameObject.SetActive(true);
                return projectile;
            }
        }
        Projectile newProjectile = Object.Instantiate(ProjPrefab);
        pooledProjectiles.Add(newProjectile);
        return newProjectile;
    }

    public void ReleaseProjectile(Projectile projectile)
    {
        projectile.target = null;
        projectile.projData = null;
        projectile.gameObject.SetActive(false);
    }

}
