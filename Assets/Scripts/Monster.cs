﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExitGames.Client.Photon;

public class Monster : MonoBehaviour {
    
    public PlayerGameState gameState;
    private SpriteRenderer spriteR;
    public int serializeId;

    // Monster data
    public int monsterId; // Type of monster.
    public int price;
    public int hp;
    public int damage;
    public float speed;
    public int maxHp;
    public int reward;

    // Pathfinding data
    private List<ViewTile> path;
    public int pathDestIndex;

    //effects
    public byte effects; // Bitmask for effects. 76543210 -> 0 : stun, 1 : slow, 2-7: not implemented yet.
    public int stunTill;
    public int slowTill;

    // 2*4 byte int hp, pathDestIndex.
    // 2*4 byte float position.x and y.
    // 1 byte effects bitmask
    // 2*4 byte int slowTill, stunTill.
    // 2*4 byte serializeId and monsterId;
    // NOTE: serializeId and monsterId are serialized inside, but deserialized outside, in playerGameState.
    public const int serialize_size = 41;

    void Awake() {
        spriteR = GetComponent<SpriteRenderer>();
        hp = maxHp;
    }

    // Use this for initialization
    void Start() {
    }

    // Update is called once per frame
    void Update() {
        float hp_ratio = hp / (float)maxHp;
        spriteR.color = new Color(1, hp_ratio, hp_ratio);

        if (checkEffectBit(0) && GameManager.instance.getTime() >= stunTill) {
            setEffectBit(0, false);
            stunTill = 0;
        }
        if (checkEffectBit(1) && GameManager.instance.getTime() >= slowTill) {
            setEffectBit(1, false);
            slowTill = 0;
        }

        if (checkEffectBit(1)) {
            spriteR.color = new Color(spriteR.color.r * 0.5f, spriteR.color.g * 0.5f, spriteR.color.b, 0.8f);
        }

        Move();

        if (hp <= 0) {
            gameState.gold += reward;
            gameState.destroyMonster(this);
        }
    }

    private void Move() {
        Vector3 destination = path[pathDestIndex].transform.position;

        float currSpeed = speed;
        if (checkEffectBit(0) && GameManager.instance.getTime() < stunTill) {
            currSpeed = 0;
        } else if (checkEffectBit(1) && GameManager.instance.getTime() < slowTill) {
            currSpeed = currSpeed / 2;
        }

        transform.position = Vector2.MoveTowards(transform.position, destination, currSpeed * Time.deltaTime);

        if (transform.position == destination) {
            if (pathDestIndex+1 < path.Count) {
                ++pathDestIndex;
            } else {
                gameState.takeDamage(damage);
                gameState.destroyMonster(this);
            }
        }
    }

    #region public api
    public void SetPath(List<ViewTile> pathList) {
        if (pathList != null) {
            path = pathList;
            pathDestIndex = 0;
            transform.position = path[pathDestIndex].transform.position;
        }
    }

    public void TakeDamage(int damage) {
        hp -= damage;
    }

    public void inflictStun(int msDuration) {
        setEffectBit(0, true);
        stunTill = GameManager.instance.getTime() + msDuration;
    }

    public void inflictSlow(int msDuration) {
        setEffectBit(1, true);
        slowTill = GameManager.instance.getTime() + msDuration;
    }
    #endregion

    void setEffectBit(int bit, bool activate) {
        if (activate) {
            effects |= (byte)(1 << bit);
        } else {
            effects &= (byte)~(1 << bit);
        }
    }

    bool checkEffectBit(int bit) {
        byte extracted = (byte) (effects & (1 << bit));
        return extracted == (1 << bit);
    }

    #region serialisation
    public int serializeTo(byte[] dest, ref int index) {
        Protocol.Serialize(serializeId, dest, ref index);
        Protocol.Serialize(monsterId, dest, ref index);
        Protocol.Serialize(hp, dest, ref index);
        Protocol.Serialize(pathDestIndex, dest, ref index);
        Protocol.Serialize(transform.position.x, dest, ref index);
        Protocol.Serialize(transform.position.y, dest, ref index);
        dest[index] = effects;
        ++index;
        Protocol.Serialize(stunTill, dest, ref index);
        Protocol.Serialize(slowTill, dest, ref index);
        return serialize_size;
    }

    public void deserializeFrom(byte[] from, ref int index) {
        Protocol.Deserialize(out hp, from, ref index);
        Protocol.Deserialize(out pathDestIndex, from, ref index);
        float x;
        float y;
        Protocol.Deserialize(out x, from, ref index);
        Protocol.Deserialize(out y, from, ref index);
        effects = from[index];
        ++index;
        Protocol.Deserialize(out stunTill, from, ref index);
        Protocol.Deserialize(out slowTill, from, ref index);
        transform.position = new Vector2(x+50, y); // hardcode +50 to x to offset view map.
    }
    #endregion
}
