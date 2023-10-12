using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowBoom : Monster
{
    public Animator modelAnim;
    private void Awake()
    {
        this.id = 104;
    }

    public override void MonsterStart() { }
    public override void MonsterUpdate() { }

    public override int MonsterSituationMove0(eDirection dir, float magnititude, Vector2Int playerLocation)
    {
        if (magnititude < 1.3f)
        {
            this.modelAnim.SetInteger("attack", 1);
            this.movingSpan = 3;
            return 2423;
        }
        return 0;
    }

    public override int MonsterSituationMove1(eDirection dir, float magnititude, Vector2Int playerLocation)
    {
        return 0;
    }

    public override int MonsterSituationMove2(eDirection dir, float magnititude, Vector2Int playerLocation)
    {
        return 0;
    }
}
