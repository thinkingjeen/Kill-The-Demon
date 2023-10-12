using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using DG.Tweening;

public class StageShop : StageBase
{
    public Transform merchantPosition;

    public GameObject shopPopUp;
        public Button btnCloseShop;
        public Transform weaponContents;
            public UIStageShopItem[] weaponItems;
            int[] weaponPrice = new int[3];
        public Transform skillContents;
            public UIStageShopItem[] skillItems;
            int[] skillPrice = new int[3];
        public GameObject infoBox;
            public Button btnPerchaseItem;
                public Image itemIcon;
                public Text itemDesc;
                public Text itemPrice;
                int currentItemId;
                int currentItemPrice;


        public Button btnRefresh;
        public Text currentGoldText;
    bool refreshing = false;

    public Material pixelateMat;

    public UnityAction<int, int, UnityAction> onItemPerchaseAction;//id, price, callback
    public UnityAction onShopCloseAction;
    public override void Init(Player player)
    {
        base.Init(player);
        Vector2Int merchantLocation = Vector2Int.RoundToInt(merchantPosition.position) - bl;
        tileInfoArray[merchantLocation.x, merchantLocation.y].objId = 14;
        tileInfoArray[Mathf.RoundToInt(portal.position.x) - bl.x, Mathf.RoundToInt(portal.position.y) - bl.y].objId = ConstantIDs.OPENED_PORTAL;

        btnCloseShop.onClick.AddListener(() => {
            App.instance.NoAudio();
            onShopCloseAction();
            CloseShopPopUp();
        });

        RefreshShopItems();
        currentGoldText.text = InfoManager.instance.gameInfo.gold.ToString();

        foreach (UIStageShopItem item in weaponItems)
        { }
        for(int i = 0; i < 3; i++) {
            int tmp = i;
            weaponItems[tmp].onButtonClickAction += (itemId) => {
                WeaponData d = DataManager.instance.dicWeapon[itemId];
                infoBox.SetActive(true);
                itemIcon.sprite = DataManager.instance.dicAtlas["Weapon"].GetSprite(DataManager.instance.dicWeapon[itemId].atlasName);
/*                itemDesc.text = string.Format("id: {0}\nname: {1}\ngrade: {2}\ntype: {3}\n��Ÿ ��� ���� ǥ��",
                                                d.id, d.name, d.grade, d.type);*/
                string type = null;
                switch (d.type)
                {
                    case 1: type = "����"; break;
                    case 2: type = "���Ÿ�"; break;
                    case 3: type = "����"; break;
                }
                string grade = null;
                switch (d.grade)
                {
                    case 0: grade = "�븻"; break;
                    case 1: grade = "����"; break;
                    case 2: grade = "����"; break;
                    case 3: grade = "��������"; break;

                }
                itemDesc.text = string.Format("�̸� : {0}\n���� : {1}\n��� : {2}\n���ݷ� : {3}\n���ݼӵ� : {4}\n��ų��� : {5}", d.name, type, grade, d.attack, (1 / d.delay).ToString("F2"), d.coefficient);

                currentItemId = itemId;
                currentItemPrice = weaponPrice[tmp];
                itemPrice.text = currentItemPrice.ToString();
            };
            skillItems[tmp].onButtonClickAction += (itemId) => {
                ActiveSkillData d = DataManager.instance.dicActiveSkill[itemId];
                infoBox.SetActive(true);
                itemIcon.sprite = DataManager.instance.dicAtlas["Skill"].GetSprite(DataManager.instance.dicActiveSkill[itemId].atlasName);
                itemIcon.material = pixelateMat;
                /*itemDesc.text = string.Format("id: {0}\nname: {1}\ngrade: {2}\ntype: {3}\n��Ÿ ��� ���� ǥ��",
                                                d.id, d.name, d.grade, d.type);*/

                string grade = null;
                switch (d.grade)
                {
                    case 0: grade = "�븻"; break;
                    case 1: grade = "����"; break;
                    case 2: grade = "����"; break;
                    case 3: grade = "��������"; break;

                }
                itemDesc.text = string.Format("�̸� : {0}\n��� : {1}\n���� : {2}\n������ : {3}\n��Ÿ�� : {4}", d.name, grade, d.information, d.damage, d.coolTime);


                currentItemId = itemId;
                currentItemPrice = skillPrice[tmp];
                itemPrice.text = currentItemPrice.ToString();
            };
        }
        foreach(UIStageShopItem item in skillItems)
        {
            
        }

        btnPerchaseItem.onClick.AddListener(() =>
        {
            if (InfoManager.instance.gameInfo.gold >= currentItemPrice)
            {
                for(int i = 0; i < 3; i++)
                {
                    if (weaponItems[i].itemId == currentItemId) weaponItems[i].gameObject.SetActive(false);
                    if (skillItems[i].itemId == currentItemId) skillItems[i].gameObject.SetActive(false);
                }
                onItemPerchaseAction(currentItemId, currentItemPrice, () => { btnCloseShop.onClick.Invoke(); });
                infoBox.SetActive(false);
                currentGoldText.text = InfoManager.instance.gameInfo.gold.ToString();
            }
        });

        btnRefresh.onClick.AddListener(() =>
        {
            App.instance.YesAudio();

            if (InfoManager.instance.gameInfo.gold >= 25)
            {
                if (!refreshing)
                {
                    StartCoroutine(RefreshShopItemsRoutine());
                    onItemPerchaseAction(0, 25, () => { });
                    currentGoldText.text = InfoManager.instance.gameInfo.gold.ToString();
                }
            }
        });
    }

