using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.U2D;
using System.Linq;
using DG.Tweening;
using UnityEngine.Rendering;

public class ChapterMain : MonoBehaviour
{
    public MapManager mapManager;
    public GameObject chapterMap;
    public MonsterController monsterController;
    public List<GameObject> chapter1BattlePrefabs;
    public List<GameObject> chapter1ElitePrefabs;
    public List<GameObject> chapter1ShopPrefabs;
    public List<GameObject> chapter1QuestionPrefabs;
    public List<GameObject> chapter1BossPrefabs;

    public List<GameObject> chapter2BattlePrefabs;
    public List<GameObject> chapter2ElitePrefabs;
    public List<GameObject> chapter2ShopPrefabs;
    public List<GameObject> chapter2QuestionPrefabs;
    public List<GameObject> chapter2BossPrefabs;

    public List<GameObject> chapter3BattlePrefabs;
    public List<GameObject> chapter3ElitePrefabs;
    public List<GameObject> chapter3ShopPrefabs;
    public List<GameObject> chapter3QuestionPrefabs;
    public List<GameObject> chapter3BossPrefabs;

    public Player player;
    public PlayerController playerController;

    public EffectManager effectManager;

    int[] resultWeaponId = new int[3];
    int[] resultSkillId = new int[3];

    public GameObject[] weaponChestPrefabs;
    public GameObject[] skillChestPrefabs;

    private StageBase nowStage;
    int mapId;
    int nowMapId;
    private GameObject nowStageField;
    public int nowChapter = 0;

    public UnityAction<int> onChapterSceneEndAction;

    private GameObject weaponChestModelGO;
    private GameObject skillChestModelGO;

    public UIChapterMain uiMain;
    public UIJoysticks uiJoystick;

    public Image mapBackGroundImage;
    public Sprite[] mapBackGroundSprites;

    public AudioClip[] chapterNAudios;
    public AudioClip[] chapterEAudios;
    public AudioClip[] chapterBAudios;

    public AudioClip boxOpenAudio;

