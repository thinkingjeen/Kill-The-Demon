using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DragonBones;
using DG.Tweening;
using UnityEngine.Events;

public class Chapter2Boss : Monster
{
    UnityArmatureComponent anim;

    const string ANIM_IDLE = "Idle";
    const string ANIM_ATTACK_A = "Attack Slash";
    const string ANIM_ATTACK_B = "Shield Charge";
    const string ANIM_ATTACK_C = "Fierce Slash";
    const string ANIM_ATTACK_D = "Attack thrust";
    const string ANIM_ATTACK_E = "Shield Throw";

    //2606 A 2607 B 2608 C 2609 D 2610 2611 E
    float[] skillCoolTimes = new float[5];
    float[] coolTimeCheck = new float[5];
    bool isSkillCasting = false;
    int skillNo = -1;

    public UnityEngine.Transform model;
    Coroutine coroutine;
    public override void MonsterStart()
    {
        anim = model.GetComponent<UnityArmatureComponent>();
        anim.animation.Play(ANIM_IDLE, 0);
        anim.animation.timeScale = 1f;

        dir = eDirection.left;

        for (int i = 0; i < skillCoolTimes.Length; i++)
        {
            skillCoolTimes[i] = DataManager.instance.dicMonsterSkill[2606 + i].coolTime;
        }
    }
    private void Update()
    {
        for (int i = 0; i < 5; i++)
        {
            coolTimeCheck[i] += Time.deltaTime;
        
        }

        if (!isSkillCasting)
        {
            List<int> indexes = new();
            for (int i = 0; i < skillCoolTimes.Length; i++)
            {
                if (coolTimeCheck[i] >= skillCoolTimes[i])
                {
                    indexes.Add(i);
                }
            }

            if (indexes.Count > 0)
            {
                int index = indexes[Random.Range(0, indexes.Count)];
                isSkillCasting = true;
                skillNo = index;
                onMonsterGenerateMoveAction();
            }
        }
    }
    public override void MonsterUpdate() { }

    public override int MonsterSituationMove0(eDirection dir, float magnititude, Vector2Int playerLocation)
    {
        return ReturnResult(skillNo, playerLocation);
    }

    public override int MonsterSituationMove1(eDirection dir, float magnititude, Vector2Int playerLocation)
    {
        return ReturnResult(skillNo, playerLocation);
    }

