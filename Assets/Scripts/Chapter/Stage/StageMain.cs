using System.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Tilemaps;
using DG.Tweening;
using UnityEngine.U2D;
using System.Runtime.Serialization;

public class StageMain : StageBase
{
    private float stageSpan = 0;

    public List<Monster> monsters;
    public bool chapter3_phase1 = false;

    public bool[,] chestVisited;

    public UnityAction<Vector2Int[]> onStageClearAction;
    public UnityAction<int, Vector2> onMonsterHitAction;
    public UnityAction<int, Monster> onMonsterDieAction;
    public UnityAction<int> onPlayerHitAction;
    public UnityAction onChapter3Phase2Action;

    public GameObject alertPrefab;
    public GameObject projectilePrefab;

    public SpriteAtlas projectileAtlas;

    public List<Projectile> projectiles;

    public EffectManager effectManager;


    public override void Init(Player player)
    {
        base.Init(player);

        // 타일 데이터에 몬스터 위치 추가
        int k = 0;
        if (this.monsters.Count > 0)
        {
            foreach (Monster m in monsters)
            {
                m.location = Vector2Int.RoundToInt(m.transform.position) - bl;
                if (m.sizeX == 0 && m.sizeY == 0)
                {
                    tileInfoArray[m.location.x, m.location.y].objId = m.id * 100 + k; // monster id * 100 + list index
                }
                else
                {
                    
                    for (int i = 0; i < m.sizeX; ++i)
                    {
                        for (int j = 0; j < m.sizeY; ++j)
                        {
                            tileInfoArray[m.location.x+i, m.location.y+j].objId = m.id * 100 + k;
                        }
                    }
                }
                k++;
                m.onDieAction += () =>
                { // 몬스터 사망 이벤트
                    TileInfo info = tileInfoArray[m.location.x, m.location.y];
                    monsters[info.objId % 100] = null; // monster list 요소 null로
                    if (m.id < 200)
                    {
                        info.objId = ConstantIDs.BLANK;
                    }
                    else if (m.id == 306 || m.id == 403)
                    {
                        if (m.sizeX > 0)
                        {
                            for (int i = 0; i < MathF.Abs(m.sizeX); ++i)
                            {
                                for (int j = 0; j < MathF.Abs(m.sizeY); ++j)
                                {
                                    tileInfoArray[m.location.x + i, m.location.y + j].objId = ConstantIDs.WALL;
                                }
                            }
                        }
                        else
                        {
                            for (int i = 0; i < MathF.Abs(m.sizeX); ++i)
                            {
                                for (int j = 0; j < MathF.Abs(m.sizeY); ++j)
                                {
                                    tileInfoArray[m.location.x - i, m.location.y + j].objId = ConstantIDs.WALL;
                                }
                            }
                        }
                    }
                    else
                    {
                        if (m.sizeX > 0)
                        {
                            for (int i = 0; i < MathF.Abs(m.sizeX); ++i)
                            {
                                for (int j = 0; j < MathF.Abs(m.sizeY); ++j)
                                {
                                    tileInfoArray[m.location.x + i, m.location.y + j].objId = ConstantIDs.BLANK;
                                }
                            }
                        }
                        else
                        {
                            for (int i = 0; i < MathF.Abs(m.sizeX); ++i)
                            {
                                for (int j = 0; j < MathF.Abs(m.sizeY); ++j)
                                {
                                    tileInfoArray[m.location.x - i, m.location.y + j].objId = ConstantIDs.BLANK;
                                }
                            }
                        }
                    }
                    Debug.Log(m);
                    onMonsterDieAction(UnityEngine.Random.Range(m.monData.goldDrop - (int)(m.monData.goldDrop * 0.2f), m.monData.goldDrop + (int)(m.monData.goldDrop * 0.2f)), m);
                    
                    if (!monsters.Any(m => m != null)) // list가 전부 null이면 실행
                    {
                        this.chestVisited = new bool[this.tileInfoArray.GetLength(0), this.tileInfoArray.GetLength(1)];
                        ChestDFS(this.player.location); // 챕터메인에 이벤트로 넘겨줌
                        onStageClearAction(FindChestPos());

                        PortalApear();
                    }

                    if(m.id == 403) 
                    {
                        chapter3_phase1 = false;
                        ((Chapter3Boss)m).Die(() =>
                        {
                            m.gameObject.SetActive(false);
                            monsters[0].gameObject.SetActive(true);
                            monsters[0].transform.position = m.location + (Vector2)bl;
                            Debug.Log(onChapter3Phase2Action.GetInvocationList().Length);
                            onChapter3Phase2Action?.Invoke();
                            ((FinalBoss)monsters[0]).Apear(() =>
                            {
                                for (int i = 0; i < 3; i++) for (int j = 0; j < 2; j++) tileInfoArray[m.location.x + i, m.location.y + j].objId = 404 * 100;
                                monsters[0].location = m.location;
                            });
                        }); 
                    }
                };
                m.onHitAction += (damgage, location) => { onMonsterHitAction?.Invoke(damgage, location + bl); };
            }
        }
        else
        {
            PortalApear();
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TileInfo[] check = Check4D();
            foreach (TileInfo i in check)
            {
                Debug.Log(i.objId);
            }
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            KillALLMonsters();
        }


        stageSpan += Time.deltaTime;


        for (int i = 0; i < projectiles.Count; ++i)
        {
            SuperviseProjectile(projectiles[i]);
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            PrintTileInfo();
        }
    }

