using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.U2D;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

public class MapManager : MonoBehaviour
{
    public Dictionary<int, Map> dicMap;
    public List<int> mapIds;

    public Dictionary<int, GameObject> dicMapBtnGO;

    public Map startMap;
    public Map endMap;
    public Map bossMap;

    public GameObject btnPrefab;
    public GameObject linePrefab;
    public GameObject Canvas;

    public int x = 3;
    public int y = 5;

    public SpriteAtlas mapIconAtlas;

    private int nowMapId = 0;

    public UnityAction<int> OnMapSelectAction;

    private void Awake()
    {
        this.dicMap = new Dictionary<int, Map>();
        this.dicMapBtnGO = new Dictionary<int, GameObject>();
        this.startMap = new Map(this.x / 2, 0);
        this.endMap = new Map(this.x / 2, this.y + 1);
        this.bossMap = new Map(this.x / 2, this.y + 2);
    }
    // Start is called before the first frame update
    void Start()
    {
        if (InfoManager.instance.gameInfo.isPlayingSaved)
        {
            if(InfoManager.instance.gameInfo.stageId == this.bossMap.id)
            {
                InfoManager.instance.gameInfo.stageId = 0;
                this.Init(this.startMap, 0, this.y);
                this.Init(this.endMap, 2, this.y);
                this.Init(this.bossMap, 4, this.y);

                this.dicMap.Add(this.startMap.id, this.startMap);
                this.dicMap.Add(this.endMap.id, this.endMap);
                this.dicMap.Add(this.bossMap.id, this.bossMap);

                Debug.Log(this.dicMap.Count);

                this.RandomMapSet(Random.Range(2, (this.mapIds.Count / 8) + 2)/*, Random.Range(1, (this.mapIds.Count / 10) + 2)*/, Random.Range(2, (this.mapIds.Count / 5) + 1));
                InfoManager.instance.gameInfo.chapterMap = dicMap;
            }
            Debug.Log(InfoManager.instance.gameInfo.isPlayingSaved);
            dicMap = InfoManager.instance.gameInfo.chapterMap;
            nowMapId = InfoManager.instance.gameInfo.stageId;
        }
        else
        {
            this.Init(this.startMap, 0, this.y);
            this.Init(this.endMap, 2, this.y);
            this.Init(this.bossMap, 4, this.y);

            this.dicMap.Add(this.startMap.id, this.startMap);
            this.dicMap.Add(this.endMap.id, this.endMap);
            this.dicMap.Add(this.bossMap.id, this.bossMap);

            Debug.Log(this.dicMap.Count);

            this.RandomMapSet(Random.Range(2, (this.mapIds.Count / 8) + 2)/*, Random.Range(1, (this.mapIds.Count / 10) + 2)*/, Random.Range(2, (this.mapIds.Count / 5) + 1));
            InfoManager.instance.gameInfo.chapterMap = dicMap;
        }
        this.MapSetting(this.dicMap);

    }
    private void MapCreate(int x, int y, int a, Map map)
    {
        if (!this.dicMap.ContainsKey(10000 + (100 * (x + a)) + y + 1))
        {
            if (!((x + a) < 0 || (x + a) > this.x - 1))
            {
                var newMap = new Map(x + a, y + 1);
                this.Init(newMap, 0, this.y);
                dicMap.Add(newMap.id, newMap);
                this.mapIds.Add(newMap.id);
            }

        }
        if (!((x + a) < 0 || (x + a) > this.x - 1))
        {
            map.nextMapIds.Add(10000 + (100 * (x + a)) + y + 1);
        }

    }
    private void MapCreateOneLine(int x, int y, int a, Map map)
    {
        int next = a;
        if (((x + next) < 0 || (x + next) > this.x - 1))
        {
            next *= -1;
        }
        if (!this.dicMap.ContainsKey(10000 + (100 * (x + next)) + y + 1))
        {
            if (!((x + next) < 0 || (x + next) > this.x - 1))
            {
                var newMap = new Map(x + next, y + 1);
                this.Init(newMap, 0, this.y);
                dicMap.Add(newMap.id, newMap);
                this.mapIds.Add(newMap.id);
            }

        }
        if (!((x + next) < 0 || (x + next) > this.x - 1))
        {
            map.nextMapIds.Add(10000 + (100 * (x + next)) + y + 1);
        }

    }
    private void RandomMapSet(int eliteCnt/*, int shopCnt*/, int QuesCnt)
    {
        for (int i = 0; i < eliteCnt; i++)
        {
            var ran = Random.Range(0, this.mapIds.Count);
            this.dicMap[this.mapIds[ran]].mapKind = eMapKind.elite;
            this.mapIds.Remove(this.mapIds[ran]);
        }
        /*for (int i = 0; i < shopCnt; i++)
        {
            var ran = Random.Range(0, this.mapIds.Count);
            this.dicMap[this.mapIds[ran]].mapKind = eMapKind.shop;
            this.mapIds.Remove(this.mapIds[ran]);
        }*/
        for (int i = 0; i < QuesCnt; i++)
        {
            var ran = Random.Range(0, this.mapIds.Count);
            this.dicMap[this.mapIds[ran]].mapKind = eMapKind.question;
            this.mapIds.Remove(this.mapIds[ran]);
        }
    }
    public void MapSetting(Dictionary<int, Map> dicMap)
    {
        foreach (var a in dicMap)
        {
            foreach (var next in a.Value.nextMapIds)
            {
                var line = Instantiate(this.linePrefab, this.Canvas.transform);
                line.GetComponent<UILineRenderer>().Points[0] = new Vector2(-830 + (a.Value.y * 237.14f), (-180 * (this.x / 2)) + (180 * a.Value.x));
                line.GetComponent<UILineRenderer>().Points[1] = new Vector2(-830 + ((a.Value.y + 1) * 237.14f), (-180 * (this.x / 2)) + (180 * dicMap[next].x));
            }
        }


        foreach (var a in dicMap)
        {
            var btn = Instantiate(this.btnPrefab, this.Canvas.transform);
            btn.GetComponent<Image>().color = new Color(0, 0, 0, 1);
            switch ((int)a.Value.mapKind)
            {
                case 1:
                    {
                        btn.GetComponent<Image>().sprite = this.mapIconAtlas.GetSprite("Elite");
                        break;
                    }
                case 2:
                    {
                        btn.GetComponent<Image>().sprite = this.mapIconAtlas.GetSprite("Shop");
                        break;
                    }
                case 3:
                    {
                        btn.GetComponent<Image>().sprite = this.mapIconAtlas.GetSprite("Events");
                        break;
                    }
                case 4:
                    {
                        btn.GetComponent<Image>().sprite = this.mapIconAtlas.GetSprite("Boss");
                        break;
                    }
                default:
                    {
                        btn.GetComponent<Image>().sprite = this.mapIconAtlas.GetSprite("Normal");
                        break;
                    }
            }
            btn.GetComponent<RectTransform>().localPosition = new Vector3(-830 + (a.Value.y * 237.14f), (-180 * (this.x / 2)) + (180 * a.Value.x), 0);
            btn.GetComponent<Button>().onClick.AddListener(() => {
                // 스테이지 선택하는 부분
                if (this.MapSelect(a.Value.id))
                {
                    //스테이지 선택 완료된 부분
                    //여기서 NowMapId 쓰면 선택한거 Id 사용 가능
                    //InfoManager.instance.gameInfo.stageId = nowMapId;
                    this.OnMapSelectAction(this.nowMapId);
                    this.StageEnter();

                }
            });
            this.dicMapBtnGO.Add(a.Key, btn);
        }

        if (InfoManager.instance.gameInfo.isPlayingSaved)
        {
            if (InfoManager.instance.gameInfo.stageId == 0)
            {
                this.dicMapBtnGO[this.startMap.id].GetComponent<Image>().color = new Color(1, 1, 1, 1);
            }
            else
            {
                dicMapBtnGO[nowMapId].GetComponent<Image>().color = new Color(1, 1, 0, 1);
                foreach (int i in dicMap[nowMapId].nextMapIds)
                {
                    dicMapBtnGO[i].GetComponent<Image>().color = new Color(1, 1, 1, 1);
                }
            }
        }
        else
        {
            this.dicMapBtnGO[this.startMap.id].GetComponent<Image>().color = new Color(1, 1, 1, 1);
            InfoManager.instance.gameInfo.isPlayingSaved = true;
        }
    }
    private void Init(Map map, int kind, int y)
    {
        map.mapKind = (eMapKind)kind;
        if (map.y < y)
        {
            if (map.y == 0)
            {
                this.Next(map, map.x, map.y, Random.Range(2, 4));
                return;
            }
            this.Next(map, map.x, map.y, Random.Range(1, 4));
        }
        else if (map.y == y)
        {
            this.NextBossPrev(map);
        }
        else if(map.y == y + 1)
        {
            this.NextBoss(map);
        }
    }
    private void Next(Map map, int x, int y, int count)
    {
        switch (count)
        {
            case 1:
                {
                    var ran = Random.Range(-1, 2);
                    this.MapCreateOneLine(x, y, ran, map);

                    break;
                }
            case 2:
                {
                    List<int> nextXs = new List<int>();
                    var ran = Random.Range(-1, 2);
                    switch (ran)
                    {
                        case -1:
                            {
                                nextXs.Add(0);
                                nextXs.Add(1);
                                break;
                            }
                        case 0:
                            {
                                nextXs.Add(-1);
                                nextXs.Add(1);
                                break;
                            }
                        case 1:
                            {
                                nextXs.Add(-1);
                                nextXs.Add(0);
                                break;
                            }
                    }
                    foreach (int a in nextXs)
                    {
                        this.MapCreate(x, y, a, map);
                    }

                    break;
                }
            case 3:
                {
                    for (int i = -1; i < 2; i++)
                    {
                        this.MapCreate(x, y, i, map);
                    }
                    break;
                }
        }
    }
    private void NextBossPrev(Map map)
    {
        map.nextMapIds.Add(this.endMap.id);
    }
    private void NextBoss(Map map)
    {
        map.nextMapIds.Add(this.bossMap.id);
    }

