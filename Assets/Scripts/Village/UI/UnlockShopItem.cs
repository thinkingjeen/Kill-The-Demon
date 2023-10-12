using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.U2D;
using UnityEngine.UI;

public class UnlockShopItem : MonoBehaviour
{
    public int id { get; private set; }
    public Image icon;
    public Button button;
    public GameObject lockSign;

    public UnityAction<int> onButtonClickAction;
    public void Init(int id, SpriteAtlas sprites)
    {
        string name = "";
        this.id = id;
        if (id < 2000)
        {
            name = DataManager.instance.dicWeapon[id].atlasName;
        }
        else if(id >= 2000)
        {
            name = DataManager.instance.dicActiveSkill[id].atlasName;
        }
        icon.sprite = sprites.GetSprite(name);
        button.onClick.AddListener(() => {
            App.instance.YesAudio();
            onButtonClickAction(this.id); 
        });
        if (InfoManager.instance.playerInfo.unlockWaeponIds.Exists(x => x == this.id)
            || InfoManager.instance.playerInfo.unlockSkillIds.Exists(x => x == this.id))
        {
            lockSign.SetActive(false);
        }
    }

    public void UnlockItem()
    {
        if (id < 2000)
        {
            InfoManager.instance.playerInfo.unlockWaeponIds.Add(id);
        }
        else if(id >= 2000)
        {
            InfoManager.instance.playerInfo.unlockSkillIds.Add(id);
        }
        InfoManager.instance.SaveInfos();
        lockSign.SetActive(false);
    }
}
