using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using DG.Tweening;

public class UIChapterMain : MonoBehaviour
{
    public Button btnPause;
    public Button moveOption0;
    public GameObject option0joystick;
    public Image option0Boundary;
    public Button moveOption1;
    public GameObject option1joystick;
    public Image option1Boundary;

    public GameObject pausePopUp;
    public Button btnClosePause;
    public Button btnBackToTitle;
    public GameObject warningPopUp;
    public Button btnWarningYes;
    public Button btnWarningNo;
    
    public UIStatus uiStatus;
    public UIResultPopUp uiResult;
    public GameObject uiBossHp;
    public Image bossHp;

    public GameObject texts;
    public GameObject damageTextPrefab;
    public GameObject goldTextPrefab;
    List<Text> damageTextList;
    List<Text> goldTextList;
    int textIndex = 0;


    public UIChestRewardPopUp chestRewardPopUp;
    public UISkillExchangePopUp skillExchangePopUp;
    public UnityAction<int> onRewardCompleteAction;
    public UnityAction<int,int> onExchangeCompleteAction;


    void Start()
    {
        if (InfoManager.instance.optionInfo.joystickOption == 0)
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


        btnPause.onClick.AddListener(() => {
            App.instance.YesAudio();
            pausePopUp.SetActive(true);
            Time.timeScale = 0f;
        });

        btnClosePause.onClick.AddListener(() => {
            App.instance.NoAudio();
            pausePopUp.SetActive(false);
            Time.timeScale = 1f;
        });
        btnBackToTitle.onClick.AddListener(() => {
            App.instance.YesAudio();
            warningPopUp.SetActive(true);
        });
        btnWarningYes.onClick.AddListener(() => {
            App.instance.YesAudio(); 
            BackToTitle(); 
        });
        btnWarningNo.onClick.AddListener(() => {
            App.instance.NoAudio();
            warningPopUp.SetActive(false); 
        });
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



        this.chestRewardPopUp.onRewardCompleteAction += (id) => 
        {
            this.chestRewardPopUp.gameObject.SetActive(false);
            this.onRewardCompleteAction(id);
        };

        this.skillExchangePopUp.onExchangeCompleteAction += (idx,id) =>
        {
            this.skillExchangePopUp.gameObject.SetActive(false);
            this.onExchangeCompleteAction(idx,id);
        };

        damageTextList = new();
        goldTextList = new();
        for (int i = 0; i < 30; i++)
        {

            GameObject dgo = Instantiate(damageTextPrefab, texts.transform);
            damageTextList.Add(dgo.GetComponent<Text>());
            if(i< 10)
            {
                GameObject ggo = Instantiate(goldTextPrefab, texts.transform);
                goldTextList.Add(ggo.GetComponent<Text>());
            }
        }
        

    }

    public void DamageTextApear(int damage, Vector2 pos, Color color)
    {   
        Text damageText = damageTextList.FirstOrDefault(x => !x.gameObject.activeInHierarchy);
        if(damageText == null)
        {
            damageText = Instantiate(damageTextPrefab, texts.transform).GetComponent<Text>();
            damageTextList.Add(damageText);
        }
        Vector2 screenpos = new((pos.x - 1), (pos.y+ textIndex * 0.13f));
        textIndex++;
        damageText.rectTransform.position = screenpos;
        damageText.text = damage.ToString();
        damageText.color = color;
        StartCoroutine(textApear(damageText));
    }
    public void GoldTextApear(int gold, Vector2 pos)
    {
        Text goldText = goldTextList.FirstOrDefault(x => !x.gameObject.activeInHierarchy);
        if (goldText == null)
        {
            goldText = Instantiate(damageTextPrefab, texts.transform).GetComponent<Text>();
            goldTextList.Add(goldText);
        }
        
        Vector2 screenpos = new((pos.x - 1) * Screen.width * 0.0005f, (pos.y + textIndex * 0.5f) * Screen.height * 0.001f);
        textIndex++;
        goldText.rectTransform.position = screenpos;
        goldText.text = "+" + gold.ToString();
        StartCoroutine(textApear(goldText));
        uiStatus.EarnGold(gold);
    }

    

