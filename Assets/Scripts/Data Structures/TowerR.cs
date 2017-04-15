using System.Collections.Generic;
using UnityEngine;

// Tower resources. Reference to all the tower prefabs, by numbering.

public static class TowerR {

    // Mapping of tower IDs to the tower script in the prefab.
    // Tower ids start from tens digit. ones digit is the tower level (starting from 0).
    private static Dictionary<int, Tower> IdMap;

    static TowerR() {
        IdMap = new Dictionary<int, Tower>();
        // Level 1 Towers
        IdMap.Add(10, Resources.Load<Tower>("Towers/Level 1 Towers/BasicTower"));
        IdMap.Add(20, Resources.Load<Tower>("Towers/Level 1 Towers/FrozeTower"));
        IdMap.Add(30, Resources.Load<Tower>("Towers/Level 1 Towers/BombTower"));
        IdMap.Add(40, Resources.Load<Tower>("Towers/Level 1 Towers/PoisonTower"));
        IdMap.Add(50, Resources.Load<Tower>("Towers/Level 1 Towers/TeslaTower"));
        IdMap.Add(60, Resources.Load<Tower>("Towers/Level 1 Towers/AttackAuraTower"));
        IdMap.Add(70, Resources.Load<Tower>("Towers/Level 1 Towers/SpeedAuraTower"));
        IdMap.Add(80, Resources.Load<Tower>("Towers/Level 1 Towers/RangeAuraTower"));
        IdMap.Add(90, Resources.Load<Tower>("Towers/Level 1 Towers/GoldTower"));

        // Level 2 Towers
        IdMap.Add(11, Resources.Load<Tower>("Towers/Level 2 Towers/BasicTower2"));
        IdMap.Add(21, Resources.Load<Tower>("Towers/Level 2 Towers/FrozeTower2"));
        IdMap.Add(31, Resources.Load<Tower>("Towers/Level 2 Towers/BombTower2"));
        IdMap.Add(41, Resources.Load<Tower>("Towers/Level 2 Towers/PoisonTower2"));
        IdMap.Add(51, Resources.Load<Tower>("Towers/Level 2 Towers/TeslaTower2"));

        // Level 3 Towers
        IdMap.Add(12, Resources.Load<Tower>("Towers/Level 3 Towers/BasicTower3"));
        IdMap.Add(22, Resources.Load<Tower>("Towers/Level 3 Towers/FrozeTower3"));
        IdMap.Add(32, Resources.Load<Tower>("Towers/Level 3 Towers/BombTower3"));
        IdMap.Add(42, Resources.Load<Tower>("Towers/Level 3 Towers/PoisonTower3"));
        IdMap.Add(52, Resources.Load<Tower>("Towers/Level 3 Towers/TeslaTower3"));

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