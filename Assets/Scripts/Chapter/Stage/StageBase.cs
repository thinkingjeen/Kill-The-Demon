using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class StageBase : MonoBehaviour
{
    public Player player;
    public Transform playerStartPosition;
    public Transform portal;

    public Tilemap[] tilemaps;
    public Tilemap[] WallTilemaps;
    public Tilemap fieldTile;
    public TileInfo[,] tileInfoArray;

    public Vector2Int bl { get; private set; }
    public Vector2Int tr { get; private set; }
    public int sizeX { get; private set; }
    public int sizeY { get; private set; }
    public virtual void Init(Player player)
    {
        // 타일 데이터 2차원 배열 사이즈 설정
        foreach (Tilemap t in tilemaps)
        {
            t.CompressBounds();
            if (bl.x > t.cellBounds.xMin || bl.y > t.cellBounds.yMin)
            {
                bl = (Vector2Int)t.cellBounds.min;
            }
            if (tr.x < t.cellBounds.xMax || tr.y < t.cellBounds.yMax)
            {
                tr = (Vector2Int)t.cellBounds.max;
            }
        }
        /// bl = [0, 0]
        /// tr = [sizeX - 1, sizeY - 1]

        sizeX = tr.x - bl.x; // 타일맵 가로 크기
        sizeY = tr.y - bl.y; // 타일맵 세로 크기

        tileInfoArray = new TileInfo[sizeX, sizeY];
        for (int i = 0; i < tileInfoArray.GetLength(0); i++)
        {
            for (int j = 0; j < tileInfoArray.GetLength(1); j++)
            {
                tileInfoArray[i, j] = new();
                tileInfoArray[i, j].location = new Vector2Int(i, j);
                tileInfoArray[i, j].position = tileInfoArray[i, j].location + bl;
            }
        }

        // 타일 데이터에 벽 위치 추가
        foreach (Tilemap t in WallTilemaps) WallTileScan(t);

        // 플레이어 배치
        player.transform.position = playerStartPosition.position - new Vector3(0, 0.3f, 0);
        player.location = Vector2Int.RoundToInt(playerStartPosition.position) - bl;

        // 타일 데이터에 플레이어 위치 추가
        tileInfoArray[player.location.x, player.location.y].objId = ConstantIDs.PLAYER; //player test id
        this.player = player;

        // 타일 데이터에 포탈 아이디 추가
        if(portal !=null)
            tileInfoArray[Mathf.RoundToInt(portal.position.x) - bl.x, Mathf.RoundToInt(portal.position.y) - bl.y].objId = ConstantIDs.CLOSED_PORTAL;

    }
    public void WallTileScan(Tilemap tilemap)
    {
        for (int i = bl.x; i < tr.x; i++)
        {
            for (int j = bl.y; j < tr.y; j++)
            {
                if (tilemap.HasTile(new Vector3Int(i, j)))
                {
                    tileInfoArray[i - bl.x, j - bl.y].objId = ConstantIDs.WALL; // wall tile id
                }
            }
        }
    }
    public void PortalApear()
    {
        if (portal != null)
        {
            portal.gameObject.SetActive(true);
            tileInfoArray[Mathf.RoundToInt(portal.position.x) - bl.x, Mathf.RoundToInt(portal.position.y) - bl.y].objId = ConstantIDs.OPENED_PORTAL;
        }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="location">타일의 로컬 트랜스폼 포지션</param>
    /// <returns></returns>
    public IEnumerator ChangeTileColor(Vector2Int location)
    {
        fieldTile.SetTileFlags((Vector3Int)location, TileFlags.None);
        fieldTile.SetColor((Vector3Int)location, new Color(1, 1, 0, 0.5f));
        yield return new WaitForSeconds(0.2f);
        if (fieldTile == null) yield break;
        fieldTile.SetColor((Vector3Int)location, Color.white);
    }

    /// <summary>
    /// 플레이어 기준 4방향 TileInfo 체크
    /// </summary>
    /// <returns></returns>
    public TileInfo[] Check4D()
    {
        TileInfo[] check = new TileInfo[4];

        check[0] = tileInfoArray[player.location.x, player.location.y + 1];
        check[1] = tileInfoArray[player.location.x, player.location.y - 1];
        check[2] = tileInfoArray[player.location.x - 1, player.location.y];
        check[3] = tileInfoArray[player.location.x + 1, player.location.y];

        return check;
    }

    /// <summary>
    /// 현재 스테이지의 objId 배열을 콘솔 창에 출력
    /// </summary>
    public void PrintTileInfo()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            string str = "";
            for (int j = tileInfoArray.GetLength(1) - 1; j >= 0; j--)
            {
                str += "| ";
                for (int i = 0; i < tileInfoArray.GetLength(0); i++)
                {
                    str += tileInfoArray[i, j].objId + "\t|";
                }
                str += "\n";
            }

            Debug.Log(str);
        }
    }

    /// <summary>
    /// 입력된 좌표를 (0, 0)을 중심으로 입력된 방향으로 회전
    /// </summary>
    /// <param name="coord">입력 좌표</param>
    /// <param name="direction">회전 각도</param>
    /// <returns>회전 변환된 좌표</returns>
    public Vector2Int RotationalTransform(Vector2Int coord, eDirection direction)
    {
        float angle = 0;

        if (direction == eDirection.none) direction = player.dir;

        switch (direction)
        {
            case eDirection.up: angle = 0; break;               // 0°
            case eDirection.down: angle = Mathf.PI; ; break;    // 180°
            case eDirection.left: angle = Mathf.PI / 2; break;  // 90°
            case eDirection.right: angle = -(Mathf.PI / 2); break;// -90° = 270°
        }

        Vector2 rotatedCoord = new Vector2(
            coord.x * Mathf.Cos(angle) - coord.y * Mathf.Sin(angle),
            coord.x * Mathf.Sin(angle) + coord.y * Mathf.Cos(angle)); // 각도에 맞춰 좌표 회전 변환

        return Vector2Int.RoundToInt(rotatedCoord);
    }
}
