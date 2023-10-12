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
            //������ �ǳ� �ٰ� �����Ȳ �ε��ؼ� é�� �� ����
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
            textCloudStatusPopUp.text = "������...";
            GPGSManager.instance.SaveToCloud(MessageCallBack);
        });

        this.btnLoadCloud.onClick.AddListener(() =>
        {
            App.instance.YesAudio();
            cloudStatusPopUp.SetActive(true);
            btnCloudStatusPopUpOk.gameObject.SetActive(false);
            textCloudStatusPopUp.text = "�ε���...";
            GPGSManager.instance.LoadFromCloud(MessageCallBack);
        });
        btnCloudStatusPopUpOk.onClick.AddListener(() => { cloudStatusPopUp.SetActive(false); });
    }

    void MessageCallBack(string message) // Ŭ���� ���̺�/�ε� �� ���� �޽��� �ݹ� �޴� �޼���
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
            //BtnsGO => ��ư���� �� ���� ������Ʈ�� �ڽ�����
            //���⿡ Ÿ��Ʋ ������ BGM ���� �޼��� �߰�
        });
    }

    private void DenoteRecordInfo()
    {
        string recordInfo = string.Format("���� Ƚ�� : {0}\n ������ Ƚ�� : {1}\n ������� �� ��� : {2}\n óġ�� �Ϲ� ���� : {3}\n óġ�� ����Ʈ ���� : {4}\n óġ�� ���� ���� : {5}\n �¸��� �������� �� : {6}", InfoManager.instance.recordInfo.Death, InfoManager.instance.recordInfo.Moves, InfoManager.instance.recordInfo.Gold,InfoManager.instance.recordInfo.NormalMonsters,InfoManager.instance.recordInfo.EliteMonsters,InfoManager.instance.recordInfo.BossMonsters,InfoManager.instance.recordInfo.winCombat);
        this.AchievePopUpText.text = recordInfo;
    }
}
