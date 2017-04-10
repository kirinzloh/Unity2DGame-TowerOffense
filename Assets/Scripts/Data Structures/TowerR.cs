using System.Collections.Generic;
using UnityEngine;

// Tower resources. Reference to all the tower prefabs, by numbering.

public static class TowerR {

    // Mapping of tower IDs to the tower script in the prefab.
    // Tower ids start from 1st digit. 2nd digit is the tower level (starting from 0).
    private static Dictionary<int, Tower> IdMap;

    static TowerR() {
        IdMap = new Dictionary<int, Tower>();
        IdMap.Add(10, Resources.Load<Tower>("Towers/BasicTower"));
        IdMap.Add(20, Resources.Load<Tower>("Towers/FrozeTower"));
        IdMap.Add(30, Resources.Load<Tower>("Towers/PoisonTower"));
        foreach (KeyValuePair<int, Tower> pair in IdMap) {
            pair.Value.towerId = pair.Key;
        }
    }

    public static Tower getById(int id) {
        return IdMap[id];
    }

    public static List<Tower> getBaseTowers() {
        List<Tower> towers = new List<Tower>();
        foreach (KeyValuePair<int, Tower> pair in IdMap) {
            if (pair.Key % 10 == 0) {
                towers.Add(pair.Value);
            }
        }
        towers.Sort((x, y) => x.towerId.CompareTo(y.towerId));
        return towers;
    }

    public static List<Tower> getAllTowers() {
        List<Tower> towers = new List<Tower>();
        foreach (KeyValuePair<int, Tower> pair in IdMap) {
            towers.Add(pair.Value);
        }
        towers.Sort((x, y) => x.towerId.CompareTo(y.towerId));
        return towers;
    }
}