    private void Awake()
    {
        App.instance.chapterMain = this;

        List<List<GameObject>> Chapter1stageMapPrefabsList = new List<List<GameObject>>
        {
            this.chapter1BattlePrefabs,
            this.chapter1ElitePrefabs,
            this.chapter1ShopPrefabs,
            this.chapter1QuestionPrefabs,
            this.chapter1BossPrefabs
        };
        List<List<GameObject>> Chapter2stageMapPrefabsList = new List<List<GameObject>>
        {
            this.chapter2BattlePrefabs,
            this.chapter2ElitePrefabs,
            this.chapter2ShopPrefabs,
            this.chapter2QuestionPrefabs,
            this.chapter2BossPrefabs
        };
        List<List<GameObject>> Chapter3stageMapPrefabsList = new List<List<GameObject>>
        {
            this.chapter3BattlePrefabs,
            this.chapter3ElitePrefabs,
            this.chapter3ShopPrefabs,
            this.chapter3QuestionPrefabs,
            this.chapter3BossPrefabs
        };

        playerController.onMoveAction += (direction) => { PlayerMove(direction); };

        this.mapManager.OnMapSelectAction = (mapId) =>
        {
            //mapId : 선택한 맵 id
            //battle, elite, shop, question, boss
            this.nowMapId = mapId;
            if (nowChapter == 0)
            {
                this.nowStageField = Instantiate(Chapter1stageMapPrefabsList
                           [(int)this.mapManager.dicMap[mapId].mapKind] //선택한 맵 id의 종류
                           [RewardRandom(0, Chapter1stageMapPrefabsList[(int)this.mapManager.dicMap[mapId].mapKind].Count)]); //선택한 맵 id의 종류가 담긴 리스트에서 랜덤으로
            }
            else if(nowChapter == 1)
            {
                this.nowStageField = Instantiate(Chapter2stageMapPrefabsList
                           [(int)this.mapManager.dicMap[mapId].mapKind] //선택한 맵 id의 종류
                           [RewardRandom(0, Chapter2stageMapPrefabsList[(int)this.mapManager.dicMap[mapId].mapKind].Count)]);
            }
            else if(nowChapter == 2)
            {
                this.nowStageField = Instantiate(Chapter3stageMapPrefabsList
                           [(int)this.mapManager.dicMap[mapId].mapKind] //선택한 맵 id의 종류
                           [RewardRandom(0, Chapter3stageMapPrefabsList[(int)this.mapManager.dicMap[mapId].mapKind].Count)]);
            }

            if (this.mapManager.dicMap[mapId].mapKind == eMapKind.elite)
            {
                App.instance.BGAudioPlay(this.chapterEAudios[nowChapter]);
            }
            else if (this.mapManager.dicMap[mapId].mapKind == eMapKind.boss)
            {
                App.instance.BGAudioPlay(this.chapterBAudios[nowChapter]);
            }
            else
            {
                App.instance.BGAudioPlay(this.chapterNAudios[nowChapter]);
            }
            this.chapterMap.SetActive(false);

            playerController.SetPlayerMovable();

            if (mapManager.dicMap[mapId].mapKind == eMapKind.shop)
            {
                nowStage = nowStageField.GetComponent<StageShop>();
                ((StageShop)nowStage).onItemPerchaseAction += (itemId, price, callback) =>
                {
                    Debug.Log(itemId);

                    uiMain.uiStatus.SpendGold(price);
                    if (itemId == 0) callback?.Invoke();
                    else if (itemId < 2000)
                    {
                        InfoManager.instance.gameInfo.weapon = itemId;
                        InfoManager.instance.gameInfo.skills[0] = 2000 + (100 * DataManager.instance.dicWeapon[itemId].type);
                        player.ChangeWeaponSprite();
                    }
                    else if (itemId >= 2000)
                    {
                        for (int i = 1; i < 4; ++i)
                        {
                            if (InfoManager.instance.gameInfo.skills[i] == 0)
                            {
                                InfoManager.instance.gameInfo.skills[i] = itemId;
                                break;
                            }
                            else if (i == 3)
                            {
                                callback?.Invoke();
                                uiMain.SkillExchangePopUp(itemId);
                            }
                        }
                        this.uiJoystick.ChangePlayerSkillSprite();
                    }
                    

                };
                ((StageShop)nowStage).onShopCloseAction += () => { playerController.SetPlayerMovable(); };
            }
            else if(mapManager.dicMap[mapId].mapKind == eMapKind.question)
            {
                this.nowStage = this.nowStageField.GetComponent<StageRest>();
            }
            else
            {
                this.nowStage = this.nowStageField.GetComponent<StageMain>();
                ((StageMain)nowStage).onChapter3Phase2Action = () => {
                    Debug.Log("phase 2");
                    uiMain.uiBossHp.SetActive(true);
                    StartCoroutine(uiMain.BossApear(((StageMain)nowStage).monsters[0])); };

                monsterController.Init((StageMain)nowStage);
                monsterController.onPlayerRestrictAction = () => { playerController.SetPlayerImmovable(); };
                monsterController.onPlayerUnrestrictAction = () => { playerController.SetPlayerMovable(); };
                monsterController.onPlayerKnockbackAction = (knockbackDistnace,dir) =>
                {
                    int xdir = 0;
                    if (dir == eDirection.left) xdir = -1;
                    else if (dir == eDirection.right) xdir = 1;

                    TileInfo[] tileInfos = new TileInfo[knockbackDistnace];
                    for(int i = 0; i < tileInfos.Length; i++)
                    {
                        tileInfos[i] = nowStage.tileInfoArray[player.location.x + (i + 1)* xdir, player.location.y];
                    }
                    playerController.SetPlayerImmovable();
                    Vector2Int knockbackLocation;
                    float knockbackTime = 0;
                    nowStage.tileInfoArray[player.location.x, player.location.y].objId = ConstantIDs.BLANK;
                    
                    if (tileInfos.Any(x=>x.objId == -1))
                    {
                        knockbackLocation = tileInfos.First(x => x.objId == -1).location + Vector2Int.right * -xdir;
                        knockbackTime = 0.5f / (6 - Mathf.Abs(knockbackLocation.x - player.location.x));
                    }
                    else
                    {
                        knockbackLocation = player.location + Vector2Int.right * knockbackDistnace * xdir;
                        knockbackTime = 0.5f;
                    }
                    player.location = knockbackLocation;
                    Debug.Log(knockbackLocation);
                    nowStage.tileInfoArray[player.location.x, player.location.y].objId = ConstantIDs.PLAYER;
                    player.transform.DOMoveX(player.location.x + nowStage.bl.x, knockbackTime).onComplete = () => { playerController.SetPlayerMovable(); };
                };

                
                if (mapManager.dicMap[mapId].mapKind == eMapKind.boss || mapManager.dicMap[mapId].mapKind == eMapKind.elite)
                {
                    uiMain.uiBossHp.SetActive(true);
                    if (mapManager.dicMap[mapId].mapKind == eMapKind.boss && nowChapter == 2)
                    {
                        ((StageMain)nowStage).chapter3_phase1 = true;
                        StartCoroutine(uiMain.BossApear(((StageMain)nowStage).monsters[1]));
                    }
                    else
                    {
                        StartCoroutine(uiMain.BossApear(((StageMain)nowStage).monsters[0]));
                    }
                    ((StageMain)nowStage).onMonsterHitAction += (damage, position) =>
                    {
                        if (((StageMain)nowStage).chapter3_phase1)
                        {
                            uiMain.BossHit(((StageMain)nowStage).monsters[1]);
                        }
                        else
                        {
                            uiMain.BossHit(((StageMain)nowStage).monsters[0]);
                        }
                    };
                }
                playerController.Init((StageMain)nowStage);
                ((StageMain)nowStage).onMonsterHitAction += (damage, position) =>
                {
                    uiMain.DamageTextApear(damage, position, Color.white);
                };
                ((StageMain)nowStage).onStageClearAction += (chestpos) => // 마지막 몬스터 처치 시 실행
                {
                    DecideSkillReward(this.mapManager.dicMap[mapId].mapKind);
                    DecideWeaponReward(this.mapManager.dicMap[mapId].mapKind);

                    int chestGrade = (int)this.mapManager.dicMap[mapId].mapKind;
                    if (chestGrade == 4)
                    {
                        chestGrade = 2;
                    }
                    if(chestGrade == 3)
                    {
                        chestGrade = 0;
                    }
                    this.weaponChestModelGO = Instantiate(this.weaponChestPrefabs[chestGrade],this.nowStage.transform);
                    this.skillChestModelGO = Instantiate(this.skillChestPrefabs[chestGrade],this.nowStage.transform);

                    this.weaponChestModelGO.transform.position = (Vector2)chestpos[0];
                    this.skillChestModelGO.transform.position = (Vector2)chestpos[1];

                };
                ((StageMain)nowStage).onMonsterDieAction += (gold, mon) =>
                {
                    Debug.Log("monster die");
                    uiMain.GoldTextApear(gold, mon.location + this.nowStage.bl);

                    if (mon.id < 300) InfoManager.instance.recordInfo.NormalMonsters++;
                    else if(mon.id < 400) InfoManager.instance.recordInfo.EliteMonsters++;
                    else InfoManager.instance.recordInfo.BossMonsters++;

                    foreach(var a in this.monsterController.AlertPool)
                    {
                        if (!a.activeInHierarchy)
                        {
                            continue;
                        }
                        else
                        {
                            if(a.GetComponent<Alert>().mon == mon)
                            {
                                a.SetActive(false);
                            }
                        }
                    }
                };
                ((StageMain)nowStage).onPlayerHitAction += (damage) =>
                {
                    uiMain.DamageTextApear(damage, player.transform.position, Color.red);
                    if (InfoManager.instance.gameInfo.hp > 0)
                    {
                        InfoManager.instance.gameInfo.hp -= damage;
                        Debug.LogFormat("hp : {0}", InfoManager.instance.gameInfo.hp);
                        uiMain.uiStatus.hpBar.fillAmount = (float)InfoManager.instance.gameInfo.hp / (float)InfoManager.instance.gameInfo.maxHp;
                        if (InfoManager.instance.gameInfo.hp <= 0)
                        {
                            Debug.Log("죽음");
                            PlayerDeath();
                        };
                    }
                };
                ((StageMain)nowStage).effectManager = this.effectManager;
            }
            nowStage.Init(this.player);

            this.player.ChangeWeaponSprite();
            this.uiJoystick.ChangePlayerSkillSprite();
            uiJoystick.CoolTimeReset();
            uiJoystick.Refresh();
        };
    }

