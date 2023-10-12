using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunicStone : Monster
{
    public GameObject projectilePrefab;

    private float skill2416CoolTime;
    private float skill2417CoolTime;
    private float skill2418CoolTime;

    private float skill2416CoolTimeCheck;
    private float skill2417CoolTimeCheck;
    private float skill2418CoolTimeCheck;

    private void Awake()
    {
        this.id = 305;
        this.sizeX = 2;
        this.sizeY = 3;
    }

    public override void MonsterStart()
    {
        this.modelGO.animation.Play("Idle", 0);
        this.modelGO.animationName = "Idle";

        this.skill2416CoolTime = DataManager.instance.dicMonsterSkill[2416].coolTime;
        this.skill2416CoolTimeCheck = this.skill2416CoolTime;
        this.skill2417CoolTime = DataManager.instance.dicMonsterSkill[2417].coolTime;
        this.skill2417CoolTimeCheck = this.skill2417CoolTime;
        this.skill2418CoolTime = DataManager.instance.dicMonsterSkill[2418].coolTime;
        this.skill2418CoolTimeCheck = this.skill2418CoolTime;

        /*2416 : 전방 공격 강화

          2417 : 전방 찌르기

          2418 : 맵 전역 파도*/
    }

    public override void MonsterUpdate()
    {
        this.skill2416CoolTimeCheck -= Time.deltaTime;
        this.skill2417CoolTimeCheck -= Time.deltaTime;
        this.skill2418CoolTimeCheck -= Time.deltaTime;

        if (this.modelGO.animationName != "Idle" && this.movingSpan < 0.1f)
        {
            this.modelGO.animation.Play("Idle", 0);
            this.modelGO.animation.timeScale = 1;
            this.modelGO.animationName = "Idle";
        }
    }

    public override int MonsterSituationMove0(eDirection dir, float magnititude, Vector2Int playerLocation)
    {

        if (skill2418CoolTimeCheck < 0)
        {
            skill2418CoolTimeCheck = skill2418CoolTime;
            this.AnimationPlay("Attack E");
            this.movingSpan = 2;
            return 2418;
        }
        else if (magnititude < 4)
        {
            if (this.skill2416CoolTimeCheck < 0)
            {
                skill2416CoolTimeCheck = skill2416CoolTime;
                this.AnimationPlay("Attack D");
                this.movingSpan = 2;
                return 2416;
            }
            this.AnimationPlay("Attack B");
            this.movingSpan = 2;
            return 1557;
        }
        else if (skill2417CoolTimeCheck < 0 && magnititude < 8)
        {
            skill2417CoolTimeCheck = skill2417CoolTime;
            this.AnimationPlay("Attack C");
            this.movingSpan = 3;
            return 2417;
        }
        else return 0;
    }

    public override int MonsterSituationMove1(eDirection dir, float magnititude, Vector2Int playerLocation)
    {
        if (skill2418CoolTimeCheck < 0)
        {
            skill2418CoolTimeCheck = skill2418CoolTime;
            this.AnimationPlay("Attack E");
            this.movingSpan = 2;
            return 2418;
        }
        else
        {
            this.skill2418CoolTimeCheck -= 1;
            return 0;
        }
    }

    public override int MonsterSituationMove2(eDirection dir, float magnititude, Vector2Int playerLocation)
    {
        if (skill2418CoolTimeCheck < 0)
        {
            skill2418CoolTimeCheck = skill2418CoolTime;
            this.AnimationPlay("Attack E");
            this.movingSpan = 2;
            return 2418;
        }
        else if (Mathf.Abs(playerLocation.x - this.location.x) <= 1)
        {
            if (this.skill2416CoolTimeCheck < 0)
            {
                skill2416CoolTimeCheck = skill2416CoolTime;
                this.AnimationPlay("Attack D");
                this.movingSpan = 2;
                return 24161;
            }
            this.AnimationPlay("Attack B");
            this.movingSpan = 2;
            return 15571;
        }
        else
        {
            this.skill2418CoolTimeCheck -= 1;
            return 1;
        }
    }
}