    IEnumerator textApear(Text text)
    {
        text.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        text.gameObject.SetActive(false);
        textIndex--;
    }

    public void ResultPopUpApear()
    {
        StartCoroutine(ResultPopUpApearCoroutine());
    }

    IEnumerator ResultPopUpApearCoroutine()
    {
        uiResult.gameObject.SetActive(true);
        uiResult.gameover.gameObject.SetActive(true);
        uiResult.gameover.DOLocalMoveY(400, 1f);

        yield return new WaitForSeconds(2f);
        int[] previous = InfoManager.instance.recordInfo.GetPreviousStatus();
        int[] current = InfoManager.instance.recordInfo.GetCurrentStatus();
        string[] strs = { "잡은 일반 몬스터 수\n", "잡은 엘리트 수\n", "잡은 보스 수\n", "승리한 전투 수\n", "획득 골드\n", "얻은 다이아\n" };
        for (int i = 0; i < previous.Length; i++)
        {
            if (previous[i] == current[i]) continue;
            else
            {
                uiResult.resultText.text += strs[i];
                uiResult.resultText2.text += (current[i] - previous[i]).ToString() + "\n";
            }
            yield return new WaitForSeconds(0.3f);
        }
        int diaCnt = 0;
        diaCnt += Mathf.RoundToInt((current[0] - previous[0]) * 0.2f);
        diaCnt += (current[1] - previous[1]) * 2;
        diaCnt += current[2] - previous[2] == 1 ? 10 : current[2] - previous[2] == 2 ? 30 : current[2] - previous[2] == 3 ? 60 : current[2] - previous[2] == 4 ? 90 : 0;
        diaCnt *= InfoManager.instance.playerInfo.stats[3] + 100;
        diaCnt = Mathf.RoundToInt((float)diaCnt / 100f);
        uiResult.resultText.text += strs[previous.Length];
        uiResult.resultText2.text += diaCnt.ToString() + "\n";

        InfoManager.instance.playerInfo.dia += diaCnt;
        Debug.Log(diaCnt);
        Debug.Log(strs[previous.Length - 1]);
        Debug.Log(uiResult.resultText.text);
        InfoManager.instance.SaveInfos();

        yield return new WaitForSeconds(1f);
        

        uiResult.btnBackToVillage.gameObject.SetActive(true);
    }

    public IEnumerator BossApear(Monster mon)
    {
        float t = 0;
        bossHp.fillAmount = 0;
        Debug.LogFormat("chap1 boss maxHp : {0}", DataManager.instance.dicMonster[mon.id].maxHp);
        yield return null;
        while (true)
        {
            t += Time.deltaTime;
            bossHp.fillAmount = t / 3;
            if (t > 3) break;
            if (bossHp.fillAmount > (float)(mon.hp) / DataManager.instance.dicMonster[mon.id].maxHp) break;
            yield return null;
        }
    }


    public void BossHit(Monster mon)
    {
        bossHp.fillAmount = (float)(mon.hp) / DataManager.instance.dicMonster[mon.id].maxHp;
        if (mon.hp <= 0) uiBossHp.SetActive(false);
    }

    void BackToTitle() { 
        Time.timeScale = 1f;
        InfoManager.instance.LoadInfos();
        App.instance.LoadTitleScene(dataLoad:false);
    }

    public void ChestRewardPopUp(int chestKind, int[] resultIds)
    {
        this.StartCoroutine(ChestRewardPopUpRoutine(chestKind,resultIds));
    }

    private IEnumerator ChestRewardPopUpRoutine(int chestKind, int[] resultIds)
    {
        yield return new WaitForSeconds(0.55f);
        this.chestRewardPopUp.gameObject.SetActive(true);
        chestRewardPopUp.Init(chestKind, resultIds);
    }

    public void SkillExchangePopUp(int id)
    {
        this.skillExchangePopUp.gameObject.SetActive(true);
        skillExchangePopUp.Init(id);
    }


}