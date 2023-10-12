using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slime : Monster
{
    public Animator modelAnim;
    private void Awake()
    {
        this.id = 101;
    }

    public override void MonsterStart() 
    {
        this.nAttackDelay = 0.45f;
    }
    public override void MonsterUpdate() 
    {
        if (this.movingSpan < 1.25f && this.modelAnim.GetInteger("attack") != 0)
        {
            this.modelAnim.SetInteger("attack", 0);
        }
    }

    public override int MonsterSituationMove0(eDirection dir, float magnititude, Vector2Int playerLocation)
    {
        this.modelAnim.gameObject.GetComponent<SpriteRenderer>().flipX = (playerLocation.x - this.location.x < 0);
        if (magnititude < 1.3f)
        {
            this.modelAnim.SetInteger("attack", 1);
            this.movingSpan = 2f;
            return 2400;
        }
        return 0;
    }

    public override int MonsterSituationMove1(eDirection dir, float magnititude, Vector2Int playerLocation)
    {
        this.modelAnim.gameObject.GetComponent<SpriteRenderer>().flipX = (playerLocation.x - this.location.x < 0);
        return 0;
    }

    public override int MonsterSituationMove2(eDirection dir, float magnititude, Vector2Int playerLocation)
    {
        this.modelAnim.gameObject.GetComponent<SpriteRenderer>().flipX = (playerLocation.x - this.location.x < 0);
        return 0;
    }
}