    public override int MonsterSituationMove2(eDirection dir, float magnititude, Vector2Int playerLocation)
    {
        return ReturnResult(skillNo, playerLocation);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="playerX"></param>
    /// <returns>-1: near left, 0: not near, 1: right near</returns>
    int PlayerNearCheck(Vector2Int playerLocation)
    {
        int xDistance = playerLocation.x - location.x;
        int yDistnace = playerLocation.y - location.y;
        if (yDistnace > 3 || yDistnace < -1 || Mathf.Abs(xDistance) > 5) return 0;
        else
        {
            if (xDistance >= 0) return 1;
            else return -1;
        }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <returns>-1: left, 0: not, 1: right</returns>
    int PlayerLineCheck(Vector2Int playerLocation)
    {
        int result = 0;
        int yDistance = playerLocation.y - location.y;
        int xDistance = 0;
        if (-1 <= yDistance && yDistance <= 4)
        {
            xDistance = playerLocation.x - location.x;
            if (xDistance >= 2) result = 1;
            else if (xDistance <= -2) result = -1;
        }
        return result;
    }
    int ReturnResult(int index, Vector2Int playerLocation)
    {
        int near = PlayerNearCheck(playerLocation);
        int line = PlayerLineCheck(playerLocation);
        int result = -1;
        switch (index)
        {
            case 0:
                result = 15;
                break;
            case 1:
                if (near == 0) SkillCastWait();
                else result = 16;
                break;
            case 2:
                if (line == 0) SkillCastWait();
                else result = 17;
                break;
            case 3:
                if (near == 0) SkillCastWait();
                else result = 18;
                break;
            case 4:
                if (near == 0) SkillCastWait();
                else result = 19;
                break;
        }
        return result;
    }

    public void Skill_1(Vector2Int destination, UnityAction callback)
    {
        coroutine = StartCoroutine(Skill_1Routine(destination, callback));
    }
    IEnumerator Skill_1Routine(Vector2Int destination,UnityAction callback)
    {
        DragonBones.AnimationState state = anim.animation.Play(ANIM_ATTACK_A, 1);
        anim.animation.timeScale = 1;
        yield return new WaitForSeconds(0.5f);
        transform.DOMove((Vector2)destination, 1f);
        yield return new WaitForSeconds(0.1f);
        anim.animation.timeScale = 0;
        yield return new WaitForSeconds(0.5f);
        anim.animation.timeScale = 1f;
        yield return new WaitForSeconds(0.1f);
        callback();
        yield return new WaitUntil(() => state.isCompleted );
        anim.animation.Play(ANIM_IDLE, 0);
        SkillCastEnd();
    }

    public void Skill_2(UnityAction callback)
    {
        coroutine = StartCoroutine(Skill_2Routine(callback));
    }
    IEnumerator Skill_2Routine(UnityAction callback)
    {
        DragonBones.AnimationState state = anim.animation.Play(ANIM_ATTACK_B, 1);
        anim.animation.timeScale = 0.5f;
        yield return new WaitForSeconds(0.6f);
        anim.animation.timeScale = 0f;
        yield return new WaitForSeconds(0.4f);
        anim.animation.timeScale = 1f;
        callback();
        yield return new WaitUntil(() => state.isCompleted );
        anim.animation.Play(ANIM_IDLE, 0);
        SkillCastEnd();
    }

    public void Skill_3(UnityAction callback)
    {
        coroutine = StartCoroutine(Skill_3Routine(callback));
    }
    IEnumerator Skill_3Routine(UnityAction callback)
    {
        DragonBones.AnimationState state = anim.animation.Play(ANIM_ATTACK_C, 1);
        yield return new WaitForSeconds(0.8f);
        callback();
        yield return new WaitUntil(() => state.isCompleted);
        anim.animation.Play(ANIM_IDLE, 0);
        SkillCastEnd();
    }

    public void Skill_4(UnityAction callback)
    {
        coroutine = StartCoroutine(Skill_4Routine(callback));
    }
    IEnumerator Skill_4Routine(UnityAction callback)
    {
        DragonBones.AnimationState state = anim.animation.Play(ANIM_ATTACK_D, 1);
        yield return new WaitForSeconds(0.3f);
        anim.animation.timeScale = 0f;
        yield return new WaitForSeconds(0.35f);
        anim.animation.timeScale = 1f;
        yield return new WaitForSeconds(0.15f);
        callback();
        yield return new WaitUntil(() => state.isCompleted);
        anim.animation.Play(ANIM_IDLE, 0);
        SkillCastEnd();
    }
    public void Skill_5(UnityAction AttackCallback, UnityAction SubsequentCallback)
    {
        coroutine = StartCoroutine(Skill_5Routine(AttackCallback, SubsequentCallback));
    }
    IEnumerator Skill_5Routine(UnityAction AttackCallback, UnityAction SubsequentCallback)
    {
        DragonBones.AnimationState state = anim.animation.Play(ANIM_ATTACK_E, 1);
        yield return new WaitForSeconds(0.87f);
        anim.animation.timeScale = 0f;
        yield return new WaitForSeconds(0.5f);
        anim.animation.timeScale = 1f;
        yield return new WaitForSeconds(0.13f);
        AttackCallback();
        yield return new WaitUntil(() => state.isCompleted);
        anim.animation.Play(ANIM_IDLE, 0);
        SubsequentCallback();
        yield return new WaitForSeconds(1f);
        SkillCastEnd();
    }
    void SkillCastEnd()
    {
        isSkillCasting = false;
        if (skillNo != -1) coolTimeCheck[skillNo] = 0;
        skillNo = -1;
        coroutine = null;
    }
    void SkillCastWait()
    {
        isSkillCasting = false;
        skillNo = -1;
    }
    public void Flip(bool right)
    {
        if (right)
        {
            anim.armature.flipX = true;
            dir = eDirection.right;
        }
        else
        {
            anim.armature.flipX = false;
            dir = eDirection.left;
        }
    }
}
