using System.Collections.Generic;



public class MapData {

    public int numRows;
    public int numCols;

    private List<TileData> path;
    private TileData[,] grid;

    public MapData(int i, int j) {
        numRows = i;
        numCols = j;
        grid = new TileData[numRows, numCols];
        path = new List<TileData>();
    }

    public TileData getTileData(int i, int j) {
        if (i < 0 || j < 0 || i >= grid.GetLength(0) || j >= grid.GetLength(1)) {
            return null;
        }
        return grid[i, j];
    }

    public List<TileData> getPath() {
        return path;
    }

    public void setGrid(BuildTile[,] buildGrid) {
        if (buildGrid.GetLength(0) != numRows || buildGrid.GetLength(1) != numCols) {
            throw new System.Exception("Grid size mismatch.");
        }
        for (int i = 0; i < numRows; i++) {
            for (int j = 0; j < numCols; j++) {
                grid[i, j] = buildGrid[i, j].tileData;
            }
        }
    }

    public void setPath(List<BuildTile> buildPath) {
        foreach (BuildTile tile in buildPath) {
            path.Add(tile.tileData);
        }
    }
}



public class TileData {
    public enum State { EMPTY, TOWER, PATH, OBSTACLE }
    public enum Direction { UNSET, UP, DOWN, LEFT, RIGHT }

    public Coord coord;
    public State state; // Default EMPTY
    public Direction startDirection; // Default UNSET
    public Direction endDirection; // Default UNSET
}