    public void OpenShopPopUp()
    {
        shopPopUp.SetActive(true);
        shopPopUp.GetComponent<RectTransform>().DOMoveY(Screen.height/2f, 0.25f);
        weaponContents.DOMoveY(Screen.height/2f, 0.25f);
    }
    public void CloseShopPopUp()
    {

        if (!refreshing) shopPopUp.GetComponent<RectTransform>().DOMoveY(Screen.height*1.5f, 0.25f).onComplete = () => { shopPopUp.SetActive(false); };
    }
    void RefreshShopItems()
    {   
        Debug.Log("shop item coroutine");


        //���� 10% ���� 40% ������ ����
        List<int> weaponIdList = (from WeaponData data in DataManager.instance.dicWeapon.Values
                            where InfoManager.instance.playerInfo.unlockWaeponIds.Exists(
                                  id => id == data.id)
                            select data.id).ToList();
        List<int> skillIdList = (from ActiveSkillData data in DataManager.instance.dicActiveSkill.Values
                                 where InfoManager.instance.playerInfo.unlockSkillIds.Exists(
                                       id => id == data.id)
                                 select data.id).ToList();


        for (int i = 0; i < 3; i++)
        {
            int p1 = Random.Range(0, 100);
            int p2 = Random.Range(0, 100);

            List<int> wIdList = new();
            if (p1 < 10) wIdList = weaponIdList.Where(id => DataManager.instance.dicWeapon[id].grade == 3).ToList();
            else if (p1< 50) wIdList = weaponIdList.Where(id => DataManager.instance.dicWeapon[id].grade == 2).ToList();
            else wIdList = weaponIdList.Where(id => DataManager.instance.dicWeapon[id].grade == 1).ToList();

            List<int> sIdList = new();
            if (p2 < 10)
            {
                sIdList = skillIdList.Where(id => DataManager.instance.dicActiveSkill[id].grade == 3).ToList();
            }
            else if (p2 < 50) sIdList = skillIdList.Where(id => DataManager.instance.dicActiveSkill[id].grade == 2).ToList();
            else sIdList = skillIdList.Where(id => DataManager.instance.dicActiveSkill[id].grade == 1).ToList();

            if(wIdList.Count == 0) wIdList = weaponIdList.Where(id => DataManager.instance.dicWeapon[id].grade == 1).ToList();
            if(sIdList.Count == 0) sIdList = skillIdList.Where(id => DataManager.instance.dicActiveSkill[id].grade == 1).ToList();

            int rndw = Random.Range(0, wIdList.Count);
            int rnds = Random.Range(0, sIdList.Count);

            weaponItems[i].gameObject.SetActive(true);
            weaponItems[i].Setting(wIdList[rndw]);
            skillItems[i].gameObject.SetActive(true);
            skillItems[i].Setting(sIdList[rnds]);

            weaponIdList.Remove(wIdList[rndw]);
            skillIdList.Remove(sIdList[rnds]);

            WeaponData wd = DataManager.instance.dicWeapon[wIdList[rndw]];
            ActiveSkillData sd = DataManager.instance.dicActiveSkill[sIdList[rnds]];
            weaponPrice[i] = wd.grade == 1 ? 500 + Random.Range(-25, 26) : (wd.grade == 2 ? 1000 + Random.Range(-50, 51) : 2000 + Random.Range(-100, 101));
            skillPrice[i] = sd.grade == 1 ? 500 + Random.Range(-25, 26) : (sd.grade == 2 ? 1000 + Random.Range(-50, 51) : 2000 + Random.Range(-100, 101));
        }
    }

    IEnumerator RefreshShopItemsRoutine()
    {
        refreshing = true;
        weaponContents.DOMoveY(Screen.height * 1.5f, 0.5f).SetEase(Ease.InBack).onComplete = () => {
            infoBox.SetActive(false);
            RefreshShopItems();
        };
        skillContents.DOMoveY(Screen.height * 1.5f, 0.5f).SetEase(Ease.InBack);
        yield return new WaitForSeconds(1f);
        weaponContents.DOMoveY(Screen.height / 2f, 0.5f).SetEase(Ease.OutBack).onComplete = () => { refreshing = false; };
        skillContents.DOMoveY(Screen.height / 2f, 0.5f).SetEase(Ease.OutBack);
    }
}
