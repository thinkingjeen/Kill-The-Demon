using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
public class VillageMain : MonoBehaviour
{
    private int cntWalk = 0;
    public Transform portal;

    public Player player;
    public PlayerController playerController;

    public Tilemap[] walls;

    public VillageUIMain villageUIMain;
    
    TileInfo[,] tileInfoArray;

    public NPCManager npcManager;

    Vector2Int bl;
    Vector2Int tr;

    public AudioClip villageAudio;

    private void Awake()
    {
        App.instance.villageMain = this;
        
    }
    void Start()
    {
        App.instance.BGAudioPlay(this.villageAudio);
        playerController.SetPlayerMovable();
        bl = new(walls[0].cellBounds.xMin, walls[0].cellBounds.yMin); // Bottom Left
        tr = new(walls[0].cellBounds.xMax, walls[0].cellBounds.yMax); // Top Right

        int sizeX = tr.x - bl.x; // 타일맵 가로 크기
        int sizeY = tr.y - bl.y; // 타일맵 세로 크기

        //타일맵 크기와 동일한 2차원 배열 할당 
        tileInfoArray = new TileInfo[sizeX, sizeY];
        Debug.LogFormat("0:{0},1:{1}", tileInfoArray.GetLength(0), tileInfoArray.GetLength(1));
        for(int i = 0; i < tileInfoArray.GetLength(0); i++)
        {
            for(int j = 0; j < tileInfoArray.GetLength(1); j++)
            {
                tileInfoArray[i, j] = new ();
                tileInfoArray[i, j].location = new Vector2Int(i, j);
            }
        }

        TileScan(walls[0]);

        player.location.x -= bl.x;
        player.location.y -= bl.y;
        tileInfoArray[player.location.x, player.location.y].objId = ConstantIDs.PLAYER; // player test id

        tileInfoArray[1 - bl.x, 1 - bl.y].objId = 14; //dummy npc for test
        tileInfoArray[Mathf.RoundToInt(npcManager.npcs[1].transform.position.x) - bl.x, Mathf.RoundToInt(npcManager.npcs[1].transform.position.y) - bl.y].objId = 13;
        Debug.Log(player.location);
        tileInfoArray[Mathf.RoundToInt(portal.position.x) - bl.x, Mathf.RoundToInt(portal.position.y) - bl.y].objId = ConstantIDs.OPENED_PORTAL; // portal test id

        playerController.onMoveAction += (direction) =>
        {
            PlayerMove(direction);
        };
        playerController.onConversationAction += () =>
        {
            int[] check = Check4D();
            foreach (int id in check)
            {
                Debug.Log(id);
                if (11 <= id && id <= 99)
                {
                    villageUIMain.NPCConversation(id);
                    break;
                }
                else playerController.SetPlayerMovable();

            }
        };

        this.villageUIMain.onWeaponSelectCancelAction += () => { 
            playerController.SetPlayerMovable();
        };
        this.villageUIMain.onWeaponSelectCompleteAction += () => 
        {
            int maxHp = 1500/*임시값*/;
            InfoManager.instance.gameInfo.maxHp = maxHp + (int)(InfoManager.instance.playerInfo.stats[0] * 100);
            InfoManager.instance.gameInfo.hp = InfoManager.instance.gameInfo.maxHp;
            InfoManager.instance.recordInfo.SavePreviousStatus();
            App.instance.LoadChapterScene();
        };
        villageUIMain.onUnlockShopCloseAction += () => { playerController.SetPlayerMovable(); };
        villageUIMain.onStatShopCloseAction += () => { playerController.SetPlayerMovable(); };
    }

    void TileScan(Tilemap tilemap)
    {
        int sizeX = tr.x - bl.x; // 타일맵 가로 크기
        int sizeY = tr.y - bl.y; // 타일맵 세로 크기

        for (int i = bl.x; i < tr.x; i++)
        {
            for (int j = bl.y; j < tr.y; j++)
            {
                if (tilemap.HasTile(new Vector3Int(i, j)))
                {
                    /// bl => (0, 0)
                    /// tr => (sizeX - 1, sizeY - 1)
                    tileInfoArray[i - bl.x, j - bl.y].objId = 1;
                }
            }
        }
    }

    void PlayerMove(eDirection direction)
    {   
        int id = IdCheck(direction);
        if (id == 0)
        {
            tileInfoArray[player.location.x, player.location.y].objId = 0;
            switch (direction)
            {
                case eDirection.up: player.location.y += 1; break; // 상
                case eDirection.down: player.location.y -= 1; break; // 하
                case eDirection.left: player.location.x -= 1; break; // 좌
                case eDirection.right: player.location.x += 1; break; // 우
            }
            tileInfoArray[player.location.x, player.location.y].objId = ConstantIDs.PLAYER;
            this.cntWalk++;
            player.Move(this.bl);
        }
        else if(id == ConstantIDs.OPENED_PORTAL)
        {
            villageUIMain.weaponSelectPopUp.gameObject.SetActive(true);
        }
        else
        {
            playerController.SetPlayerMovable();
        }

        int[] ids = Check4D();
        if (ids.Any(x =>  11 <= x && x <= 99))
        {
            playerController.uiJoysticks.ConversationButtonApear();
        }
        else
        {
            playerController.uiJoysticks.ConversationButtonDisapear();
        }
    }

    int[] Check4D()
    {
        int[] check = new int[4];

        check[0] = tileInfoArray[player.location.x, player.location.y + 1].objId;
        check[1] = tileInfoArray[player.location.x, player.location.y - 1].objId;
        check[2] = tileInfoArray[player.location.x - 1, player.location.y].objId;
        check[3] = tileInfoArray[player.location.x + 1, player.location.y].objId;

        return check;
    }

    int IdCheck(eDirection direction)
    {
        int playerX = player.location.x;//Mathf.RoundToInt(player.transform.position.x) - bl.x;
        int playerY = player.location.y;//Mathf.RoundToInt(player.transform.position.y) - bl.y;

        int id = 0;
        switch (direction)
        {
            case eDirection.up:
                if (playerY < tileInfoArray.GetLength(1)) id = tileInfoArray[playerX, playerY + 1].objId;
                break; //상
            case eDirection.down:
                if (playerY > 0) id = tileInfoArray[playerX, playerY - 1].objId;
                break; //하
            case eDirection.left:
                if (playerX < tileInfoArray.GetLength(0)) id = tileInfoArray[playerX - 1, playerY].objId;
                break; //좌
            case eDirection.right:
                if (playerX > 0) id = tileInfoArray[playerX + 1, playerY].objId;
                break; //우
        }
        return id;
    }
}
