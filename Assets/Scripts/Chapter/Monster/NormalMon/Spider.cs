using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spider : Monster
{
    public GameObject projectilePrefab;

    public Sprite projectileSprite;

    public Animator modelAnim;

    private float skill2412CoolTime;
    private float skill2412CoolTimeCheck;
    private void Awake()
    {
        this.id = 102;
        this.nAttackDelay = 0.2f;
    }

    public override void MonsterStart() 
    {
        this.skill2412CoolTime = DataManager.instance.dicMonsterSkill[2412].coolTime;
        this.skill2412CoolTimeCheck = this.skill2412CoolTime;
    }
    public override void MonsterUpdate() 
    {
        this.skill2412CoolTimeCheck -= Time.deltaTime;

        if (this.movingSpan < 1f && this.modelAnim.GetInteger("attack") != 0)
        {
            this.modelAnim.SetInteger("attack", 0);
        }
    }

    public override int MonsterSituationMove0(eDirection dir, float magnititude, Vector2Int playerLocation)
    {
        if (this.skill2412CoolTimeCheck < 0)
        {
            this.skill2412CoolTimeCheck = this.skill2412CoolTime;
            this.modelAnim.SetInteger("attack", 1);
            this.movingSpan = 2;
            return 2412;
        }
        return 1;
    }

    public override int MonsterSituationMove1(eDirection dir, float magnititude, Vector2Int playerLocation)
    {

        return 1;
    }

    public override int MonsterSituationMove2(eDirection dir, float magnititude, Vector2Int playerLocation)
    {

        return 1;
    }
}