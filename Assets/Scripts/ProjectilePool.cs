using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectilePool : MonoBehaviour {

    public GameObject[] projectilePrefabs;

    private List<GameObject> pooledProjectiles = new List<GameObject>();

    public GameObject GetProjectile(string type)
    {
        foreach (GameObject projectile in pooledProjectiles)
        {
            if (projectile.name == type && !projectile.activeInHierarchy)
            {
                projectile.SetActive(true);
                return projectile;
            }
        }

        for (int i =0; i < projectilePrefabs.Length; i++)
        {
            if (projectilePrefabs[i].name == type)
            {
                GameObject projectile = Instantiate(projectilePrefabs[i]);
                pooledProjectiles.Add(projectile);
                projectile.name = type;
                return projectile;
            }
        }
        return null;
    }

    public void ReleaseProjectile(GameObject projectile)
    {
        projectile.SetActive(false);
    }

}
