using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

public class InfoManager
{
    public static InfoManager instance = new();

    public RecordInfo recordInfo;
    public GameInfo gameInfo;
    public PlayerInfo playerInfo;
    public OptionInfo optionInfo;

    private InfoManager() { }

    public void LoadInfos()
    {
        recordInfo = LoadInfo<RecordInfo>("/recordinfo.json");
        gameInfo = LoadInfo<GameInfo>("/gameinfo.json");
        playerInfo = LoadInfo<PlayerInfo>("/playerinfo.json");
        optionInfo = LoadInfo<OptionInfo>("/optioninfo.json");

        SoundManager.SetVolumeMusic(optionInfo.musicVolume);
        SoundManager.SetVolumeSFX(optionInfo.SFXVolume);
    }

    T LoadInfo<T>(string filename) where T : RawInfo, new()
    {
        T info;
        Debug.Log(typeof(T));
        if (File.Exists(Application.persistentDataPath + filename))
        {
            string json = File.ReadAllText(Application.persistentDataPath + filename);
            info = JsonConvert.DeserializeObject<T>(json);
        }
        else
        {
            info = new();
            
            if(typeof(T) == typeof(PlayerInfo))
            {
                (info as PlayerInfo).dia = 0;
                (info as PlayerInfo).unlockWaeponIds.AddRange(
                    DataManager.instance.dicWeapon
                    .Where(pair => pair.Value.grade == 0 || pair.Value.grade == 1)
                    .Select(pair => pair.Key)
                    );
                (info as PlayerInfo).unlockWaeponIds.AddRange(new int[] { 1133, 1143, 1223, 1323, 1333 }); //에픽

                (info as PlayerInfo).unlockSkillIds.AddRange(
                    DataManager.instance.dicActiveSkill
                    .Where(pair => pair.Value.grade == 0 || pair.Value.grade == 1)
                    .Select(pair => pair.Key));
                (info as PlayerInfo).unlockSkillIds.AddRange(new int[] { 2103, 2106, 2203, 2206, 2303 }); //에픽
            }

            if(typeof(T) == typeof(OptionInfo))
            {
                (info as OptionInfo).musicVolume = 1f;
                (info as OptionInfo).SFXVolume = 1f;
                (info as OptionInfo).joystickOption = 0;
            }

            string json = JsonConvert.SerializeObject(info);
            File.WriteAllText(Application.persistentDataPath + filename, json);
        }
        return info;
    }

    public void SaveInfos()
    {
        string recordJson = JsonConvert.SerializeObject(recordInfo);
        string gameJson = JsonConvert.SerializeObject(gameInfo);
        string playerJson = JsonConvert.SerializeObject(playerInfo);
        string optionJson = JsonConvert.SerializeObject(optionInfo);

        File.WriteAllText(Application.persistentDataPath + "/recordinfo.json", recordJson);
        File.WriteAllText(Application.persistentDataPath + "/gameinfo.json", gameJson);
        File.WriteAllText(Application.persistentDataPath + "/playerinfo.json", playerJson);
        File.WriteAllText(Application.persistentDataPath + "/optioninfo.json", optionJson);
    }
}
