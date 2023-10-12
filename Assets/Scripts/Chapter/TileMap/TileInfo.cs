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
    /// �迭 ���� ��ǥ
    /// </summary>
    public Vector2Int location;
    /// <summary>
    /// ���� ������ ��ǥ
    /// </summary>
    public Vector2Int position;
}
