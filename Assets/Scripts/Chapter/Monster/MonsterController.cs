using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;
using DG.Tweening;
using GooglePlayGames.BasicApi;

public class MonsterController : MonoBehaviour
{
    public struct Situation
    {
        public int situation;
        public eDirection dir;
        public float magnititude;
    }

    private StageMain stageMain;
    public GameObject[] AlertPool;
    private int alertPoolCursor = 0;
    private int alertPoolCnt = 300;

    public UnityAction onPlayerRestrictAction;
    public UnityAction onPlayerUnrestrictAction;
    public UnityAction<int, eDirection> onPlayerKnockbackAction;//int: 넉백 거리 , eDirection: 방향

    public EffectManager effectManager;

    public void Init(StageMain stageMain)
    {

        this.stageMain = stageMain;
        foreach (Monster mon in this.stageMain.monsters)
        {
            mon.onMonsterGenerateMoveAction += () =>
            {
                Situation curSit = JudgeSituation(mon);
                int resultBehavior = mon.MonsterGenerateMove(curSit.situation, curSit.dir, curSit.magnititude, stageMain.player.location);
                switch (resultBehavior)
                {
                    case -1: break; //안움직임
                    case 0: MoveAstar(stageMain.player.location, mon); break;
                    case 1: StraightLineAstar(mon); break; //플레이어의 XY축중에 가까운 일직선상으로 Astar

                    /****chapter 1 boss****/
                    case 10: Boss1Skill_1(mon, stageMain.player.location); break; // 플레이어 위치에 범위 공격
                    case 11: Boss1Skill_2(mon); break; // 범위 공격
                    case 12: Boss1Skill_3(mon); break; // 텔레포트 후 공격
                    case 13: Boss1Skill_4(mon); break; // 충격파
                    case 14: Boss1Skill_5(mon); break; // 돌진
                    /****chapter 1 boss****/
                    /****chapter 2 boss****/
                    case 15: Boss2Skill_1(mon); break; // 도약 공격
                    case 16: Boss2Skill_2(mon); break; // 넉백
                    case 17: Boss2Skill_3(mon); break; // 검기
                    case 18: Boss2Skill_4(mon); break; // 전방 찌르기
                    case 19: Boss2Skill_5(mon); break; // 방패 던지기
                    /****chapter 2 boss****/
                    /****chapter 3 boss****/
                    case 20: Boss3Skill_1(mon); break; // 플레이어 근처로 이동 후 찌르기
                    case 21: Boss3Skill_2(mon); break; // 플레이어 근처로 이동 후 베기
                    case 22: Boss3Skill_3(mon); break; // 플레이어 위치에 예고 공격
                    case 23: Boss3Skill_4(mon); break; // 돌진 * 4
                    /****chapter 3 boss****/
                    /****chapter 3 boss phase 2****/
                    case 24: BossFinalSkill_1(mon); break; // 플레이어 근처로 이동 후 베기
                    case 25: BossFinalSkill_2(mon); break; // 플레이어 근처로 이동 후 찌르기
                    case 26: BossFinalSkill_3(mon); break; // 무작위 지점 예고 공격 * 3
                    case 27: BossFinalSkill_4(mon); break; // 몬스터 주변 점점 넓어지는 폭발 * 4
                    case 28: BossFinalSkill_5(mon); break; // 8방향 투사체
                    /****chapter 3 boss phase 2****/

                    case 2400: MonsterSkill2400(mon, curSit.dir, mon.nAttackDelay); break; //근접몬스터의 근접공격
                    case 2405: MonsterSkill2405(mon); break; //낙석
                    case 2406: MonsterSkill2406(mon, curSit.dir); break; //투사체
                    case 2408: MonsterSkill2408(mon, curSit.dir); break; //레이저
                    case 2409: MonsterSkill2409(mon, curSit.dir); break; //나무정령 전방폭발
                    case 2410: MonsterSkill2410(mon, curSit.dir); break; //매직슬라임 폭발투사체
                    case 2411: MonsterSkill2411(mon); break; //매직슬라임 점프
                    case 2412: MonsterSkill2412(mon, curSit.dir, mon.nAttackDelay); break; //투사체몹 투사체
                    case 2413: MonsterSkill2413(mon); break; //터렛식물 공격
                    case 2407: MonsterSkill2407(mon, curSit.dir); break; //얼음귀신 투사체
                    case 2414: MonsterSkill2414(mon, curSit.dir); break; //얼음귀신 평타
                    case 2415: MonsterSkill2415(mon); break; //얼음귀신 범위공격
                    case 1557: MonsterSkill1557(mon, curSit.dir); break; //루비스톤 평타
                    case 2416: MonsterSkill2416(mon, curSit.dir); break; //루비스톤 강화평타
                    case 15571: MonsterSkill1557_1(mon, eDirection.up); break; //루비스톤 위 평타
                    case 24161: MonsterSkill2416_1(mon, eDirection.up); break; //루비스톤 위 강화평타
                    case 2417: MonsterSkill2417(mon, curSit.dir); break; //루비스톤 돌진
                    case 2418: MonsterSkill2418(mon); break; //루비스톤 맵 파도
                    case 2419: MonsterSkill2419(mon); break; //소서리스 메태오
                    case 2420: MonsterSkill2420(mon); break; //소서리스 투사체
                    case 2421: MonsterSkill2421(mon); break; //소서리스 레이저
                    case 2422: MonsterSkill2422(mon); break; //소서리스 플레이어 위치 공격
                    case 2423: MonsterSkill2423(mon); break; //아이스봄 자폭
                    case 2424: MonsterSkill2424(mon); break; //얼음정령, 마왕군마법사 공격
                    case 2425: MonsterSkill2425(mon, curSit.dir, mon.nAttackDelay); break; //예티 공격, 마왕군전사 공격
                    case 2426: MonsterSkill2426(mon, curSit.dir); break; //마법골렘 공격

                    case 2801: MonsterSkill2801(mon, curSit.dir); break; //아이스골렘 8방향투사체
                    case 2802: MonsterSkill2802(mon, curSit.dir); break; //아이스골렘 전방3열 투사체
                    case 2803: MonsterSkill2803(mon, curSit.dir); break; //아이스골렘 예고공격
                }
            };
        }
        this.AlertPool = new GameObject[this.alertPoolCnt];
        for (int i = 0; i < this.alertPoolCnt; i++)
        {
            this.AlertPool[i] = Instantiate(stageMain.alertPrefab, new Vector3(0, 0), Quaternion.identity);
            this.AlertPool[i].SetActive(false);
        }
    }
    /// <summary>
    ///  situaion구조체의 situaiton : 0 => 일직선상 장애물X, situation : 1 => 일직선상 장애물O, situation : 2 => 일직선상 X
    ///     situation 구조체의 dir은 몬스터가 투사체 공격시 바라볼 플레이어의 방향
    ///     situation 구조체의 magnititude는 몬스터와 플레이어 사이의 거리
    /// </summary>
    /// <param name="mon"></param>
    /// <returns></returns>
    private Situation JudgeSituation(Monster mon)
    {
        Situation currentSit = new();
        Vector2Int playerLoc = stageMain.player.location;
        currentSit.magnititude = Mathf.Round((playerLoc - mon.location).magnitude * 10) * 0.1f;  //소숫점 첫째자리까지 넘기기
        if (playerLoc.x != mon.location.x && playerLoc.y != mon.location.y)
        {
            currentSit.situation = 2;
            currentSit.dir = eDirection.none;
        }
        else
        {
            if (playerLoc.y > mon.location.y) { currentSit.dir = eDirection.up; }
            else if (playerLoc.y < mon.location.y) { currentSit.dir = eDirection.down; }
            else if (playerLoc.x > mon.location.x) { currentSit.dir = eDirection.right; }
            else if (playerLoc.x < mon.location.x) { currentSit.dir = eDirection.left; }

            if (CheckFront(mon.location, currentSit.dir, Mathf.RoundToInt(currentSit.magnititude)))
            {
                currentSit.situation = 1;
            }
            else
            {
                currentSit.situation = 0;
            }
        }
        return currentSit;
    }

    public void MonsterMove(Vector2Int location, Monster mon, bool domove = false, float domoveTime = 0f)
    {
        if (mon.sizeX == 0 && mon.sizeY == 0)
        {
            if (domove)
            {
                mon.transform.DOMove(new Vector3(stageMain.tileInfoArray[location.x, location.y].position.x, stageMain.tileInfoArray[location.x, location.y].position.y), domoveTime);
            }
            else
            {
                mon.gameObject.transform.position = new Vector3(stageMain.tileInfoArray[location.x, location.y].position.x, stageMain.tileInfoArray[location.x, location.y].position.y);
            }
        }
        else
        {
            if (mon.sizeX > 0)
            {
                if (domove)
                {
                    mon.transform.DOMove(new Vector3(stageMain.tileInfoArray[location.x, location.y].position.x, stageMain.tileInfoArray[location.x, location.y].position.y), domoveTime);
                }
                else
                {
                    mon.gameObject.transform.position = new Vector3(stageMain.tileInfoArray[location.x, location.y].position.x, stageMain.tileInfoArray[location.x, location.y].position.y);
                }
            }
            else
            {
                if (domove)
                {
                    mon.transform.DOMove(new Vector3(stageMain.tileInfoArray[location.x + mon.sizeX, location.y].position.x, stageMain.tileInfoArray[location.x + mon.sizeX, location.y].position.y), domoveTime);
                }
                else
                {
                    mon.gameObject.transform.position = new Vector3(stageMain.tileInfoArray[location.x + mon.sizeX + 1, location.y].position.x, stageMain.tileInfoArray[location.x + mon.sizeX + 1, location.y].position.y);
                }
            }
        }
        if (mon.id < 400)
        {
            this.DirChange(location, mon);
        }
        int temp = stageMain.tileInfoArray[mon.location.x, mon.location.y].objId;

        if (mon.sizeX == 0 && mon.sizeY == 0)
        {
            stageMain.tileInfoArray[location.x, location.y].objId = temp;
            stageMain.tileInfoArray[mon.location.x, mon.location.y].objId = 0;
        }
        else
        {
            if (mon.sizeX > 0)
            {
                for (int i = 0; i < Mathf.Abs(mon.sizeX); ++i)
                {
                    for (int j = 0; j < Mathf.Abs(mon.sizeY); ++j)
                    {
                        stageMain.tileInfoArray[mon.location.x + i, mon.location.y + j].objId = 0;
                    }
                }
                for (int i = 0; i < Mathf.Abs(mon.sizeX); ++i)
                {
                    for (int j = 0; j < Mathf.Abs(mon.sizeY); ++j)
                    {
                        stageMain.tileInfoArray[location.x + i, location.y + j].objId = temp;
                    }
                }
            }
            else
            {
                for (int i = 0; i < Mathf.Abs(mon.sizeX); ++i)
                {
                    for (int j = 0; j < Mathf.Abs(mon.sizeY); ++j)
                    {
                        stageMain.tileInfoArray[mon.location.x - i, mon.location.y + j].objId = 0;
                    }
                }
                for (int i = 0; i < Mathf.Abs(mon.sizeX); ++i)
                {
                    for (int j = 0; j < Mathf.Abs(mon.sizeY); ++j)
                    {
                        stageMain.tileInfoArray[location.x - i, location.y + j].objId = temp;
                    }
                }
            }
        }
        mon.location = location;
    }

