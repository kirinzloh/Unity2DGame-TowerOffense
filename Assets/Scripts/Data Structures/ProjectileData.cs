using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;

public class ProjectileData {

    public int targetSerializeId;
    public int towerId;
    public Coord startCoord;
    public int startTime;
    public int hitTime;

    public int damage;
    public int stunTime;
    public int slowTime;
    public float splashRadius;

    // 7*4byte (int) + 1*4byte float + Coord: 2*4byte (int).
    public const int serialize_size = 40;

    public byte[] serialize() {
        byte[] dest = new byte[serialize_size];
        int index = 0;
        Protocol.Serialize(targetSerializeId, dest, ref index);
        Protocol.Serialize(towerId, dest, ref index);
        startCoord.serializeTo(dest, ref index);
        Protocol.Serialize(startTime, dest, ref index);
        Protocol.Serialize(hitTime, dest, ref index);
        Protocol.Serialize(damage, dest, ref index);
        Protocol.Serialize(stunTime, dest, ref index);
        Protocol.Serialize(slowTime, dest, ref index);
        Protocol.Serialize(splashRadius, dest, ref index);
        return dest;
    }

    public static ProjectileData deserialize(byte[] from) {
        ProjectileData proj = new ProjectileData();
        int index = 0;
        Protocol.Deserialize(out proj.targetSerializeId, from, ref index);
        Protocol.Deserialize(out proj.towerId, from, ref index);
        proj.startCoord = Coord.deserialize(from, ref index);
        Protocol.Deserialize(out proj.startTime, from, ref index);
        Protocol.Deserialize(out proj.hitTime, from, ref index);
        Protocol.Deserialize(out proj.damage, from, ref index);
        Protocol.Deserialize(out proj.stunTime, from, ref index);
        Protocol.Deserialize(out proj.slowTime, from, ref index);
        Protocol.Deserialize(out proj.splashRadius, from, ref index);
        return proj;
    }
}
