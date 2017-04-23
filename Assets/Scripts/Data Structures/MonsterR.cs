using System.Collections.Generic;
using UnityEngine;

// Monster resources. Reference to all the monster prefabs, by numbering.

public static class MonsterR {

    // Mapping of monster IDs to the monster script in the prefab.
    private static Dictionary<int, Monster> IdMap;

    static MonsterR() {
        IdMap = new Dictionary<int, Monster>();
        IdMap.Add(1, Resources.Load<Monster>("Monsters/Monster1"));
        IdMap.Add(2, Resources.Load<Monster>("Monsters/Monster2"));
        IdMap.Add(3, Resources.Load<Monster>("Monsters/Monster3"));
        IdMap.Add(4, Resources.Load<Monster>("Monsters/Monster4"));
        IdMap.Add(5, Resources.Load<Monster>("Monsters/Monster5"));
        IdMap.Add(6, Resources.Load<Monster>("Monsters/Monster6"));
        foreach (KeyValuePair<int, Monster> pair in IdMap) {
            pair.Value.monsterId = pair.Key;
        }
    }

    public static Monster getById(int id) {
        return IdMap[id];
    }

    public static List<Monster> getAllMonsters() {
        List<Monster> monsters = new List<Monster>();
        foreach (KeyValuePair<int, Monster> pair in IdMap) {
            monsters.Add(pair.Value);
        }
        monsters.Sort((x, y) => x.monsterId.CompareTo(y.monsterId));
        return monsters;
    }
}