    public bool MonsterMovable(Vector2Int location, Monster mon, bool overlapPlayer = false)
    {
        int temp = stageMain.tileInfoArray[mon.location.x, mon.location.y].objId;
        if (mon.sizeX == 0 && mon.sizeY == 0)
        {
            if (location.x >= 0 && location.x < stageMain.tileInfoArray.GetLength(0) && location.y >= 0 && location.y < stageMain.tileInfoArray.GetLength(1))
            {
                if (stageMain.tileInfoArray[location.x, location.y].objId == ConstantIDs.BLANK) return true;
                else if (overlapPlayer == true && stageMain.tileInfoArray[location.x, location.y].objId == ConstantIDs.PLAYER) return true;
                else return false;
            }
            else return false;
        }
        else
        {
            if (mon.sizeX > 0)
            {
                for (int i = 0; i < Mathf.Abs(mon.sizeX); ++i)
                {
                    for (int j = 0; j < Mathf.Abs(mon.sizeY); ++j)
                    {
                        if (location.x + i >= 0 && location.x + i < stageMain.tileInfoArray.GetLength(0) && location.y + j >= 0 && location.y + j < stageMain.tileInfoArray.GetLength(1))
                        {
                            if (overlapPlayer)
                            {
                                if (stageMain.tileInfoArray[location.x + i, location.y + j].objId != ConstantIDs.BLANK && stageMain.tileInfoArray[location.x + i, location.y + j].objId != ConstantIDs.PLAYER && stageMain.tileInfoArray[location.x + i, location.y + j].objId != temp) return false;
                            }
                            else
                            {
                                if (stageMain.tileInfoArray[location.x + i, location.y + j].objId != ConstantIDs.BLANK && stageMain.tileInfoArray[location.x + i, location.y + j].objId != temp) return false;
                            }
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
            }
            else
            {
                for (int i = 0; i < Mathf.Abs(mon.sizeX); ++i)
                {
                    for (int j = 0; j < Mathf.Abs(mon.sizeY); ++j)
                    {
                        if (location.x - i >= 0 && location.x - i < stageMain.tileInfoArray.GetLength(0) && location.y + j >= 0 && location.y + j < stageMain.tileInfoArray.GetLength(1))
                        {
                            if (overlapPlayer)
                            {
                                if (stageMain.tileInfoArray[location.x - i, location.y + j].objId != ConstantIDs.BLANK && stageMain.tileInfoArray[location.x - i, location.y + j].objId != ConstantIDs.PLAYER && stageMain.tileInfoArray[location.x + i, location.y + j].objId != temp) return false;
                            }
                            else
                            {
                                if (stageMain.tileInfoArray[location.x - i, location.y + j].objId != ConstantIDs.BLANK && stageMain.tileInfoArray[location.x + i, location.y + j].objId != temp) return false;
                            }
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }
    }

    private void StraightLineAstar(Monster mon)
    {
        int disX = stageMain.player.location.x - mon.location.x;
        int disY = stageMain.player.location.y - mon.location.y;

        if ((disX == 0 || disY == 0) && !CheckWall(mon.location, stageMain.player.location))
        {
            mon.movingSpan += mon.monData.moveDelay;
            return;
        }

        Vector2Int selection1;
        Vector2Int selection2;
        eDirection dir1;
        eDirection dir2;
        int magnititude1;
        int magnititude2;

        if (Mathf.Abs(disX) >= Mathf.Abs(disY))
        {
            if (disX > 0) dir1 = eDirection.right;
            else dir1 = eDirection.left;
            if (disY > 0) dir2 = eDirection.up;
            else dir2 = eDirection.down;

            selection1 = new Vector2Int(mon.location.x, stageMain.player.location.y);
            selection2 = new Vector2Int(stageMain.player.location.x, mon.location.y);
        }
        else
        {
            if (disY > 0) dir1 = eDirection.up;
            else dir1 = eDirection.down;
            if (disX > 0) dir2 = eDirection.right;
            else dir2 = eDirection.left;

            selection1 = new Vector2Int(stageMain.player.location.x, mon.location.y);
            selection2 = new Vector2Int(mon.location.x, stageMain.player.location.y);
        }
        magnititude1 = Mathf.RoundToInt((stageMain.player.location - selection1).magnitude);
        magnititude2 = Mathf.RoundToInt((stageMain.player.location - selection2).magnitude);

        if (!CheckFront(selection1, dir1, magnititude1, true, mon))
        {
            Debug.LogFormat("selection1 :{0}", selection1);
            MoveAstar(selection1, mon);
        }
        else if (!CheckFront(selection2, dir2, magnititude2, true, mon))
        {
            Debug.LogFormat("selection2 : {0}", selection2);
            MoveAstar(selection2, mon);
        }
        else
        {
            Debug.Log("justAstar");
            MoveAstar(stageMain.player.location, mon);
        }
    }

    private void MoveAstar(Vector2Int destination, Monster mon)
    {
        TileInfo curTile = stageMain.tileInfoArray[mon.location.x, mon.location.y];

        List<TileInfo> openList = new List<TileInfo>() { curTile };
        List<TileInfo> closedList = new List<TileInfo>();
        List<TileInfo> finalMoveList = new List<TileInfo>();

        while (openList.Count > 0)
        {
            curTile = openList[0];
            for (int i = 1; i < openList.Count; i++)
            {
                if (openList[i].F <= curTile.F && openList[i].H < curTile.H)
                {
                    curTile = openList[i];
                }
            }

            openList.Remove(curTile);
            closedList.Add(curTile);
            //if (mon.sizeX == 0 && mon.sizeY == 0)
            //{
            if (curTile.location == destination)
            {
                TileInfo targetCurTile = curTile;
                while (targetCurTile != stageMain.tileInfoArray[mon.location.x, mon.location.y])
                {
                    finalMoveList.Add(targetCurTile);
                    targetCurTile = targetCurTile.parentTile;
                }
                finalMoveList.Add(stageMain.tileInfoArray[mon.location.x, mon.location.y]);
                finalMoveList.Reverse();

                if (finalMoveList.Count > 1 && (finalMoveList[1].objId == ConstantIDs.BLANK || finalMoveList[1].objId == stageMain.tileInfoArray[mon.location.x, mon.location.y].objId))
                {
                    MonsterMove(finalMoveList[1].location, mon);
                    return;
                }
            }

            openList = OpenListAdd(curTile.location.x, curTile.location.y + 1, openList, closedList, curTile, mon);
            openList = OpenListAdd(curTile.location.x + 1, curTile.location.y, openList, closedList, curTile, mon);
            openList = OpenListAdd(curTile.location.x, curTile.location.y - 1, openList, closedList, curTile, mon);
            openList = OpenListAdd(curTile.location.x - 1, curTile.location.y, openList, closedList, curTile, mon);
        }
    }

    List<TileInfo> OpenListAdd(int checkX, int checkY, List<TileInfo> OpenList, List<TileInfo> ClosedList, TileInfo curTile, Monster mon)
    {
        var tileInfoArray = stageMain.tileInfoArray;
        if (mon.sizeX == 0 && mon.sizeY == 0)
        {
            if (checkX >= 0 && checkX < tileInfoArray.GetLength(0) && checkY >= 0 && checkY < tileInfoArray.GetLength(1) && !ClosedList.Contains(tileInfoArray[checkX, checkY])
                    && (tileInfoArray[checkX, checkY].objId == 0 || tileInfoArray[checkX, checkY].objId == ConstantIDs.PLAYER))
            {
                // 이웃노드에 넣고, 직선은 10, 대각선은 14비용
                TileInfo NeighborTile = tileInfoArray[checkX, checkY];
                int MoveCost = curTile.G + (curTile.location.x - checkX == 0 || curTile.location.y - checkY == 0 ? 10 : 14);

                // 이동비용이 이웃노드G보다 작거나 또는 열린리스트에 이웃노드가 없다면 G, H, ParentNode를 설정 후 열린리스트에 추가
                if (MoveCost < NeighborTile.G || !OpenList.Contains(NeighborTile))
                {
                    NeighborTile.G = MoveCost;
                    NeighborTile.H = (Mathf.Abs(NeighborTile.location.x - stageMain.player.location.x) + Mathf.Abs(NeighborTile.location.y - stageMain.player.location.y)) * 10;
                    NeighborTile.parentTile = curTile;

                    OpenList.Add(NeighborTile);
                }
            }
            return OpenList;
        }
        else
        {
            if (mon.sizeX > 0)
            {
                for (int i = 0; i < Mathf.Abs(mon.sizeX); ++i)
                {
                    for (int j = 0; j < Mathf.Abs(mon.sizeY); ++j)
                    {
                        // 상하좌우 범위를 벗어나고, 닫힌리스트에 있거나, 갈 수 없는 타일이면
                        if (checkX + i < 0 || tileInfoArray.GetLength(0) <= checkX + i || checkY + j < 0 || tileInfoArray.GetLength(1) <= checkY + j || ClosedList.Contains(tileInfoArray[checkX, checkY]))
                        {
                            return OpenList;
                        }
                        else
                        {
                            if (tileInfoArray[checkX + i, checkY + j].objId != ConstantIDs.BLANK && tileInfoArray[checkX + i, checkY + j].objId != ConstantIDs.PLAYER && tileInfoArray[checkX + i, checkY + j].objId != tileInfoArray[mon.location.x, mon.location.y].objId)
                            {
                                return OpenList;
                            }
                        }
                    }
                }

                TileInfo NeighborTile = tileInfoArray[checkX, checkY];
                int MoveCost = curTile.G + (curTile.location.x - checkX == 0 || curTile.location.y - checkY == 0 ? 10 : 14);

                // 이동비용이 이웃노드G보다 작거나 또는 열린리스트에 이웃노드가 없다면 G, H, ParentNode를 설정 후 열린리스트에 추가
                if (MoveCost < NeighborTile.G || !OpenList.Contains(NeighborTile))
                {
                    NeighborTile.G = MoveCost;
                    NeighborTile.H = (Mathf.Abs(NeighborTile.location.x - stageMain.player.location.x) + Mathf.Abs(NeighborTile.location.y - stageMain.player.location.y)) * 10;
                    NeighborTile.parentTile = curTile;

                    OpenList.Add(NeighborTile);
                }
                return OpenList;
            }
            else
            {
                for (int i = 0; i < Mathf.Abs(mon.sizeX); ++i)
                {
                    for (int j = 0; j < Mathf.Abs(mon.sizeY); ++j)
                    {
                        // 상하좌우 범위를 벗어나고, 닫힌리스트에 있거나, 갈 수 없는 타일이면
                        if (checkX - i < 0 || tileInfoArray.GetLength(0) <= checkX - i || checkY + j < 0 || tileInfoArray.GetLength(1) <= checkY + j || ClosedList.Contains(tileInfoArray[checkX, checkY]))
                        {
                            return OpenList;
                        }
                        else
                        {
                            if (tileInfoArray[checkX - i, checkY + j].objId != ConstantIDs.BLANK && tileInfoArray[checkX - i, checkY + j].objId != ConstantIDs.PLAYER && tileInfoArray[checkX - i, checkY + j].objId != tileInfoArray[mon.location.x, mon.location.y].objId)
                            {
                                return OpenList;
                            }
                        }
                    }
                }

                TileInfo NeighborTile = tileInfoArray[checkX, checkY];
                int MoveCost = curTile.G + (curTile.location.x - checkX == 0 || curTile.location.y - checkY == 0 ? 10 : 14);

                // 이동비용이 이웃노드G보다 작거나 또는 열린리스트에 이웃노드가 없다면 G, H, ParentNode를 설정 후 열린리스트에 추가
                if (MoveCost < NeighborTile.G || !OpenList.Contains(NeighborTile))
                {
                    NeighborTile.G = MoveCost;
                    NeighborTile.H = (Mathf.Abs(NeighborTile.location.x - stageMain.player.location.x) + Mathf.Abs(NeighborTile.location.y - stageMain.player.location.y)) * 10;
                    NeighborTile.parentTile = curTile;

                    OpenList.Add(NeighborTile);
                }
                return OpenList;
            }
        }
    }

    bool CheckFront(Vector2Int startLoc, eDirection dir, int magnititude, bool self = false, Monster mon = null)
    {
        List<Vector2Int> checkWallCoords = new List<Vector2Int>();
        List<Vector2Int> checkSizeCoords = new List<Vector2Int>();
        if (self)
        {
            if (mon == null)
            {
                checkWallCoords.Add(new Vector2Int(0, 0));
            }
            else if (mon.sizeX == 0 && mon.sizeY == 0)
            {
                checkWallCoords.Add(new Vector2Int(0, 0));
            }
            else
            {
                if (mon.sizeX > 0)
                {
                    for (int i = 0; i < Mathf.Abs(mon.sizeX); ++i)
                    {
                        for (int j = 0; j < Mathf.Abs(mon.sizeY); ++j)
                        {
                            checkSizeCoords.Add(new Vector2Int(i, j));
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < Mathf.Abs(mon.sizeX); ++i)
                    {
                        for (int j = 0; j < Mathf.Abs(mon.sizeY); ++j)
                        {
                            checkSizeCoords.Add(new Vector2Int(-i, j));
                        }
                    }
                }
            }
        }

        for (int i = 1; i < magnititude; ++i)
        {
            checkWallCoords.Add(new Vector2Int(0, i));
        }
        bool checkFront = false;

        foreach (Vector2Int checkCoord in checkWallCoords)
        {
            Vector2Int finalCoord = startLoc + stageMain.RotationalTransform(checkCoord, dir);
            if (stageMain.tileInfoArray[finalCoord.x, finalCoord.y].objId == ConstantIDs.WALL)
            {
                checkFront = true;
                break;
            }
        }
        if (mon != null)
        {
            foreach (Vector2Int checkCoord in checkSizeCoords)
            {
                Vector2Int finalCoord = startLoc + checkCoord;
                if (finalCoord.x < 0 || finalCoord.x >= stageMain.tileInfoArray.GetLength(0) || finalCoord.y < 0 || finalCoord.y >= stageMain.tileInfoArray.GetLength(1))
                {
                    checkFront = true;
                    break;
                }
                else
                {
                    if (stageMain.tileInfoArray[finalCoord.x, finalCoord.y].objId != ConstantIDs.BLANK)
                    {
                        checkFront = true;
                        break;
                    }
                }
            }
        }
        return checkFront;
    }

    bool CheckWall(Vector2Int a, Vector2Int b)
    {
        Vector2Int blLoc = new();
        Vector2Int trLoc = new();

        if (a.x >= b.x) { blLoc.x = b.x; trLoc.x = a.x; }
        else { blLoc.x = a.x; trLoc.x = b.x; }

        if (a.y >= b.y) { blLoc.y = b.y; trLoc.y = a.y; }
        else { blLoc.y = a.y; trLoc.y = b.y; }

        for (int i = 0; i < trLoc.x - blLoc.x + 1; i++)
        {
            for (int j = 0; j < trLoc.y - blLoc.y + 1; j++)
            {
                if (stageMain.tileInfoArray[blLoc.x + i, blLoc.y + j].objId == ConstantIDs.WALL)
                {
                    return true;
                }
            }
        }
        return false;
    }

    private void DirChange(Vector2Int target, Monster mon)
    {
        var dirCheck = target - mon.location;
        if (dirCheck.x >= 1)
        {
            mon.dir = eDirection.right;
        }
        else if (dirCheck.x <= -1)
        {
            mon.dir = eDirection.left;
        }
        else if (dirCheck.y >= 1)
        {
            mon.dir = eDirection.up;
        }
        else if (dirCheck.y <= -1)
        {
            mon.dir = eDirection.down;
        }
    }
    private void UseAlertPool(Monster mon, int x, int y, int skillId, bool isSelf = false, UnityAction callback = null)
    {
        if (this.alertPoolCursor >= this.alertPoolCnt)
        {
            this.alertPoolCursor = 0;
        }
        if (!(x < 0 || x >= stageMain.sizeX || y < 0 || y >= stageMain.sizeY))
        {
            this.AlertPool[this.alertPoolCursor].SetActive(true);
            this.AlertPool[this.alertPoolCursor].transform.position = new Vector3(stageMain.tileInfoArray[x, y].position.x, stageMain.tileInfoArray[x, y].position.y);
            this.AlertPool[this.alertPoolCursor].GetComponent<Alert>().Init(DataManager.instance.dicMonsterSkill[skillId].judgeDelay, mon);
            this.AlertPool[this.alertPoolCursor].GetComponent<Alert>().onEndAction = () =>
            {
                StartCoroutine(stageMain.ChangeTileColor(new Vector2Int(x, y) + stageMain.bl));
                if (stageMain.tileInfoArray[x, y].objId == ConstantIDs.PLAYER)
                {
                    var damage = mon.monData.attack * DataManager.instance.dicMonsterSkill[skillId].damage;
                    //Debug.LogFormat("플레이어에게 {0} 데미지", damage);
                    this.stageMain.onPlayerHitAction(damage);
                    callback?.Invoke(); // == if(callback!=null){callback();}
                }
                if (isSelf && x == mon.location.x && y == mon.location.y)
                {
                    mon.Hit(DataManager.instance.dicMonster[mon.id].attack);
                }
            };
            this.alertPoolCursor++;
        }
    }
    private int MonsterJump(Monster mon)
    {
        int tempId = this.stageMain.tileInfoArray[mon.location.x, mon.location.y].objId;
        //mon.gameObject.SetActive(false);
        ((Flynn)mon).modelColor.color = new Color(1, 1, 1, 0);
        ((Flynn)mon).modelGO.animation.timeScale = 0;
        if (mon.sizeX > 0)
        {
            for (int i = 0; i < Mathf.Abs(mon.sizeX); ++i)
            {
                for (int j = 0; j < Mathf.Abs(mon.sizeY); ++j)
                {
                    stageMain.tileInfoArray[mon.location.x + i, mon.location.y + j].objId = 0;
                }
            }
        }
        else
        {
            for (int i = 0; i < Mathf.Abs(mon.sizeX); ++i)
            {
                for (int j = 0; j < Mathf.Abs(mon.sizeY); ++j)
                {
                    stageMain.tileInfoArray[mon.location.x - i, mon.location.y + j].objId = 0;
                }
            }
        }
        return tempId;
    }
    private void MonsterFall(Vector2Int pos, Monster mon, int monId)// pos = 몬스터가 떨어질 좌표
    {
        //떨어질 위치에 플레이어가 튕겨나갈 떄, 상하좌우 중 벽인 공간이 있는지 확인
        //벽이 없는 쪽으로 플레이어 위치 강제 변경
        //mon.gameObject.SetActive(true);
        ((Flynn)mon).modelColor.color = new Color(1, 1, 1, 1);
        ((Flynn)mon).modelGO.animation.timeScale = -0.5f;
        bool isOverlap = false;
        if (mon.sizeX > 0)
        {
            for (int i = 0; i < Mathf.Abs(mon.sizeX); ++i)
            {
                for (int j = 0; j < Mathf.Abs(mon.sizeY); ++j)
                {
                    if (stageMain.tileInfoArray[pos.x + i, pos.y + j].objId == ConstantIDs.PLAYER)
                    {
                        isOverlap = true;
                        break;
                    }
                }
                if (isOverlap) break;
            }
        }
        else
        {
            for (int i = 0; i < Mathf.Abs(mon.sizeX); ++i)
            {
                for (int j = 0; j < Mathf.Abs(mon.sizeY); ++j)
                {
                    if (stageMain.tileInfoArray[pos.x - i, pos.y + j].objId == ConstantIDs.PLAYER)
                    {
                        isOverlap = true;
                        break;
                    }
                }
                if (isOverlap) break;
            }
        }
        if (isOverlap) // 떨어질 위치에 플레이어가 겹치면
        {
            if (!CheckFront(stageMain.player.location, eDirection.left, 4)) // 왼쪽으로 밀려날수 있음
            {
                this.PlayerForcedMove(new Vector2Int(-3, 0));
            }
            else if (!CheckFront(stageMain.player.location, eDirection.up, 4)) // 위쪽으로 밀려날수 있음
            {
                this.PlayerForcedMove(new Vector2Int(0, 3));
            }
            else if (!CheckFront(stageMain.player.location, eDirection.right, 4)) // 오른쪽으로 밀려날수 있음
            {
                this.PlayerForcedMove(new Vector2Int(3, 0));
            }
            else if (!CheckFront(stageMain.player.location, eDirection.down, 4)) // 아래쪽으로 밀려날수 있음
            {
                this.PlayerForcedMove(new Vector2Int(0, -3));
            }
            else
            {
                Debug.Log("??????");
            }
        }
        if (mon.sizeX > 0)
        {
            for (int i = 0; i < Mathf.Abs(mon.sizeX); ++i)
            {
                for (int j = 0; j < Mathf.Abs(mon.sizeY); ++j)
                {
                    stageMain.tileInfoArray[pos.x + i, pos.y + j].objId = monId;
                }
            }
            mon.location = pos;
            mon.gameObject.transform.position = new Vector3(stageMain.tileInfoArray[pos.x, pos.y].position.x, stageMain.tileInfoArray[pos.x, pos.y].position.y);
        }
        else
        {
            for (int i = 0; i < Mathf.Abs(mon.sizeX); ++i)
            {
                for (int j = 0; j < Mathf.Abs(mon.sizeY); ++j)
                {
                    stageMain.tileInfoArray[pos.x - i, pos.y + j].objId = monId;
                }
            }
            mon.location = pos;
            mon.gameObject.transform.position = new Vector3(stageMain.tileInfoArray[pos.x + mon.sizeX, pos.y].position.x, stageMain.tileInfoArray[pos.x + mon.sizeX, pos.y].position.y);
        }
    }

    private void PlayerForcedMove(Vector2Int dir)
    {
        this.stageMain.tileInfoArray[this.stageMain.player.location.x, this.stageMain.player.location.y].objId = 0;
        this.stageMain.player.location += dir;
        this.stageMain.tileInfoArray[this.stageMain.player.location.x, this.stageMain.player.location.y].objId = ConstantIDs.PLAYER;
        this.stageMain.player.gameObject.transform.position = new Vector3(this.stageMain.tileInfoArray[this.stageMain.player.location.x, this.stageMain.player.location.y].position.x, this.stageMain.tileInfoArray[this.stageMain.player.location.x, this.stageMain.player.location.y].position.y);
    }
    private void MonsterSkill2400(Monster mon, eDirection dir, float time)
    {
        DirChange(stageMain.player.location, mon);
        //애니메이션 실행하고
        //다 알고 때리는 거니까 바로 피격
        //Debug.LogFormat("{0}가 플레이어를 {1}데미지로 공격", mon.monData.name, mon.monData.attack);
        StartCoroutine(this.Skill2400Routine(mon, dir, time));
    }
    private IEnumerator Skill2400Routine(Monster mon, eDirection dir, float time)
    {
        yield return new WaitForSeconds(time);
        //this.stageMain.tileInfoArray[mon.location.x,mon.location.y]
        var rangeList = DataManager.instance.dicAreaSkill.Where((range) => range.Value.skillId == 2400);
        var finalRange = stageMain.RangeMaking(rangeList);

        for (int i = 0; i < finalRange.Count; ++i)
        {
            var range = stageMain.RotationalTransform(finalRange[i], dir);
            var x = mon.location.x + range.x;
            var y = mon.location.y + range.y;
            StartCoroutine(stageMain.ChangeTileColor(mon.location + range + this.stageMain.bl));
            if (x >= 0 && x < this.stageMain.sizeX && y >= 0 && y < this.stageMain.sizeY && this.stageMain.tileInfoArray[x, y].objId == ConstantIDs.PLAYER)
            {
                //Debug.LogFormat("{0}가 플레이어를 {1}데미지로 공격", mon.monData.name, mon.monData.attack);
                this.stageMain.onPlayerHitAction(mon.monData.attack);
            }
        }
    }
    private void MonsterSkill2405(Monster mon)
    {
        StartCoroutine(this.Skill2405Routine(mon));
    }
    private IEnumerator Skill2405Routine(Monster mon)
    {
        yield return new WaitForSeconds(0.5f);

        var rangeList = DataManager.instance.dicAreaSkill.Where((range) => range.Value.skillId == 2405);
        var finalRange = stageMain.RangeMaking(rangeList);

        for (int i = 0; i < finalRange.Count; ++i)
        {
            var x = stageMain.player.location.x + finalRange[i].x;
            var y = stageMain.player.location.y + finalRange[i].y;
            this.UseAlertPool(mon, x, y, 2405);
        }
    }

    private void MonsterSkill2406(Monster mon, eDirection dir)
    {
        StartCoroutine(this.Skill2406Routine(mon, dir));
    }
    private IEnumerator Skill2406Routine(Monster mon, eDirection dir)
    {
        yield return new WaitForSeconds(0.2f);
        stageMain.InstantiateProjectile(mon.location, dir, 2406, mon.id, 1, mon.monData.attack,((Turtle)mon).projectileSprite);
    }

    private void MonsterSkill2408(Monster mon, eDirection dir)
    {
        var rangeList = DataManager.instance.dicAreaSkill.Where((range) => range.Value.skillId == 2408);
        var finalRange = stageMain.RangeMaking(rangeList);

        for (int i = 0; i < finalRange.Count; ++i)
        {
            var range = stageMain.RotationalTransform(finalRange[i], dir);
            var x = mon.location.x + range.x;
            var y = mon.location.y + range.y;
            this.UseAlertPool(mon, x, y, 2408);
        }
    }
    private void MonsterSkill2409(Monster mon, eDirection dir)
    {
        var rangeList = DataManager.instance.dicAreaSkill.Where((range) => range.Value.skillId == 2409);
        var finalRange = stageMain.RangeMaking(rangeList);
        StartCoroutine(this.Skill2409Routine(mon, dir, finalRange));
    }
    private IEnumerator Skill2409Routine(Monster mon, eDirection dir, List<Vector2Int> finalRange)
    {
        int j = 0;
        while (j < 8)
        {
            for (int i = 0; i < finalRange.Count; ++i)
            {
                var range = stageMain.RotationalTransform(finalRange[i] + new Vector2Int(0, j), dir);
                var x = mon.location.x + range.x;
                var y = mon.location.y + range.y;
                this.UseAlertPool(mon, x, y, 2409);
            }
            yield return new WaitForSeconds(0.2f);
            j++;
        }
    }
    private void MonsterSkill2410(Monster mon, eDirection dir)
    {
        StartCoroutine(Skill2410Routine(mon, dir));
    }
    private IEnumerator Skill2410Routine(Monster mon, eDirection dir)
    {
        yield return new WaitForSeconds(0.4f);
        stageMain.InstantiateProjectile(mon.location, dir, 2410, mon.id, 1, mon.monData.attack,((Flynn)mon).projectileSprite);
    }
    private void MonsterSkill2411(Monster mon)
    {
        var rangeList = DataManager.instance.dicAreaSkill.Where((range) => range.Value.skillId == 2411);
        var finalRange = stageMain.RangeMaking(rangeList);

        StartCoroutine(this.Skill2411PrevRoutine(mon, finalRange));
    }
    private IEnumerator Skill2411PrevRoutine(Monster mon, List<Vector2Int> finalRange)
    {
        if (MonsterMovable(stageMain.player.location - new Vector2Int(mon.sizeX / 3, 0), mon, true))
        {
            yield return new WaitForSeconds(0.4f);
            //여기서 데이터상 몬스터는 지도에 없다(하늘로 점프했음)
            var jumpMonId = this.MonsterJump(mon);
            var targetCoord = stageMain.player.location - new Vector2Int(mon.sizeX / 3, 0);
            for (int i = 0; i < finalRange.Count; ++i)
            {
                var x = stageMain.player.location.x + finalRange[i].x;
                var y = stageMain.player.location.y + finalRange[i].y;
                this.UseAlertPool(mon, x, y, 2411);
            }
            StartCoroutine(this.Skill2411Routine(2411, targetCoord, mon, jumpMonId));
        }
        else
        {
            int[] nx = { -1, 0, 1, -1, 0, -1, 0, 1 };
            int[] ny = { 1, 1, 1, 0, 0, -1, -1, -1 };
            for (int i = 0; i < 8; ++i)
            {
                Vector2Int targetCoord = stageMain.player.location - new Vector2Int(mon.sizeX / 3, 0) + new Vector2Int(nx[i], ny[i]);
                if (MonsterMovable(targetCoord, mon, true))
                {
                    yield return new WaitForSeconds(0.4f);
                    //여기서 데이터상 몬스터는 지도에 없다(하늘로 점프했음)
                    var jumpMonId = this.MonsterJump(mon);
                    for (int j = 0; j < finalRange.Count; ++j)
                    {
                        var x = stageMain.player.location.x + nx[i] + finalRange[j].x;
                        var y = stageMain.player.location.y + ny[i] + finalRange[j].y;
                        this.UseAlertPool(mon, x, y, 2411);
                    }
                    StartCoroutine(this.Skill2411Routine(2411, targetCoord, mon, jumpMonId));
                    break;
                }
            }
        }
    }
    private IEnumerator Skill2411Routine(int skillId, Vector2Int pos, Monster mon, int id)
    {
        yield return new WaitForSeconds(DataManager.instance.dicMonsterSkill[skillId].castDelay);
        this.MonsterFall(pos, mon, id); //여기서 다시 데이터상으로 띄우고 플레이어 위치를 옮긴다.
    }
    private void MonsterSkill2412(Monster mon, eDirection dir, float nAttackDelay)
    {
        StartCoroutine(this.Skill2412Routine(mon, dir, nAttackDelay));
    }
    private IEnumerator Skill2412Routine(Monster mon, eDirection dir, float nAttackDelay)
    {
        yield return new WaitForSeconds(nAttackDelay);
        if (mon.id == 102)
        {
            stageMain.InstantiateProjectile(mon.location, dir, 2412, mon.id, 1, mon.monData.attack, ((Spider)mon).projectileSprite);
        }
        else if (mon.id == 110)
        {
            stageMain.InstantiateProjectile(mon.location, dir, 2412, mon.id, 1, mon.monData.attack);
        }
        else
        {
            stageMain.InstantiateProjectile(mon.location, dir, 2412, mon.id, 1, mon.monData.attack);
        }
    }
    private void MonsterSkill2413(Monster mon)
    {
        var rangeList = DataManager.instance.dicAreaSkill.Where((range) => range.Value.skillId == 2410);
        var finalRange = stageMain.RangeMaking(rangeList);

        for (int i = 0; i < finalRange.Count; ++i)
        {
            var x = stageMain.player.location.x + finalRange[i].x;
            var y = stageMain.player.location.y + finalRange[i].y;
            this.UseAlertPool(mon, x, y, 2413);
            this.StartCoroutine(MonsterSkill2413Routine(x, y));
        }
    }

    private IEnumerator MonsterSkill2413Routine(int x, int y)
    {
        yield return new WaitForSeconds(1f);
        effectManager.IndependentEffect(12, new Vector2(x, y) + stageMain.bl, 0.42f);
    }
    private void MonsterSkill2407(Monster mon, eDirection dir)
    {
        StartCoroutine(this.Skill2407Routine(mon, dir));
    }
    private IEnumerator Skill2407Routine(Monster mon, eDirection dir)
    {
        yield return new WaitForSeconds(0.2f);
        stageMain.InstantiateProjectile(mon.location, dir, 2407, mon.id, 1, mon.monData.attack);
    }
    private void MonsterSkill2414(Monster mon, eDirection dir)
    {
        DirChange(stageMain.player.location, mon);
        StartCoroutine(this.Skill2414Routine(mon, dir));
    }
    private IEnumerator Skill2414Routine(Monster mon, eDirection dir)
    {
        yield return new WaitForSeconds(0.35f);
        //this.stageMain.tileInfoArray[mon.location.x,mon.location.y]
        var rangeList = DataManager.instance.dicAreaSkill.Where((range) => range.Value.skillId == 2414);
        var finalRange = stageMain.RangeMaking(rangeList);

        for (int i = 0; i < finalRange.Count; ++i)
        {
            var range = stageMain.RotationalTransform(finalRange[i], dir);
            var x = mon.location.x + range.x;
            var y = mon.location.y + range.y;
            StartCoroutine(stageMain.ChangeTileColor(mon.location + range + this.stageMain.bl));
            if (x >= 0 && x < this.stageMain.sizeX && y >= 0 && y < this.stageMain.sizeY && this.stageMain.tileInfoArray[x, y].objId == ConstantIDs.PLAYER)
            {
                //Debug.LogFormat("{0}가 플레이어를 {1}데미지로 공격", mon.monData.name, mon.monData.attack);
                this.stageMain.onPlayerHitAction(mon.monData.attack * DataManager.instance.dicMonsterSkill[2414].damage);
            }
        }
    }
    private void MonsterSkill2415(Monster mon)
    {
        var rangeList = DataManager.instance.dicAreaSkill.Where((range) => range.Value.skillId == 2410);
        var finalRange = stageMain.RangeMaking(rangeList);
        StartCoroutine(this.Skill2415Routine(mon, finalRange));
    }
    private IEnumerator Skill2415Routine(Monster mon, List<Vector2Int> finalRange)
    {
        int j = 0;
        while (j < 7)
        {
            for (int i = 0; i < finalRange.Count; ++i)
            {
                var x = stageMain.player.location.x + finalRange[i].x;
                var y = stageMain.player.location.y + finalRange[i].y;
                this.UseAlertPool(mon, x, y, 2415);
            }
            yield return new WaitForSeconds(0.2f);
            j++;
        }
    }
    private void MonsterSkill1557(Monster mon, eDirection dir)
    {
        StartCoroutine(this.Skill1557Routine(mon, dir));
    }
    private IEnumerator Skill1557Routine(Monster mon, eDirection dir)
    {
        yield return new WaitForSeconds(0.43f);
        var rangeList = DataManager.instance.dicAreaSkill.Where((range) => range.Value.skillId == 2414);
        var finalRange = stageMain.RangeMaking(rangeList);

        for (int i = 0; i < finalRange.Count; ++i)
        {
            var range = stageMain.RotationalTransform(finalRange[i], dir);
            var x = mon.location.x + range.x;
            var y = mon.location.y + range.y;
            StartCoroutine(stageMain.ChangeTileColor(mon.location + range + this.stageMain.bl));
            if (x >= 0 && x < this.stageMain.sizeX && y >= 0 && y < this.stageMain.sizeY && this.stageMain.tileInfoArray[x, y].objId == ConstantIDs.PLAYER)
            {
                this.stageMain.onPlayerHitAction(mon.monData.attack * DataManager.instance.dicMonsterSkill[2414].damage);
            }
        }
    }
    private void MonsterSkill1557_1(Monster mon, eDirection dir)
    {
        StartCoroutine(this.Skill1557_1Routine(mon, dir));
    }
    private IEnumerator Skill1557_1Routine(Monster mon, eDirection dir)
    {
        yield return new WaitForSeconds(0.43f);
        var rangeList = DataManager.instance.dicAreaSkill.Where((range) => range.Value.skillId == 2414);
        var finalRange = stageMain.RangeMaking(rangeList);

        for (int i = 0; i < finalRange.Count; ++i)
        {
            var range = stageMain.RotationalTransform(finalRange[i], dir);
            var x = mon.location.x + range.x;
            var y = mon.location.y + range.y;
            StartCoroutine(stageMain.ChangeTileColor(mon.location + range + this.stageMain.bl));
            if (x >= 0 && x < this.stageMain.sizeX && y >= 0 && y < this.stageMain.sizeY && this.stageMain.tileInfoArray[x, y].objId == ConstantIDs.PLAYER)
            {
                this.stageMain.onPlayerHitAction(mon.monData.attack * DataManager.instance.dicMonsterSkill[2414].damage);
            }
        }
    }
    private void MonsterSkill2416(Monster mon, eDirection dir)
    {
        StartCoroutine(this.Skill2416Routine(mon, dir));
    }
    private IEnumerator Skill2416Routine(Monster mon, eDirection dir)
    {
        yield return new WaitForSeconds(0.35f);
        mon.modelGO.animation.timeScale = 0;
        yield return new WaitForSeconds(0.4f);
        mon.modelGO.animation.timeScale = 1;
        var rangeList = DataManager.instance.dicAreaSkill.Where((range) => range.Value.skillId == 2414);
        var finalRange = stageMain.RangeMaking(rangeList);

        for (int j = 0; j < 4; j++)
        {
            for (int i = 0; i < finalRange.Count; ++i)
            {
                var range = stageMain.RotationalTransform(finalRange[i], dir);
                var x = mon.location.x + range.x;
                var y = mon.location.y + range.y;
                StartCoroutine(stageMain.ChangeTileColor(mon.location + range + this.stageMain.bl));
                if (x >= 0 && x < this.stageMain.sizeX && y >= 0 && y < this.stageMain.sizeY && this.stageMain.tileInfoArray[x, y].objId == ConstantIDs.PLAYER)
                {
                    this.stageMain.onPlayerHitAction(mon.monData.attack * DataManager.instance.dicMonsterSkill[2414].damage);
                }
            }
            yield return new WaitForSeconds(0.2f);
        }
    }
    private void MonsterSkill2416_1(Monster mon, eDirection dir)
    {
        StartCoroutine(this.Skill2416_1Routine(mon, dir));
    }
    private IEnumerator Skill2416_1Routine(Monster mon, eDirection dir)
    {
        yield return new WaitForSeconds(0.35f);
        mon.modelGO.animation.timeScale = 0;
        yield return new WaitForSeconds(0.4f);
        mon.modelGO.animation.timeScale = 1;
        var rangeList = DataManager.instance.dicAreaSkill.Where((range) => range.Value.skillId == 2414);
        var finalRange = stageMain.RangeMaking(rangeList);

        for (int j = 0; j < 4; j++)
        {
            for (int i = 0; i < finalRange.Count; ++i)
            {
                var range = stageMain.RotationalTransform(finalRange[i], dir);
                var x = mon.location.x + range.x;
                var y = mon.location.y + range.y;
                StartCoroutine(stageMain.ChangeTileColor(mon.location + range + this.stageMain.bl));
                if (x >= 0 && x < this.stageMain.sizeX && y >= 0 && y < this.stageMain.sizeY && this.stageMain.tileInfoArray[x, y].objId == ConstantIDs.PLAYER)
                {
                    this.stageMain.onPlayerHitAction(mon.monData.attack * DataManager.instance.dicMonsterSkill[2414].damage);
                }
            }
            yield return new WaitForSeconds(0.2f);
        }
    }
    private void MonsterSkill2417(Monster mon, eDirection dir)
    {
        StartCoroutine(this.Skill2417Routine(mon, dir));
    }
    private IEnumerator Skill2417Routine(Monster mon, eDirection dir)
    {
        yield return new WaitForSeconds(0.2f);
        Vector2Int attDir = new Vector2Int(0, 0);
        switch (dir)
        {
            case eDirection.up:
                {
                    attDir = Vector2Int.up;
                    break;
                }
            case eDirection.down:
                {
                    attDir = Vector2Int.down;
                    break;
                }
            case eDirection.left:
                {
                    attDir = Vector2Int.left;
                    break;
                }
            case eDirection.right:
                {
                    attDir = Vector2Int.right;
                    break;
                }
        }
        Vector2Int attPos = mon.location;
        for (int i = 0; i < 5; i++)
        {
            attPos += attDir;
            if (this.stageMain.tileInfoArray[attPos.x, attPos.y].objId != ConstantIDs.BLANK)
            {
                break;
            }
        }
        attPos -= attDir;
        mon.modelGO.animation.timeScale = 0;
        yield return new WaitForSeconds(0.5f);
        mon.modelGO.animation.timeScale = 1;
        this.MonsterMove(new Vector2Int(attPos.x, attPos.y), mon, true, 0.1f);
        var rangeList = DataManager.instance.dicAreaSkill.Where((range) => range.Value.skillId == 2417);
        var finalRange = stageMain.RangeMaking(rangeList);

        for (int i = 0; i < finalRange.Count; ++i)
        {
            var range = stageMain.RotationalTransform(finalRange[i], dir);
            var x = mon.location.x + range.x;
            var y = mon.location.y + range.y;
            StartCoroutine(stageMain.ChangeTileColor(mon.location + range + this.stageMain.bl));
            if (x >= 0 && x < this.stageMain.sizeX && y >= 0 && y < this.stageMain.sizeY && this.stageMain.tileInfoArray[x, y].objId == ConstantIDs.PLAYER)
            {
                //Debug.LogFormat("{0}가 플레이어를 {1}데미지로 공격", mon.monData.name, mon.monData.attack);
                this.stageMain.onPlayerHitAction(mon.monData.attack * DataManager.instance.dicMonsterSkill[2417].damage);
            }
        }
    }
    private void MonsterSkill2418(Monster mon)
    {
        StartCoroutine(this.Skill2418Routine(mon));
    }
    private IEnumerator Skill2418Routine(Monster mon)
    {
        yield return new WaitForSeconds(0.2f);
        var rangeList = DataManager.instance.dicAreaSkill.Where((range) => range.Value.skillId == 2418);
        var finalRange = stageMain.RangeMaking(rangeList);



        for (int j = 0; j < this.stageMain.sizeX; j++)
        {
            int cnt = 0;
            for (int i = 0; i < finalRange.Count; ++i)
            {
                var x = j + finalRange[i].x;
                var y = 0 + finalRange[i].y;
                if (x >= 0 && x < this.stageMain.sizeX && y >= 0 && y < this.stageMain.sizeY && (this.stageMain.tileInfoArray[x, y].objId == ConstantIDs.BLANK || this.stageMain.tileInfoArray[x, y].objId == ConstantIDs.PLAYER))
                {
                    cnt++;
                }
            }
            int oneOut = Random.Range(0, cnt);
            int check = 0;
            for (int i = 0; i < finalRange.Count; ++i)
            {
                var x = j + finalRange[i].x;
                var y = 0 + finalRange[i].y;
                if (x >= 0 && x < this.stageMain.sizeX && y >= 0 && y < this.stageMain.sizeY && (this.stageMain.tileInfoArray[x, y].objId == ConstantIDs.BLANK || this.stageMain.tileInfoArray[x, y].objId == ConstantIDs.PLAYER))
                {
                    check++;
                    if (check != oneOut && check != oneOut - 1 && check != oneOut + 1)
                    {
                        UseAlertPool(mon, x, y, 2418);
                    }
                }
            }

            yield return new WaitForSeconds(0.4f);
        }
    }
    private void MonsterSkill2419(Monster mon)
    {
        StartCoroutine(this.Skill2419Routine(mon));
    }
    private IEnumerator Skill2419Routine(Monster mon)
    {
        yield return new WaitForSeconds(0.35f);
        var rangeList = DataManager.instance.dicAreaSkill.Where((range) => range.Value.skillId == 2419);
        var finalRange = stageMain.RangeMaking(rangeList);

        for (int j = 0; j < 10; j++)
        {
            int x = 0;
            int y = 0;
            do
            {
                x = Random.Range(0, this.stageMain.sizeX);
                y = Random.Range(0, this.stageMain.sizeY);
            } while (this.stageMain.tileInfoArray[x, y].objId != ConstantIDs.BLANK);
            for (int i = 0; i < finalRange.Count; ++i)
            {
                var finX = x + finalRange[i].x;
                var finY = y + finalRange[i].y;
                this.UseAlertPool(mon, finX, finY, 2419);
            }
            yield return new WaitForSeconds(1);
            if (mon.gameObject.activeInHierarchy == false)
            {
                yield break;
            }
        }
    }
    private void MonsterSkill2420(Monster mon)
    {
        StartCoroutine(this.Skill2420Routine(mon));
    }
    private IEnumerator Skill2420Routine(Monster mon)
    {
        yield return new WaitForSeconds(0.35f);
        for (int i = 0; i < 9; i++)
        {
            Vector2Int finLoc = new Vector2Int(0, 0);
            do
            {
                finLoc = new Vector2Int(mon.location.x - 1, Random.Range(0, this.stageMain.sizeY));
            } while (this.stageMain.tileInfoArray[finLoc.x, finLoc.y].objId != ConstantIDs.BLANK && this.stageMain.tileInfoArray[finLoc.x, finLoc.y].objId != ConstantIDs.PLAYER);

            stageMain.InstantiateProjectile(finLoc, eDirection.left, 2420, mon.id, 1, mon.monData.attack);
            yield return new WaitForSeconds(0.1f);
        }
    }
    private void MonsterSkill2421(Monster mon)
    {
        StartCoroutine(this.Skill2421Routine(mon));
    }
    private IEnumerator Skill2421Routine(Monster mon)
    {
        yield return new WaitForSeconds(0.35f);
        mon.modelGO.animation.timeScale = 0;
        var rangeList = DataManager.instance.dicAreaSkill.Where((range) => range.Value.skillId == 2421);
        var finalRange = stageMain.RangeMaking(rangeList);

        for (int i = 0; i < finalRange.Count; ++i)
        {
            var range = stageMain.RotationalTransform(finalRange[i], eDirection.left);
            var x = mon.location.x + range.x;
            var y = mon.location.y + range.y;
            this.UseAlertPool(mon, x, y, 2421);
        }
        for (int i = 0; i < 12; i++)
        {
            Vector2Int movDir = Vector2Int.zero;
            if (this.stageMain.player.location.y != mon.location.y)
            {
                movDir = this.stageMain.player.location.y > mon.location.y ? Vector2Int.down : Vector2Int.up;
            }
            this.stageMain.tileInfoArray[this.stageMain.player.location.x, this.stageMain.player.location.y].objId = ConstantIDs.BLANK;
            this.stageMain.player.location += movDir;
            this.stageMain.tileInfoArray[this.stageMain.player.location.x, this.stageMain.player.location.y].objId = ConstantIDs.PLAYER;
            this.stageMain.player.gameObject.transform.DOMove(new Vector3(this.stageMain.player.location.x + this.stageMain.bl.x, this.stageMain.player.location.y + this.stageMain.bl.y), 0.25f);
            yield return new WaitForSeconds(0.25f);
        }
        mon.modelGO.animation.timeScale = 1;
        for (int j = 0; j < 5; j++)
        {
            for (int i = 0; i < finalRange.Count; ++i)
            {
                var range = stageMain.RotationalTransform(finalRange[i], eDirection.left);
                var x = mon.location.x + range.x;
                var y = mon.location.y + range.y;
                StartCoroutine(stageMain.ChangeTileColor(mon.location + range + this.stageMain.bl));
                if (x >= 0 && x < this.stageMain.sizeX && y >= 0 && y < this.stageMain.sizeY && this.stageMain.tileInfoArray[x, y].objId == ConstantIDs.PLAYER)
                {
                    //Debug.LogFormat("{0}가 플레이어를 {1}데미지로 공격", mon.monData.name, mon.monData.attack * DataManager.instance.dicMonsterSkill[2421].damage);
                    this.stageMain.onPlayerHitAction(mon.monData.attack * DataManager.instance.dicMonsterSkill[2421].damage);
                }
            }
            yield return new WaitForSeconds(0.2f);
        }
    }
    private void MonsterSkill2422(Monster mon)
    {
        StartCoroutine(this.Skill2422Routine(mon));
    }
    private IEnumerator Skill2422Routine(Monster mon)
    {
        yield return new WaitForSeconds(0.35f);
        var rangeList = DataManager.instance.dicAreaSkill.Where((range) => range.Value.skillId == 2422);
        var finalRange = stageMain.RangeMaking(rangeList);

        for (int i = 0; i < finalRange.Count; ++i)
        {
            var x = stageMain.player.location.x + finalRange[i].x;
            var y = stageMain.player.location.y + finalRange[i].y;
            this.UseAlertPool(mon, x, y, 2422);
        }
    }
    private void MonsterSkill2423(Monster mon)
    {
        var rangeList = DataManager.instance.dicAreaSkill.Where((range) => range.Value.skillId == 2423);
        var finalRange = stageMain.RangeMaking(rangeList);
        StartCoroutine(this.Skill2423Routine(mon, finalRange));
    }
    private IEnumerator Skill2423Routine(Monster mon, List<Vector2Int> finalRange)
    {
        for (int i = 0; i < finalRange.Count; ++i)
        {
            var x = mon.location.x + finalRange[i].x;
            var y = mon.location.y + finalRange[i].y;
            if (finalRange[i].x == 0 && finalRange[i].y == 0)
            {
            }
            else
            {
                this.UseAlertPool(mon, x, y, 2423, true);
            }
        }
        this.UseAlertPool(mon, mon.location.x, mon.location.y, 2423, true);
        yield return null;
    }
    private void MonsterSkill2424(Monster mon)
    {
        StartCoroutine(this.Skill2424Routine(mon));
    }
    private IEnumerator Skill2424Routine(Monster mon)
    {
        yield return new WaitForSeconds(0.35f);
        var rangeList = DataManager.instance.dicAreaSkill.Where((range) => range.Value.skillId == 2424);
        var finalRange = stageMain.RangeMaking(rangeList);

        for (int i = 0; i < finalRange.Count; ++i)
        {
            var x = stageMain.player.location.x + finalRange[i].x;
            var y = stageMain.player.location.y + finalRange[i].y;
            this.UseAlertPool(mon, x, y, 2424);
        }
    }
    private void MonsterSkill2425(Monster mon, eDirection dir, float delay)
    {
        StartCoroutine(this.Skill2425Routine(mon, dir, delay));
    }
    private IEnumerator Skill2425Routine(Monster mon, eDirection dir, float delay)
    {
        yield return new WaitForSeconds(delay);
        var rangeList = DataManager.instance.dicAreaSkill.Where((range) => range.Value.skillId == 2425);
        var finalRange = stageMain.RangeMaking(rangeList);

        for (int i = 0; i < finalRange.Count; ++i)
        {
            var range = stageMain.RotationalTransform(finalRange[i], dir);
            var x = mon.location.x + range.x;
            var y = mon.location.y + range.y;
            StartCoroutine(stageMain.ChangeTileColor(mon.location + range + this.stageMain.bl));
            if (x >= 0 && x < this.stageMain.sizeX && y >= 0 && y < this.stageMain.sizeY && this.stageMain.tileInfoArray[x, y].objId == ConstantIDs.PLAYER)
            {
                Debug.LogFormat("{0}가 플레이어를 {1}데미지로 공격", mon.monData.name, mon.monData.attack * DataManager.instance.dicMonsterSkill[2425].damage);
                this.stageMain.onPlayerHitAction(mon.monData.attack * DataManager.instance.dicMonsterSkill[2425].damage);
            }
        }
    }
    private void MonsterSkill2426(Monster mon, eDirection dir)
    {
        StartCoroutine(this.Skill2426Routine(mon, dir));
    }
    private IEnumerator Skill2426Routine(Monster mon, eDirection dir)
    {
        yield return new WaitForSeconds(0.35f);
        var rangeList = DataManager.instance.dicAreaSkill.Where((range) => range.Value.skillId == 2426);
        var finalRange = stageMain.RangeMaking(rangeList);

        for (int i = 0; i < finalRange.Count; ++i)
        {
            var range = stageMain.RotationalTransform(finalRange[i], dir);
            var x = mon.location.x + range.x;
            var y = mon.location.y + range.y;
            this.UseAlertPool(mon, x, y, 2426);
        }
    }

    private void MonsterSkill2801(Monster mon, eDirection dir)
    {
        this.StartCoroutine(WaitForMonsterAnim(0.8f, () =>
        {
            stageMain.InstantiateProjectile(mon.location, dir, 2801, mon.id, 1, mon.monData.attack, ((ArcaneGolem)mon).projectileSprite);
        }));
    }

    private IEnumerator WaitForMonsterAnim(float playtime, UnityAction callback)
    {
        yield return new WaitForSeconds(playtime);
        callback();
    }

    private void MonsterSkill2802(Monster mon, eDirection dir)
    {
        Vector2Int monLoc2;
        Vector2Int monLoc3;
        this.StartCoroutine(WaitForMonsterAnim(0.4f, () =>
        {
            if (dir == eDirection.left || dir == eDirection.right)
            {
                monLoc2 = new Vector2Int(mon.location.x, mon.location.y - 1);
                monLoc3 = new Vector2Int(mon.location.x, mon.location.y + 1);
            }
            else
            {
                monLoc2 = new Vector2Int(mon.location.x - 1, mon.location.y);
                monLoc3 = new Vector2Int(mon.location.x + 1, mon.location.y);
            }
            stageMain.InstantiateProjectile(mon.location, dir, 2802, mon.id, 1, mon.monData.attack,((ArcaneGolem)mon).projectileSprite);
            stageMain.InstantiateProjectile(monLoc2, dir, 2802, mon.id, 1, mon.monData.attack, ((ArcaneGolem)mon).projectileSprite);
            stageMain.InstantiateProjectile(monLoc3, dir, 2802, mon.id, 1, mon.monData.attack, ((ArcaneGolem)mon).projectileSprite);
        }));
    }

    private void MonsterSkill2803(Monster mon, eDirection dir)
    {
        var rangeList = DataManager.instance.dicAreaSkill.Where((range) => range.Value.skillId == 2803);
        var finalRange = stageMain.RangeMaking(rangeList);
        this.StartCoroutine(MonsterSkill2803Routine(mon, () =>
        {
            for (int i = 0; i < finalRange.Count; ++i)
            {
                var x = stageMain.player.location.x + finalRange[i].x;
                var y = stageMain.player.location.y + finalRange[i].y;
                this.UseAlertPool(mon, x, y, 2803);
            }
        }));
    }

    private IEnumerator MonsterSkill2803Routine(Monster mon, UnityAction callback)
    {
        yield return new WaitForSeconds(0.33f);
        mon.modelGO.armature.animation.timeScale = -0.1f;
        yield return new WaitForSeconds(0.15f);
        mon.modelGO.armature.animation.timeScale = 0.1f;
        callback();
        yield return new WaitForSeconds(0.15f);
        mon.modelGO.armature.animation.timeScale = -0.1f;
        yield return new WaitForSeconds(0.15f);
        mon.modelGO.armature.animation.timeScale = 1f;
        yield return null;
    }

    void Boss1Skill_1(Monster mon, Vector2Int targetPosition)
    {
        if (mon.id == 401)
        {
            IEnumerable<KeyValuePair<int, AreaSkillData>> rangeData = DataManager.instance.dicAreaSkill.Where(x => x.Value.skillId == 2601);
            List<Vector2Int> range = stageMain.RangeMaking(rangeData);
            for (int i = 0; i < range.Count; i++)
            {
                range[i] += targetPosition;
                UseAlertPool(mon, range[i].x, range[i].y, 2601);
            }
        }
    }
    void Boss1Skill_2(Monster mon)
    {
        if (mon.id == 401)
        {
            IEnumerable<KeyValuePair<int, AreaSkillData>> rangeData = DataManager.instance.dicAreaSkill.Where(x => x.Value.skillId == 2602);
            List<Vector2Int> range = stageMain.RangeMaking(rangeData);

            for (int i = 0; i < range.Count; i++)
            {
                range[i] = stageMain.RotationalTransform(range[i], mon.dir);
                range[i] += mon.location;
                UseAlertPool(mon, range[i].x, range[i].y, 2602);
            }
        }
    }
    void Boss1Skill_3(Monster mon)
    {
        if (mon.id == 401)
        {
            for (int i = 0; i < mon.sizeX; i++)
            {
                for (int j = 0; j < mon.sizeY; j++)
                {
                    stageMain.tileInfoArray[mon.location.x + i, mon.location.y + j].objId = 0;
                }
            }

            ((Chapter1Boss)mon).Pattern_3(
                () =>
                {
                    Vector2Int playerLocation = stageMain.player.location;
                    List<Vector2Int> emptyLocations = new() { playerLocation + Vector2Int.right * 2, playerLocation - Vector2Int.right * 4 };
                    Vector2Int destination;

                    if (MonsterMovable(emptyLocations[0], mon) && MonsterMovable(emptyLocations[1], mon))
                    {
                        destination = emptyLocations[Random.Range(0, emptyLocations.Count)];
                    }
                    else if (MonsterMovable(emptyLocations[0], mon))
                    {
                        destination = emptyLocations[0];
                    }
                    else if (MonsterMovable(emptyLocations[1], mon))
                    {
                        destination = emptyLocations[1];
                    }
                    else
                    {
                        if (MonsterMovable(emptyLocations[0] + Vector2Int.down, mon))
                        {
                            destination = emptyLocations[0] + Vector2Int.down;
                        }
                        else
                        {
                            destination = emptyLocations[1] + Vector2Int.down;
                        }
                    }

                    mon.location = destination;
                    mon.transform.position = destination + (Vector2)stageMain.bl;

                    for (int i = 0; i < mon.sizeX; i++)
                    {
                        for (int j = 0; j < mon.sizeY; j++)
                        {

                            stageMain.tileInfoArray[mon.location.x + i, mon.location.y + j].objId = mon.id * 100;
                        }
                    }

                    if (playerLocation.x - mon.location.x >= 0) ((Chapter1Boss)mon).Flip(true);
                    else ((Chapter1Boss)mon).Flip(false);
                },
                () =>
                {
                    IEnumerable<KeyValuePair<int, AreaSkillData>> rangeData = DataManager.instance.dicAreaSkill.Where(x => x.Value.skillId == 2603);
                    List<Vector2Int> range = stageMain.RangeMaking(rangeData);
                    bool playerHit = false;
                    for (int i = 0; i < range.Count; i++)
                    {
                        range[i] = stageMain.RotationalTransform(range[i], mon.dir);
                        range[i] += mon.location;
                        if (mon.modelGO.armature.flipX) range[i] += Vector2Int.right * 2;
                        StartCoroutine(stageMain.ChangeTileColor(range[i] + stageMain.bl));
                        if (!(range[i].x < 0 || range[i].x >= stageMain.sizeX || range[i].y < 0 || range[i].y >= stageMain.sizeY) && stageMain.tileInfoArray[range[i].x, range[i].y].objId == ConstantIDs.PLAYER) playerHit = true;
                    }

                    if (playerHit)
                    {
                        Debug.Log("플레이어 피격");
                        this.stageMain.onPlayerHitAction(mon.monData.attack * DataManager.instance.dicMonsterSkill[2603].damage);
                    }
                }
            );
        }
    }
    void Boss1Skill_4(Monster mon)
    {

        if (mon.id == 401)
        {
            ((Chapter1Boss)mon).Pattern_4((x) =>
            {
                int xdir = 0;
                if (mon.dir == eDirection.left) xdir = -1;
                else if (mon.dir == eDirection.right) xdir = 1;

                if (mon.location.x + x * xdir < 0 || mon.location.x + x * xdir >= stageMain.sizeX) { return true; }
                else
                {
                    IEnumerable<KeyValuePair<int, AreaSkillData>> rangeData = DataManager.instance.dicAreaSkill.Where(x => x.Value.skillId == 2604);
                    List<Vector2Int> range = stageMain.RangeMaking(rangeData);
                    for (int i = 0; i < range.Count; i++)
                    {
                        range[i] = stageMain.RotationalTransform(range[i], mon.dir);
                        range[i] += mon.location + Vector2Int.right * xdir * x;
                        StartCoroutine(stageMain.ChangeTileColor(range[i] + stageMain.bl));
                        if (!(range[i].x < 0 || range[i].x >= stageMain.sizeX || range[i].y < 0 || range[i].y >= stageMain.sizeY) && stageMain.tileInfoArray[range[i].x, range[i].y].objId == ConstantIDs.PLAYER)
                        {
                            this.stageMain.onPlayerHitAction(mon.monData.attack * DataManager.instance.dicMonsterSkill[2604].damage);
                        }
                    }
                    return false;
                }

            });
        }
    }
    void Boss1Skill_5(Monster mon)
    {
        if (mon.location.x - stageMain.player.location.x > 0) ((Chapter1Boss)mon).Flip(false);
        else ((Chapter1Boss)mon).Flip(true);
        bool valid = false;
        if ((mon.dir == eDirection.left && mon.location.x < 5) || (mon.dir == eDirection.right && mon.location.x > stageMain.sizeX - 5))
        {
            valid = false;
        }
        else
        {
            valid = true;
        }
        int xdir = mon.dir == eDirection.left ? -1 : 1;
        Vector2Int destination = new();
        bool playerHit = false;

        ((Chapter1Boss)mon).Pattern_5(() =>
        {
            return valid;
        }, () =>
         {
             if (!playerHit)
             {
                 int[] frontColumn = new int[15];
                 for (int j = 0; j < 5; j++)
                 {
                     for (int i = 0; i < 3; i++)
                     {
                         frontColumn[j * 3 + i] = stageMain.tileInfoArray[mon.location.x + xdir * j + 1, mon.location.y + i].objId;
                         StartCoroutine(stageMain.ChangeTileColor(new Vector2Int(mon.location.x + xdir * j + 1, mon.location.y + i) + stageMain.bl));
                     }
                 }

                 if (frontColumn.Contains(ConstantIDs.PLAYER))
                 {

                     onPlayerRestrictAction();
                     playerHit = true;
                     Vector2Int v = mon.location + new Vector2Int(4 * xdir + 1, 2);
                     for (int i = 1; true; i++)
                     {
                         v += Vector2Int.right * xdir;
                         if (stageMain.tileInfoArray[v.x, v.y].objId != 0)
                         {
                             destination = v - Vector2Int.right * xdir;
                             break;
                         }
                     }

                     if (stageMain.tileInfoArray[(mon.location + new Vector2Int(4 * xdir + 1, 2)).x, (mon.location + new Vector2Int(4 * xdir + 1, 2)).y].objId == ConstantIDs.BLANK)
                     {
                         stageMain.tileInfoArray[stageMain.player.location.x, stageMain.player.location.y].objId = 0;
                         stageMain.player.location = mon.location + new Vector2Int(4 * xdir + 1, 2);
                         stageMain.tileInfoArray[stageMain.player.location.x, stageMain.player.location.y].objId = ConstantIDs.PLAYER;
                         stageMain.player.transform.DOMove((Vector3)(Vector2)(stageMain.player.location + stageMain.bl) - new Vector3(0, 0.3f, 0), 0.1f);
                     }
                 }
                 if (frontColumn.Contains(ConstantIDs.WALL))
                 {
                     return false;
                 }

                 MonsterMove(mon.location + Vector2Int.right * xdir, mon, true, 0.1f);
                 return true;
             }
             else
             {
                 onPlayerRestrictAction();
                 stageMain.tileInfoArray[stageMain.player.location.x, stageMain.player.location.y].objId = 0;
                 stageMain.player.location += Vector2Int.right * xdir;
                 stageMain.tileInfoArray[stageMain.player.location.x, stageMain.player.location.y].objId = ConstantIDs.PLAYER;
                 stageMain.player.transform.DOMove((Vector3)(Vector2)(stageMain.player.location + stageMain.bl) - new Vector3(0, 0.3f, 0), 0.1f);
                 MonsterMove(mon.location + Vector2Int.right * xdir, mon, true, 0.1f);
                 if (stageMain.player.location.x == destination.x)
                 {
                     Debug.Log("player hit");
                     this.stageMain.onPlayerHitAction(mon.monData.attack * DataManager.instance.dicMonsterSkill[2605].damage);
                     onPlayerUnrestrictAction();
                     return false;
                 }
                 return true;
             }
         });
    }

    void Boss2Skill_1(Monster mon)
    {
        Vector2Int destinationL;
        for (int i = 0; i < mon.sizeX; i++) for (int j = 0; j < mon.sizeY; j++) stageMain.tileInfoArray[mon.location.x + i, mon.location.y + j].objId = 0;



        if (mon.location.x > stageMain.player.location.x)
        {
            ((Chapter2Boss)mon).Flip(false);
            destinationL = stageMain.player.location + Vector2Int.right * 5;
            if (destinationL.x > stageMain.sizeX - 2)
            {
                ((Chapter2Boss)mon).Flip(true);
                destinationL = stageMain.player.location - Vector2Int.right * 5;
            }
        }
        else
        {
            ((Chapter2Boss)mon).Flip(true);
            destinationL = stageMain.player.location - Vector2Int.right * 5;
            if (destinationL.x < 1)
            {
                ((Chapter2Boss)mon).Flip(true);
                destinationL = stageMain.player.location + Vector2Int.right * 5;
            }
        }

        if (!MonsterMovable(destinationL, mon)) destinationL += Vector2Int.down;

        Vector2Int destinationP = destinationL + stageMain.bl;
        IEnumerable<KeyValuePair<int, AreaSkillData>> rangeData = DataManager.instance.dicAreaSkill.Where(x => x.Value.skillId == 2606);
        List<Vector2Int> range = stageMain.RangeMaking(rangeData);

        for (int i = 0; i < range.Count; i++)
        {
            range[i] = stageMain.RotationalTransform(range[i], mon.dir);
            range[i] += stageMain.player.location;
            UseAlertPool(mon, range[i].x, range[i].y, 2606);
        }
        for (int i = 0; i < mon.sizeX; i++) for (int j = 0; j < mon.sizeY; j++) stageMain.tileInfoArray[destinationL.x + i, destinationL.y + j].objId = -1;
        ((Chapter2Boss)mon).Skill_1(destinationP, () =>
        {
            mon.location = destinationL;
            //MonsterMove(destinationL, mon);
            for (int i = 0; i < mon.sizeX; i++) for (int j = 0; j < mon.sizeY; j++) stageMain.tileInfoArray[mon.location.x + i, mon.location.y + j].objId = 401 * 100;
            //stageMain.tileInfoArray[destinationL.x, destinationL.y].objId = 401 * 100;
        });
    }
    void Boss2Skill_2(Monster mon)
    {
        if (mon.location.x > stageMain.player.location.x) { ((Chapter2Boss)mon).Flip(false); }
        else { ((Chapter2Boss)mon).Flip(true); }

        ((Chapter2Boss)mon).Skill_2(() =>
        {
            IEnumerable<KeyValuePair<int, AreaSkillData>> rangeData = DataManager.instance.dicAreaSkill.Where(x => x.Value.skillId == 2606);
            List<Vector2Int> range = stageMain.RangeMaking(rangeData);

            int xdir = 0;
            if (mon.dir == eDirection.left) xdir = -1;
            else if (mon.dir == eDirection.right) xdir = 1;
            bool playerHit = false;
            for (int i = 0; i < range.Count; i++)
            {
                range[i] = stageMain.RotationalTransform(range[i], mon.dir);
                range[i] += mon.location + Vector2Int.up + Vector2Int.right * xdir * 3;
                StartCoroutine(stageMain.ChangeTileColor(range[i] + stageMain.bl));
                if (!(range[i].x < 0 || range[i].x >= stageMain.sizeX || range[i].y < 0 || range[i].y >= stageMain.sizeY)
                && stageMain.tileInfoArray[range[i].x, range[i].y].objId == ConstantIDs.PLAYER) playerHit = true;
            }
            if (playerHit)
            {
                Debug.Log("player hit");
                this.stageMain.onPlayerHitAction(mon.monData.attack * DataManager.instance.dicMonsterSkill[2607].damage);
                onPlayerKnockbackAction(5, mon.dir);
            }
        });
    }
    void Boss2Skill_3(Monster mon)
    {
        if (mon.location.x > stageMain.player.location.x) { ((Chapter2Boss)mon).Flip(false); }
        else { ((Chapter2Boss)mon).Flip(true); }

        ((Chapter2Boss)mon).Skill_3(() =>
        {
            stageMain.InstantiateProjectile(mon.location + Vector2Int.up, mon.dir, 2608, mon.id, 1, 100, null, 2);
        });
    }
    void Boss2Skill_4(Monster mon)
    {
        if (mon.location.x > stageMain.player.location.x) { ((Chapter2Boss)mon).Flip(false); }
        else { ((Chapter2Boss)mon).Flip(true); }
        ((Chapter2Boss)mon).Skill_4(() =>
        {
            IEnumerable<KeyValuePair<int, AreaSkillData>> rangeData = DataManager.instance.dicAreaSkill.Where(x => x.Value.skillId == 2609);
            List<Vector2Int> range = stageMain.RangeMaking(rangeData);
            bool playerHit = false;
            for (int i = 0; i < range.Count; i++)
            {
                range[i] = stageMain.RotationalTransform(range[i], mon.dir);
                range[i] += mon.location + Vector2Int.up;
                StartCoroutine(stageMain.ChangeTileColor(range[i] + stageMain.bl));
                if (!(range[i].x < 0 || range[i].x >= stageMain.sizeX || range[i].y < 0 || range[i].y >= stageMain.sizeY)
                && stageMain.tileInfoArray[range[i].x, range[i].y].objId == ConstantIDs.PLAYER) playerHit = true;
            }
            if (playerHit)
            {
                Debug.Log("player hit");
                this.stageMain.onPlayerHitAction(mon.monData.attack * DataManager.instance.dicMonsterSkill[2609].damage);
            }
        });
    }
    void Boss2Skill_5(Monster mon)
    {
        if (mon.location.x > stageMain.player.location.x) { ((Chapter2Boss)mon).Flip(false); }
        else { ((Chapter2Boss)mon).Flip(true); }
        ((Chapter2Boss)mon).Skill_5(() =>
        {
            IEnumerable<KeyValuePair<int, AreaSkillData>> rangeData = DataManager.instance.dicAreaSkill.Where(x => x.Value.skillId == 2610);
            List<Vector2Int> range = stageMain.RangeMaking(rangeData);

            bool playerHit = false;
            for (int i = 0; i < range.Count; i++)
            {
                range[i] = stageMain.RotationalTransform(range[i], mon.dir);
                range[i] += mon.location + Vector2Int.up;
                StartCoroutine(stageMain.ChangeTileColor(range[i] + stageMain.bl));
                if (!(range[i].x < 0 || range[i].x >= stageMain.sizeX || range[i].y < 0 || range[i].y >= stageMain.sizeY)
                && stageMain.tileInfoArray[range[i].x, range[i].y].objId == ConstantIDs.PLAYER) playerHit = true;
            }
            if (playerHit)
            {
                Debug.Log("player hit");
                this.stageMain.onPlayerHitAction(mon.monData.attack * DataManager.instance.dicMonsterSkill[2610].damage);
                onPlayerRestrictAction();
            }
        },
        () =>
        {
            IEnumerable<KeyValuePair<int, AreaSkillData>> rangeData = DataManager.instance.dicAreaSkill.Where(x => x.Value.skillId == 2611);
            List<Vector2Int> range = stageMain.RangeMaking(rangeData);
            StartCoroutine(Boss2Skill_5Routine(range, mon));
        });
    }
    IEnumerator Boss2Skill_5Routine(List<Vector2Int> range, Monster mon)
    {
        int xdir = 0;
        if (mon.dir == eDirection.left) xdir = -1;
        else if (mon.dir == eDirection.right) xdir = 1;
        bool playerHit = false;
        for (int i = 0; i < range.Count; i++)
        {
            range[i] = stageMain.RotationalTransform(range[i], mon.dir);
            range[i] += mon.location + Vector2Int.up + xdir * Vector2Int.right;
        }

        for (int j = 0; j < 3; j++)
        {
            for (int i = 0; i < range.Count; i++)
            {
                StartCoroutine(stageMain.ChangeTileColor(range[i] + stageMain.bl));
                if (!(range[i].x < 0 || range[i].x >= stageMain.sizeX || range[i].y < 0 || range[i].y >= stageMain.sizeY)
                && stageMain.tileInfoArray[range[i].x, range[i].y].objId == ConstantIDs.PLAYER) playerHit = true;
            }
            for (int i = 0; i < range.Count; i++)
            {
                range[i] += 3 * xdir * Vector2Int.right;
            }
            if (playerHit)
            {
                Debug.Log("player Hit");
                this.stageMain.onPlayerHitAction(mon.monData.attack * DataManager.instance.dicMonsterSkill[2610].damage);
            }
            yield return new WaitForSeconds(0.25f);
            onPlayerUnrestrictAction();
        }
    }

    void Boss3Skill_1(Monster mon)
    {
        Vector2Int destination = new();
        ((Chapter3Boss)mon).Skill_1(() =>
        {
            int idx = Random.Range(0, 2);
            Vector2Int pL = stageMain.player.location;
            destination = pL + Vector2Int.right * (idx == 0 ? 3 : -3);
            bool moveReturn = false;

            if (destination.x > pL.x) ((Chapter3Boss)mon).Flip(false);
            else ((Chapter3Boss)mon).Flip(true);

            if (/*destination.x < 0 || destination.x > stageMain.sizeX - 1
                || stageMain.tileInfoArray[destination.x, destination.y].objId != 0*/
                !MonsterMovable(destination, mon))
            {
                for (int i = 0; i < mon.sizeX; i++) for (int j = 0; j < mon.sizeY; j++) stageMain.tileInfoArray[mon.location.x + i, mon.location.y + j].objId = 0;
                mon.transform.DOMove((Vector2)(destination + stageMain.bl), 0.5f);
                moveReturn = true;
            }
            else
            {
                MonsterMove(destination, mon, true, 0.5f);
                /*mon.transform.DOMove((Vector2)(destination + stageMain.bl), 0.5f).onComplete = () =>
                {
                    TileInfo tileinfo = stageMain.tileInfoArray[destination.x, destination.y];
                    stageMain.tileInfoArray[mon.location.x, mon.location.y].objId = 0;
                    mon.location = tileinfo.location;
                    tileinfo.objId = mon.id * 100;
                };*/
                moveReturn = false;
            }
            return moveReturn;
        },
        () =>
        {
            IEnumerable<KeyValuePair<int, AreaSkillData>> rangeData = DataManager.instance.dicAreaSkill.Where(x => x.Value.skillId == 2612);
            List<Vector2Int> range = stageMain.RangeMaking(rangeData);
            for (int i = 0; i < range.Count; i++)
            {
                range[i] = stageMain.RotationalTransform(range[i], mon.dir) + destination;
                StartCoroutine(stageMain.ChangeTileColor(range[i] + stageMain.bl));
                if (!(range[i].x < 0 || range[i].x >= stageMain.sizeX || range[i].y < 0 || range[i].y >= stageMain.sizeY) && stageMain.tileInfoArray[range[i].x, range[i].y].objId == ConstantIDs.PLAYER)
                {
                    this.stageMain.onPlayerHitAction(mon.monData.attack * DataManager.instance.dicMonsterSkill[2612].damage);
                }
            }
        }, () =>
         {
             int rx, ry;
             while (true)
             {
                 rx = Random.Range(0, stageMain.sizeX);
                 ry = Random.Range(0, stageMain.sizeY);

                 if (MonsterMovable(new Vector2Int(rx, ry), mon)) break;
             }

             mon.location = new Vector2Int(rx, ry);
             mon.transform.DOMove((Vector2)(mon.location + stageMain.bl), 0.5f);
             for (int i = 0; i < mon.sizeX; i++) for (int j = 0; j < mon.sizeY; j++) stageMain.tileInfoArray[mon.location.x + i, mon.location.y + j].objId = mon.id * 100 + 1;

         });
    }
    void Boss3Skill_2(Monster mon)
    {
        Vector2Int destination = new();
        ((Chapter3Boss)mon).Skill_2(() =>
         {
             int idx = Random.Range(0, 2);
             Vector2Int pL = stageMain.player.location;
             destination = pL + Vector2Int.right * (idx == 0 ? 4 : -4);
             bool moveReturn = false;

             if (destination.x > pL.x) ((Chapter3Boss)mon).Flip(false);
             else ((Chapter3Boss)mon).Flip(true);

             if (/*destination.x < 0 || destination.x > stageMain.sizeX - 1
                    || stageMain.tileInfoArray[destination.x, destination.y].objId != 0*/
                !MonsterMovable(destination, mon))
             {
                 for (int i = 0; i < mon.sizeX; i++) for (int j = 0; j < mon.sizeY; j++) stageMain.tileInfoArray[mon.location.x + i, mon.location.y + j].objId = 0;
                 mon.transform.DOMove((Vector2)(destination + stageMain.bl), 0.5f);
                 moveReturn = true;
             }
             else
             {
                 MonsterMove(destination, mon, true, 0.5f);
                 /*mon.transform.DOMove((Vector2)(destination + stageMain.bl), 0.5f).onComplete = () =>
                    {
                        TileInfo tileinfo = stageMain.tileInfoArray[destination.x, destination.y];
                        stageMain.tileInfoArray[mon.location.x, mon.location.y].objId = 0;
                        mon.location = tileinfo.location;
                        tileinfo.objId = mon.id * 100;
                    };*/
                 moveReturn = false;
             }
             return moveReturn;
         },
     () =>
     {
         IEnumerable<KeyValuePair<int, AreaSkillData>> rangeData = DataManager.instance.dicAreaSkill.Where(x => x.Value.skillId == 2613);
         List<Vector2Int> range = stageMain.RangeMaking(rangeData);
         for (int i = 0; i < range.Count; i++)
         {
             range[i] = stageMain.RotationalTransform(range[i], mon.dir) + destination + Vector2Int.up;
             StartCoroutine(stageMain.ChangeTileColor(range[i] + stageMain.bl));
             if (!(range[i].x < 0 || range[i].x >= stageMain.sizeX || range[i].y < 0 || range[i].y >= stageMain.sizeY) && stageMain.tileInfoArray[range[i].x, range[i].y].objId == ConstantIDs.PLAYER)
             {
                 this.stageMain.onPlayerHitAction(mon.monData.attack * DataManager.instance.dicMonsterSkill[2612].damage);
             }
         }
     }, () =>
     {
         int rx, ry;
         while (true)
         {
             rx = Random.Range(0, stageMain.sizeX);
             ry = Random.Range(0, stageMain.sizeY);

             if (MonsterMovable(new Vector2Int(rx, ry), mon)) break;
         }

         mon.location = new Vector2Int(rx, ry);
         mon.transform.DOMove((Vector2)(mon.location + stageMain.bl), 0.5f);
         for (int i = 0; i < mon.sizeX; i++) for (int j = 0; j < mon.sizeY; j++) stageMain.tileInfoArray[mon.location.x + i, mon.location.y + j].objId = mon.id * 100 + 1;

     });
    }
    void Boss3Skill_3(Monster mon)
    {
        ((Chapter3Boss)mon).Skill_3(() =>
        {
            IEnumerable<KeyValuePair<int, AreaSkillData>> rangeData = DataManager.instance.dicAreaSkill.Where(x => x.Value.skillId == 2614);
            List<Vector2Int> range = stageMain.RangeMaking(rangeData);
            for (int i = 0; i < range.Count; i++)
            {
                range[i] += stageMain.player.location;
                UseAlertPool(mon, range[i].x, range[i].y, 2614);
            }

        });
    }
    void Boss3Skill_4(Monster mon)
    {
        if (mon.location.x > stageMain.player.location.x) { ((Chapter3Boss)mon).Flip(false); }
        else { ((Chapter3Boss)mon).Flip(true); }
        Vector2 destination = new();
        //stageMain.tileInfoArray[mon.location.x, mon.location.y].objId = 0;
        for (int i = 0; i < mon.sizeX; i++) for (int j = 0; j < mon.sizeY; j++) stageMain.tileInfoArray[mon.location.x + i, mon.location.y + j].objId = 0;
        ((Chapter3Boss)mon).Skill_4((index) =>
        {

            if (mon.dir == eDirection.left)
            {
                destination = new(stageMain.bl.x - 10, index == 0 ? mon.transform.position.y : stageMain.player.position.y);
            }
            else if (mon.dir == eDirection.right)
            {
                destination = new(stageMain.sizeX + 10 + stageMain.bl.x, index == 0 ? mon.transform.position.y : stageMain.player.position.y);
            }

            return destination;
        }, (index) =>
        {
            for (int i = 0; i < stageMain.sizeX; i++)
            {
                if (index == 0)
                {
                    if (mon.dir == eDirection.left && i > mon.location.x) continue;
                    if (mon.dir == eDirection.right && i < mon.location.x) continue;
                }
                for (int j = 0; j < 4; j++)
                {
                    List<Vector2Int> range = new() { Vector2Int.down, Vector2Int.zero, Vector2Int.up, Vector2Int.up * 2 };
                    range[j] += new Vector2Int(i, Mathf.RoundToInt(destination.y) - stageMain.bl.y);
                    UseAlertPool(mon, range[j].x, range[j].y, 2615);
                }
            }

        }, () =>
        {
            int rx, ry;
            while (true)
            {
                rx = Random.Range(0, stageMain.sizeX);
                ry = Random.Range(0, stageMain.sizeY);

                if (MonsterMovable(new Vector2Int(rx, ry),mon)) break;
            }

            //stageMain.tileInfoArray[rx, ry].objId = mon.id * 100;
            mon.location = new Vector2Int(rx, ry);
            for (int i = 0; i < mon.sizeX; i++) for (int j = 0; j < mon.sizeY; j++) stageMain.tileInfoArray[mon.location.x + i, mon.location.y + j].objId = mon.id * 100 + 1;
            
            mon.transform.DOMove(new Vector2(rx, ry) + stageMain.bl, 0.5f);
        });
    }

    void BossFinalSkill_1(Monster mon)
    {
        ((FinalBoss)mon).Skill_1(() =>
        {
            int r = Random.Range(0, 2);
            Vector2Int destination = stageMain.player.location + Vector2Int.right * 4 * (r == 0 ? 1 : -1) + Vector2Int.down;
            bool movable = MonsterMovable(destination, mon);
            if (movable)
            {
                MonsterMove(destination, mon, true, 0.5f);
                if (mon.location.x - stageMain.player.location.x > 0) ((FinalBoss)mon).Flip(false);
                else ((FinalBoss)mon).Flip(true);
            }
            return movable;
        }, () =>
        {
            IEnumerable<KeyValuePair<int, AreaSkillData>> rangeData = DataManager.instance.dicAreaSkill.Where(x => x.Value.skillId == 2617);
            List<Vector2Int> range = stageMain.RangeMaking(rangeData);
            for (int i = 0; i < range.Count; i++)
            {
                range[i] = stageMain.RotationalTransform(range[i], mon.dir) + mon.location + Vector2Int.right + Vector2Int.up;
                StartCoroutine(stageMain.ChangeTileColor(range[i] + stageMain.bl));
                if (!(range[i].x < 0 || range[i].x >= stageMain.sizeX || range[i].y < 0 || range[i].y >= stageMain.sizeY) && stageMain.tileInfoArray[range[i].x, range[i].y].objId == ConstantIDs.PLAYER)
                {
                    this.stageMain.onPlayerHitAction(mon.monData.attack * DataManager.instance.dicMonsterSkill[2612].damage);
                }
            }
        });
    }
    void BossFinalSkill_2(Monster mon)
    {
        ((FinalBoss)mon).Skill_2(() =>
        {
            int r = Random.Range(0, 2);
            Vector2Int destination = stageMain.player.location + Vector2Int.right * 4 * (r == 0 ? 1 : -1) + Vector2Int.down;
            bool movable = MonsterMovable(destination, mon);
            if (movable)
            {
                MonsterMove(destination, mon, true, 0.5f);
                if (mon.location.x - stageMain.player.location.x > 0) ((FinalBoss)mon).Flip(false);
                else ((FinalBoss)mon).Flip(true);
            }
            return movable;
        }, () =>
        {
            IEnumerable<KeyValuePair<int, AreaSkillData>> rangeData = DataManager.instance.dicAreaSkill.Where(x => x.Value.skillId == 2616);
            List<Vector2Int> range = stageMain.RangeMaking(rangeData);
            for (int i = 0; i < range.Count; i++)
            {
                range[i] = stageMain.RotationalTransform(range[i], mon.dir) + mon.location + Vector2Int.right + Vector2Int.up;
                StartCoroutine(stageMain.ChangeTileColor(range[i] + stageMain.bl));
                if (!(range[i].x < 0 || range[i].x >= stageMain.sizeX || range[i].y < 0 || range[i].y >= stageMain.sizeY) && stageMain.tileInfoArray[range[i].x, range[i].y].objId == ConstantIDs.PLAYER)
                {
                    this.stageMain.onPlayerHitAction(mon.monData.attack * DataManager.instance.dicMonsterSkill[2612].damage);
                }
            }
        });
    }
    void BossFinalSkill_3(Monster mon)
    {
        ((FinalBoss)mon).Skill_3(() =>
        {
            List<Vector2Int> range = new();
            while (true)
            {
                int x = Random.Range(0, stageMain.sizeX);
                int y = Random.Range(0, stageMain.sizeY);
                Vector2Int v2i = new Vector2Int(x, y);
                if (stageMain.tileInfoArray[x, y].objId != -1 && !range.Exists(x => x == v2i))
                {
                    range.Add(new Vector2Int(x, y));
                }
                if (range.Count == 75) break;
            }

            foreach (Vector2Int v in range)
            {
                UseAlertPool(mon, v.x, v.y, 2618);
            }
        });
    }
    void BossFinalSkill_4(Monster mon)
    {
        IEnumerable<KeyValuePair<int, AreaSkillData>> rangeData = DataManager.instance.dicAreaSkill.Where(x => x.Value.skillId == 2619);
        List<List<Vector2Int>> rangeList = new();
        foreach (KeyValuePair<int, AreaSkillData> range in rangeData)
        {
            rangeList.Add(stageMain.RangeMaking(range));
        }
        ((FinalBoss)mon).Skill_4(() =>
        {
            Vector2Int destination = new();
            while (true)
            {
                destination.x = Random.Range(0, stageMain.sizeX);
                destination.y = Random.Range(0, stageMain.sizeY);

                if (MonsterMovable(destination, mon)) break;
            }

            MonsterMove(destination, mon, true, 0.25f);
        }, (i) =>
        {
            List<Vector2Int> range = rangeList[i + 1].ToList();
            foreach (Vector2Int v2i in rangeList[i])
            {
                range.Remove(v2i);
            }

            for (int j = 0; j < range.Count; j++)
            {
                range[j] += mon.location + Vector2Int.right;
                StartCoroutine(stageMain.ChangeTileColor(range[j] + stageMain.bl));
                if (!(range[j].x < 0 || range[j].x >= stageMain.sizeX || range[j].y < 0 || range[j].y >= stageMain.sizeY) && stageMain.tileInfoArray[range[j].x, range[j].y].objId == ConstantIDs.PLAYER)
                {
                    this.stageMain.onPlayerHitAction(mon.monData.attack * DataManager.instance.dicMonsterSkill[2612].damage);
                }
            }

        });
    }
    void BossFinalSkill_5(Monster mon)
    {
        ((FinalBoss)mon).Skill_5(() =>
        {
            stageMain.InstantiateProjectile(mon.location + Vector2Int.up + Vector2Int.right, mon.dir, 2620, mon.id, 1, 100, null, 1);
        });
    }
}
