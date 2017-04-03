using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectilePool : MonoBehaviour {

    public GameObject[] projectilePrefabs;

    public GameObject GetProjectile(string type)
    {
        for (int i =0; i < projectilePrefabs.Length; i++)
        {
            if (projectilePrefabs[i].name == type)
            {
                GameObject projectile = Instantiate(projectilePrefabs[i]);
                projectile.name = type;
                return projectile;
            }
        }
        return null;
    }
}
