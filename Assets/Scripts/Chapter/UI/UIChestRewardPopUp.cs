using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using DG.Tweening;

public class UIChestRewardPopUp : MonoBehaviour
{
    public Button[] rewardPopupBtns = new Button[3];
    public Image[] rewardPopupBgImages = new Image[3];
    public Image[] rewardPopupIcons = new Image[3];
    public Text[] rewardPopupTexts = new Text[3];
    public Image[] rewardPopupGrades = new Image[3];
    public Button getRewardBtn;
    public Button skipRewardBtn;
    public Material weaponIconMaterial;
    public Material skillIconMaterial;

    public UnityAction<int> onRewardCompleteAction;
    int[] resultIds;
    int selectedId;
    int nowChestKind;

    public void Init(int chestKind, int[] resultIds)
    {
        this.resultIds = resultIds;
        if (chestKind == ConstantIDs.WEAPON_CHEST)
        {
            nowChestKind = -1;
            WeaponData[] weaponDatas = new WeaponData[3];
            for (int i = 0; i < 3; ++i)
            {
                int temp = i;
                string type = "";
                weaponDatas[i] = DataManager.instance.dicWeapon[resultIds[i]];
                switch (weaponDatas[i].grade)
                {
                    case 0: rewardPopupGrades[i].color = new Color32(255, 255, 255, 150); break;
                    case 1: rewardPopupGrades[i].color = new Color32(0, 90, 255, 150); break;
                    case 2: rewardPopupGrades[i].color = new Color32(200, 0, 255, 150); break;
                    case 3: rewardPopupGrades[i].color = new Color32(255, 100, 0, 150); break;
                }
                rewardPopupIcons[i].sprite = DataManager.instance.dicAtlas["Weapon"].GetSprite(weaponDatas[i].atlasName);
                rewardPopupIcons[i].material = this.weaponIconMaterial;

                switch (weaponDatas[i].type)
                {
                    case 1: type = "근접"; break;
                    case 2: type = "원거리"; break;
                    case 3: type = "마법"; break;
                }
                rewardPopupTexts[i].text = string.Format("이름 : {0}\n종류 : {1}\n공격력 : {2}\n공격속도 : {3}\n스킬계수 : {4}", weaponDatas[i].name, type, weaponDatas[i].attack, (1 / weaponDatas[i].delay).ToString("F2"), weaponDatas[i].coefficient);
            }
        }

        else
        {
            nowChestKind = -2;
            for (int i = 0; i < 3; ++i)
            {
                int temp = i;
                ActiveSkillData skillData = DataManager.instance.dicActiveSkill[resultIds[i]];
                switch (skillData.grade)
                {
                    case 0: rewardPopupGrades[i].color = new Color32(255, 255, 255, 150); break;
                    case 1: rewardPopupGrades[i].color = new Color32(0, 90, 255, 150); break;
                    case 2: rewardPopupGrades[i].color = new Color32(200, 0, 255, 150); break;
                    case 3: rewardPopupGrades[i].color = new Color32(255, 100, 0, 150); break;
                }
                rewardPopupIcons[i].sprite = DataManager.instance.dicAtlas["Skill"].GetSprite(skillData.atlasName);
                rewardPopupIcons[i].material = this.skillIconMaterial;
                rewardPopupTexts[i].text = string.Format("이름 : {0}\n설명 : {1}\n데미지 : {2}\n쿨타임 : {3}", skillData.name, skillData.information, skillData.damage, skillData.coolTime);
            }
        }
    }

    private void Start()
    {
        getRewardBtn.onClick.AddListener(() =>
        {
            App.instance.YesAudio();
            getRewardBtn.gameObject.SetActive(false);
            foreach (Button b in rewardPopupBtns)
            {
                b.GetComponent<RectTransform>().rotation = Quaternion.Euler(new Vector3(45, 90, 0));
                b.image.color = new Color32(255, 255, 255, 150);
            }
            onRewardCompleteAction(selectedId);
        });
        skipRewardBtn.onClick.AddListener(() =>
        {
            App.instance.NoAudio();
            getRewardBtn.gameObject.SetActive(false);
            foreach (Button b in rewardPopupBtns)
            {
                b.GetComponent<RectTransform>().rotation = Quaternion.Euler(new Vector3(45, 90, 0));
                b.image.color = new Color32(255, 255, 255, 150);
            }
            onRewardCompleteAction(nowChestKind);
        });
        for (int i = 0; i < 3; ++i)
        {
            int temp = i;
            rewardPopupBtns[temp].onClick.AddListener(() =>
            {
                App.instance.YesAudio();
                foreach (Image img in rewardPopupBgImages)
                {
                    img.color = new Color32(255, 255, 255, 150);
                }
                rewardPopupBgImages[temp].color = new Color32(255, 230, 40, 150);
                selectedId = resultIds[temp];
                this.getRewardBtn.gameObject.SetActive(true);
            });
        }
    }

    private void OnEnable()
    {
        this.StartCoroutine(ShowRewardItems());
    }

    private IEnumerator ShowRewardItems()
    {
        rewardPopupBtns[0].GetComponent<RectTransform>().DORotate(Vector3.zero, 0.75f);
        yield return new WaitForSeconds(0.2f);
        rewardPopupBtns[1].GetComponent<RectTransform>().DORotate(Vector3.zero, 0.75f);
        yield return new WaitForSeconds(0.2f);
        rewardPopupBtns[2].GetComponent<RectTransform>().DORotate(Vector3.zero, 0.75f);
    }
}
