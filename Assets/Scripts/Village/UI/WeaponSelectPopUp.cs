using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.U2D;

public class WeaponSelectPopUp : MonoBehaviour
{
    private enum eGrade { 노말, 레어, 에픽, 레전드리  }
    private enum eType { 근접 = 1, 원거리 = 2, 마법 = 3}
    
    public SpriteAtlas weaponAtlas;

    //인덱스 0은 근접무기, 1은 원거리무기, 2는 마법무기
    public Button[] weaponBtns;
    public Image[] weaponIcons;
    public Text[] weaponTexts;

    public Button selectCompleteBtn;
    public Button selectCancelBtn;
    public Button btnUnlockAdditory;
        public GameObject additoryItem;

    public UnityAction onWeaponSelectCompleteAction;
    public UnityAction onWeaponSelectCancelAction;
    public UnityAction<int> onDiaSpendAction;
    private int selectedWeaponId = -1;
    private int[] randWeaponIds = new int[4];

    private void Awake()
    {
        for (int i = 0; i < 4; i++)
        {
            var tempList = DataManager.instance.dicWeapon.Where(
                x => x.Value.grade == ((i == 3) ? 1 : 0) && ((i == 3) ? true : x.Value.type == i + 1)
                && InfoManager.instance.playerInfo.unlockWaeponIds.Exists(y => y == x.Key) // 해금 아이템 목록에 있는지 확인
                ).ToList();
            int randX = Random.Range(0, tempList.Count);
            randWeaponIds[i] = tempList[randX].Key;
        }
    }

    void Start()
    {
        for (int i = 0; i < 4; i++)
        {
            WeaponData tempWeapon = DataManager.instance.dicWeapon[randWeaponIds[i]];
            this.weaponIcons[i].sprite = this.weaponAtlas.GetSprite(tempWeapon.atlasName);
            this.weaponTexts[i].text = string.Format("이름 : {0}\n등급 : {1}\n타입 : {2}\n공격력 : {3}\n공격속도 : {4}\n스킬계수 : {5}",
                tempWeapon.name, (eGrade)tempWeapon.grade, (eType)tempWeapon.type, tempWeapon.attack, tempWeapon.delay, tempWeapon.coefficient);
            int temp = i;
            weaponBtns[temp].onClick.AddListener(() => 
            {
                App.instance.YesAudio();
                this.selectedWeaponId = randWeaponIds[temp];
                this.selectCompleteBtn.gameObject.SetActive(true);
            });
        }

        this.selectCompleteBtn.onClick.AddListener(() => 
        {
            App.instance.YesAudio();
            InfoManager.instance.gameInfo.weapon = this.selectedWeaponId;
            InfoManager.instance.gameInfo.skills[0] = 2000 + (100 * (DataManager.instance.dicWeapon[selectedWeaponId].type));
            InfoManager.instance.gameInfo.skills[1] = 2000 + (100 * (DataManager.instance.dicWeapon[selectedWeaponId].type)) +1;
            onWeaponSelectCompleteAction();
        });

        this.selectCancelBtn.onClick.AddListener(() => 
        {
            App.instance.NoAudio();
            this.selectedWeaponId = -1;
            this.selectCompleteBtn.gameObject.SetActive(false);
            onWeaponSelectCancelAction();
            this.gameObject.SetActive(false);
        });

        btnUnlockAdditory.onClick.AddListener(() => {
            if (InfoManager.instance.playerInfo.dia >= 2)
            {
                onDiaSpendAction(2);
                additoryItem.SetActive(true);
                btnUnlockAdditory.gameObject.SetActive(false);
            }
        });

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
