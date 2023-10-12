using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using GooglePlayGames;
using Firebase.Auth;
using Firebase.Extensions;
using GooglePlayGames.BasicApi;

public enum eDirection
{
    none = 0,
    up = 1,
    down = 2,
    left = 3,
    right = 4
}

public class App : MonoBehaviour
{
    public bool isUnityTest = false;

    private PlayGamesLocalUser localUser;
    
    public static App instance;
    public LogoMain logoMain { private get; set; }
    public TitleMain titleMain { private get; set; }
    public VillageMain villageMain { private get; set; }
    public ChapterMain chapterMain { private get; set; }

    public GameObject LoadingCanvasGO;

    private string nowAudioName;

    public AudioClip SFXYes;
    public AudioClip SFXNo;
    public AudioClip boomAudio;

    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
        Application.targetFrameRate = 60;
        PlayGamesPlatform.Activate();
        if (!isUnityTest)
        {
            PlayGamesPlatform.Instance.Authenticate((status) =>
            {
                if (status == GooglePlayGames.BasicApi.SignInStatus.Success)
                {
                    this.StartCoroutine(this.WaitForAuthenticate(() =>
                    {
                        PlayGamesPlatform.Instance.RequestServerSideAccess(true, (token) =>
                        {
                            Firebase.Auth.FirebaseAuth auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
                            Credential credential = PlayGamesAuthProvider.GetCredential(token);
                            auth.SignInWithCredentialAsync(credential).ContinueWithOnMainThread(task =>
                            {
                                FirebaseUser currentUser = auth.CurrentUser;

                                Debug.LogFormat("[currentUser]DisplayName : {0}, [currentUser]UserId : {1}", currentUser.DisplayName, currentUser.UserId);
                            }
                            );
                        });
                    }));
                }
            });
        }
        DontDestroyOnLoad(this.gameObject);
        DataManager.instance.onLoadCompleteAction += () => { this.LoadingCanvasGO.SetActive(false); };
        LoadLogoScene();
    }


    private void LoadLogoScene()
    {
        SceneManager.LoadSceneAsync("Logo").completed += (async) => 
        {
            this.logoMain.onLogoSceneEndAction += () => 
            { 
                this.LoadingCanvasGO.SetActive(true);
                LoadTitleScene(dataLoad:true); 
            }; 
        };
    }

    public void LoadTitleScene(bool dataLoad)
    {
        SceneManager.LoadSceneAsync("Title").completed += (async) =>
        {
            this.LoadingCanvasGO.SetActive(false);
            titleMain.dataLoad = dataLoad;
            this.titleMain.onTitleSceneEndAction += () => 
            {
                LoadVillageScene();
            };
        };
    }

    public void LoadVillageScene()
    {
        this.LoadingCanvasGO.SetActive(true);
        SceneManager.LoadSceneAsync("Village").completed += (async) => 
        {
            this.LoadingCanvasGO.SetActive(false);
        };
    }

    public void LoadChapterScene()
    {
        if (!InfoManager.instance.gameInfo.isPlayingSaved)
        {
            var rand = Random.Range(0, 2100000000);
            InfoManager.instance.gameInfo.gold = 0;
            for (int i =2; i < 4; ++i)
            {
                InfoManager.instance.gameInfo.skills[i] = 0;
            }
            InfoManager.instance.gameInfo.chapter = 0;
            InfoManager.instance.gameInfo.rewardSeed = rand;
            InfoManager.instance.gameInfo.seedNextCnt = 0;
            InfoManager.instance.SaveInfos();
        }
        SceneManager.LoadScene("Title");
        this.LoadingCanvasGO.SetActive(true);
        SceneManager.LoadSceneAsync("Chapter").completed += (async) =>
        {
            this.chapterMain.nowChapter = InfoManager.instance.gameInfo.chapter;
            this.chapterMain.onChapterSceneEndAction += (chapterCnt) =>
             {
                 if (chapterCnt == 2)
                 {
                     chapterMain.Victory();
                 }
                 else
                 {
                     InfoManager.instance.gameInfo.chapter = chapterCnt + 1;
                     InfoManager.instance.SaveInfos();
                     LoadChapterScene();
                 }
             };
            this.LoadingCanvasGO.SetActive(false);
        };

    }

    public void AppReturn(UnityAction callback)
    {
        callback();
    }
    private IEnumerator WaitForAuthenticate(System.Action callback)
    {
        while (true)
        {
            if (this.localUser == null)
            {
                this.localUser = PlayGamesPlatform.Instance.localUser as PlayGamesLocalUser;
                break;
            }
            yield return null;
        }
        callback();
    }
    public void BGAudioPlay(AudioClip audio)
    {
        if (audio.name != this.nowAudioName)
        {
            /*SoundManager.StopMusicImmediately();*/
            SoundManager.PlayImmediately(audio, true);
            this.nowAudioName = audio.name;
        }
    }
    public void YesAudio()
    {
        SoundManager.PlaySFX(this.SFXYes);
    }
    public void NoAudio()
    {
        SoundManager.PlaySFX(this.SFXNo);
    }
}