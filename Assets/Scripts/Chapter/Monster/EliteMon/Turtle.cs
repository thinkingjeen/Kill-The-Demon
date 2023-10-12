using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using DragonBones;

public class Turtle : Monster
{
    public GameObject projectilePrefab;

    private float skill2405CoolTime;
    private float skill2406CoolTime;
    private float skill2408CoolTime;

    private float skill2406CoolTimeCheck;
    private float skill2408CoolTimeCheck;
    private float skill2405CoolTimeCheck;

    public Sprite projectileSprite;

    private void Awake()
    {
        this.id = 301;
        this.sizeX = 5;
        this.sizeY = 2;
    }

    public override void MonsterStart()
    {
        this.modelGO.animation.Play("Idle", 0);
        this.modelGO.animationName = "Idle";

        this.skill2406CoolTime = DataManager.instance.dicMonsterSkill[2406].coolTime;
        this.skill2408CoolTime = DataManager.instance.dicMonsterSkill[2408].coolTime;
        this.skill2405CoolTime = DataManager.instance.dicMonsterSkill[2405].coolTime;
        this.skill2406CoolTimeCheck = this.skill2406CoolTime;
        this.skill2408CoolTimeCheck = this.skill2408CoolTime;
        this.skill2405CoolTimeCheck = this.skill2405CoolTime;

        //2405가 8방향 낙석스킬
        //2406이 투사체날리기
        //2408이 일직선 레이저
    }

    public override void MonsterUpdate()
    {
        this.skill2405CoolTimeCheck -= Time.deltaTime;
        this.skill2406CoolTimeCheck -= Time.deltaTime;
        this.skill2408CoolTimeCheck -= Time.deltaTime;

        if (this.modelGO.animationName != "Idle" && this.movingSpan < 0.1f)
        {
            this.modelGO.animation.Play("Idle", 0);
            this.modelGO.animation.timeScale = 1;
            this.modelGO.animationName = "Idle";
        }
    }

    public override int MonsterSituationMove0(eDirection dir, float magnititude, Vector2Int playerLocation)
    {
        if (skill2408CoolTimeCheck < 0)
        {
            skill2408CoolTimeCheck = skill2408CoolTime;
            this.movingSpan = DataManager.instance.dicMonsterSkill[2408].castDelay;
            this.AnimationPlay("Attack C");
            this.movingSpan = 4;
            return 2408;
        }
        else if (skill2406CoolTimeCheck < 0)
        {
            skill2406CoolTimeCheck = skill2406CoolTime;
            this.AnimationPlay("Attack A");
            this.movingSpan = 2;
            return 2406;
        }
        else return 1;
    }

    public override int MonsterSituationMove1(eDirection dir, float magnititude, Vector2Int playerLocation)
    {
        if (skill2408CoolTimeCheck < 0)
        {
            skill2408CoolTimeCheck = skill2408CoolTime;
            this.movingSpan = DataManager.instance.dicMonsterSkill[2408].castDelay;
            this.AnimationPlay("Attack C");
            this.movingSpan = 4;
            return 2408;
        }
        else return 1;
    }

    public override int MonsterSituationMove2(eDirection dir, float magnititude, Vector2Int playerLocation)
    {
        if (skill2405CoolTimeCheck < 0)
        {
            skill2405CoolTimeCheck = skill2405CoolTime;
            this.AnimationPlay("Attack B");
            this.movingSpan = 3;
            return 2405;
        }
        else return 1;
    }
}