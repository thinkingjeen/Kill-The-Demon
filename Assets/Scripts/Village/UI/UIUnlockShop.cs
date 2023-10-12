using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;
using UnityEngine.Events;
using DG.Tweening;
using Unity.VisualScripting;

public class UIUnlockShop : MonoBehaviour
{
    public Button btnClose;

    public Material pixelateMat;
    public Material defaultMat;
    public GameObject itemPrefab;
    public Button btnWeaponTab;
        public GameObject weaponScroll;
            public GameObject weaponScrollContents;
                public List<UnlockShopItem> weaponItems = new();
    public Button btnSkillTab;
        public GameObject skillScroll;
            public GameObject skillScrollContents;
                public List<UnlockShopItem> skillItems = new();

    public GameObject itemInfoBox;
        int boxItemId;
        public Image icon;
        public Text dataText;
        public Button btnUnlock;
            public Text unlockButtonText;

    public UnityAction onUnlockShopCloseAction;
    public UnityAction<int> onItemUnlockAction;
    void Start()
    {
        btnClose.onClick.AddListener(() => {
            App.instance.NoAudio();
            gameObject.SetActive(false);
            onUnlockShopCloseAction();
        });
        btnWeaponTab.onClick.AddListener(() =>
        {
            App.instance.YesAudio();
            if (skillScroll.activeInHierarchy)
            {
                weaponScroll.SetActive(true);
                skillScroll.SetActive(false);
                itemInfoBox.SetActive(false);
                boxItemId = 0;
                for(int i = 0; i < weaponItems.Count; i++) { weaponItems[i].icon.material = defaultMat; }
                btnWeaponTab.GetComponent<RectTransform>().DOLocalMoveY(300, 0.1f);
                btnSkillTab.GetComponent<RectTransform>().DOLocalMoveY(200, 0.1f);
            }

        });
        btnSkillTab.onClick.AddListener(() =>
        {
            App.instance.YesAudio();
            if (weaponScroll.activeInHierarchy)
            {
                weaponScroll.SetActive(false);
                skillScroll.SetActive(true);
                itemInfoBox.SetActive(false);
                boxItemId = 0;
                for (int i = 0; i < skillItems.Count; i++) { skillItems[i].icon.material = pixelateMat; }

                btnWeaponTab.GetComponent<RectTransform>().DOLocalMoveY(200, 0.1f);
                btnSkillTab.GetComponent<RectTransform>().DOLocalMoveY(300, 0.1f);
            }
        });

        for (int i = 0; i < DataManager.instance.dicWeapon.Count; i++)
        {
            int id = DataManager.instance.dicWeapon.ElementAt(i).Key;
            GameObject go = Instantiate(itemPrefab, weaponScrollContents.transform);
            UnlockShopItem item = go.GetComponent<UnlockShopItem>();
            weaponItems.Add(item);
            item.Init(id, DataManager.instance.dicAtlas["Weapon"]);
            item.onButtonClickAction += (id) => { InfoBoxChange(id); };

        }
        for(int i = 0; i < DataManager.instance.dicActiveSkill.Count; i++)
        {
            int id = DataManager.instance.dicActiveSkill.ElementAt(i).Key;
            if (id % 100 == 0) continue;

            GameObject go = Instantiate(itemPrefab, skillScrollContents.transform);
            UnlockShopItem item = go.GetComponent<UnlockShopItem>();
            skillItems.Add(item);
            item.Init(id, DataManager.instance.dicAtlas["Skill"]);
            item.onButtonClickAction += (id) => { InfoBoxChange(id); };
        }


        btnUnlock.onClick.AddListener(() => {
            Debug.LogFormat("id : {0}", boxItemId);

            int requireDia = 0;
            int requireEpicDia = 15;
            int requireLegendDia = 55;

            if (boxItemId < 2000)
            {
                requireDia = DataManager.instance.dicWeapon[boxItemId].grade == 2 ? requireEpicDia : requireLegendDia;
                if (InfoManager.instance.playerInfo.dia >= requireDia)
                {
                    weaponItems.Find(x => boxItemId == x.id).UnlockItem();
                    onItemUnlockAction(-requireDia);
                }
                else
                {
                    return;
                }
            }
            else if(boxItemId >= 2000)
            {
                requireDia = DataManager.instance.dicActiveSkill[boxItemId].grade == 2 ? requireEpicDia : requireLegendDia;
                if (InfoManager.instance.playerInfo.dia >= requireDia)
                {
                    skillItems.Find(x => boxItemId == x.id).UnlockItem();
                    onItemUnlockAction(-requireDia);
                }
                else { return; }
            }
            btnUnlock.gameObject.SetActive(false);

            App.instance.YesAudio(); //���� �ٸ� ���� ������ �װŷ� ������ �� ����
        });
    }

    void InfoBoxChange(int id)
    {
        itemInfoBox.SetActive(true);
        Debug.Log(id);
        boxItemId = id;

        int requireEpicDia = 15;
        int requireLegendDia = 55;

        if (id < 2000)
        {
            WeaponData d = DataManager.instance.dicWeapon[id];
            this.icon.sprite = DataManager.instance.dicAtlas["Weapon"].GetSprite(d.atlasName);

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
            dataText.text = string.Format("�̸� : {0}\n���� : {1}\n��� : {2}\n���ݷ� : {3}\n���ݼӵ� : {4}\n��ų��� : {5}", d.name, type, grade, d.attack, (1 / d.delay).ToString("F2"), d.coefficient);
            
            
            if (InfoManager.instance.playerInfo.unlockWaeponIds.Exists(x => x == boxItemId)) btnUnlock.gameObject.SetActive(false);
            else btnUnlock.gameObject.SetActive(true);
            icon.material = defaultMat;
            unlockButtonText.text = d.grade == 2 ? requireEpicDia.ToString() : requireLegendDia.ToString();
        }
        else if(id >= 2000)
        {
            ActiveSkillData d = DataManager.instance.dicActiveSkill[id];
            icon.sprite = DataManager.instance.dicAtlas["Skill"].GetSprite(d.atlasName);
            //dataText.text = string.Format("id : {0}\nname : {1}\ngrade:{2}\ntype : {3}\n��Ÿ ��� ���� ǥ��",d.id, d.name, d.grade, d.type);
            string grade = null;
            switch (d.grade)
            {
                case 0: grade = "�븻"; break;
                case 1: grade = "����"; break;
                case 2: grade = "����"; break;
                case 3: grade = "��������"; break;

            }
            dataText.text = string.Format("�̸� : {0}\n��� : {1}\n���� : {2}\n������ : {3}\n��Ÿ�� : {4}", d.name, grade, d.information, d.damage, d.coolTime);

            if (InfoManager.instance.playerInfo.unlockSkillIds.Exists(x => x == boxItemId)) btnUnlock.gameObject.SetActive(false);
            else btnUnlock.gameObject.SetActive(true);
            icon.material = pixelateMat;
            unlockButtonText.text = d.grade == 2 ? requireEpicDia.ToString() : requireLegendDia.ToString();
        }
    }

    
}
