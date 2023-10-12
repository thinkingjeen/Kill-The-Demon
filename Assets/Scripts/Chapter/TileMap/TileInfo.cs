using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileInfo
{
    public int objId;
    public TileInfo parentTile;
    public int G;
    public int H;
    public int F { get { return G + H; } }
    /// <summary>
    /// 배열 상의 좌표
    /// </summary>
    public Vector2Int location;
    /// <summary>
    /// 월드 포지션 좌표
    /// </summary>
    public Vector2Int position;
}
