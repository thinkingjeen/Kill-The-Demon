using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using DG.Tweening;

public class UISkillExchangePopUp : MonoBehaviour
{
    public Button[] exchangePopupBtns = new Button[3];
    public Image[] exchangePopupBgImages = new Image[3];
    public Image[] exchangePopupIcons = new Image[3];
    public Text[] exchangePopupTexts = new Text[3];
    public Image[] exchangePopupGrades = new Image[3];

    public int selectedId;
    public Button exchangeBtn;
    public Button cancelBtn;

    private int selectedIdx;

    public UnityAction<int,int> onExchangeCompleteAction;
    public void Init(int id)
    {
        this.selectedId = id;
        for (int i = 0; i < 3; ++i)
        {
            int temp = i;
            ActiveSkillData skillData = DataManager.instance.dicActiveSkill[InfoManager.instance.gameInfo.skills[i+1]];
            switch (skillData.grade)
            {
                case 0: exchangePopupGrades[i].color = new Color32(255, 255, 255, 150); break;
                case 1: exchangePopupGrades[i].color = new Color32(0, 90, 255, 150); break;
                case 2: exchangePopupGrades[i].color = new Color32(200, 0, 255, 150); break;
                case 3: exchangePopupGrades[i].color = new Color32(255, 100, 0, 150); break;
            }
            exchangePopupIcons[i].sprite = DataManager.instance.dicAtlas["Skill"].GetSprite(skillData.atlasName);
            exchangePopupTexts[i].text = string.Format("이름 : {0}\n설명 : {1}\n데미지 : {2}\n쿨타임 : {3}", skillData.name, skillData.information, skillData.damage, skillData.coolTime);
        }
    }
    void Start()
    {
        for (int i = 0; i < 3; ++i)
        {
            int temp = i;
            exchangePopupBtns[temp].onClick.AddListener(() =>
            {
                foreach (Image img in exchangePopupBgImages)
                {
                    img.color = new Color32(255, 255, 255, 150);
                }
                exchangePopupBgImages[temp].color = new Color32(255, 230, 40, 150);
                selectedIdx = temp+1;
                this.exchangeBtn.gameObject.SetActive(true);
            });
        }

        exchangeBtn.onClick.AddListener(() =>
        {
            exchangeBtn.gameObject.SetActive(false);
            foreach (Button b in exchangePopupBtns)
            {
                b.GetComponent<RectTransform>().rotation = Quaternion.Euler(new Vector3(45, 90, 0));
                b.image.color = new Color32(255, 255, 255, 150);
            }
            onExchangeCompleteAction(selectedIdx,selectedId);
        });
        cancelBtn.onClick.AddListener(() =>
        {
            exchangeBtn.gameObject.SetActive(false);
            foreach (Button b in exchangePopupBtns)
            {
                b.GetComponent<RectTransform>().rotation = Quaternion.Euler(new Vector3(45, 90, 0));
                b.image.color = new Color32(255, 255, 255, 150);
            }
            onExchangeCompleteAction(-1, 0);
        });
    }
    private void OnEnable()
    {
        this.StartCoroutine(ShowRewardItems());
    }

    private IEnumerator ShowRewardItems()
    {
        exchangePopupBtns[0].GetComponent<RectTransform>().DORotate(Vector3.zero, 0.75f);
        yield return new WaitForSeconds(0.2f);
        exchangePopupBtns[1].GetComponent<RectTransform>().DORotate(Vector3.zero, 0.75f);
        yield return new WaitForSeconds(0.2f);
        exchangePopupBtns[2].GetComponent<RectTransform>().DORotate(Vector3.zero, 0.75f);
    }

}