    /// <summary>
    /// target : 0 => 몬스터만 맞고, 1 => 플레이어만 맞고, 2 => 둘 다 맞음
    /// </summary>
    /// <param name="startLoc"></param>
    /// <param name="dir"></param>
    /// <param name="skillId"></param>
    /// <param name="target">0 => 몬스터만 맞고, 1 => 플레이어만 맞고, 2 => 둘 다 맞음</param>
    public void InstantiateProjectile(Vector2Int startLoc, eDirection dir, int skillId, int monId, int target, int userDamage, Sprite projectileSprite = null, int size = 0)
    {
        int speed = 0;
        int damage = 0;
        string allDir;
        bool isRemain;

        bool[] compare = new bool[8];
        List<int> resultDir = new List<int>();

        if (skillId >= 2400)
        {
            var skillData = DataManager.instance.dicMonsterSkill[skillId];
            speed = skillData.speed;
            damage = skillData.damage * DataManager.instance.dicMonster[monId].attack;
            allDir = skillData.direction.ToString();
            isRemain = false;
        }
        else
        {
            var skillData = DataManager.instance.dicActiveSkill[skillId];
            speed = skillData.speed;
            damage = userDamage;
            allDir = skillData.direction.ToString();
            isRemain = skillData.isRemain;
        }

        for (int i = 0; i < 8; i++)
        {
            if (allDir[i] == '1') resultDir.Add(Mathf.RoundToInt(Mathf.Pow(2, 7 - i)));
        }

        for (int i = 0; i < resultDir.Count; ++i)
        {
            GameObject projectileGo = Instantiate(this.projectilePrefab);
            Projectile projectile = projectileGo.GetComponent<Projectile>();

            Vector2Int realDir;

            switch ((eProjectileDirection)resultDir[i])
            {
                case eProjectileDirection.Up:
                    {
                        realDir = new Vector2Int(0, 1);
                        break;
                    }
                case eProjectileDirection.UpRight:
                    {
                        realDir = new Vector2Int(1, 1);
                        break;
                    }
                case eProjectileDirection.Right:
                    {
                        realDir = new Vector2Int(1, 0);
                        break;
                    }
                case eProjectileDirection.DownRight:
                    {
                        realDir = new Vector2Int(1, -1);
                        break;
                    }
                case eProjectileDirection.Down:
                    {
                        realDir = new Vector2Int(0, -1);
                        break;
                    }
                case eProjectileDirection.DownLeft:
                    {
                        realDir = new Vector2Int(-1, -1);
                        break;
                    }
                case eProjectileDirection.Left:
                    {
                        realDir = new Vector2Int(-1, 0);
                        break;
                    }
                case eProjectileDirection.UpLeft:
                    {
                        realDir = new Vector2Int(-1, 1);
                        break;
                    }
                default:
                    {
                        realDir = Vector2Int.up;
                        break;
                    }
            }

            Vector2Int finalDir = RotationalTransform(realDir, dir);

            projectile.Init(skillId, damage, this.stageSpan, dir, target, startLoc, finalDir, this.stageSpan, size);

            this.projectiles.Add(projectile);

            projectile.transform.position = new Vector3(tileInfoArray[startLoc.x, startLoc.y].position.x, tileInfoArray[startLoc.x, startLoc.y].position.y, 0) - (new Vector3(finalDir.x, finalDir.y, 0) / 2);
            if (projectileSprite != null)
            {
                projectile.gameObject.GetComponent<SpriteRenderer>().sprite = projectileSprite;
            }
            projectile.gameObject.GetComponent<SpriteRenderer>().enabled = false;
            projectile.transform.Rotate(0, 0, -Mathf.Atan2(finalDir.x, finalDir.y) * Mathf.Rad2Deg);
            if (skillId == 2208 || skillId == 2209 || skillId == 2210)
            {
                Animator anim = projectile.gameObject.GetComponent<Animator>();
                anim.enabled = true;
                anim.SetTrigger("2208");
            }
            if (skillId == 2211 || skillId == 2212 || skillId == 2213)
            {
                Animator anim = projectile.gameObject.GetComponent<Animator>();
                anim.enabled = true;
                anim.SetTrigger("2211");
            }
            if (skillId == 2301 || skillId == 2302 || skillId == 2303 || skillId == 2304)
            {
                Animator anim = projectile.gameObject.GetComponent<Animator>();
                anim.enabled = true;
                anim.SetTrigger("2301");
            }
            if (monId == 110 || monId == 306)
            {
                Animator anim = projectile.gameObject.GetComponent<Animator>();
                anim.enabled = true;
                anim.SetTrigger("110");
            }
            if (monId == 304 && skillId == 2407)
            {
                Animator anim = projectile.gameObject.GetComponent<Animator>();
                anim.enabled = true;
                anim.SetTrigger("2407");
            }
        }
    }

