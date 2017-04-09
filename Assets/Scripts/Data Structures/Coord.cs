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

    #region serialisation
    public int serializeTo(byte[] dest, ref int index) {
        ExitGames.Client.Photon.Protocol.Serialize(row, dest, ref index);
        ExitGames.Client.Photon.Protocol.Serialize(col, dest, ref index);
        return 8;
    }

    public static Coord deserialize(byte[] from, ref int index) {
        int d_row;
        int d_col;
        ExitGames.Client.Photon.Protocol.Deserialize(out d_row, from, ref index);
        ExitGames.Client.Photon.Protocol.Deserialize(out d_col, from, ref index);
        return new Coord(d_row, d_col);
    }
    #endregion
}
