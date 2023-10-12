using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class VillageUIMain : MonoBehaviour
{
    public NPCManager npcManager;

    public Button btnPause;
    public Button adGetDiaBtn;

    public Text dia;

    public GameObject pausePopUp;
        public Button btnClosePausePopUp;
        public Button btnBackToTitle;
        public Button moveOption0;
        public Image option0Boundary;
        public GameObject option0joystick;
        public Button moveOption1;
        public Image option1Boundary;
        public GameObject option1joystick;

    public WeaponSelectPopUp weaponSelectPopUp;
        public UnityAction onWeaponSelectCancelAction;
        public UnityAction onWeaponSelectCompleteAction;
    public GameObject dialoguePopUp;
        int npcId;
        public Text textDialogue;
        public Button btnDialogueCover;
    public UIUnlockShop unlockShopPopUp;
        public UnityAction onUnlockShopCloseAction;
    public UIStatUpgrade statUpgradePopUp;
    public GameObject adRewardPopUp;
        public GameObject rewardText;
        public GameObject adErrText;
        public Button btnAdRewardOK;
    public System.Action onStatShopCloseAction;


    void Start()
    {
        if(InfoManager.instance.optionInfo.joystickOption == 0)
        {
            option0joystick.gameObject.SetActive(true);
            option0joystick.gameObject.SetActive(true);
            option0Boundary.gameObject.SetActive(true);
            option1joystick.gameObject.SetActive(false);
            option1joystick.gameObject.SetActive(false);
            option1Boundary.gameObject.SetActive(false);
        }
        else
        {
            option0joystick.gameObject.SetActive(false);
            option0joystick.gameObject.SetActive(false);
            option0Boundary.gameObject.SetActive(false);
            option1joystick.gameObject.SetActive(true);
            option1joystick.gameObject.SetActive(true);
            option1Boundary.gameObject.SetActive(true);
        }

        dia.text = InfoManager.instance.playerInfo.dia.ToString();

        adGetDiaBtn.onClick.AddListener(() => {
            App.instance.LoadingCanvasGO.SetActive(true);
            GoogleADMob.instance.Init();
            GoogleADMob.instance.LoadAds();
        });

        GoogleADMob.instance.onHandleRewardedAdClosed = () => {
            App.instance.LoadingCanvasGO.SetActive(false);
        };
        GoogleADMob.instance.onHandleRewardedAdFailedToLoad = (args) => {
            adRewardPopUp.SetActive(true);
            rewardText.SetActive(false);
            adErrText.SetActive(true);
            App.instance.LoadingCanvasGO.SetActive(false);
        };
        GoogleADMob.instance.onHandleRewardedAdFailedToShow = () => {
            adRewardPopUp.SetActive(true);
            rewardText.SetActive(false);
            adErrText.SetActive(true);
            App.instance.LoadingCanvasGO.SetActive(false);
        };
        GoogleADMob.instance.onHandleUserEarnedReward += (reward) => {
            Debug.LogFormat("{0},{1}",reward.Type, reward.Amount);
            InfoManager.instance.playerInfo.dia += 10;
            InfoManager.instance.SaveInfos();
            dia.text = InfoManager.instance.playerInfo.dia.ToString();
            adRewardPopUp.SetActive(true);
            rewardText.SetActive(true);
            adErrText.SetActive(false);
            App.instance.LoadingCanvasGO.SetActive(false);
        };
        btnAdRewardOK.onClick.AddListener(() => { adRewardPopUp.SetActive(false); });

        btnPause.onClick.AddListener(() => {
            App.instance.YesAudio();
            pausePopUp.SetActive(true); 
        });

        btnClosePausePopUp.onClick.AddListener(() => {
            App.instance.NoAudio();
            pausePopUp.SetActive(false); 
        });
        btnBackToTitle.onClick.AddListener(() => {
            App.instance.YesAudio();
            pausePopUp.SetActive(false); 
            App.instance.LoadTitleScene(dataLoad: false); });
        moveOption0.onClick.AddListener(() =>
        {
            InfoManager.instance.optionInfo.joystickOption = 0;
            option0Boundary.gameObject.SetActive(true);
            option0joystick.gameObject.SetActive(true);
            option1Boundary.gameObject.SetActive(false);
            option1joystick.gameObject.SetActive(false);
            InfoManager.instance.SaveInfos();
            App.instance.YesAudio();
        });
        moveOption1.onClick.AddListener(() =>
        {
            InfoManager.instance.optionInfo.joystickOption = 1;
            option0Boundary.gameObject.SetActive(false);
            option0joystick.gameObject.SetActive(false);
            option1Boundary.gameObject.SetActive(true);
            option1joystick.gameObject.SetActive(true);
            InfoManager.instance.SaveInfos();
            App.instance.YesAudio();
        });

        this.weaponSelectPopUp.onWeaponSelectCancelAction += () => { this.onWeaponSelectCancelAction(); };
        this.weaponSelectPopUp.onWeaponSelectCompleteAction += () => { this.onWeaponSelectCompleteAction(); };
        weaponSelectPopUp.onDiaSpendAction += (num) => { DiaAmountChange(-2); };
        btnDialogueCover.onClick.AddListener(() =>
        {
            App.instance.YesAudio();
            NPCAct(npcId);
            dialoguePopUp.SetActive(false);
        });

        unlockShopPopUp.onUnlockShopCloseAction += () => { onUnlockShopCloseAction(); };
        unlockShopPopUp.onItemUnlockAction += (num) => { DiaAmountChange(num); };
        statUpgradePopUp.onStatShopCloseAction += () => { onStatShopCloseAction(); };
        statUpgradePopUp.onStatChangeAction += (num) => { DiaAmountChange(num); };
    }

    void DiaAmountChange(int num)
    {
        InfoManager.instance.playerInfo.dia += num;
        InfoManager.instance.SaveInfos();
        dia.text = InfoManager.instance.playerInfo.dia.ToString();
    }

    public void NPCConversation(int id)
    {
        npcId = id;
        npcManager.npcs.First(x => x.npcData.id == id).PrintDialogueScripts();
        dialoguePopUp.gameObject.SetActive(true);
        List<string> dialogues =
           (from KeyValuePair<int, NpcScriptData> pair in DataManager.instance.dicScript
            where pair.Value.npcId == id && pair.Value.stateNext == 1
            select pair.Value.script).ToList();
        textDialogue.text = dialogues[Random.Range(0, dialogues.Count())];
    }

    void NPCAct(int id)
    {
        if(id == 14)
        {
            unlockShopPopUp.gameObject.SetActive(true);
        }
        else if(id == 13)
        {
            statUpgradePopUp.gameObject.SetActive(true);
        }
        //else if(id == 11, 12, 13 ....) { }
    }
}
