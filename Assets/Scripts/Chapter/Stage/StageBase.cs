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
        // Ÿ�� ������ 2���� �迭 ������ ����
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

        sizeX = tr.x - bl.x; // Ÿ�ϸ� ���� ũ��
        sizeY = tr.y - bl.y; // Ÿ�ϸ� ���� ũ��

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

        // Ÿ�� �����Ϳ� �� ��ġ �߰�
        foreach (Tilemap t in WallTilemaps) WallTileScan(t);

        // �÷��̾� ��ġ
        player.transform.position = playerStartPosition.position - new Vector3(0, 0.3f, 0);
        player.location = Vector2Int.RoundToInt(playerStartPosition.position) - bl;

        // Ÿ�� �����Ϳ� �÷��̾� ��ġ �߰�
        tileInfoArray[player.location.x, player.location.y].objId = ConstantIDs.PLAYER; //player test id
        this.player = player;

        // Ÿ�� �����Ϳ� ��Ż ���̵� �߰�
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
    /// <param name="location">Ÿ���� ���� Ʈ������ ������</param>
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
    /// �÷��̾� ���� 4���� TileInfo üũ
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
    /// ���� ���������� objId �迭�� �ܼ� â�� ���
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
    /// �Էµ� ��ǥ�� (0, 0)�� �߽����� �Էµ� �������� ȸ��
    /// </summary>
    /// <param name="coord">�Է� ��ǥ</param>
    /// <param name="direction">ȸ�� ����</param>
    /// <returns>ȸ�� ��ȯ�� ��ǥ</returns>
    public Vector2Int RotationalTransform(Vector2Int coord, eDirection direction)
    {
        float angle = 0;

        if (direction == eDirection.none) direction = player.dir;

        switch (direction)
        {
            case eDirection.up: angle = 0; break;               // 0��
            case eDirection.down: angle = Mathf.PI; ; break;    // 180��
            case eDirection.left: angle = Mathf.PI / 2; break;  // 90��
            case eDirection.right: angle = -(Mathf.PI / 2); break;// -90�� = 270��
        }

        Vector2 rotatedCoord = new Vector2(
            coord.x * Mathf.Cos(angle) - coord.y * Mathf.Sin(angle),
            coord.x * Mathf.Sin(angle) + coord.y * Mathf.Cos(angle)); // ������ ���� ��ǥ ȸ�� ��ȯ

        return Vector2Int.RoundToInt(rotatedCoord);
    }
}
