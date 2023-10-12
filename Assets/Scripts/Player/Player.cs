using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

public class Player : MonoBehaviour
{
    public Vector2Int location;
    public Vector2Int position;
    public eDirection dir;

    public Transform model;
    public GameObject weaponGO;
    public SpriteRenderer weaponSpriteRenderer;
    public GameObject bowstrings;
    Animator weaponAnim;
    public Transform playerCamera;
    public bool rightDirection;
    public Weapon weapon;
    public GameObject satellite;

    public UnityAction onMoveCompleteAction;

    public SpriteRenderer resultBackground;

    public AudioClip swordAudio;
    public AudioClip lanceAudio;
    public AudioClip bowAudio;
    public AudioClip gunAudio;
    public AudioClip magicAudio;
    void Start()
    { 
        weaponGO.transform.localPosition = Vector3.zero;
        weaponGO.transform.localEulerAngles = new Vector3(0, 0, rightDirection ? 45 : -45);
        weaponGO.transform.position += Vector3.up * 0.3f;
        weaponAnim = weaponGO.GetComponent<Animator>();

        transform.position = Vector3Int.RoundToInt(transform.position) - new Vector3(0, 0.3f, 0);
        position = Vector2Int.RoundToInt(transform.position);
    }

    public void ChangeWeaponSprite(Vector3? scale = null)
    {
        weaponSpriteRenderer.sprite = DataManager.instance.dicAtlas["Weapon"].GetSprite(DataManager.instance.dicWeapon[InfoManager.instance.gameInfo.weapon].atlasName);
        int id = InfoManager.instance.gameInfo.weapon / 10;
        if (id == 121 || id == 122) bowstrings.SetActive(true);
        else bowstrings.SetActive(false);
        weaponSpriteRenderer.transform.localScale = scale ?? Vector3.one; // == scale.HasValue ? scale.Value : Vector3.one
    }

    public void Move(Vector2Int bl)
    {
        transform.DOMove((Vector3)(Vector2)(location + bl) - new Vector3(0, 0.3f,0), 0.1f).onComplete += () => {
            position = Vector2Int.RoundToInt(transform.position);
            onMoveCompleteAction();
        };
        
    }

    public void Rotate(eDirection direction)
    {
        if(direction != eDirection.none) dir = direction;
        //1-상 2-하 3-좌 4-우
        switch (direction)
        {
            case eDirection.up:
                transform.rotation = Quaternion.Euler(0, 0, 0);
                model.localRotation = Quaternion.Euler(0, rightDirection ? 180 : 0, -transform.rotation.eulerAngles.z);
                break;
            case eDirection.down:
                transform.rotation = Quaternion.Euler(0, 0, 180);
                model.localRotation = Quaternion.Euler(0, rightDirection ? 180 : 0, -transform.rotation.eulerAngles.z);
                break;
            case eDirection.left:
                transform.rotation = Quaternion.Euler(0, 0, 90);
                rightDirection = false;
                model.localRotation = Quaternion.Euler(0, 0, -transform.rotation.eulerAngles.z);
                break;
            case eDirection.right:
                transform.rotation = Quaternion.Euler(0, 0, -90);
                rightDirection = true;
                model.localRotation = Quaternion.Euler(180, 0, -transform.rotation.eulerAngles.z);
                break;
        }
        weaponGO.transform.localPosition = Vector3.zero;
        weaponSpriteRenderer.flipX = !rightDirection;
        weaponGO.transform.localEulerAngles = new Vector3(0, 0, rightDirection ? 45 : -45);
        
        weaponGO.transform.position += Vector3.up * 0.3f;
        playerCamera.transform.localRotation = Quaternion.Euler(0, 0, -transform.rotation.eulerAngles.z);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="weaponType">111 ~ 113: 근접, 114: 창, 121~122: 활, 123: 총 131~133: 마법</param>
    /// <param name="attackKind">0: 공격, 1: 스킬 1, 2: 스킬 2, 3: 스킬 3</param>
    public void PlayWeaponAnimation(int weaponType, int attackKind = 0)
    {   
        string trigger = "";
        if (attackKind == 0)
        {
            if (111 <= weaponType && weaponType <= 113) {
                SoundManager.PlaySFX(this.swordAudio);
                trigger += "ShortAttack"; 
            }
            else if (weaponType == 114) {
                SoundManager.PlaySFX(this.lanceAudio);
                trigger +=  "LanceAttack"; 
            }
            else if (121 <= weaponType && weaponType <= 122) {
                SoundManager.PlaySFX(this.bowAudio);
                trigger += "BowAttack"; 
            }
            else if (weaponType == 123) {
                SoundManager.PlaySFX(this.gunAudio);
                trigger +=  "GunAttack"; 
            }
            else if (131 <= weaponType && weaponType <= 133) {
                SoundManager.PlaySFX(this.magicAudio);
                trigger +=  "MagicAttack"; 
            }

            trigger += rightDirection ? "Right" : "Left";
        }
        else if(attackKind == 1)
        {
            // 스킬 1
        }
        else if (attackKind == 2)
        {
            // 스킬 2
        }
        else if (attackKind == 3)
        {
            // 스킬 3
        }
        Debug.Log(trigger);
        weaponAnim.SetTrigger(trigger);
    }

    public void PlayWeaponAnimation(string trigger)
    {
        weaponAnim.SetTrigger(trigger);
    }

    public void Death()
    {
        resultBackground.DOColor(new Color(0, 0, 0, 1), 2f).onComplete = () =>
        {
            Camera.main.transform.DOMoveY(transform.position.y + 2.5f, 1f);
        };
    }
}
