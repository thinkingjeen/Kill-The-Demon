using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Duesa : Monster
{
    public GameObject projectilePrefab;

    private float skill2419CoolTime;
    private float skill2420CoolTime;
    private float skill2421CoolTime;
    private float skill2422CoolTime;

    private float skill2419CoolTimeCheck;
    private float skill2420CoolTimeCheck;
    private float skill2421CoolTimeCheck;
    private float skill2422CoolTimeCheck;

    private void Awake()
    {
        this.id = 306;
        this.sizeX = 6;
        this.sizeY = 3;
    }

    public override void MonsterStart()
    {
        this.modelGO.animation.Play("Idle", 0);
        this.modelGO.animationName = "Idle";

        this.skill2419CoolTime = DataManager.instance.dicMonsterSkill[2419].coolTime;
        this.skill2419CoolTimeCheck = this.skill2419CoolTime;
        this.skill2420CoolTime = DataManager.instance.dicMonsterSkill[2420].coolTime;
        this.skill2420CoolTimeCheck = this.skill2420CoolTime;
        this.skill2421CoolTime = DataManager.instance.dicMonsterSkill[2421].coolTime;
        this.skill2421CoolTimeCheck = 0;
        this.skill2422CoolTime = DataManager.instance.dicMonsterSkill[2422].coolTime;
        this.skill2422CoolTimeCheck = this.skill2422CoolTime;

        /*2419 : 랜덤위치 매태오 ,A

          2420 : 큰범위 폭발 투사체, 이동속도 느림, 3발, 몬스터 위치가 아니라 랜덤y좌표 벽부터, B

          2421 : 중앙 범위 흡입 레이저, D

          2422 : 플레이어 위치 예고공격, A */
    }

    public override void MonsterUpdate()
    {
        this.skill2419CoolTimeCheck -= Time.deltaTime;
        this.skill2420CoolTimeCheck -= Time.deltaTime;
        this.skill2421CoolTimeCheck -= Time.deltaTime;
        this.skill2422CoolTimeCheck -= Time.deltaTime;

        if (this.modelGO.animationName != "Idle" && this.movingSpan < 0.1f)
        {
            this.modelGO.animation.Play("Idle", 0);
            this.modelGO.animation.timeScale = 1;
            this.modelGO.animationName = "Idle";
        }
    }

    public override int MonsterSituationMove0(eDirection dir, float magnititude, Vector2Int playerLocation)
    {
        if(magnititude < 10)
        {
            this.skill2421CoolTimeCheck -= 1;
        }
        if (skill2421CoolTimeCheck < 0)
        {
            skill2421CoolTimeCheck = skill2421CoolTime;
            this.AnimationPlay("Attack D");
            this.movingSpan = 7;
            return 2421;
        }
        else if (this.skill2419CoolTimeCheck < 0)
        {
            skill2419CoolTimeCheck = skill2419CoolTime;
            this.AnimationPlay("Attack A");
            this.movingSpan = 1.5f;
            return 2419;
        }
        else if (skill2420CoolTimeCheck < 0)
        {
            skill2420CoolTimeCheck = skill2420CoolTime;
            this.AnimationPlay("Attack B");
            this.movingSpan = 1.5f;
            return 2420;
        }
        else if (skill2422CoolTimeCheck < 0)
        {
            skill2422CoolTimeCheck = skill2422CoolTime;
            this.AnimationPlay("Attack A");
            this.movingSpan = 1.5f;
            return 2422;
        }
        else return -1;
    }

    public override int MonsterSituationMove1(eDirection dir, float magnititude, Vector2Int playerLocation)
    {
        if (magnititude < 10)
        {
            this.skill2421CoolTimeCheck -= 1;
        }
        if (skill2421CoolTimeCheck < 0)
        {
            skill2421CoolTimeCheck = skill2421CoolTime;
            this.AnimationPlay("Attack D");
            this.movingSpan = 7;
            return 2421;
        }
        else if (this.skill2419CoolTimeCheck < 0)
        {
            skill2419CoolTimeCheck = skill2419CoolTime;
            this.AnimationPlay("Attack A");
            this.movingSpan = 1.5f;
            return 2419;
        }
        else if (skill2420CoolTimeCheck < 0)
        {
            skill2420CoolTimeCheck = skill2420CoolTime;
            this.AnimationPlay("Attack B");
            this.movingSpan = 1.5f;
            return 2420;
        }
        else if (skill2422CoolTimeCheck < 0)
        {
            skill2422CoolTimeCheck = skill2422CoolTime;
            this.AnimationPlay("Attack A");
            this.movingSpan = 1.5f;
            return 2422;
        }
        else return -1;
    }

    public override int MonsterSituationMove2(eDirection dir, float magnititude, Vector2Int playerLocation)
    {
        if (magnititude < 10)
        {
            this.skill2421CoolTimeCheck -= 1;
        }
        if (skill2421CoolTimeCheck < 0)
        {
            skill2421CoolTimeCheck = skill2421CoolTime;
            this.AnimationPlay("Attack D");
            this.movingSpan = 7;
            return 2421;
        }
        else if (this.skill2419CoolTimeCheck < 0)
        {
            skill2419CoolTimeCheck = skill2419CoolTime;
            this.AnimationPlay("Attack A");
            this.movingSpan = 1.5f;
            return 2419;
        }
        else if (skill2420CoolTimeCheck < 0)
        {
            skill2420CoolTimeCheck = skill2420CoolTime;
            this.AnimationPlay("Attack B");
            this.movingSpan = 1.5f;
            return 2420;
        }
        else if (skill2422CoolTimeCheck < 0)
        {
            skill2422CoolTimeCheck = skill2422CoolTime;
            this.AnimationPlay("Attack A");
            this.movingSpan = 1.5f;
            return 2422;
        }
        else return -1;
    }
}
