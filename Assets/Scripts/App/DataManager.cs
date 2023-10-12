using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.Events;
using Newtonsoft.Json;
using System;



public class DataManager
{
    public static readonly DataManager instance = new DataManager();

    public Dictionary<int, ActiveSkillData> dicActiveSkill;
    public Dictionary<int, NpcData> dicNPC;
    public Dictionary<int, NpcScriptData> dicScript;
    public Dictionary<int, WeaponData> dicWeapon;
    public Dictionary<int, MonsterData> dicMonster;
    public Dictionary<int, MonsterSkillData> dicMonsterSkill;
    public Dictionary<int, AreaSkillData> dicAreaSkill;
    public Dictionary<string, SpriteAtlas> dicAtlas;

    public UnityAction onLoadCompleteAction;
    private int cnt = 0;

    private const int NUM_OF_DICTIONARIES = 7;
    string[] jsons = new string[NUM_OF_DICTIONARIES];

    private DataManager(){}

    public void LoadData()
    {
        string[] dataFileNames = {
            "PlayerSkill/ActiveSkillData" , "Npc/NpcData"
            , "Npc/NpcScriptData", "Weapon/WeaponData"
            , "Monster/MonsterData", "Monster/MonsterSkillData" , "PlayerSkill/AreaSkillData"
        };
        Type[] dataTypes = {
            typeof(ActiveSkillData),typeof(NpcData)
            ,typeof(NpcScriptData),typeof(WeaponData)
            ,typeof(MonsterData), typeof(MonsterSkillData), typeof(AreaSkillData)
        };


        LoadSpriteAtlases(()=> {
            for (int i = 0; i < NUM_OF_DICTIONARIES; i++)
            {
                typeof(DataManager) //해당 타입의
                    .GetMethod(nameof(LoadJson)) // 해당 이름의 메서드를 가져와서
                    .MakeGenericMethod(dataTypes[i]) // 해당 타입의 제네릭 메서드를 만들고
                    .Invoke(instance, new object[] { dataFileNames[i], i }); // 인스턴스화 되어있는 객체에서(this.instance) 매개변수 할당(dataFileNames[i], i) 후 invoke
            }
        });
    }

    public void LoadJson<T>(string filename, int i) where T : RawData
    {
        ResourceRequest req = Resources.LoadAsync<TextAsset>("Data/" + filename);
        req.completed += (oper) =>
        {
            jsons[i] = req.asset.ToString();
            cnt++;
            if (cnt == NUM_OF_DICTIONARIES)
            {
                JsonToDictionary();
                this.onLoadCompleteAction();
            }
        };
    }

    void JsonToDictionary()
    {
        dicActiveSkill = JsonToDictionary<ActiveSkillData>(0);
        dicNPC = JsonToDictionary<NpcData>(1);
        dicScript = JsonToDictionary<NpcScriptData>(2);
        dicWeapon = JsonToDictionary<WeaponData>(3);
        dicMonster = JsonToDictionary<MonsterData>(4);
        dicMonsterSkill = JsonToDictionary<MonsterSkillData>(5);
        dicAreaSkill = JsonToDictionary<AreaSkillData>(6);
    }

    Dictionary<int, T> JsonToDictionary<T>(int i) where T : RawData
    {
        return JsonConvert.DeserializeObject<T[]>(jsons[i]).ToDictionary(x => x.id);
    }

    void LoadSpriteAtlases(UnityAction callback)
    {
        dicAtlas = new();
        ResourceRequest req = Resources.LoadAsync<SpriteAtlas>("SpriteAtlases/WeaponAtlas");
        req.completed += (oper) =>
        {
            dicAtlas.Add("Weapon", (SpriteAtlas)req.asset);
            ResourceRequest skillReq = Resources.LoadAsync<SpriteAtlas>("SpriteAtlases/SkillIconAtlas");
            skillReq.completed += (oper2) => {
                dicAtlas.Add("Skill", (SpriteAtlas)skillReq.asset);
                callback();
            };
        };
    }

}