using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class UIStageShopItem : MonoBehaviour
{
    public int itemId;
    public Image icon;
    public Button button;

    public UnityAction<int> onButtonClickAction;
    void Start()
    {
        button.onClick.AddListener(() => {
            App.instance.YesAudio();
            onButtonClickAction(itemId);
        });
    }

    public void Setting(int itemId)
    {

        this.itemId = itemId;
        Debug.Log(itemId);
        if (itemId < 2000)
        {
            icon.sprite = DataManager.instance.dicAtlas["Weapon"].GetSprite(DataManager.instance.dicWeapon[itemId].atlasName);
        }
        else
        {
            icon.sprite = DataManager.instance.dicAtlas["Skill"].GetSprite(DataManager.instance.dicActiveSkill[itemId].atlasName);
        }
    }
}
