using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIStatUpgrade : MonoBehaviour
{

    public Button btnClose;
    public Button[] btnStatPlus;
    public Button[] btnStatMinus;
    public Text[] textStats;
    public System.Action onStatShopCloseAction;
    public System.Action<int> onStatChangeAction;
    void Start()
    {
        btnClose.onClick.AddListener(() => {
            App.instance.NoAudio();
            this.onStatShopCloseAction();
            gameObject.SetActive(false); 
        });


        for (int i = 0; i < InfoManager.instance.playerInfo.stats.Length; i++)
        {
            int tmp = i;

            textStats[tmp].text = string.Format("{0}", GetStatString(tmp));

            btnStatPlus[tmp].onClick.AddListener(() =>
            {
                App.instance.YesAudio();
                if (InfoManager.instance.playerInfo.stats[tmp] < 10)
                {
                    if (Mathf.Pow(InfoManager.instance.playerInfo.stats[tmp]+1, 2) <= InfoManager.instance.playerInfo.dia)
                    {
                        onStatChangeAction(-(int)Mathf.Pow(InfoManager.instance.playerInfo.stats[tmp] + 1, 2));
                        InfoManager.instance.playerInfo.stats[tmp]++;
                        InfoManager.instance.SaveInfos();
                        textStats[tmp].text = string.Format("+ {0}", GetStatString(tmp));
                    }
                }
            });
            btnStatMinus[tmp].onClick.AddListener(() =>
            {
                App.instance.NoAudio();
                if (InfoManager.instance.playerInfo.stats[tmp] > 0)
                {
                    onStatChangeAction((int)Mathf.Pow(InfoManager.instance.playerInfo.stats[tmp] , 2));
                    InfoManager.instance.playerInfo.stats[tmp]--;
                    InfoManager.instance.SaveInfos();
                    textStats[tmp].text = string.Format("+ {0}", GetStatString(tmp));
                }   
            });
        }
    }

    string GetStatString(int i)
    {
        string t = "";
        int s = InfoManager.instance.playerInfo.stats[i];
        switch (i)
        {
            case 0: t = "ÃÖ´ë Ã¼·Â\n+" +(s * 100); break;
            case 1: t = "°ø°Ý·Â\n+" + (s * 5); break;
            case 2: t = "°ñµå È¹µæ·®\n" + (s * 5) + "%"; break;
            case 3: t = "´ÙÀÌ¾Æ È¹µæ·®\n" + s + "%"; break;
        }

        return t;
    }
}
