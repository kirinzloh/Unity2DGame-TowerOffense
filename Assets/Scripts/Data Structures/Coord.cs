using System.Collections;
using System.Collections.Generic;

public struct Coord {
    private readonly int _row;
    private readonly int _col;

    public Coord(int row, int col) {
        this._row = row;
        this._col = col;
    }

    public int row { get { return _row; } }
    public int col { get { return _col; } }
}
