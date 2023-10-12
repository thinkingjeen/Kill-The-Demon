using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UIStatus : MonoBehaviour
{
    public Image hpBar;
    public RectTransform status;
        public Text txtStatus;
        public Button btnStatusHideShow;
            public RectTransform buttonImage;
        bool statusShow = false;
    public Text gold;
    void Start()    
    {
        hpBar.fillAmount = 1;
        txtStatus.text = string.Format("공격력 : {0}\n 공격 속도 : {1}\n기타\n다른\n스탯들", 1, 2);
        Debug.Log("ui status start");
        gold.text = "0";
        btnStatusHideShow.onClick.AddListener(() =>
        {
            Debug.Log("button");
            if (statusShow)
            {
                status.DOMoveX(-status.rect.width, 0.25f);
                buttonImage.localRotation = Quaternion.Euler(0, 0, 0);
            }
            else
            {
                status.DOMoveX(0, 0.25f);
                buttonImage.localRotation = Quaternion.Euler(180, 0, 0);
            }
            statusShow = !statusShow;
        });
    }

    public void EarnGold(int gold)
    {
        int g = Mathf.RoundToInt(gold *(1 + InfoManager.instance.playerInfo.stats[2] * 5 / 100f));
        InfoManager.instance.gameInfo.gold += g ;
        InfoManager.instance.recordInfo.Gold += g;
        this.gold.text = InfoManager.instance.gameInfo.gold.ToString();
    }
    public void SpendGold(int gold)
    {
        InfoManager.instance.gameInfo.gold -= gold;
        this.gold.text = InfoManager.instance.gameInfo.gold.ToString();
    }
    public void SetHpBar()
    {
        //hpBar.fillAmount = (float)InfoManager.instance.gameInfo.hp/Infomanager.instance.gameInfo.maxHp;
    }
}
