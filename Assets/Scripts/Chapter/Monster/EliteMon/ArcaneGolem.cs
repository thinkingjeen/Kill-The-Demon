using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ArcaneGolem : Monster
{
    private float skill2801CoolTime;
    private float skill2802CoolTime;
    private float skill2803CoolTime;

    private float skill2801CoolTimeCheck;
    private float skill2802CoolTimeCheck;
    private float skill2803CoolTimeCheck;

    public Sprite projectileSprite;

    private void Awake()
    {
        this.id = 381;
        this.sizeX = 2;
        this.sizeY = 2;
    }
    public override void MonsterStart()
    {
        this.modelGO.animation.Play("Idle", 0);
        this.modelGO.animationName = "Idle";

        this.skill2801CoolTime = DataManager.instance.dicMonsterSkill[2801].coolTime;
        this.skill2801CoolTimeCheck = this.skill2801CoolTime;

        this.skill2802CoolTime = DataManager.instance.dicMonsterSkill[2802].coolTime;
        this.skill2802CoolTimeCheck = this.skill2802CoolTime;

        this.skill2803CoolTime = DataManager.instance.dicMonsterSkill[2803].coolTime;
        this.skill2803CoolTimeCheck = this.skill2803CoolTime;

        //2801은 8방향 투사체발사
        //2802는 3열 투사체
        //2803은 예고공격
    }

    public override void MonsterUpdate()
    {
        this.skill2801CoolTimeCheck -= Time.deltaTime;
        this.skill2802CoolTimeCheck -= Time.deltaTime;
        this.skill2803CoolTimeCheck -= Time.deltaTime;

        if (this.modelGO.animationName != "Idle" && this.movingSpan < 0.1f)
        {
            this.modelGO.animation.Play("Idle", 0);
            this.modelGO.animation.timeScale = 1;
            this.modelGO.animationName = "Idle";
        }
    }
    public override int MonsterSituationMove0(eDirection dir, float magnititude, Vector2Int playerLocation)
    {
        if (skill2801CoolTimeCheck < 0)
        {
            skill2801CoolTimeCheck = skill2801CoolTime;
            this.AnimationPlay("Attack F");
            this.movingSpan = 2;
            return 2801;
        }
        else if (skill2802CoolTimeCheck < 0)
        {
            skill2802CoolTimeCheck = skill2802CoolTime;
            this.AnimationPlay("Attack A");
            this.movingSpan = 1.5f;
            return 2802;
        }
        return 1;
    }

    public override int MonsterSituationMove1(eDirection dir, float magnititude, Vector2Int playerLocation)
    {
        if (skill2803CoolTimeCheck < 0)
        {
            skill2803CoolTimeCheck = skill2803CoolTime;
            this.AnimationPlay("Attack B");
            this.movingSpan = 2;
            return 2803;
        }
        return 1;
    }

    public override int MonsterSituationMove2(eDirection dir, float magnititude, Vector2Int playerLocation)
    {
        if (skill2803CoolTimeCheck < 0)
        {
            skill2803CoolTimeCheck = skill2803CoolTime;
            this.AnimationPlay("Attack B");
            this.movingSpan = 2;
            return 2803;
        }
        else if (skill2801CoolTimeCheck < 0)
        {
            skill2801CoolTimeCheck = skill2801CoolTime;
            this.AnimationPlay("Attack F");
            this.movingSpan = 2;
            return 2801;
        }
        return 1;
    }

}