    private void Start()
    {
        uiMain.uiStatus.hpBar.fillAmount = (float)InfoManager.instance.gameInfo.hp / (float)InfoManager.instance.gameInfo.maxHp;
        uiMain.uiStatus.gold.text = InfoManager.instance.gameInfo.gold.ToString();
        this.mapBackGroundImage.sprite = this.mapBackGroundSprites[this.nowChapter];
        this.uiMain.onRewardCompleteAction += (id) => 
        {
            if (id == -1) //무기보상 건너뛰기
            {
                this.nowStage.tileInfoArray[Mathf.RoundToInt(this.weaponChestModelGO.transform.position.x - this.nowStage.bl.x), Mathf.RoundToInt(this.weaponChestModelGO.transform.position.y - this.nowStage.bl.y)].objId = ConstantIDs.BLANK;
                Destroy(this.weaponChestModelGO);
            }
            else if (id == -2) //스킬보상 건너뛰기
            {
                this.nowStage.tileInfoArray[Mathf.RoundToInt(this.skillChestModelGO.transform.position.x - this.nowStage.bl.x), Mathf.RoundToInt(this.skillChestModelGO.transform.position.y - this.nowStage.bl.y)].objId = ConstantIDs.BLANK;
                Destroy(this.skillChestModelGO);
            }
            else if (id < 2000) //weapon
            {
                InfoManager.instance.gameInfo.weapon = id;
                InfoManager.instance.gameInfo.skills[0] = 2000 + (100 * DataManager.instance.dicWeapon[id].type);
                player.ChangeWeaponSprite();
                Destroy(this.weaponChestModelGO);
            }
            else //skill
            {
                for (int i = 1; i < 4; ++i)
                {
                    if (InfoManager.instance.gameInfo.skills[i] == 0)
                    {
                        InfoManager.instance.gameInfo.skills[i] = id;
                        break;
                    }
                    else if (i == 3)
                    {
                        uiMain.SkillExchangePopUp(id);
                    }
                }
                this.uiJoystick.ChangePlayerSkillSprite();
                Destroy(this.skillChestModelGO);
            }
            playerController.SetPlayerMovable();
            uiJoystick.Refresh();
        };

        uiMain.onExchangeCompleteAction += (idx, id) => 
        {
            if (idx != -1)
            {
                InfoManager.instance.gameInfo.skills[idx] = id;
            }
            this.uiJoystick.ChangePlayerSkillSprite();
            uiJoystick.Refresh();
        };
    }

