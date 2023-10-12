using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Daidara : Monster
{
    private float skill2409CoolTime;

    private float skill2409CoolTimeCheck;

    private void Awake()
    {
        this.id = 302;
        this.sizeX = 2;
        this.sizeY = 2;
    }

    public override void MonsterStart()
    {
        this.modelGO.animation.Play("Idle", 0);
        this.modelGO.animationName = "Idle";

        this.skill2409CoolTime = DataManager.instance.dicMonsterSkill[2406].coolTime;
        this.skill2409CoolTimeCheck = this.skill2409CoolTime;
        this.nAttackDelay = 0.5f;
        //2409°¡ Àü¹æ Æø¹ß
    }

    public override void MonsterUpdate()
    {
        this.skill2409CoolTimeCheck -= Time.deltaTime;

        if (this.modelGO.animationName != "Idle" && this.movingSpan < 0.1f)
        {
            this.modelGO.animation.Play("Idle", 0);
            this.modelGO.animation.timeScale = 1;
            this.modelGO.animationName = "Idle";
        }
    }

    public override int MonsterSituationMove0(eDirection dir, float magnititude, Vector2Int playerLocation)
    {
        if (skill2409CoolTimeCheck < 0)
        {
            skill2409CoolTimeCheck = skill2409CoolTime;
            this.movingSpan = DataManager.instance.dicMonsterSkill[2409].castDelay;
            this.AnimationPlay("Attack");
            this.movingSpan = 3;
            return 2409;
        }
        else if (magnititude < 1.3f)
        {
            this.AnimationPlay("Skill");
            this.movingSpan = 1.5f;
            return 2400;
        }
        else return 0;
    }

    public override int MonsterSituationMove1(eDirection dir, float magnititude, Vector2Int playerLocation)
    {
        if (skill2409CoolTimeCheck < 0)
        {
            if (magnititude < 3)
            {
                skill2409CoolTimeCheck = skill2409CoolTime;
                this.movingSpan = DataManager.instance.dicMonsterSkill[2409].castDelay;
                this.AnimationPlay("Attack");
                this.movingSpan = 3;
                return 2409;
            }
            else return 0;
        }
        else return 0;
    }

    public override int MonsterSituationMove2(eDirection dir, float magnititude, Vector2Int playerLocation)
    {
        return 0;
    }
}
