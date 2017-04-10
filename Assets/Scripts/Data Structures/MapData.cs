using System.Collections.Generic;
using ExitGames.Client.Photon;

public class MapData
{

    public int numRows;
    public int numCols;

    private TileData[,] grid;
    private List<TileData> path;

    public MapData(int r, int c)
    {
        numRows = r;
        numCols = c;
        grid = new TileData[numRows, numCols];
        for (int i = 0; i < numRows; i++)
        {
            for (int j = 0; j < numCols; j++)
            {
                grid[i, j] = new TileData();
            }
        }
        path = new List<TileData>();
    }

    public TileData getTileData(int i, int j)
    {
        if (i < 0 || j < 0 || i >= grid.GetLength(0) || j >= grid.GetLength(1))
        {
            return null;
        }
        return grid[i, j];
    }

    public List<TileData> getPath()
    {
        return path;
    }

    public void setGrid(BuildTile[,] buildGrid)
    {
        if (buildGrid.GetLength(0) != numRows || buildGrid.GetLength(1) != numCols)
        {
            throw new System.Exception("Grid size mismatch.");
        }
        for (int i = 0; i < numRows; i++)
        {
            for (int j = 0; j < numCols; j++)
            {
                grid[i, j] = buildGrid[i, j].tileData;
            }
        }
    }

    public void setPath(List<BuildTile> buildPath)
    {
        foreach (BuildTile tile in buildPath)
        {
            path.Add(tile.tileData);
        }
    }

    #region serialisation
    public byte[] serializeNew()
    {
        int size = 8; // 2*4 bytes for numRows and numCols.
        size += numRows * numCols * TileData.serialize_size; // For grid
        size += 4 + (8 * path.Count); // 2*4 bytes (coordinates) * path length. + 4 bytes path length (int). For path.
        byte[] mapBytes = new byte[size];
        int index = 0;
        // Serialize rows and cols.
        Protocol.Serialize(numRows, mapBytes, ref index);
        Protocol.Serialize(numCols, mapBytes, ref index);
        // Serialise grid
        for (int i = 0; i < numRows; i++)
        {
            for (int j = 0; j < numCols; j++)
            {
                getTileData(i, j).serializeTo(mapBytes, ref index);
            }
        }
        // Serialize Path
        Protocol.Serialize(path.Count, mapBytes, ref index);
        for (int i = 0; i < path.Count; i++)
        {
            path[i].coord.serializeTo(mapBytes, ref index);
        }
        return mapBytes;
    }

    public static MapData deserializeNew(byte[] mapBytes)
    {
        int index = 0;
        int numRows;
        int numCols;
        // Deserialize rows and cols.
        Protocol.Deserialize(out numRows, mapBytes, ref index);
        Protocol.Deserialize(out numCols, mapBytes, ref index);
        MapData map = new MapData(numRows, numCols);
        // Deserialise grid
        for (int i = 0; i < numRows; i++)
        {
            for (int j = 0; j < numCols; j++)
            {
                map.getTileData(i, j).deserializeFrom(mapBytes, ref index);
            }
        }
        // Deserialize Path
        int pathLength;
        Protocol.Deserialize(out pathLength, mapBytes, ref index);
        List<TileData> path = map.getPath();
        for (int i = 0; i < pathLength; i++)
        {
            Coord c = Coord.deserialize(mapBytes, ref index);
            path.Add(map.getTileData(c.row, c.col));
        }
        return map;
    }

    public byte[] serializePlay()
    {
        int size = numRows * numCols * TileData.serialize_size; // For grid
        byte[] mapBytes = new byte[size];
        int index = 0;
        // Serialise grid
        for (int i = 0; i < numRows; i++)
        {
            for (int j = 0; j < numCols; j++)
            {
                getTileData(i, j).serializeTo(mapBytes, ref index);
            }
        }
        string x = "";
        for (int i = 0; i < size; ++i) { x += mapBytes[i]; } // DEBUG
        UnityEngine.Debug.Log(x);
        return mapBytes;
    }

    public void deserializePlay(byte[] mapBytes)
    {
        int index = 0;
        // Deserialise grid
        for (int i = 0; i < numRows; i++)
        {
            for (int j = 0; j < numCols; j++)
            {
                this.getTileData(i, j).deserializeFrom(mapBytes, ref index);
            }
        }
        string x = "";
        for (int i = 0; i < mapBytes.Length; ++i) { x += mapBytes[i]; } // DEBUG
        UnityEngine.Debug.Log(x);
    }
    #endregion
}



public class TileData
{
    public enum State { EMPTY, TOWER, PATH, OBSTACLE }
    public enum Direction { UNSET, UP, DOWN, LEFT, RIGHT }

    public Coord coord;
    public State state; // Default EMPTY
    public Direction startDirection; // Default UNSET
    public Direction endDirection; // Default UNSET
    public int towerType; // Default 0. 0 is no tower.

    public const int serialize_size = 15; // Coord: 2*4byte (int). State + Direction: converted into 1 byte. 3*1 byte. towerType: 4byte (int).

    #region serialisation
    public int serializeTo(byte[] dest, ref int index)
    {
        coord.serializeTo(dest, ref index);
        dest[index] = (byte)state;
        ++index;
        dest[index] = (byte)startDirection;
        ++index;
        dest[index] = (byte)endDirection;
        ++index;
        Protocol.Serialize(towerType, dest, ref index);
        return serialize_size;
    }

    public void deserializeFrom(byte[] from, ref int index)
    {
        coord = Coord.deserialize(from, ref index);
        state = (TileData.State)from[index];
        ++index;
        startDirection = (TileData.Direction)from[index];
        ++index;
        endDirection = (TileData.Direction)from[index];
        ++index;
        Protocol.Deserialize(out towerType, from, ref index);
    }
    #endregion
}