    public int RewardRandom(int minValue, int maxValue)
    {
        System.Random random = new System.Random(InfoManager.instance.gameInfo.rewardSeed);
        for (int i = 0; i < InfoManager.instance.gameInfo.seedNextCnt; ++i)
        {
            random.Next();
        }

        var result = random.Next(minValue, maxValue);
        InfoManager.instance.gameInfo.seedNextCnt++;
        return result;
    }

    private int[] DecideRewardGrade(eMapKind mapKind)
    {
        int chestGrade = 0;

        switch (mapKind)
        {
            case eMapKind.boss: chestGrade = 2; break;
            case eMapKind.elite: chestGrade = 1; break;
            case eMapKind.battle: chestGrade = 0; break;
        }

        int[] rewardGrades = new int[3];
        int fixRewardGrade = chestGrade;
        bool fixReward = false;

        for (int i = 0; i < 3; ++i)
        {
            var rand = RewardRandom(0, 100);
            switch (chestGrade)
            {
                case 0:
                    if (rand >= 90) rewardGrades[i] = 1;
                    else rewardGrades[i] = 0;
                    break;
                case 1:
                    if (rand >= 90) rewardGrades[i] = 2;
                    else if (rand >= 60) rewardGrades[i] = 1;
                    else rewardGrades[i] = 0;
                    break;
                case 2:
                    if (rand >= 80) rewardGrades[i] = 3;
                    else if (rand >= 50) rewardGrades[i] = 2;
                    else rewardGrades[i] = 1;
                    break;
            }
        }

        for (int i = 0; i < 3; ++i)
        {
            if (rewardGrades[i] == fixRewardGrade)
            {
                fixReward = true;
                break;
            }
        }

        if (!fixReward)
        {
            var rand = RewardRandom(0, 3);

            rewardGrades[rand] = fixRewardGrade;
        }
        return rewardGrades;
    }

