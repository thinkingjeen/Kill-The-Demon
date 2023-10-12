using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceTitan : Monster
{
    public GameObject projectilePrefab;

    private float skill2407CoolTime;
    private float skill2414CoolTime;
    private float skill2415CoolTime;

    private float skill2407CoolTimeCheck;
    private float skill2414CoolTimeCheck;
    private float skill2415CoolTimeCheck;

    private void Awake()
    {
        this.id = 304;
        this.sizeX = 2;
        this.sizeY = 2;
    }

    public override void MonsterStart()
    {
        this.modelGO.animation.Play("Idle", 0);
        this.modelGO.animationName = "Idle";

        this.skill2407CoolTime = DataManager.instance.dicMonsterSkill[2407].coolTime;
        this.skill2407CoolTimeCheck = this.skill2407CoolTime;
        this.skill2414CoolTime = DataManager.instance.dicMonsterSkill[2414].coolTime;
        this.skill2414CoolTimeCheck = this.skill2414CoolTime;
        this.skill2415CoolTime = DataManager.instance.dicMonsterSkill[2415].coolTime;
        this.skill2415CoolTimeCheck = this.skill2415CoolTime;

        //2407가 3방향 폭발 투사체
        //2414이 전방 베기
        //2415이 고속 플레이어위치 범위공격 고드름 떨구기 (Attack C)
    }

    public override void MonsterUpdate()
    {
        this.skill2407CoolTimeCheck -= Time.deltaTime;
        this.skill2414CoolTimeCheck -= Time.deltaTime;
        this.skill2415CoolTimeCheck -= Time.deltaTime;

        if (this.modelGO.animationName != "Idle" && this.movingSpan < 0.1f)
        {
            this.modelGO.animation.Play("Idle", 0);
            this.modelGO.animation.timeScale = 1;
            this.modelGO.animationName = "Idle";
        }
    }

    public override int MonsterSituationMove0(eDirection dir, float magnititude, Vector2Int playerLocation)
    {
        if (skill2415CoolTimeCheck < 0)
        {
            skill2415CoolTimeCheck = skill2415CoolTime;
            this.movingSpan = DataManager.instance.dicMonsterSkill[2415].castDelay;
            this.AnimationPlay("Attack C");
            this.movingSpan = 3;
            return 2415;
        }
        else if (skill2407CoolTimeCheck < 0)
        {
            skill2407CoolTimeCheck = skill2407CoolTime;
            this.AnimationPlay("Attack A");
            this.movingSpan = 2;
            return 2407;
        }
        else if(magnititude < 3)
        {
            skill2414CoolTimeCheck = skill2414CoolTime;
            this.AnimationPlay("Attack B");
            this.movingSpan = 2;
            return 2414;
        }
        else if (skill2407CoolTimeCheck < 2)
        {
            return 1;
        }
        else return 0;
    }

    public override int MonsterSituationMove1(eDirection dir, float magnititude, Vector2Int playerLocation)
    {
        if (skill2415CoolTimeCheck < 0)
        {
            skill2415CoolTimeCheck = skill2415CoolTime;
            this.movingSpan = DataManager.instance.dicMonsterSkill[2415].castDelay;
            this.AnimationPlay("Attack C");
            this.movingSpan = 3;
            return 2415;
        }
        else return 1;
    }

    public override int MonsterSituationMove2(eDirection dir, float magnititude, Vector2Int playerLocation)
    {
        if (skill2415CoolTimeCheck < 0)
        {
            skill2415CoolTimeCheck = skill2415CoolTime;
            this.movingSpan = DataManager.instance.dicMonsterSkill[2415].castDelay;
            this.AnimationPlay("Attack C");
            this.movingSpan = 3;
            return 2415;
        }
        else return 1;
    }
}
