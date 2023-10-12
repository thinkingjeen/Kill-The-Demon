using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

public class UIJoysticks : MonoBehaviour
{
    public BtnTurbo btnUp;
    public BtnTurbo btnDown;
    public BtnTurbo btnLeft;
    public BtnTurbo btnRight;

    public VariableJoystick joyStick;
    private float joyTime;
    private bool joyXTrigger = false;
    private bool joyYTrigger = false;
    private bool joyTrigger = false;

    public SwipeButton[] swipeButtons = new SwipeButton[4];
    public Button btnConversation;
    public Image[] skillIconImages;

    public UnityAction<eDirection> onMoveAction;
    /// <summary>
    /// 0 - 공격 | 1,2,3 - 스킬 1,2,3
    /// </summary>
    public UnityAction<eDirection>[] onAttackAction = new UnityAction<eDirection>[4];
    public UnityAction onConversationAction;

    /*public Text debugText;*/
    public void Refresh()
    {
        for (int i = 0; i < 4; i++)
            if(InfoManager.instance.gameInfo.skills[i] != 0)
                swipeButtons[i].coolTime = DataManager.instance.dicActiveSkill[InfoManager.instance.gameInfo.skills[i]].coolTime;
        
    }
    public void CoolTimeReset() 
    {
        foreach (SwipeButton b in swipeButtons) b.CoolTimeReset();
    }

    void Start()
    {
        //이동 버튼
        //btnUp.onClick.AddListener(() => { onMoveAction(eDirection.up); });
        btnUp.onBtnAction = () =>
        {
            onMoveAction(eDirection.up);
        };
        //btnDown.onClick.AddListener(() => { onMoveAction(eDirection.down); });
        btnDown.onBtnAction = () =>
        {
            onMoveAction(eDirection.down);
        };
        //btnLeft.onClick.AddListener(() => { onMoveAction(eDirection.left); });
        btnLeft.onBtnAction = () =>
        {
            onMoveAction(eDirection.left);
        };
        //btnRight.onClick.AddListener(() => { onMoveAction(eDirection.right); });
        btnRight.onBtnAction = () =>
        {
            onMoveAction(eDirection.right);
        };

        //공격, 스킬 버튼
        for (int i = 0; i < swipeButtons.Length; i++)
        {
            int tmp = i;
            swipeButtons[tmp].buttonActive = true;
            swipeButtons[tmp].onButtonDownAction += () =>
            {
                for (int j = 0; j < swipeButtons.Length; j++) swipeButtons[j].buttonActive = false;
                swipeButtons[tmp].buttonActive = true;
            };
            swipeButtons[tmp].onButtonUpAction += (direction) =>
            {
                onAttackAction[tmp](direction);
                for (int j = 0; j < swipeButtons.Length; j++)
                {
                    swipeButtons[j].buttonActive = true;
                }

                /*if (debugText.gameObject.activeInHierarchy) debugText.text = string.Format("{0} - {1}", swipeButtons[tmp].name, direction);*/
            };
        }

        btnConversation.onClick.AddListener(() => { onConversationAction(); });
        btnConversation.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (Mathf.Abs(this.joyStick.Horizontal) > Mathf.Abs(this.joyStick.Vertical))
        {
            this.joyYTrigger = false;
            if (this.joyStick.Horizontal > 0.3f && !this.joyXTrigger)
            {
                onMoveAction(eDirection.right);
                this.joyXTrigger = true;
            }
            else if (this.joyStick.Horizontal < -0.3f && !this.joyXTrigger)
            {
                onMoveAction(eDirection.left);
                this.joyXTrigger = true;
            }
        }
        else
        {
            this.joyXTrigger = false;
            if (this.joyStick.Vertical > 0.3f && !this.joyYTrigger)
            {
                onMoveAction(eDirection.up);
                this.joyYTrigger = true;
            }
            else if (this.joyStick.Vertical < -0.3f && !this.joyYTrigger)
            {
                onMoveAction(eDirection.down);
                this.joyYTrigger = true;
            }
        }
        if (this.joyXTrigger)
        {
            if (Mathf.Abs(this.joyStick.Horizontal) <= 0.3f)
            {
                this.joyXTrigger = false;
                this.joyTime = 0;
            }
            else
            {
                this.joyTime += Time.deltaTime;
                if (this.joyTime > 0.25f)
                {
                    if (this.joyStick.Horizontal > 0)
                    {
                        onMoveAction(eDirection.right);
                        this.joyTime = 0;
                    }
                    else
                    {
                        onMoveAction(eDirection.left);
                        this.joyTime = 0;
                    }
                }
            }
        }
        if (this.joyYTrigger)
        {
            if (Mathf.Abs(this.joyStick.Vertical) <= 0.3f)
            {
                this.joyYTrigger = false;
                this.joyTime = 0;
            }
            else
            {
                this.joyTime += Time.deltaTime;
                if (this.joyTime > 0.25f)
                {
                    if (this.joyStick.Vertical > 0)
                    {
                        onMoveAction(eDirection.up);
                        this.joyTime = 0;
                    }
                    else
                    {
                        onMoveAction(eDirection.down);
                        this.joyTime = 0;
                    }
                }
            }
        }
        if (this.joyXTrigger&&this.joyYTrigger)
        {
            Debug.Log("이속2배");
        }
    }
    public void ConversationButtonApear()
    {
        btnConversation.gameObject.SetActive(true);
        btnConversation.transform.localScale = Vector3.zero;
        btnConversation.transform.DOScale(0.7f, 0.1f);
    }
    public void ConversationButtonDisapear()
    {
        btnConversation.transform.DOScale(0f, 0.1f).onComplete +=()=> {
            btnConversation.gameObject.SetActive(false);
        };
    }
    public void ChangePlayerSkillSprite()
    {
        for (int i = 1; i < 4; ++i)
        {
            int skillId = InfoManager.instance.gameInfo.skills[i];
            if ( skillId == 0)
            {
                this.swipeButtons[i].gameObject.SetActive(false);
            }
            else
            {
                this.swipeButtons[i].gameObject.SetActive(true);
                this.skillIconImages[i].sprite = DataManager.instance.dicAtlas["Skill"].GetSprite(DataManager.instance.dicActiveSkill[skillId].atlasName);
            }
        }
    }
}
