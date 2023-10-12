using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wolf : Monster
{
    public Animator modelAnim;
    private void Awake()
    {
        this.id = 105;
    }

    public override void MonsterStart()
    {
        this.nAttackDelay = 0.1f;
    }
    public override void MonsterUpdate()
    {
        if (this.movingSpan < 0.2f && this.modelAnim.GetInteger("attack") != 0)
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
            this.movingSpan = 1f;
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
