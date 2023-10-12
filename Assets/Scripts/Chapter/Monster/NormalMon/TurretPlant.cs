using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretPlant : Monster
{
    public Animator modelAnim;

    private float skill2413CoolTime;
    private float skill2413CoolTimeCheck;
    private void Awake()
    {
        this.id = 103;
    }

    public override void MonsterStart()
    {
        this.skill2413CoolTime = DataManager.instance.dicMonsterSkill[2413].coolTime;
        this.skill2413CoolTimeCheck = this.skill2413CoolTime;
    }
    public override void MonsterUpdate()
    {
        this.skill2413CoolTimeCheck -= Time.deltaTime;

        if (this.movingSpan < 0.5f&&this.modelAnim.GetInteger("attack") == 2)
        {
            this.modelAnim.SetInteger("attack", 0);
        }
    }

    public override int MonsterSituationMove0(eDirection dir, float magnititude, Vector2Int playerLocation)
    {
        if (magnititude < 6)
        {
            if (this.skill2413CoolTimeCheck < 0)
            {
                this.skill2413CoolTimeCheck = this.skill2413CoolTime;
                this.modelAnim.SetInteger("attack", 2);
                this.movingSpan = 2;
                return 2413;
            }
            else return -1;
        }
        else return -1;
    }

    public override int MonsterSituationMove1(eDirection dir, float magnititude, Vector2Int playerLocation)
    {
        if (magnititude < 6)
        {
            if (this.skill2413CoolTimeCheck < 0)
            {
                this.skill2413CoolTimeCheck = this.skill2413CoolTime;
                this.modelAnim.SetInteger("attack", 2);
                this.movingSpan = 2;
                return 2413;
            }
            else return -1;
        }
        else return -1;
    }

    public override int MonsterSituationMove2(eDirection dir, float magnititude, Vector2Int playerLocation)
    {
        if (magnititude < 6)
        {
            if (this.skill2413CoolTimeCheck < 0)
            {
                this.skill2413CoolTimeCheck = this.skill2413CoolTime;
                this.modelAnim.SetInteger("attack", 2);
                this.movingSpan = 2;
                return 2413;
            }
            else return -1;
        }
        else return -1;
    }
}