    private void DecideWeaponReward(eMapKind mapKind)
    {
        int[] rewardWeaponGrades = DecideRewardGrade(mapKind);

        List<int> normalWeaponIds = (from pair in DataManager.instance.dicWeapon where pair.Value.grade == 0 select pair.Value.id).ToList();
        List<int> rareWeaponIds = (from pair in DataManager.instance.dicWeapon where pair.Value.grade == 1 select pair.Value.id).ToList();
        List<int> epicWeaponIds = (from pair in DataManager.instance.dicWeapon where InfoManager.instance.playerInfo.unlockWaeponIds.Exists((x) => x == pair.Key) && pair.Value.grade == 2 select pair.Key).ToList();
        List<int> legendryWeaponIds = (from pair in DataManager.instance.dicWeapon where InfoManager.instance.playerInfo.unlockWaeponIds.Exists((x) => x == pair.Key) && pair.Value.grade == 3 select pair.Key).ToList();

        for (int i = 0; i < 3; ++i)
        {
            int range = 0;
            List<int> weaponIds = null;

            switch (rewardWeaponGrades[i])
            {
                case 0:
                    range = normalWeaponIds.Count;
                    weaponIds = normalWeaponIds;
                    break;
                case 1:
                    range = rareWeaponIds.Count;
                    weaponIds = rareWeaponIds;
                    break;
                case 2:
                    range = epicWeaponIds.Count;
                    weaponIds = epicWeaponIds;
                    break;
                case 3:
                    if (legendryWeaponIds.Count > 0)
                    {
                        range = legendryWeaponIds.Count;
                        weaponIds = legendryWeaponIds;
                    }
                    else
                    {
                        range = epicWeaponIds.Count;
                        weaponIds = epicWeaponIds;
                    }
                    break;
            }
            var rand = RewardRandom(0, range);
            resultWeaponId[i] = weaponIds[rand];
            weaponIds.RemoveAt(rand);
        }
    }

