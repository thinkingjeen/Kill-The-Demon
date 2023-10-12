using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GooglePlayGames;
using Newtonsoft.Json;
using GooglePlayGames.BasicApi.SavedGame;
using System;
using System.Linq;

public class GPGSManager : MonoBehaviour
{
    public static GPGSManager instance;

    private bool isAccessed;
    ISavedGameMetadata myMetaData;
    private const string DELIMITER = "$������$";

    private void Awake()
    {
        instance = this;
    }

    private void AccessCloud()
    {
        this.isAccessed = false;
    }

    private void OpenSavedCloud(System.Action<string> callback)
    {
        var client = PlayGamesPlatform.Instance.SavedGame;

        Debug.LogFormat("******************************[openSavedCloud]PlayGamesPlatform.Instance : {0}******************************", PlayGamesPlatform.Instance);
        Debug.LogFormat("******************************[openSavedCloud]client : {0}******************************", client);
        client.OpenWithAutomaticConflictResolution("killthedemon", GooglePlayGames.BasicApi.DataSource.ReadCacheOrNetwork, ConflictResolutionStrategy.UseLongestPlaytime,
            (status, metaData) =>
            {
                if (status == SavedGameRequestStatus.Success)
                {
                    this.myMetaData = metaData;
                    this.isAccessed = true;
                }
                else 
                {
                    this.isAccessed = false;
                    string message = string.Format("Ŭ���� ���ῡ �����߽��ϴ�. ����� ��Ʈ��ũ ���¸� Ȯ�����ּ���.status:{0}", status);
                    Debug.Log("******************************" + message + "******************************");
                    callback(message);
                }
            });
    }

    private IEnumerator SaveGame(System.Action<string> callback)
    {
        while (true)
        {
            if (this.isAccessed) break;
            yield return null;
        }

        var json1 = JsonConvert.SerializeObject(InfoManager.instance.recordInfo);
        var json2 = JsonConvert.SerializeObject(InfoManager.instance.playerInfo);
        string json = json1 + DELIMITER + json2;

        byte[] data = System.Text.Encoding.UTF8.GetBytes(json);

        var client = PlayGamesPlatform.Instance.SavedGame;
        SavedGameMetadataUpdate.Builder builder = new SavedGameMetadataUpdate.Builder();

        builder.WithUpdatedPlayedTime(new System.TimeSpan()).WithUpdatedDescription("saved at " + DateTime.Now);

        SavedGameMetadataUpdate update = builder.Build();

        client.CommitUpdate(this.myMetaData, update, data, (status, metaData) =>
        {
            if (status == SavedGameRequestStatus.Success)
            {
                Debug.Log("**************************** cloud save complete!!****************************");
                callback("���� �Ϸ�!");
            }
            else
            {
                Debug.Log("**************************** cloud save failed!!****************************");
                Debug.LogFormat("****************************status:{0}****************************", status);
                callback(string.Format("���� ����\nStatus : {0}", status));
            }
        });
    }

    public void SaveToCloud(System.Action<string> callback)
    {
        PlayGamesPlatform.Instance.Authenticate((status) =>
        {
            if (status == GooglePlayGames.BasicApi.SignInStatus.Success)
            {
                this.AccessCloud();
                this.OpenSavedCloud(callback);
                this.StartCoroutine(this.SaveGame(callback));
            }
            else
            {
                string message = "GPGS�α��ο� �����߽��ϴ�. ����� ��Ʈ��ũ ���¿� �����÷��� ���̵� Ȯ�����ּ���";
                Debug.Log(message);
                callback(message);
            }
        });
    }

    public void LoadFromCloud(System.Action<string> callback)
    {
        PlayGamesPlatform.Instance.Authenticate((status) =>
        {
            if (status == GooglePlayGames.BasicApi.SignInStatus.Success)
            {
                this.AccessCloud();
                this.OpenSavedCloud(callback);

                this.StartCoroutine(this.LoadGame(callback));
            }
            else
            {
                Debug.LogFormat("****************************[LoadFromCloud]status:{0}****************************", status);
                callback("Google �α��� ����");
            }
        });
    }

    private IEnumerator LoadGame(System.Action<string> callback)
    {
        yield return new WaitUntil(() => this.isAccessed);

        this.LoadGameData(this.myMetaData, callback);
    }

    private void LoadGameData(ISavedGameMetadata metaData, System.Action<string> callback)
    {
        ISavedGameClient client = PlayGamesPlatform.Instance.SavedGame;

        client.ReadBinaryData(metaData, (status, data) =>
         {
             if (status == SavedGameRequestStatus.Success)
             {
                 Debug.Log("****************************load complete!!****************************");
                 var json = System.Text.Encoding.UTF8.GetString(data);
                 string[] infos = json.Split(DELIMITER, StringSplitOptions.None);

                 InfoManager.instance.recordInfo = JsonConvert.DeserializeObject<RecordInfo>(infos[0]);
                 InfoManager.instance.playerInfo = JsonConvert.DeserializeObject<PlayerInfo>(infos[1]);
                 InfoManager.instance.SaveInfos();

                 callback("������ �ε� �Ϸ�");
             }
             else
             {
                 Debug.Log("****************************load failed!!****************************");

                 Debug.LogFormat("****************************status :{0}****************************",status);

                 callback(string.Format("������ �ε� ����\nStatus : {0}", status));
             }
         });
    }
}
