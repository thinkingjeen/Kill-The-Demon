using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Events;

public class TitleMain : MonoBehaviour
{
    public GameObject BtnsGO;
        public Button btnContinue;
        public Button btnGameStart;
        public Button btnOption;
        public Button btnAchieve;
        public Button btnGameQuit;

    public GameObject warningPopUp; 
        public Button btnWarningYes;
        public Button btnWarningNo;
    public GameObject optionPopUp;
        public Button btnOptionClose;
        public Button btnSaveCloud;
        public Button btnLoadCloud;
        public GameObject cloudStatusPopUp;
            public Button btnCloudStatusPopUpOk;
            public Text textCloudStatusPopUp;
    public GameObject AchievePopUp;
        public Button btnAchievePopUpClose;
        public Text AchievePopUpText;

    public UnityAction onTitleSceneEndAction;

    public bool dataLoad;

    public AudioClip titleAudio;

    private void Awake()
    {
        App.instance.titleMain = this;
        
    }
    void Start()
    {
        
        App.instance.BGAudioPlay(this.titleAudio);
        
        if (dataLoad)
        {
            DataManager.instance.LoadData();
            DataManager.instance.onLoadCompleteAction += () => {
                InfoManager.instance.LoadInfos();
                this.BtnAppear();
            };
            
        }
        else
        {
            BtnAppear();
        }

        this.btnContinue.onClick.AddListener(() => {
            //빌리지 건너 뛰고 진행상황 로드해서 챕터 씬 시작
            App.instance.YesAudio();
            App.instance.NoAudio();
            App.instance.LoadChapterScene();
        });
        this.btnGameStart.onClick.AddListener(() => {
            App.instance.YesAudio();
            if (InfoManager.instance.gameInfo.isPlayingSaved)
            {
                warningPopUp.SetActive(true);
            }
            else
            {
                InfoManager.instance.gameInfo = new GameInfo();
                InfoManager.instance.SaveInfos();
                onTitleSceneEndAction();
            }
        });
        this.btnOption.onClick.AddListener(() => {
            App.instance.YesAudio();
            this.optionPopUp.SetActive(true); 
        });
        this.btnOptionClose.onClick.AddListener(() => {
            App.instance.NoAudio();
            this.optionPopUp.SetActive(false); 
        });
        this.btnAchieve.onClick.AddListener(() => {
            App.instance.YesAudio();
            this.AchievePopUp.SetActive(true); DenoteRecordInfo(); 
        });
        this.btnGameQuit.onClick.AddListener(() => {
            App.instance.YesAudio();
            Application.Quit();
        });
        this.btnAchievePopUpClose.onClick.AddListener(() => {
            App.instance.NoAudio();
            this.AchievePopUp.SetActive(false); 
        });
        

        this.btnWarningYes.onClick.AddListener(() =>
        {
            App.instance.YesAudio();
            InfoManager.instance.gameInfo = new GameInfo();
            InfoManager.instance.SaveInfos();
            onTitleSceneEndAction();
        });
        this.btnWarningNo.onClick.AddListener(() =>
        {
            App.instance.NoAudio();
            warningPopUp.SetActive(false);
        });

        this.btnSaveCloud.onClick.AddListener(() => 
        {
            App.instance.YesAudio();
            cloudStatusPopUp.SetActive(true);
            btnCloudStatusPopUpOk.gameObject.SetActive(false);
            textCloudStatusPopUp.text = "저장중...";
            GPGSManager.instance.SaveToCloud(MessageCallBack);
        });

        this.btnLoadCloud.onClick.AddListener(() =>
        {
            App.instance.YesAudio();
            cloudStatusPopUp.SetActive(true);
            btnCloudStatusPopUpOk.gameObject.SetActive(false);
            textCloudStatusPopUp.text = "로드중...";
            GPGSManager.instance.LoadFromCloud(MessageCallBack);
        });
        btnCloudStatusPopUpOk.onClick.AddListener(() => { cloudStatusPopUp.SetActive(false); });
    }

    void MessageCallBack(string message) // 클라우드 세이브/로드 시 상태 메시지 콜백 받는 메서드
    {
        textCloudStatusPopUp.text = message;
        btnCloudStatusPopUpOk.gameObject.SetActive(true);
    }

    public void BtnAppear()
    {
        if (InfoManager.instance.gameInfo.isPlayingSaved)
        {
            btnContinue.gameObject.SetActive(true);
        }
        this.BtnsGO.transform.DOLocalMoveY(-100, 1).OnComplete(() => {
            //BtnsGO => 버튼들을 한 게임 오브젝트의 자식으로
            //여기에 타이틀 메인의 BGM 실행 메서드 추가
        });
    }

    private void DenoteRecordInfo()
    {
        string recordInfo = string.Format("죽은 횟수 : {0}\n 움직인 횟수 : {1}\n 벌어들인 총 골드 : {2}\n 처치한 일반 몬스터 : {3}\n 처치한 엘리트 몬스터 : {4}\n 처치한 보스 몬스터 : {5}\n 승리한 스테이지 수 : {6}", InfoManager.instance.recordInfo.Death, InfoManager.instance.recordInfo.Moves, InfoManager.instance.recordInfo.Gold,InfoManager.instance.recordInfo.NormalMonsters,InfoManager.instance.recordInfo.EliteMonsters,InfoManager.instance.recordInfo.BossMonsters,InfoManager.instance.recordInfo.winCombat);
        this.AchievePopUpText.text = recordInfo;
    }
}