    private bool MapSelect(int mapId)
    {
        bool isCorrect = false;
        if (this.nowMapId == 0)
        {
            if (mapId == this.startMap.id)
            {
                this.nowMapId = mapId;
                isCorrect = true;
            }
        }
        else
        {
            
            foreach(var a in this.dicMap[this.nowMapId].nextMapIds)
            {
                if(mapId == a)
                {
                    isCorrect = true;
                    break;
                }
            }
            if (isCorrect)
            {
                this.dicMapBtnGO[this.nowMapId].GetComponent<Image>().color = new Color(0, 0, 0, 1);
                foreach (var a in this.dicMap[nowMapId].nextMapIds)
                {
                    this.dicMapBtnGO[a].GetComponent<Image>().color = new Color(0, 0, 0, 1);
                }

                this.nowMapId = mapId;
            }
        }
        return isCorrect;
    }
    private void StageEnter()
    {
        this.dicMapBtnGO[this.nowMapId].GetComponent<Image>().color = new Color(1, 1, 0, 1);
        foreach (var a in this.dicMap[nowMapId].nextMapIds)
        {
            this.dicMapBtnGO[a].GetComponent<Image>().color = new Color(1, 1, 1, 1);
        }
        //InfoManager.instance.gameInfo.stageId = nowMapId;
    }
}