    private void SuperviseProjectile(Projectile p)
    {
        bool isRemove = false;
        p.transform.position += p.transform.up * (p.speed * Time.deltaTime / 100f)  * p.finalDir.magnitude;

        if (p.renewalTime + ((100f / p.speed) * p.renewalCnt)  < this.stageSpan)
        {
            p.gameObject.GetComponent<SpriteRenderer>().enabled = true;
            for (int i = 0; i < p.projectileRange.Count; ++i)
            {
                fieldTile.SetTileFlags((Vector3Int)(p.location + p.projectileRange[i] + bl), TileFlags.None);
                fieldTile.SetColor((Vector3Int)(p.location + p.projectileRange[i] + bl), Color.white);
            }

            p.location += p.finalDir;
            p.renewalCnt += 1;

            for (int i = 0; i < p.projectileRange.Count; ++i)
            {
                fieldTile.SetTileFlags((Vector3Int)(p.location + p.projectileRange[i] + bl), TileFlags.None);
                fieldTile.SetColor((Vector3Int)(p.location + p.projectileRange[i] + bl), new Color(1, 1, 0, 0.5f));
            }
        }

        for (int r = 0; r < p.projectileRange.Count; ++r)
        {
            if (0 <= p.location.x + p.projectileRange[r].x && p.location.x + p.projectileRange[r].x < sizeX && 0 <= p.location.y + p.projectileRange[r].y && p.location.y + p.projectileRange[r].y < sizeY) 
            {
                if (10000 <= tileInfoArray[p.location.x + p.projectileRange[r].x, p.location.y + p.projectileRange[r].y].objId && tileInfoArray[p.location.x + p.projectileRange[r].x, p.location.y + p.projectileRange[r].y].objId <= 100000 && (p.target == 0 || p.target == 2) && tileInfoArray[p.location.x + p.projectileRange[r].x, p.location.y + p.projectileRange[r].y].objId != p.preHittedObjId)
                {
                    fieldTile.SetColor((Vector3Int)(p.location + p.projectileRange[r] + bl), Color.white);
                    p.preHittedObjId = tileInfoArray[p.location.x + p.projectileRange[r].x, p.location.y + p.projectileRange[r].y].objId;
                    monsters[tileInfoArray[p.location.x + p.projectileRange[r].x, p.location.y + p.projectileRange[r].y].objId % 100].Hit(p.damage);
                    if (p.isBomb)
                    {
                        List<int> bombHitMonsters = new();
                        var areaData = DataManager.instance.dicAreaSkill.Where(x => x.Value.skillId == p.skillId);
                        var bombRange = RangeMaking(areaData);
                        SoundManager.PlaySFX(App.instance.boomAudio);
                        for (int i = 0; i < bombRange.Count; ++i)
                        {
                            var x = p.location.x + p.projectileRange[r].x + bombRange[i].x;
                            var y = p.location.y + p.projectileRange[r].y + bombRange[i].y;
                            StartCoroutine(this.ChangeTileColor(p.location + bombRange[i] + this.bl));
                            if (!(x < 0 || x >= this.sizeX || y < 0 || y >= this.sizeY) && (10000 <= this.tileInfoArray[x, y].objId && this.tileInfoArray[x, y].objId <= 100000))
                            {
                                bombHitMonsters.Add(tileInfoArray[x, y].objId%100);
                            }
                        }
                        bombHitMonsters = bombHitMonsters.Distinct().ToList();
                        foreach (int id in bombHitMonsters)
                        {
                            monsters[id].Hit(p.damage);
                        }

                        isRemove = true;
                    }
                    if (!p.isRemain)
                    {
                        isRemove = true;
                    }
                }

                else if (tileInfoArray[p.location.x + p.projectileRange[r].x, p.location.y + p.projectileRange[r].y].objId == ConstantIDs.PLAYER && (p.target == 1 || p.target == 2))
                {
                    fieldTile.SetColor((Vector3Int)(p.location + p.projectileRange[r] + bl), Color.white);
                    Debug.Log("플레이어가 투사체맞음");
                    this.onPlayerHitAction(p.damage);
                    if (p.isBomb)
                    {
                        SoundManager.PlaySFX(App.instance.boomAudio);
                        var areaData = DataManager.instance.dicAreaSkill.Where(x => x.Value.skillId == p.skillId);
                        var bombRange = RangeMaking(areaData);
                        for (int i = 0; i < bombRange.Count; ++i)
                        {
                            var x = p.location.x + p.projectileRange[r].x + bombRange[i].x;
                            var y = p.location.y + p.projectileRange[r].y + bombRange[i].y;
                            StartCoroutine(this.ChangeTileColor(p.location + bombRange[i] + this.bl));
                            if (!(x < 0 || x >= this.sizeX || y < 0 || y >= this.sizeY) && this.tileInfoArray[x, y].objId == ConstantIDs.PLAYER)
                            {
                                //Debug.LogFormat("플레이어가 {0}데미지로 폭발피해 입음", p.damage);
                                this.onPlayerHitAction(p.damage);
                            }
                        }
                        isRemove = true;
                    }
                    if (!p.isRemain)
                    {
                        isRemove = true;
                    }
                }
            }
            else
            {
                isRemove = true; continue;
            }
        }

        //벽에서 봄브샷 터지면 몬스터도 맞아야됨
        for (int r = 0; r < p.projectileRange.Count; ++r)
        {
            if (0 <= p.location.x + p.projectileRange[r].x && p.location.x + p.projectileRange[r].x < sizeX && 0 <= p.location.y + p.projectileRange[r].y && p.location.y + p.projectileRange[r].y < sizeY)
            {
                if (tileInfoArray[p.location.x + p.projectileRange[r].x, p.location.y + p.projectileRange[r].y].objId < 0) //벽일때
                {
                    fieldTile.SetColor((Vector3Int)(p.location + p.projectileRange[r] + bl), Color.white);
                    if (p.isBomb)
                    {
                        SoundManager.PlaySFX(App.instance.boomAudio);
                        List<int> hitMonsters = new();
                        var areaData = DataManager.instance.dicAreaSkill.Where(x => x.Value.skillId == p.skillId);
                        var bombRange = RangeMaking(areaData);
                        for (int i = 0; i < bombRange.Count; ++i)
                        {
                            var x = p.location.x + p.projectileRange[r].x + bombRange[i].x;
                            var y = p.location.y + p.projectileRange[r].y + bombRange[i].y;
                            StartCoroutine(this.ChangeTileColor(p.location + p.projectileRange[r] + bombRange[i] + this.bl));
                            if (!(x < 0 || x >= this.sizeX || y < 0 || y >= this.sizeY))
                            {
                                if (this.tileInfoArray[x, y].objId == ConstantIDs.PLAYER && (p.target == 1 || p.target == 2))
                                this.onPlayerHitAction(p.damage);
                                //Debug.LogFormat("플레이어가 {0}데미지로 폭발피해 입음", p.damage);
                                else if (10000<= this.tileInfoArray[x, y].objId && this.tileInfoArray[x, y].objId <100000 && (p.target == 0 || p.target == 2))
                                {
                                    hitMonsters.Add(tileInfoArray[x, y].objId % 100);
                                }
                            }
                        }
                        hitMonsters = hitMonsters.Distinct().ToList();
                        foreach (int id in hitMonsters)
                        {
                            monsters[id].Hit(p.damage);
                        }
                    }
                    if (p.projectileRange[r].x == 0 && p.projectileRange[r].y == 0)
                    {
                        isRemove = true;
                    }
                }
            }
            else
            {
                isRemove = true; continue;
            }
        }
        if (isRemove)
        {
            if (p.isBomb)
            {
                effectManager.IndependentEffect(4, (Vector2)p.location + bl);
            }
            for (int r = 0; r < p.projectileRange.Count; ++r)
            {
                fieldTile.SetTileFlags((Vector3Int)(p.location + p.projectileRange[r] + bl), TileFlags.None);
                fieldTile.SetColor((Vector3Int)(p.location + p.projectileRange[r] + bl), Color.white);
            }
            projectiles.Remove(p);
            Destroy(p.gameObject);
        }
    }

