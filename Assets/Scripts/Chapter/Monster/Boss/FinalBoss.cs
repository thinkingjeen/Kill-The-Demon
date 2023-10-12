using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DragonBones;
using DG.Tweening;

public class FinalBoss : Monster
{
    UnityArmatureComponent anim;
    Animator animator;

    const string ANIM_IDLE = "Idle";
    const string ANIM_ATTACK_A = "Attack A";
    const string ANIM_ATTACK_B = "Attack B";
    const string ANIM_ATTACK_C = "Attack C";
    const string ANIM_ATTACK_D = "Attack D";
    const string ANIM_ATTACK_E = "Attack E";

    //2616 2617 2618 2619 2620
    float[] skillCoolTimes = new float[5];
    float[] coolTimeCheck = new float[5];
    bool isSkillCasting = false;
    int skillNo = -1;

    public UnityEngine.Transform model;
    Coroutine coroutine;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public override void MonsterStart()
    {
        anim = model.GetComponent<UnityArmatureComponent>();
        anim.animation.Play(ANIM_IDLE, 0);
        anim.animation.timeScale = 1f;
            
        dir = eDirection.left;

        for (int i = 0; i < skillCoolTimes.Length; i++)
        {
            skillCoolTimes[i] = DataManager.instance.dicMonsterSkill[2616 + i].coolTime;
        }
    }
    private void Update()
    {
        for (int i = 0; i < skillCoolTimes.Length; i++)
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

    int ReturnResult(int index, Vector2Int playerLocation)
    {
        int result = -1;
        switch (index)
        {
            case 0: result = 24; break;
            case 1: result = 25; break;
            case 2: result = 26; break;
            case 3: result = 27; break;
            case 4: result = 28; break;
        }
        return result;
    }

    public void Apear(UnityAction callback)
    {
        StartCoroutine(ApearCoroutine(callback));
    }
    IEnumerator ApearCoroutine(UnityAction callback)
    {
        animator.enabled = true;
        yield return new WaitForSeconds(4f);
        callback();
    }
    public void Skill_1(System.Func<bool> moveCallback, UnityAction attackCallback)
    {
        StartCoroutine(Skill_1Routine(moveCallback, attackCallback));
    }
    IEnumerator Skill_1Routine(System.Func<bool> moveCallback, UnityAction attackCallback)
    {
        bool movable = moveCallback();//move
        if (movable)
        {
            yield return new WaitForSeconds(0.5f);
            DragonBones.AnimationState state = anim.animation.Play(ANIM_ATTACK_A, 1);
            anim.animation.timeScale = 1;
            yield return new WaitForSeconds(0.4f);

            attackCallback();

            yield return new WaitUntil(() => state.isCompleted);
            anim.animation.Play(ANIM_IDLE, 0);
            SkillCastEnd();
        }
        else
        {
            SkillCastWait();
        }
    }
    public void Skill_2(System.Func<bool> moveCallback, UnityAction attackCallback)
    {
        StartCoroutine(Skill_2Routine(moveCallback, attackCallback));
    }
    IEnumerator Skill_2Routine(System.Func<bool> moveCallback, UnityAction attackCallback)
    {
        bool movable = moveCallback();//move
        if (movable)
        {
            yield return new WaitForSeconds(0.5f);
            DragonBones.AnimationState state = anim.animation.Play(ANIM_ATTACK_B, 1);
            anim.animation.timeScale = 1;
            yield return new WaitForSeconds(0.4f);

            attackCallback();

            yield return new WaitUntil(() => state.isCompleted);
            anim.animation.Play(ANIM_IDLE, 0);
            SkillCastEnd();
        }
        else
        {
            SkillCastWait();
        }
    }
    public void Skill_3(UnityAction callback)
    {
        StartCoroutine(Skill_3Routine(callback));
    }
    IEnumerator Skill_3Routine(UnityAction callback)
    {
        DragonBones.AnimationState state = anim.animation.Play(ANIM_ATTACK_C, 1);
        anim.animation.timeScale = 1f;
        yield return new WaitForSeconds(0.6f);
        StartCoroutine(Skill_3AttackRoutine(callback));
        yield return new WaitUntil(() => state.isCompleted);
        anim.animation.Play(ANIM_IDLE, 0);
        yield return null;
    }
    IEnumerator Skill_3AttackRoutine(UnityAction callback)
    {
        for(int i = 0; i < 3; i++)
        {
            callback();
            yield return new WaitForSeconds(1f);
        }
        SkillCastEnd();
    }
    public void Skill_4(UnityAction moveCallback, UnityAction<int> attackCallback)
    {
        StartCoroutine(Skill_4Routine(moveCallback, attackCallback));
    }
    IEnumerator Skill_4Routine(UnityAction moveCallback, UnityAction<int> attackCallback)
    {
        for (int i = 0; i < 4; i++)
        {
            moveCallback();
            yield return new WaitForSeconds(0.25f);
            anim.animation.Play(ANIM_ATTACK_D, 1);
            anim.animation.timeScale = 1f;
            yield return new WaitForSeconds(0.5f);
            for (int j = 0; j < 4; j++)
            {
                attackCallback(j);
                yield return new WaitForSeconds(0.5f);
            }
        }

        anim.animation.Play(ANIM_IDLE, 0);
        SkillCastEnd();
        yield return null;
    }
    public void Skill_5(UnityAction callback)
    {
        StartCoroutine(Skill_5Routine(callback));
    }
    IEnumerator Skill_5Routine(UnityAction callback)
    {
        DragonBones.AnimationState state = anim.animation.Play(ANIM_ATTACK_E, 1);
        anim.animation.timeScale = 1f;
        yield return new WaitForSeconds(0.5f);
        callback();

        yield return new WaitUntil(() => state.isCompleted);
        anim.animation.Play(ANIM_IDLE, 0);
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