    private void DecideSkillReward(eMapKind mapKind)
    {
        int[] rewardSkillGrades = DecideRewardGrade(mapKind);

        List<int> normalSkillIds = (from pair in DataManager.instance.dicActiveSkill where pair.Value.grade == 0 && pair.Key % 10 != 0 select pair.Value.id).ToList(); //액티브스킬 0단계인데 나머지연산자 10으로 나누어떨어지면 평타임

        List<int> rareSkillIds = (from pair in DataManager.instance.dicActiveSkill where pair.Value.grade == 1 select pair.Value.id).ToList();

        //List<int> epicSkillIds = (from pair in DataManager.instance.dicActiveSkill where pair.Value.grade == 2 select pair.Key).ToList();
        List<int> epicSkillIds = (from pair in DataManager.instance.dicActiveSkill where InfoManager.instance.playerInfo.unlockSkillIds.Exists((x) => x == pair.Key) && pair.Value.grade == 2 select pair.Key).ToList();

        //List<int> legendrySkillIds = (from pair in DataManager.instance.dicActiveSkill where pair.Value.grade == 3 select pair.Key).ToList();
        List<int> legendrySkillIds = (from pair in DataManager.instance.dicActiveSkill where InfoManager.instance.playerInfo.unlockSkillIds.Exists((x) => x == pair.Key) && pair.Value.grade == 3 select pair.Key).ToList();

        for (int i = 0; i < 3; ++i)
        {
            int range = 0;
            List<int> skillIds = null;

            switch (rewardSkillGrades[i])
            {
                case 0:
                    range = normalSkillIds.Count;
                    skillIds = normalSkillIds;
                    break;
                case 1:
                    range = rareSkillIds.Count;
                    skillIds = rareSkillIds;
                    break;
                case 2:
                    range = epicSkillIds.Count;
                    skillIds = epicSkillIds;
                    break;
                case 3:
                    if (legendrySkillIds.Count > 0)
                    {
                        range = legendrySkillIds.Count;
                        skillIds = legendrySkillIds;
                    }
                    else
                    {
                        range = epicSkillIds.Count;
                        skillIds = epicSkillIds;//epicSkillIds;
                    }
                    break;
            }
            var rand = RewardRandom(0, range);
            resultSkillId[i] = skillIds[rand];
            skillIds.RemoveAt(rand);
        }
    }


    //grade는 0이 노말상자(노말스테이지 깻을때 나오는 노말,레어뜨는 상자), 1이 레어확정(엘리트상자), 2가 에픽확정(보스)
    private void ChestOpen(int chestKind, eMapKind mapKind)
    {
        SoundManager.PlaySFX(this.boxOpenAudio);
        if (chestKind == ConstantIDs.WEAPON_CHEST) //무기상자라면
        {
            Animator chestAnim = null;
            chestAnim = this.weaponChestModelGO.GetComponent<Animator>();
            chestAnim.enabled = true;
            this.uiMain.ChestRewardPopUp(chestKind, this.resultWeaponId);
        }
        else if (chestKind == ConstantIDs.SKILL_CHEST) //스킬상자라면
        {
            Animator chestAnim = null;
            chestAnim = this.skillChestModelGO.GetComponent<Animator>();
            chestAnim.enabled = true;
            this.uiMain.ChestRewardPopUp(chestKind, this.resultSkillId);
        }
    }

