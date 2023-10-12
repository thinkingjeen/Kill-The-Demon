using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DragonBones;

public class Flynn : Monster
{
    public GameObject projectilePrefab;
    
    public Material modelColor;

    private float skill2410CoolTime;
    private float skill2411CoolTime;

    private float skill2410CoolTimeCheck;
    private float skill2411CoolTimeCheck;

    public Sprite projectileSprite;

    private void Awake()
    {
        this.id = 303;
        this.sizeX = 2;
        this.sizeY = 2;
    }

    public override void MonsterStart()
    {
        this.modelColor.color = new Color(1, 1, 1, 1);
        this.modelGO.animation.Play("Idle", 0);
        this.modelGO.animationName = "Idle";
        this.nAttackDelay = 0.3f;
        this.skill2410CoolTime = DataManager.instance.dicMonsterSkill[2410].coolTime;
        this.skill2411CoolTime = DataManager.instance.dicMonsterSkill[2411].coolTime;
        this.skill2410CoolTimeCheck = this.skill2410CoolTime;
        this.skill2411CoolTimeCheck = this.skill2411CoolTime;

        //2410가 투사체 발사
        //2411이 플레이어 위치로 점프
    }

    public override void MonsterUpdate()
    {
        this.skill2410CoolTimeCheck -= Time.deltaTime;
        this.skill2411CoolTimeCheck -= Time.deltaTime;
        if (this.modelGO.animationName != "Idle"&&this.movingSpan < 0.1f)
        {
            this.modelGO.animation.Play("Idle", 0);
            this.modelGO.animation.timeScale = 1;
            this.modelGO.animationName = "Idle";
        }
    }

    public override int MonsterSituationMove0(eDirection dir, float magnititude, Vector2Int playerLocation)
    {
        if (skill2410CoolTimeCheck < 0)
        {
            skill2410CoolTimeCheck = skill2410CoolTime;
            this.movingSpan = DataManager.instance.dicMonsterSkill[2410].castDelay;
            
            this.AnimationPlay("Skill Special");
            this.movingSpan = 2;
            return 2410;
        }
        else if (magnititude < 1.3f)
        {
            this.AnimationPlay("Attack");
            this.movingSpan = 3;
            return 2400;
        }
        else if (skill2410CoolTimeCheck < 1)
        {
            return 1;
        }
        else
        {
            return 0;
        }
    }

    public override int MonsterSituationMove1(eDirection dir, float magnititude, Vector2Int playerLocation)
    {
        if (skill2411CoolTimeCheck < 0)
        {
            skill2411CoolTimeCheck = skill2411CoolTime;
            this.movingSpan = DataManager.instance.dicMonsterSkill[2411].castDelay;
            this.AnimationPlay("Skill Special B");
            this.movingSpan = 3.3f;
            return 2411;
        }
        else
        {
            return 0;
        }
    }

    public override int MonsterSituationMove2(eDirection dir, float magnititude, Vector2Int playerLocation)
    {
        if (skill2411CoolTimeCheck < 0)
        {
            skill2411CoolTimeCheck = skill2411CoolTime;
            this.movingSpan = DataManager.instance.dicMonsterSkill[2411].castDelay;
            this.AnimationPlay("Skill Special B");
            this.movingSpan = 3.3f;
            return 2411;
        }
        else
        {
            return 0;
        }
    }
    
}
