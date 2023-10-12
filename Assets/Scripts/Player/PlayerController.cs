using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

public class PlayerController : MonoBehaviour
{
    public Player player;
    public Weapon weapon;
    public bool playerCanMove { get; private set; } = false;
    public StageMain stageMain;

    public Sprite[] projectileSprites;

    public UIJoysticks uiJoysticks;
    public EffectManager effectManager;

    public UnityAction<eDirection> onMoveAction;
    public UnityAction onConversationAction;

    private bool[,] tileMapCheck;
    private bool isTileChecked;
    public void Init(StageMain stageMain)
    {
        this.stageMain = stageMain;
        isTileChecked = false;
    }

    void Start()
    {
        this.weapon = player.weapon;
        uiJoysticks.onMoveAction += (direction) =>
        {
            PlayerMove(direction);
        };
        player.onMoveCompleteAction += () => { playerCanMove = true; };

        uiJoysticks.onConversationAction += () =>
        {
            playerCanMove = false;
            onConversationAction();
        };
        for (int i = 0; i < 4; i++)
        {
            int tmp = i;
            uiJoysticks.onAttackAction[tmp] += (direction) => 
            {
                weapon.AttackStart = false;
                PlayerAttack(direction, tmp);
            };
        }
        this.weapon.onAttackAnimCompleteAction += () => { SetPlayerMovable(); };
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow)) PlayerMove(eDirection.up);
        else if (Input.GetKeyDown(KeyCode.DownArrow)) PlayerMove(eDirection.down);
        else if (Input.GetKeyDown(KeyCode.LeftArrow)) PlayerMove(eDirection.left);
        else if (Input.GetKeyDown(KeyCode.RightArrow)) PlayerMove(eDirection.right);

        if (Input.GetKeyDown(KeyCode.A))
        {
            PlayerAttack(eDirection.up);
        }
    }

    /// <summary>
    /// isSkill == True 스킬, isSkill == false 기본공격
    /// 각 스킬의 데미지를 곱해서 사용해야함.
    /// </summary>
    /// <param name="kind"></param>
    /// <returns></returns>
    public int PlayerDamageCalculate(bool isSkill, int skillId)
    {
        int randDam = Random.Range(8, 13);
        float result;
        WeaponData weaponData = DataManager.instance.dicWeapon[InfoManager.instance.gameInfo.weapon];
        int[] playerData = InfoManager.instance.playerInfo.stats;
        if (isSkill)
        {
            result = (weaponData.attack + playerData[1]) * weaponData.coefficient * (1 / (weaponData.delay)) * (DataManager.instance.dicActiveSkill[skillId].damage/100f);
            Debug.LogFormat("calcultate : {0}", (weaponData.attack + playerData[1]) * weaponData.coefficient * (1 / (weaponData.delay)) * (DataManager.instance.dicActiveSkill[skillId].damage / 100f));
            //Debug.LogFormat("0 : {0}, 1 : {1}, 2 : {2}, 3 : {3}", (weaponData.attack + playerData[1]), weaponData.coefficient, (1 / (weaponData.delay - 0.01f * playerData[2])), (DataManager.instance.dicActiveSkill[skillId].damage / 100f));
        }
        else
        { 
            result = (weaponData.attack + playerData[1]) * (1 / (weaponData.delay));
            //Debug.LogFormat("0 : {0}, 1 : {1}, 2 : {2}", (weaponData.attack + playerData[1]), (1 / (weaponData.delay - 0.01f * playerData[2])), (DataManager.instance.dicActiveSkill[skillId].damage / 100f));
        }
        return Mathf.RoundToInt(result * randDam / 10);
    }

    public void PlayerMove(eDirection direction)
    {
        if (!playerCanMove) return;
        playerCanMove = false;
        player.Rotate(direction);
        onMoveAction(direction);
    }
    public void SetPlayerMovable() { playerCanMove = true; }
    public void SetPlayerImmovable() { playerCanMove = false; }

    public void playerDFS(Vector2Int startpos)
    {
        this.tileMapCheck[startpos.x, startpos.y] = true;
        int[] dirX = { 0, 0, -1, 1 };
        int[] dirY = { 1, -1, 0, 0 };
        for (int i = 0; i < 4; i++)
        {
            int nx = startpos.x + dirX[i];
            int ny = startpos.y + dirY[i];

            if (nx < 0 || nx >= this.stageMain.tileInfoArray.GetLength(0) || ny < 0 || ny >= this.stageMain.tileInfoArray.GetLength(1) || this.stageMain.tileInfoArray[nx, ny].objId == ConstantIDs.WALL) continue;

            if (!tileMapCheck[nx, ny]) {  playerDFS(new Vector2Int(nx, ny)); }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="direction"></param>
    /// <param name="skillIdx"> 0: 기본공격, 1: 스킬1, 2: 스킬2, 3: 스킬3 </param>
    public void PlayerAttack(eDirection direction, int skillIdx = 0)
    {
        if (this.stageMain == null || !this.playerCanMove) return;

        int skillId = InfoManager.instance.gameInfo.skills[skillIdx];
        int weaponId = InfoManager.instance.gameInfo.weapon;

        //스킬 인덱스가 넘어오면, 인포매니저의 게임인포의 스킬에 해당 인덱스 스킬의 타입이 현재 무기 타입과 같은지 확인 후, 공격시작
        if (DataManager.instance.dicActiveSkill[skillId].type != DataManager.instance.dicWeapon[weaponId].type) return;

        this.playerCanMove = false;
        player.Rotate(direction);
        int type = DataManager.instance.dicWeapon[InfoManager.instance.gameInfo.weapon].id / 10;
        IEnumerable<KeyValuePair<int, AreaSkillData>> rangeIds;
        List<Vector2Int> area;
        //상시적용 스킬, 암살같은 애니메이션이 다른 스킬 등 때문에 각자 스킬에서 애니메이션을 발생 시켜야함
        // => 기본적으로 모든 스킬구현이 애니메이션 발생 => 공격판정시작기다리는 코루틴 두 메서드를 가져야함
        //그게 싫으면 스위치문에서 가르자.
        switch (skillId)
        {
            case 2100: 
                player.PlayWeaponAnimation(type, 0);
                this.StartCoroutine(PlayerSkill2104(2104));
                this.StartCoroutine(PlayerSkill2100(2100));
                break;//근접기본공격이 들어왔을때 기본공격 강화 스킬들을 확인하고, 
            case 2101:
                player.PlayWeaponAnimation(type, 0);
                this.StartCoroutine(PlayerSkill2101(2101)); break;
            case 2102:
                player.PlayWeaponAnimation(type, 0);
                this.StartCoroutine(PlayerSkill2102(2102)); break;
            case 2103:
                player.PlayWeaponAnimation(type, 0);
                this.StartCoroutine(PlayerSkill2103(2103)); break;
            case 2104:
                this.playerCanMove = true;
                break;
            case 2105:
                rangeIds = DataManager.instance.dicAreaSkill.Where((x) => x.Value.skillId == 2105);
                area = stageMain.RangeMaking(rangeIds);
                player.PlayWeaponAnimation("BladeStorm");
                this.StartCoroutine(PlayerSkill2105(area,2105)); break;
            case 2106:
                rangeIds = DataManager.instance.dicAreaSkill.Where((x) => x.Value.skillId == 2106);
                area = stageMain.RangeMaking(rangeIds);
                player.PlayWeaponAnimation("BladeStorm");
                this.StartCoroutine(PlayerSkill2106(area, 2106)); break;
            case 2107:
                rangeIds = DataManager.instance.dicAreaSkill.Where((x) => x.Value.skillId == 2107);
                area = stageMain.RangeMaking(rangeIds);
                player.satellite.SetActive(true);
                this.StartCoroutine(PlayerSkill2107(area, 2107)); break;
            case 2108:
                player.PlayWeaponAnimation(type, 0);
                rangeIds = DataManager.instance.dicAreaSkill.Where((x) => x.Value.skillId == 2108);
                area = stageMain.RangeMaking(rangeIds);
                this.StartCoroutine(PlayerSkill2108(area, 2108)); break;
            case 2109:
                player.PlayWeaponAnimation(type, 0);
                this.StartCoroutine(PlayerSkill2109(2109)); break;
            case 2110:
                this.StartCoroutine(PlayerSkill2110(2110)); break;
            case 2111:
                this.StartCoroutine(PlayerSkill2111(2111)); break;
            case 2112:
                this.StartCoroutine(PlayerSkill2112(2112)); break;
            case 2200:
                player.PlayWeaponAnimation(type, 0);
                this.StartCoroutine(PlayerSkill2210(2210));
                break;//근접기본공격이 들어왔을때 기본공격 강화 스킬들을 확인하고, 
            case 2203:
                player.PlayWeaponAnimation(type, 0);
                rangeIds = DataManager.instance.dicAreaSkill.Where((x) => x.Value.skillId == 2203);
                area = stageMain.RangeMaking(rangeIds);
                this.StartCoroutine(PlayerSkill2203(area,2203)); break;
            case 2204:
                player.PlayWeaponAnimation(type, 0);
                this.StartCoroutine(PlayerSkill2204(2204)); break;
            case 2207:
                player.PlayWeaponAnimation(type, 0);
                this.StartCoroutine(PlayerSkill2207(2207)); break;
            case 2302:
                player.PlayWeaponAnimation(type, 0);
                this.StartCoroutine(PlayerSkill2302(2302)); break;
            case 2303:
                player.PlayWeaponAnimation(type, 0);
                this.StartCoroutine(PlayerSkill2303(2303)); break;
            case 2304:
                player.PlayWeaponAnimation(type, 0);
                this.StartCoroutine(PlayerSkill2304(2304)); break;
            case 2305:
                player.PlayWeaponAnimation(type, 0);
                rangeIds = DataManager.instance.dicAreaSkill.Where((x) => x.Value.skillId == 2305);
                area = stageMain.RangeMaking(rangeIds);
                this.StartCoroutine(PlayerSkill2305(area, 2305)); break;
            case 2306:
                player.PlayWeaponAnimation(type, 0);
                this.StartCoroutine(PlayerSkill2306(2306)); break;
            case 2307:
                player.PlayWeaponAnimation(type, 0);
                rangeIds = DataManager.instance.dicAreaSkill.Where((x) => x.Value.skillId == 2307);
                area = stageMain.RangeMaking(rangeIds);
                this.StartCoroutine(PlayerSkill2307(area)); break;
            case 2308:
                player.PlayWeaponAnimation(type, 0);
                rangeIds = DataManager.instance.dicAreaSkill.Where((x) => x.Value.skillId == 2308);
                area = stageMain.RangeMaking(rangeIds);
                this.StartCoroutine(PlayerSKill2308(area)); break;
            case 2309:
                player.PlayWeaponAnimation(type, 0);
                this.StartCoroutine(PlayerSkill2309(2309)); break;
            case 2311:
                player.PlayWeaponAnimation(type, 0);
                rangeIds = DataManager.instance.dicAreaSkill.Where((x) => x.Value.skillId == 2311);
                area = stageMain.RangeMaking(rangeIds);
                this.StartCoroutine(PlayerSkill2311(area, 2311)); break;
            case 2312:
                player.PlayWeaponAnimation(type, 0);
                rangeIds = DataManager.instance.dicAreaSkill.Where((x) => x.Value.skillId == 2312);
                area = stageMain.RangeMaking(rangeIds);
                this.StartCoroutine(PlayerSkill2312(area, 2312)); break;
            default:
                player.PlayWeaponAnimation(type, 0);
                if (DataManager.instance.dicActiveSkill[skillId].isProjectile) //투사체공격
                {
                    this.StartCoroutine(ProjectileAttackWaitAnim(skillId));
                }
                else if (DataManager.instance.dicActiveSkill[skillId].isArea) //범위공격
                {
                    rangeIds = DataManager.instance.dicAreaSkill.Where((x) => x.Value.skillId == skillId);
                    area = stageMain.RangeMaking(rangeIds);
                    this.StartCoroutine(RangeAttackWaitAnim(area, skillId));
                }
                break;
        }
    }

    private IEnumerator RangeAttackWaitAnim(List<Vector2Int> area, int skillId, UnityAction callback = null)
    {
        callback?.Invoke();
        yield return new WaitUntil(() => player.weapon.AttackStart);

        int damage;
        if (skillId == 2100 || skillId == 2300)
        {
            damage = PlayerDamageCalculate(false, skillId);
        }
        else
        {
            damage = PlayerDamageCalculate(true, skillId);
        }
        List<int> hitMonsters = new();
        foreach (Vector2Int coord in area) // 공격 범위 좌표에 몬스터가 있는지 확인
        {
            Vector2Int rotatedCoord = stageMain.RotationalTransform(coord, player.dir); // 회전 변환된 좌표
            Vector2Int finalCoord = player.location + rotatedCoord; // 플레이어 위치 기준 좌표
            if (finalCoord.x >= 0 && finalCoord.y >= 0 && finalCoord.x < stageMain.sizeX && finalCoord.y < stageMain.sizeY) // 배열 범위를 넘어가는지 확인
            {
                StartCoroutine(stageMain.ChangeTileColor(stageMain.tileInfoArray[finalCoord.x, finalCoord.y].position));
                int objId = this.stageMain.tileInfoArray[finalCoord.x, finalCoord.y].objId;
                if (10000 <= objId && objId <= 99999)
                {
                    if (skillId == 2300)
                    {
                        effectManager.IndependentEffect(7, (Vector2)stageMain.tileInfoArray[finalCoord.x, finalCoord.y].position);
                        stageMain.monsters[objId % 100].Hit(damage);
                        break;
                    }
                    hitMonsters.Add(objId % 100);
                }
            }
        }
        hitMonsters = hitMonsters.Distinct().ToList();
        foreach (int id in hitMonsters)
        {
            stageMain.monsters[id].Hit(damage);
        }
        yield return null;
        this.player.weapon.AttackStart = false;
        yield return null;
    }

    private void RangeAttack(List<Vector2Int> area, int skillId)
    {
        int damage;
        if (skillId == 2100 || skillId == 2300)
        {
            damage = PlayerDamageCalculate(false, skillId);
        }
        else
        {
            damage = PlayerDamageCalculate(true, skillId);
        }
        List<int> hitMonsters = new();
        foreach (Vector2Int coord in area) // 공격 범위 좌표에 몬스터가 있는지 확인
        {
            Vector2Int rotatedCoord = stageMain.RotationalTransform(coord, player.dir); // 회전 변환된 좌표
            Vector2Int finalCoord = player.location + rotatedCoord; // 플레이어 위치 기준 좌표
            if (finalCoord.x >= 0 && finalCoord.y >= 0 && finalCoord.x < stageMain.sizeX && finalCoord.y < stageMain.sizeY) // 배열 범위를 넘어가는지 확인
            {
                StartCoroutine(stageMain.ChangeTileColor(stageMain.tileInfoArray[finalCoord.x, finalCoord.y].position));
                int objId = this.stageMain.tileInfoArray[finalCoord.x, finalCoord.y].objId;
                if (10000 <= objId && objId <= 99999)
                {
                    hitMonsters.Add(objId % 100);
                }
            }
        }
        hitMonsters = hitMonsters.Distinct().ToList();
        foreach (int id in hitMonsters)
        {
            if (stageMain.monsters[id] != null)
            {
                stageMain.monsters[id].Hit(damage);
            }
        }
    }

    private void RangeAttack(Vector2Int coord, int skillId)
    {
        int damage;
        if (skillId == 2100 || skillId == 2300)
        {
            damage = PlayerDamageCalculate(false, skillId);
        }
        else
        {
            damage = PlayerDamageCalculate(true, skillId);
        }
        Vector2Int rotatedCoord = stageMain.RotationalTransform(coord, player.dir);
        Vector2Int finalCoord = player.location + rotatedCoord; // 플레이어 위치 기준 좌표
        if (finalCoord.x >= 0 && finalCoord.y >= 0 && finalCoord.x < stageMain.sizeX && finalCoord.y < stageMain.sizeY) // 배열 범위를 넘어가는지 확인
        {
            StartCoroutine(stageMain.ChangeTileColor(stageMain.tileInfoArray[finalCoord.x, finalCoord.y].position));
            int objId = this.stageMain.tileInfoArray[finalCoord.x, finalCoord.y].objId;
            if (10000 <= objId && objId <= 99999)
            {
                if (stageMain.monsters[objId % 100] != null)
                {
                    stageMain.monsters[objId % 100].Hit(damage);
                }
            }
        }
    }
    private void CoordRangeAttack(List<Vector2Int> area, int skillId)
    {
        int damage;
        if (skillId == 2100 || skillId == 2300)
        {
            damage = PlayerDamageCalculate(false, skillId);
        }
        else
        {
            damage = PlayerDamageCalculate(true, skillId);
        }
        List<int> hitMonsters = new();
        foreach (Vector2Int coord in area) // 공격 범위 좌표에 몬스터가 있는지 확인
        {
            if (coord.x >= 0 && coord.y >= 0 && coord.x < stageMain.sizeX && coord.y < stageMain.sizeY) // 배열 범위를 넘어가는지 확인
            {
                StartCoroutine(stageMain.ChangeTileColor(stageMain.tileInfoArray[coord.x, coord.y].position));
                int objId = this.stageMain.tileInfoArray[coord.x, coord.y].objId;
                if (10000 <= objId && objId <= 99999)
                {
                    hitMonsters.Add(objId % 100);
                }
            }
        }
        hitMonsters = hitMonsters.Distinct().ToList();
        foreach (int id in hitMonsters)
        {
            if (stageMain.monsters[id] != null)
            {
                stageMain.monsters[id].Hit(damage);
            }
        }
    }

    private void CoordRangeAttack(Vector2Int coord,int skillId)
    {
        int damage;
        if (skillId == 2100 || skillId == 2300)
        {
            damage = PlayerDamageCalculate(false, skillId);
        }
        else
        {
            damage = PlayerDamageCalculate(true, skillId);
        }
        if (coord.x >= 0 && coord.y >= 0 && coord.x < stageMain.sizeX && coord.y < stageMain.sizeY) // 배열 범위를 넘어가는지 확인
        {
            StartCoroutine(stageMain.ChangeTileColor(stageMain.tileInfoArray[coord.x, coord.y].position));
            int objId = this.stageMain.tileInfoArray[coord.x, coord.y].objId;
            if (10000 <= objId && objId <= 99999 && stageMain.monsters[objId % 100] != null)
            {
                stageMain.monsters[objId % 100].Hit(damage);
            }
        }
    }

    private IEnumerator ProjectileAttackWaitAnim(int skillId)
    {
        int type = DataManager.instance.dicWeapon[InfoManager.instance.gameInfo.weapon].id / 10;
        int damage;
        if (skillId == 2200)
        {
            damage = PlayerDamageCalculate(false, skillId);
        }
        else
        {
            damage = PlayerDamageCalculate(true, skillId);
        }
        yield return new WaitUntil(() => player.weapon.AttackStart);
        if (type == 123)
        {
            stageMain.InstantiateProjectile(player.location, player.dir, skillId, 0, 0, damage, projectileSprites[4]);
        }
        else 
        {
            stageMain.InstantiateProjectile(player.location, player.dir, skillId, 0, 0, damage, projectileSprites[3]);
        }
        yield return null;
        this.player.weapon.AttackStart = false;
        yield return null;
    }

    private Vector2 GetFinalPos(Vector2Int coord)
    {
        Vector2Int rotatedCoord = stageMain.RotationalTransform(coord, player.dir); // 회전 변환된 좌표
        Vector2Int finalCoord = player.location + rotatedCoord; // 플레이어 위치 기준 좌표
        return new Vector2(finalCoord.x + stageMain.bl.x, finalCoord.y + stageMain.bl.y);
    }

    private IEnumerator PlayerSkill2100(int skillId)
    {
        var rangeIds = DataManager.instance.dicAreaSkill.Where((x) => x.Value.skillId == 2100);
        List<Vector2Int> area = stageMain.RangeMaking(rangeIds);
        if (InfoManager.instance.gameInfo.weapon / 10 == 114)
        {
            area.Add(new Vector2Int(0, 2));
        }
        this.StartCoroutine(RangeAttackWaitAnim(area, 2100, ()=> 
        {
            effectManager.PlayEffect(0, GetFinalPos(area[1]));
        }));
        yield return null;
    }

    private IEnumerator PlayerSkill2101(int skillId)
    {
        yield return new WaitUntil(() => player.weapon.AttackStart);
        stageMain.InstantiateProjectile(player.location, player.dir, 2101, 0, 0, PlayerDamageCalculate(true, skillId), projectileSprites[0]);
        yield return null;
        this.player.weapon.AttackStart = false;
        yield return null;
    }

    private IEnumerator PlayerSkill2102(int skillId)
    {
        yield return new WaitUntil(() => player.weapon.AttackStart);
        stageMain.InstantiateProjectile(player.location, player.dir, 2102,0, 0, PlayerDamageCalculate(true,skillId),projectileSprites[1],1);
        yield return null;
        this.player.weapon.AttackStart = false;
        yield return null;
    }

    private IEnumerator PlayerSkill2103(int skillId)
    {
        yield return new WaitUntil(() => player.weapon.AttackStart);
        stageMain.InstantiateProjectile(player.location, player.dir, 2103,0, 0, PlayerDamageCalculate(true, skillId), projectileSprites[2],2);
        yield return null;
        this.player.weapon.AttackStart = false;
        yield return null;
    }

    private IEnumerator PlayerSkill2104(int skillId)
    {
        if (System.Array.Exists(InfoManager.instance.gameInfo.skills, x => x == 2104))
        {
            yield return new WaitUntil(() => player.weapon.AttackStart);
            stageMain.InstantiateProjectile(player.location, player.dir, 2104, 0, 0, PlayerDamageCalculate(true, skillId), projectileSprites[0]);
            yield return null;
            this.player.weapon.AttackStart = false;
            yield return null;
        }
    }

    private IEnumerator PlayerSkill2105(List<Vector2Int> area,int skillId)
    {
        playerCanMove = true;
        for (int i = 0; i < 4; ++i)
        {
            uiJoysticks.swipeButtons[i].gameObject.SetActive(false);
        }
        GameObject effectGo = effectManager.ContinuePlayEffect(1, player.transform.position);
        for (int i = 0; i < 4; ++i)
        {
            RangeAttack(area, skillId);
            yield return new WaitForSeconds(0.34f);
        }
        RangeAttack(area, skillId);
        Destroy(effectGo);
        for (int i = 0; i < 4; ++i)
        {
            uiJoysticks.swipeButtons[i].gameObject.SetActive(true);
        }
        this.player.weapon.AttackStart = false;
        yield return null;
    }
    private IEnumerator PlayerSkill2106(List<Vector2Int> area, int skillId)
    {
        playerCanMove = true;
        for (int i = 0; i < 4; ++i)
        {
            uiJoysticks.swipeButtons[i].gameObject.SetActive(false);
        }
        GameObject effectGo = effectManager.ContinuePlayEffect(1, player.transform.position);
        for (int i = 0; i < 4; ++i)
        {
            RangeAttack(area, skillId);
            yield return new WaitForSeconds(0.34f);
        }
        RangeAttack(area, skillId);
        Destroy(effectGo);
        for (int i = 0; i < 4; ++i)
        {
            uiJoysticks.swipeButtons[i].gameObject.SetActive(true);
        }
        this.player.weapon.AttackStart = false;
        yield return null;
    }

    private IEnumerator PlayerSkill2107(List<Vector2Int> area, int skillId)
    {
        playerCanMove = true;
        this.player.weapon.AttackStart = false;
        for (int i = 0; i < 4; ++i)
        {
            if (InfoManager.instance.gameInfo.skills[i] == 2107)
            {
                uiJoysticks.swipeButtons[i].gameObject.SetActive(false);
            }
        }
        yield return null;
        GameObject effectGo = effectManager.ContinuePlayEffect(1, player.transform.position);
        while (stageMain.fieldTile != null)
        {
            RangeAttack(area, skillId);
            yield return new WaitForSeconds(0.4f);
        }
        Destroy(effectGo);
        this.player.satellite.SetActive(false);
    }
    private IEnumerator PlayerSkill2108(List<Vector2Int> area, int skillId)
    {
        List<Vector2Int> target = new List<Vector2Int>();
        foreach (Vector2Int a in area)
        {
            target.Add(Vector2Int.RoundToInt(GetFinalPos(a)));
        }
        yield return new WaitUntil(() => player.weapon.AttackStart);
        yield return new WaitForSeconds(0.5f);
        for (int i = 0; i < target.Count; ++i)
        {
            effectManager.IndependentEffect(2, (Vector2)target[i]);
            CoordRangeAttack(target[i] - stageMain.bl, skillId);
        }
        yield return null;
        this.player.weapon.AttackStart = false;
        yield return null;
    }
    private IEnumerator PlayerSkill2109(int skillId)
    {
        if(!isTileChecked)
        {
            this.tileMapCheck = new bool[stageMain.tileInfoArray.GetLength(0), stageMain.tileInfoArray.GetLength(1)];
            playerDFS(player.location);
            this.isTileChecked = true;
        }
        yield return new WaitUntil(() => player.weapon.AttackStart);
        List<Vector2Int> area = new List<Vector2Int>();
        for (int i = 0; i < tileMapCheck.GetLength(0); ++i)
        {
            for (int j = 0; j < tileMapCheck.GetLength(1); ++j)
            {
                if (tileMapCheck[i, j])
                {
                    area.Add(new Vector2Int(i, j));
                }
            }
        }
        yield return new WaitForSeconds(0.5f);
        foreach (Vector2Int a in area)
        {
            effectManager.IndependentEffect(2, new Vector3(a.x + stageMain.bl.x ,a.y + stageMain.bl.y ));
        }
        CoordRangeAttack(area, skillId);
        yield return null;
        this.player.weapon.AttackStart = false;
        yield return null;
    }
    private IEnumerator PlayerSkill2110(int skillId)
    {
        int monIdx = -1;
        float minimum = 100000000000;
        for (int i = 0; i < stageMain.monsters.Count; ++i)
        {
            if (stageMain.monsters[i] == null) continue;
            else
            {
                if ((stageMain.monsters[i].transform.position - player.transform.position).magnitude < minimum)
                {
                    minimum = (stageMain.monsters[i].transform.position - player.transform.position).magnitude;
                    monIdx = i;
                }
            }
        }
        if (monIdx == -1)
        {
            playerCanMove = true;
            this.player.weapon.AttackStart = false;
            yield break;
        }
        List<Vector2Int> moveLoc = new List<Vector2Int>();
        int[] nx = { 0, 0, -1, 1 };
        int[] ny = { 1, -1, 0, 0 };
        for (int i = 0; i < 4; ++i)
        {
            if (stageMain.tileInfoArray[stageMain.monsters[monIdx].location.x + nx[i], stageMain.monsters[monIdx].location.y + ny[i]].objId == 0)
            {
                moveLoc.Add(new Vector2Int(stageMain.monsters[monIdx].location.x + nx[i], stageMain.monsters[monIdx].location.y + ny[i]));
            }
        }

        if (moveLoc.Count > 0)
        {
            int rand = Random.Range(0, moveLoc.Count);
            Vector2Int moveCoord = moveLoc[rand];
            stageMain.tileInfoArray[player.location.x, player.location.y].objId = ConstantIDs.BLANK;
            player.location = moveCoord;
            stageMain.tileInfoArray[moveCoord.x, moveCoord.y].objId = ConstantIDs.PLAYER;
            //player.gameObject.transform.position = (Vector2)stageMain.tileInfoArray[moveCoord.x, moveCoord.y].position;
            player.gameObject.transform.DOMove((Vector2)stageMain.tileInfoArray[moveCoord.x, moveCoord.y].position, 0.06f);
            this.StartCoroutine(stageMain.ChangeTileColor(stageMain.tileInfoArray[stageMain.monsters[monIdx].location.x, stageMain.monsters[monIdx].location.y].position));
            effectManager.IndependentEffect(3, stageMain.monsters[monIdx].transform.position);
            stageMain.monsters[monIdx].Hit(PlayerDamageCalculate(true, skillId));
        }
        playerCanMove = true;
        yield return null;
        this.player.weapon.AttackStart = false;
        yield return null;
    }

    private IEnumerator PlayerSkill2111(int skillId)
    {
        for (int i = 0; i < 3; ++i)
        {
            this.StartCoroutine(PlayerSkill2110(skillId));
            yield return new WaitForSeconds(0.3f);
        }
    }
    private IEnumerator PlayerSkill2112(int skillId)
    {

        for (int i = 0; i < stageMain.monsters.Count; ++i)
        {
            this.StartCoroutine(PlayerSkill2110(skillId));
            yield return new WaitForSeconds(0.3f);
        }
    }


    private IEnumerator PlayerSkill2203(List<Vector2Int> area, int skillId)
    {
        List<Vector2Int> target = new List<Vector2Int>();
        foreach (Vector2Int a in area)
        {
            target.Add(Vector2Int.RoundToInt(GetFinalPos(a)));
        }
        yield return new WaitUntil(() => player.weapon.AttackStart);
        int[] rand = new int[30];
        for (int i = 0; i < 30; ++i)
        {
            rand[i] = Random.Range(0, area.Count);
            effectManager.IndependentEffect(5, (Vector2)target[rand[i]]);
            StartCoroutine(PlayerArrowRainRoutine(target[rand[i]] - stageMain.bl, skillId));
            yield return new WaitForSeconds(0.1f);
        }

        this.player.weapon.AttackStart = false;
        yield return null;
    }

    private IEnumerator PlayerArrowRainRoutine(Vector2Int loc, int skillId)
    {
        for (int i = 0; i < 4; ++i)
        {
            yield return new WaitForSeconds(0.2f);
            CoordRangeAttack(loc, skillId);
        }
        yield return null;
    }

    private IEnumerator PlayerSkill2204(int skillId)
    {
        if (!isTileChecked)
        {
            this.tileMapCheck = new bool[stageMain.tileInfoArray.GetLength(0), stageMain.tileInfoArray.GetLength(1)];
            playerDFS(player.location);
            this.isTileChecked = true;
        }
        List<Vector2Int> area = new List<Vector2Int>();
        yield return new WaitUntil(() => player.weapon.AttackStart);
        for (int i = 0; i < tileMapCheck.GetLength(0); ++i)
        {
            for (int j = 0; j < tileMapCheck.GetLength(1); ++j)
            {
                if (tileMapCheck[i, j])
                {
                    area.Add(new Vector2Int(i, j));
                }
            }
        }
        List<Vector2Int> inst = area.ToList();
        for (int i = 0; i < 10; ++i)
        {
            for (int j = 0; j < inst.Count / 4; ++j)
            {
                int rand = Random.Range(0, inst.Count);
                effectManager.IndependentEffect(5, (Vector2)inst[rand]+stageMain.bl);
                StartCoroutine(PlayerArrowRainRoutine(inst[rand], skillId));
                inst.RemoveAt(rand);
            }
            yield return new WaitForSeconds(0.1f);
            inst = area.ToList();
        }
        yield return null;
        this.player.weapon.AttackStart = false;
        yield return null;
    }

    private IEnumerator PlayerSkill2207(int skillId)
    {
        if (!isTileChecked)
        {
            this.tileMapCheck = new bool[stageMain.tileInfoArray.GetLength(0), stageMain.tileInfoArray.GetLength(1)];
            playerDFS(player.location);
            this.isTileChecked = true;
        }
        List<Vector2Int> area = new List<Vector2Int>();
        yield return new WaitUntil(() => player.weapon.AttackStart);
        for (int i = 0; i < tileMapCheck.GetLength(0); ++i)
        {
            for (int j = 0; j < tileMapCheck.GetLength(1); ++j)
            {
                if (tileMapCheck[i, j])
                {
                    area.Add(new Vector2Int(i, j));
                }
            }
        }
        List<Vector2Int> range = new List<Vector2Int>();
        for (int i = 0; i < 5; ++i)
        {
            int[] randArray = new int[4];
            for (int j = 0; j < 4;)
            {
                bool isExist = false;
                int checkRand = Random.Range(0, area.Count);
                for (int k = j; k > 0; --k)
                {
                    if (randArray[k - 1] == checkRand)
                    {
                        isExist = true; 
                        break;
                    }
                }
                if (isExist) continue;
                else
                {
                    randArray[j] = checkRand;
                    ++j;
                }
            }
            for (int j = 0; j < 4; ++j)
            {
                effectManager.IndependentEffect(6, (Vector2)area[randArray[j]] + stageMain.bl, 0.797f);
            }
            yield return new WaitForSeconds(0.39f);
            for (int j = 0; j < 4; ++j)
            {
                for (int k = -3; k < 4; ++k)
                {
                    for (int l = -3; l < 4; ++l)
                    {
                        range.Add(new Vector2Int(area[randArray[j]].x + k, area[randArray[j]].y +l));
                    }
                }
                CoordRangeAttack(range, skillId);
                range.Clear();
            }
            yield return new WaitForSeconds(0.8f);
        }
        yield return null;
        this.player.weapon.AttackStart = false;
        yield return null;
    }

    private IEnumerator PlayerSkill2210(int skillId)
    {
        int type = DataManager.instance.dicWeapon[InfoManager.instance.gameInfo.weapon].id / 10;
        if (System.Array.Exists(InfoManager.instance.gameInfo.skills, x => x == 2210))
        {
            yield return new WaitUntil(() => player.weapon.AttackStart);
            if (type == 123)
            {
                stageMain.InstantiateProjectile(player.location, player.dir, 2210, 0, 0, PlayerDamageCalculate(true, skillId), projectileSprites[4]);
            }
            else
            {
                stageMain.InstantiateProjectile(player.location, player.dir, 2210, 0, 0, PlayerDamageCalculate(true, skillId), projectileSprites[3]);
            }
            yield return null;
            this.player.weapon.AttackStart = false;
            yield return null;
        }
        else
        {
            yield return new WaitUntil(() => player.weapon.AttackStart);
            if (type == 123)
            {
                stageMain.InstantiateProjectile(player.location, player.dir, 2200, 0, 0, PlayerDamageCalculate(true, 2200), projectileSprites[4]);
            }
            else
            {
                stageMain.InstantiateProjectile(player.location, player.dir, 2200, 0, 0, PlayerDamageCalculate(true, 2200), projectileSprites[3]);
            }
            yield return null;
            this.player.weapon.AttackStart = false;
            yield return null;
        }
    }
    private IEnumerator PlayerSkill2302(int skillId)
    {
        yield return new WaitUntil(() => player.weapon.AttackStart);
for (int i = 0; i < 3; ++i)
        {
            stageMain.InstantiateProjectile(player.location, player.dir, 2302,0, 0, PlayerDamageCalculate(true, skillId));
            yield return new WaitForSeconds(0.1f);
        }
        this.player.weapon.AttackStart = false;
        yield return null;
    }

    private IEnumerator PlayerSkill2303(int skillId)
    {
        yield return new WaitUntil(() => player.weapon.AttackStart);
        for (int i = 0; i < 5; ++i)
        {
            stageMain.InstantiateProjectile(player.location, player.dir, 2302,0, 0, PlayerDamageCalculate(true, skillId));
            yield return new WaitForSeconds(0.06f);
        }
        this.player.weapon.AttackStart = false;
        yield return null;
    }
    private IEnumerator PlayerSkill2304(int skillId)
    {
        yield return new WaitUntil(() => player.weapon.AttackStart);
        for (int i = 0; i < 8; ++i)
        {
            stageMain.InstantiateProjectile(player.location, player.dir, 2302,0, 0, PlayerDamageCalculate(true, skillId));
            yield return new WaitForSeconds(0.04f);
        }
        this.player.weapon.AttackStart = false;
        yield return null;
    }

    private IEnumerator PlayerSkill2305(List<Vector2Int> area, int skillId)
    {
        List<Vector2Int> target = new List<Vector2Int>();
        foreach (Vector2Int a in area)
        {
            target.Add(Vector2Int.RoundToInt(GetFinalPos(a)) - stageMain.bl);
        }
        yield return new WaitUntil(() => player.weapon.AttackStart);
        effectManager.IndependentEffect(8, (Vector2)target[0]+ stageMain.bl, 0.5f);
        yield return new WaitForSeconds(0.2f);
        CoordRangeAttack(target, skillId);
        yield return null;
        this.player.weapon.AttackStart = false;
        yield return null;
    }

    private IEnumerator PlayerSkill2306( int skillId)
    {
        if (!isTileChecked)
        {
            this.tileMapCheck = new bool[stageMain.tileInfoArray.GetLength(0), stageMain.tileInfoArray.GetLength(1)];
            playerDFS(player.location);
            this.isTileChecked = true;
        }
        List<Vector2Int> area = new List<Vector2Int>();
        yield return new WaitUntil(() => player.weapon.AttackStart);
        for (int i = 0; i < tileMapCheck.GetLength(0); ++i)
        {
            for (int j = 0; j < tileMapCheck.GetLength(1); ++j)
            {
                if (tileMapCheck[i, j])
                {
                    area.Add(new Vector2Int(i, j));
                }
            }
        }
        List<Vector2Int> range = new List<Vector2Int>();
        for (int i = 0; i < 5; ++i)
        {
            int[] randArray = new int[2];
            for (int j = 0; j < 2;)
            {
                bool isExist = false;
                int checkRand = Random.Range(0, area.Count);
                for (int k = j; k > 0; --k)
                {
                    if (randArray[k - 1] == checkRand)
                    {
                        isExist = true;
                        break;
                    }
                }
                if (isExist) continue;
                else 
                {
                    randArray[j] = checkRand;
                    ++j;
                }
            }
            for (int j = 0; j < 2; ++j)
            {
                effectManager.IndependentEffect(9, (Vector2)area[randArray[j]] + stageMain.bl, 0.85f);
            }
            yield return new WaitForSeconds(0.4f);
            for (int j = 0; j < 2; ++j)
            {
                for (int k = -4; k < 5; ++k)
                {
                    for (int l = -4; l < 5; ++l)
                    {
                        range.Add(new Vector2Int(area[randArray[j]].x + k, area[randArray[j]].y + l));
                    }
                }
                CoordRangeAttack(range, skillId);
                range.Clear();
            }
           yield return new WaitForSeconds(1.2f);
        }
        yield return null;
        this.player.weapon.AttackStart = false;
        yield return null;
    }

    private IEnumerator PlayerSkill2307(List<Vector2Int> area)
    {
        yield return new WaitUntil(() => player.weapon.AttackStart);

        int damage;
        damage = PlayerDamageCalculate(true, 2307);
        List<int> hitMonsters = new();
        foreach (Vector2Int coord in area) // 공격 범위 좌표에 몬스터가 있는지 확인
        {
            Vector2Int rotatedCoord = stageMain.RotationalTransform(coord, player.dir); // 회전 변환된 좌표
            Vector2Int finalCoord = player.location + rotatedCoord; // 플레이어 위치 기준 좌표
            if (finalCoord.x >= 0 && finalCoord.y >= 0 && finalCoord.x < stageMain.sizeX && finalCoord.y < stageMain.sizeY) // 배열 범위를 넘어가는지 확인
            {
                StartCoroutine(stageMain.ChangeTileColor(stageMain.tileInfoArray[finalCoord.x, finalCoord.y].position));
                int objId = this.stageMain.tileInfoArray[finalCoord.x, finalCoord.y].objId;
                if (10000 <= objId && objId <= 99999)
                {
                    effectManager.IndependentEffect(10, (Vector2)stageMain.tileInfoArray[finalCoord.x, finalCoord.y].position, 1);
                    yield return null;
                    hitMonsters.Add(objId % 100);
                }
            }
        }
        hitMonsters = hitMonsters.Distinct().ToList();
        foreach (int id in hitMonsters)
        {
            stageMain.monsters[id].Hit(damage);
        }
        yield return null;
        this.player.weapon.AttackStart = false;
        yield return null;
    }

    private IEnumerator PlayerSKill2308(List<Vector2Int> area)
    {
        yield return new WaitUntil(() => player.weapon.AttackStart);

        int damage;
        damage = PlayerDamageCalculate(true, 2308);
        List<int> hitMonsters = new();
        foreach (Vector2Int coord in area) // 공격 범위 좌표에 몬스터가 있는지 확인
        {
            Vector2Int rotatedCoord = stageMain.RotationalTransform(coord, player.dir); // 회전 변환된 좌표
            Vector2Int finalCoord = player.location + rotatedCoord; // 플레이어 위치 기준 좌표
            if (finalCoord.x >= 0 && finalCoord.y >= 0 && finalCoord.x < stageMain.sizeX && finalCoord.y < stageMain.sizeY) // 배열 범위를 넘어가는지 확인
            {
                StartCoroutine(stageMain.ChangeTileColor(stageMain.tileInfoArray[finalCoord.x, finalCoord.y].position));
                int objId = this.stageMain.tileInfoArray[finalCoord.x, finalCoord.y].objId;
                if (10000 <= objId && objId <= 99999)
                {
                    effectManager.IndependentEffect(10, (Vector2)stageMain.tileInfoArray[finalCoord.x, finalCoord.y].position, 1);
                    yield return null;
                    hitMonsters.Add(objId % 100);
                }
            }
        }
        hitMonsters = hitMonsters.Distinct().ToList();
        foreach (int id in hitMonsters)
        {
            stageMain.monsters[id].Hit(damage);
        }
        yield return null;
        this.player.weapon.AttackStart = false;
        yield return null;
    }

    private IEnumerator PlayerSkill2309(int skillId)
    {
        yield return new WaitUntil(() => player.weapon.AttackStart);
        for (int i =0; i<stageMain.monsters.Count; ++i)
        {
            if (stageMain.monsters[i] == null) continue;
            effectManager.IndependentEffect(10, stageMain.monsters[i].gameObject.transform.position, 1);
            yield return null;
            StartCoroutine(stageMain.ChangeTileColor(stageMain.tileInfoArray[stageMain.monsters[i].location.x, stageMain.monsters[i].location.y].position));
            stageMain.monsters[i].Hit(PlayerDamageCalculate(true, skillId));
        }
    
        yield return null;
        this.player.weapon.AttackStart = false;
        yield return null;
    }

    private IEnumerator PlayerSkill2311(List<Vector2Int> area,int skillId)
    {
        List<GameObject> effectGOs = new List<GameObject>();
        List<Vector2> rangePos = new List<Vector2>();
        foreach (Vector2Int a in area)
        {
            rangePos.Add(GetFinalPos(a));
        }
        Vector2Int fixedLoc = stageMain.player.location;
        yield return new WaitUntil(() => player.weapon.AttackStart);
        for (int i = 0; i < rangePos.Count; ++i)
        {
            GameObject effectGo = effectManager.ContinueIndependentPlayEffect(11, rangePos[i]);
            effectGOs.Add(effectGo);
        }
        yield return null;
        while (stageMain.player.location == fixedLoc)
        {
            RangeAttack(area, skillId);
            yield return new WaitForSeconds(0.2f);
        }
        for (int i = 0; i < rangePos.Count; ++i)
        {
            Destroy(effectGOs[i]);
        }
        yield return null;
    }
    private IEnumerator PlayerSkill2312(List<Vector2Int> area, int skillId)
    {
        List<GameObject> effectGOs = new List<GameObject>();
        List<Vector2> rangePos = new List<Vector2>();
        foreach (Vector2Int a in area)
        {
            rangePos.Add(GetFinalPos(a));
        }
        Vector2Int fixedLoc = stageMain.player.location;
        yield return new WaitUntil(() => player.weapon.AttackStart);
        for (int i = 0; i < rangePos.Count; ++i)
        {
            GameObject effectGo = effectManager.ContinueIndependentPlayEffect(11, rangePos[i]);
            effectGOs.Add(effectGo);
        }
        yield return null;
        while (stageMain.player.location == fixedLoc)
        {
            RangeAttack(area, skillId);
            yield return new WaitForSeconds(0.2f);
        }
        for (int i = 0; i < rangePos.Count; ++i)
        {
            Destroy(effectGOs[i]);
        }
        yield return null;
    }
}