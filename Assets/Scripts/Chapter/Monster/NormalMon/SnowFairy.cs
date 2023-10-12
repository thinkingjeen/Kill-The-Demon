using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowFairy : Monster
{
    public Animator modelAnim;

    private float skill2424CoolTime;
    private float skill2424CoolTimeCheck;
    private void Awake()
    {
        this.id = 106;
    }

    public override void MonsterStart()
    {
        this.skill2424CoolTime = DataManager.instance.dicMonsterSkill[2424].coolTime;
        this.skill2424CoolTimeCheck = this.skill2424CoolTime;
    }
    public override void MonsterUpdate()
    {
        this.skill2424CoolTimeCheck -= Time.deltaTime;

        if (this.movingSpan < 1.2f && this.modelAnim.GetInteger("attack") != 0)
        {
            this.modelAnim.SetInteger("attack", 0);
        }
    }

    public override int MonsterSituationMove0(eDirection dir, float magnititude, Vector2Int playerLocation)
    {
        if (magnititude < 30)
        {
            if (this.skill2424CoolTimeCheck < 0)
            {
                this.skill2424CoolTimeCheck = this.skill2424CoolTime;
                this.modelAnim.SetInteger("attack", 1);
                this.movingSpan = 2;
                return 2424;
            }
            else return 1;
        }
        else return 1;
    }

    public override int MonsterSituationMove1(eDirection dir, float magnititude, Vector2Int playerLocation)
    {
        if (magnititude < 30)
        {
            if (this.skill2424CoolTimeCheck < 0)
            {
                this.skill2424CoolTimeCheck = this.skill2424CoolTime;
                this.modelAnim.SetInteger("attack", 1);
                this.movingSpan = 2;
                return 2424;
            }
            else return 1;
        }
        else return 1;
    }

    public override int MonsterSituationMove2(eDirection dir, float magnititude, Vector2Int playerLocation)
    {
        if (magnititude < 6)
        {
            if (this.skill2424CoolTimeCheck < 0)
            {
                this.skill2424CoolTimeCheck = this.skill2424CoolTime;
                this.modelAnim.SetInteger("attack", 1);
                this.movingSpan = 2;
                return 2424;
            }
            else return 1;
        }
        else return 1;
    }
}