    void PlayerMove(eDirection direction)
    {
        TileInfo info = TileInfoCheck(direction);
        if (info.objId == 0) // 비어있을 때
        {
            nowStage.tileInfoArray[player.location.x, player.location.y].objId = 0;
            switch (direction)
            {
                case eDirection.up: player.location.y += 1; break; // 상
                case eDirection.down: player.location.y -= 1; break; // 하
                case eDirection.left: player.location.x -= 1; break; // 좌
                case eDirection.right: player.location.x += 1; break; // 우
            }
            nowStage.tileInfoArray[player.location.x, player.location.y].objId = ConstantIDs.PLAYER;
            player.Move(this.nowStage.bl);
        }
        else if (info.objId == ConstantIDs.OPENED_PORTAL) // 포탈일 때
        {
            this.mapId = this.nowMapId;
            InfoManager.instance.gameInfo.stageId = this.mapId;
            InfoManager.instance.recordInfo.winCombat++;

            InfoManager.instance.SaveInfos();
            if (this.mapManager.dicMap[mapId].mapKind == eMapKind.boss)
            {
                onChapterSceneEndAction(this.nowChapter);
            }

            this.chapterMap.SetActive(true);
            if(this.nowChapter == 2 && this.mapManager.dicMap[mapId].mapKind == eMapKind.boss)
            {
                this.chapterMap.SetActive(false);
            }
            Destroy(this.nowStageField);
        }
        else if (info.objId == ConstantIDs.WEAPON_CHEST) // <- 여기에 상자 id 들어갈 예정
        {
            info.objId = 0;
            this.ChestOpen(ConstantIDs.WEAPON_CHEST, this.mapManager.dicMap[nowMapId].mapKind);
        }
        else if (info.objId == ConstantIDs.SKILL_CHEST) // <- 여기에 상자 id 들어갈 예정
        {
            info.objId = 0;
            this.ChestOpen(ConstantIDs.SKILL_CHEST, this.mapManager.dicMap[nowMapId].mapKind);
        }
        else if (info.objId == 14)
        {
            ((StageShop)nowStage).OpenShopPopUp();
        }
        else if (info.objId == 15)
        {
            ((StageRest)nowStage).PlayerHeal();
            uiMain.uiStatus.hpBar.fillAmount = (float)InfoManager.instance.gameInfo.hp / (float)InfoManager.instance.gameInfo.maxHp;
            playerController.SetPlayerMovable();
        }
        else // 다른 무언가가 있을 때
        {
            playerController.SetPlayerMovable();
        }
    }
    void PlayerDeath()
    {
        StartCoroutine(PlayerDeathCoroutine());
    }
    IEnumerator PlayerDeathCoroutine()
    {
        InfoManager.instance.recordInfo.Death++;
        InfoManager.instance.gameInfo.isPlayingSaved = false;
        InfoManager.instance.SaveInfos();

        playerController.enabled = false;
        player.model.GetComponent<Animator>().SetTrigger("Death");
        player.weapon.GetComponent<Animator>().SetTrigger("Death");
        player.GetComponent<SortingGroup>().sortingOrder = 20;
        Time.timeScale = 0.25f;
        nowStage.tileInfoArray[player.location.x, player.location.y].objId = 0;
        yield return new WaitForSeconds(1f);
        Time.timeScale = 1f;

        uiJoystick.gameObject.SetActive(false);
        uiMain.uiStatus.gameObject.SetActive(false);
        uiMain.btnPause.gameObject.SetActive(false);
        uiMain.uiBossHp.gameObject.SetActive(false);

        player.Death();
        yield return new WaitForSeconds(3f);

        uiMain.ResultPopUpApear();
    }
    public void Victory()
    {
        StartCoroutine(VictoryCoroutine());
    }
    IEnumerator VictoryCoroutine()
    {
        InfoManager.instance.gameInfo.isPlayingSaved = false;
        InfoManager.instance.SaveInfos();
        playerController.enabled = false;
        player.GetComponent<SortingGroup>().sortingOrder = 20;
        uiJoystick.gameObject.SetActive(false);
        uiMain.uiStatus.gameObject.SetActive(false);
        uiMain.btnPause.gameObject.SetActive(false);
        player.Death();
        yield return new WaitForSeconds(3f);
        uiMain.ResultPopUpApear();
    }
    TileInfo TileInfoCheck(eDirection direction)
    {
        int playerX = player.location.x;
        int playerY = player.location.y;
        TileInfo info = null;
        switch (direction)
        {
            case eDirection.up:
                if (playerY < nowStage.tileInfoArray.GetLength(1)) info = nowStage.tileInfoArray[playerX, playerY + 1];
                break; //상
            case eDirection.down:
                if (playerY > 0) info = nowStage.tileInfoArray[playerX, playerY - 1];
                break; //하
            case eDirection.left:
                if (playerX < nowStage.tileInfoArray.GetLength(0)) info = nowStage.tileInfoArray[playerX - 1, playerY];
                break; //좌
            case eDirection.right:
                if (playerX > 0) info = nowStage.tileInfoArray[playerX + 1, playerY];
                break; //우
        }
        return info;
    }

}