    void ChestDFS(Vector2Int startPos)
    {
        this.chestVisited[startPos.x, startPos.y] = true;
        int[] dirX = { 0, 0, -1, 1 };
        int[] dirY = { 1, -1, 0, 0 };
        for (int i = 0; i < 4; i++)
        {
            int nx = startPos.x + dirX[i];
            int ny = startPos.y + dirY[i];

            if (nx < 0 || nx >= this.tileInfoArray.GetLength(0) || ny < 0 || ny >= this.tileInfoArray.GetLength(1) || tileInfoArray[nx, ny].objId != 0) continue;

            if (!chestVisited[nx, ny]) { ChestDFS(new Vector2Int(nx, ny)); }
        }
    }

    Vector2Int[] FindChestPos()
    {
        int destX1 = UnityEngine.Random.Range(0, tileInfoArray.GetLength(0));
        int destY1 = UnityEngine.Random.Range(0, tileInfoArray.GetLength(1));
        while (!chestVisited[destX1, destY1])
        {
            destX1 = UnityEngine.Random.Range(0, tileInfoArray.GetLength(0));
            destY1 = UnityEngine.Random.Range(0, tileInfoArray.GetLength(1));
        }
        chestVisited[destX1, destY1] = false;

        int destX2 = UnityEngine.Random.Range(0, tileInfoArray.GetLength(0));
        int destY2 = UnityEngine.Random.Range(0, tileInfoArray.GetLength(1));
        while (!chestVisited[destX2, destY2])
        {
            destX2 = UnityEngine.Random.Range(0, tileInfoArray.GetLength(0));
            destY2 = UnityEngine.Random.Range(0, tileInfoArray.GetLength(1));
        }
        this.tileInfoArray[destX1, destY1].objId = ConstantIDs.WEAPON_CHEST;
        this.tileInfoArray[destX2, destY2].objId = ConstantIDs.SKILL_CHEST;

        Vector2Int[] chestPos = new Vector2Int[2];
        chestPos[0] = new Vector2Int(bl.x + destX1, bl.y + destY1);
        chestPos[1] = new Vector2Int(bl.x + destX2, bl.y + destY2);
        return chestPos;
    }
    public List<Vector2Int> RangeMaking(KeyValuePair<int,AreaSkillData> rangeData)
    {
        List<KeyValuePair<int, AreaSkillData>> list = new();
        list.Add(rangeData);
        IEnumerable<KeyValuePair<int, AreaSkillData>> enumerable = list;
        return RangeMaking(enumerable);
    }
    public List<Vector2Int> RangeMaking(IEnumerable<KeyValuePair<int, AreaSkillData>> rangeList)
    {
        List<Vector2Int> range = new List<Vector2Int>();
        foreach (var rangeData in rangeList)
        {
            switch (rangeData.Value.skillType)
            {
                case 0: //사각형
                    {
                        for (int i = rangeData.Value.blY; i < rangeData.Value.trY + 1; i++)
                        {
                            for (int j = rangeData.Value.blX; j < rangeData.Value.trX + 1; j++)
                            {
                                if (range.IndexOf(new Vector2Int(j + rangeData.Value.centerPointX, i + rangeData.Value.centerPointY)) == -1)
                                    range.Add(new Vector2Int(j + rangeData.Value.centerPointX, i + rangeData.Value.centerPointY));
                            }
                        }
                        break;
                    }
                case 1: //반원
                    {
                        if (range.IndexOf(new Vector2Int(0 + rangeData.Value.centerPointX, 0 + rangeData.Value.centerPointY)) == -1)
                            range.Add(new Vector2Int(0 + rangeData.Value.centerPointX, 0 + rangeData.Value.centerPointY));
                        for (int i = 0; i < rangeData.Value.length + 1; i++)
                        {
                            if (i == 0)
                            {
                                for (int j = 1; j < rangeData.Value.length - i + 1; j++)
                                {
                                    if (range.IndexOf(new Vector2Int(i + rangeData.Value.centerPointX, j + rangeData.Value.centerPointY)) == -1)
                                        range.Add(new Vector2Int(i + rangeData.Value.centerPointX, j + rangeData.Value.centerPointY));
                                }
                            }
                            else
                            {
                                for (int j = 0; j < rangeData.Value.length - i + 1; j++)
                                {
                                    if (range.IndexOf(new Vector2Int(i + rangeData.Value.centerPointX, j + rangeData.Value.centerPointY)) == -1)
                                        range.Add(new Vector2Int(i + rangeData.Value.centerPointX, j + rangeData.Value.centerPointY));
                                    if (range.IndexOf(new Vector2Int(-(i + rangeData.Value.centerPointX), j + rangeData.Value.centerPointY)) == -1)
                                        range.Add(new Vector2Int(-(i + rangeData.Value.centerPointX), j + rangeData.Value.centerPointY));
                                }
                            }
                        }
                        break;
                    }
                case 2: //원
                    {
                        if (range.IndexOf(new Vector2Int(0 + rangeData.Value.centerPointX, 0 + rangeData.Value.centerPointY)) == -1)
                            range.Add(new Vector2Int(0 + rangeData.Value.centerPointX, 0 + rangeData.Value.centerPointY));
                        for (int i = 0; i < rangeData.Value.length + 1; i++)
                        {
                            if (i == 0)
                            {
                                for (int j = 1; j < rangeData.Value.length - i + 1; j++)
                                {
                                    if (range.IndexOf(new Vector2Int(i + rangeData.Value.centerPointX, j + rangeData.Value.centerPointY)) == -1)
                                        range.Add(new Vector2Int(i + rangeData.Value.centerPointX, j + rangeData.Value.centerPointY));
                                    if (range.IndexOf(new Vector2Int(i + rangeData.Value.centerPointX, -j + rangeData.Value.centerPointY)) == -1)
                                        range.Add(new Vector2Int(i + rangeData.Value.centerPointX, -j + rangeData.Value.centerPointY));
                                }
                            }
                            else
                            {
                                for (int j = 0; j < rangeData.Value.length - i + 1; j++)
                                {
                                    if (j == 0)
                                    {
                                        if (range.IndexOf(new Vector2Int(i + rangeData.Value.centerPointX, j + rangeData.Value.centerPointY)) == -1)
                                            range.Add(new Vector2Int(i + rangeData.Value.centerPointX, j + rangeData.Value.centerPointY));
                                        if (range.IndexOf(new Vector2Int(-(i + rangeData.Value.centerPointX), j + rangeData.Value.centerPointY)) == -1)
                                            range.Add(new Vector2Int(-(i + rangeData.Value.centerPointX), j + rangeData.Value.centerPointY));
                                    }
                                    else
                                    {
                                        if (range.IndexOf(new Vector2Int(i + rangeData.Value.centerPointX, j + rangeData.Value.centerPointY)) == -1)
                                            range.Add(new Vector2Int(i + rangeData.Value.centerPointX, j + rangeData.Value.centerPointY));
                                        if (range.IndexOf(new Vector2Int(-(i + rangeData.Value.centerPointX), j + rangeData.Value.centerPointY)) == -1)
                                            range.Add(new Vector2Int(-(i + rangeData.Value.centerPointX), j + rangeData.Value.centerPointY));
                                        if (range.IndexOf(new Vector2Int(i + rangeData.Value.centerPointX, -j + rangeData.Value.centerPointY)) == -1)
                                            range.Add(new Vector2Int(i + rangeData.Value.centerPointX, -j + rangeData.Value.centerPointY));
                                        if (range.IndexOf(new Vector2Int(-(i + rangeData.Value.centerPointX), -j + rangeData.Value.centerPointY)) == -1)
                                            range.Add(new Vector2Int(-(i + rangeData.Value.centerPointX), -j + rangeData.Value.centerPointY));
                                    }
                                }
                            }
                        }
                        break;
                    }
                case 3: //직선
                    {
                        List<int> direction = new List<int>();
                        int useTime = 0;
                        var dirBinary = Convert.ToInt32(rangeData.Value.direction.ToString(), 2);
                        for (int i = 0; i < 8; i++)
                        {
                            if (dirBinary / (1 << (7 - i)) >= 1)
                            {
                                dirBinary -= (1 << (7 - i));
                                direction.Add(1 << (7 - i));
                                useTime++;
                            }
                        }
                        if (range.IndexOf(new Vector2Int(0, 0)) == -1)
                            range.Add(new Vector2Int(0, 0));
                        Debug.LogFormat("{0} , directioncnt", direction.Count);
                        foreach (var a in direction)
                        {
                            switch ((eProjectileDirection)a)
                            {
                                case eProjectileDirection.Up:
                                    AddDirectionalRange(0, 1, rangeData.Value.length + 1, range);
                                    break;
                                case eProjectileDirection.UpRight:
                                    AddDirectionalRange(1, 1, rangeData.Value.length + 1, range);
                                    break;
                                case eProjectileDirection.Right:
                                    AddDirectionalRange(1, 0, rangeData.Value.length + 1, range);
                                    break;
                                case eProjectileDirection.DownRight:
                                    AddDirectionalRange(1, -1, rangeData.Value.length + 1, range);
                                    break;
                                case eProjectileDirection.Down:
                                    AddDirectionalRange(0, -1, rangeData.Value.length + 1, range);
                                    break;
                                case eProjectileDirection.DownLeft:
                                    AddDirectionalRange(-1, -1, rangeData.Value.length + 1, range);
                                    break;
                                case eProjectileDirection.Left:
                                    AddDirectionalRange(-1, 0, rangeData.Value.length + 1, range);
                                    break;
                                case eProjectileDirection.UpLeft:
                                    AddDirectionalRange(-1, 1, rangeData.Value.length + 1, range);
                                    break;
                            }
                        }
                        break;
                    }
            }
        }
        return range;
    }

    void AddDirectionalRange(int x, int y, int length, List<Vector2Int> range)
    {
        for (int i = 1; i < length; i++)
        {
            if (range.IndexOf(new Vector2Int(x * i, y * i)) == -1)
                range.Add(new Vector2Int(x * i, y * i));
        }
    }


    void KillALLMonsters()
    {
        for (int i = 0; i < monsters.Count; i++)
        {
            if (monsters[i] != null && monsters[i].gameObject.activeInHierarchy) monsters[i].Hit(99999);
        }
    }
}
