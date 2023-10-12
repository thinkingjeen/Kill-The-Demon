using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DragonBones;
using DG.Tweening;

public class Chapter3Boss : Monster
{
    UnityArmatureComponent anim;
    Animator dieAnim;

    const string ANIM_IDLE = "Idle";
    const string ANIM_ATTACK_A = "Attack A";
    const string ANIM_ATTACK_B = "Attack B";
    const string ANIM_ATTACK_C = "Attack C";
    const string ANIM_ATTACK_D = "Attack D";

    //2612 2613 2614 2615
    float[] skillCoolTimes = new float[4];
    float[] coolTimeCheck = new float[4];
    bool isSkillCasting = false;
    int skillNo = -1;

    public UnityEngine.Transform model;
    Coroutine coroutine;
    public override void MonsterStart()
    {
        anim = model.GetComponent<UnityArmatureComponent>();
        anim.animation.Play(ANIM_IDLE, 0);
        anim.animation.timeScale = 1f;

        dieAnim = GetComponent<Animator>();
        dir = eDirection.left;

        for (int i = 0; i < skillCoolTimes.Length; i++)
        {
            skillCoolTimes[i] = DataManager.instance.dicMonsterSkill[2612 + i].coolTime;
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
            case 0:
                result = 20;
                break;
            case 1:
                result = 21;
                break;
            case 2:
                result = 22;
                break;
            case 3:
                result = 23;
                break;
        }
        return result;
    }

    public void Skill_1(System.Func<bool> moveCallback, UnityAction attackCallback, UnityAction moveReturnCallback)
    {
        coroutine = StartCoroutine(Skill_1Routine(moveCallback , attackCallback, moveReturnCallback));
    }
    IEnumerator Skill_1Routine(System.Func<bool> moveCallback, UnityAction attackCallback, UnityAction moveReturnCallback)
    {
        DragonBones.AnimationState state = anim.animation.Play(ANIM_ATTACK_A, 1);
        anim.animation.timeScale = 0.2f;
        yield return new WaitForSeconds(1.15f);
        bool moveReturn = moveCallback();
        yield return new WaitForSeconds(0.5f);
        anim.animation.timeScale = 1;
        yield return new WaitForSeconds(0.15f);
        attackCallback?.Invoke();
        anim.animation.timeScale = -1.5f;
        yield return new WaitForSeconds(0.1f);
        anim.animation.timeScale = 1;
        yield return new WaitForSeconds(0.33f);
        attackCallback?.Invoke();
        yield return new WaitUntil(() => state.isCompleted);
        anim.animation.Play("Idle", 0);
        if (moveReturn)
        {
            moveReturnCallback();
            //bool moving = true;
            //transform.DOMove(currentPosition, 0.5f).onComplete = () => moving = false;
            yield return new WaitForSeconds(0.5f);
            //while (moving) yield return null;
        }
        SkillCastEnd();
    }
    public void Skill_2(System.Func<bool> moveCallback, UnityAction attackCallback, UnityAction moveReturnCallback)
    {
        coroutine = StartCoroutine(Skill_2Routine(moveCallback, attackCallback, moveReturnCallback));
    }
    IEnumerator Skill_2Routine(System.Func<bool> moveCallback, UnityAction attackCallback, UnityAction moveReturnCallback)
    {
        DragonBones.AnimationState state = anim.animation.Play(ANIM_ATTACK_B, 1);
        anim.animation.timeScale = 0.2f;
        yield return new WaitForSeconds(1.2f);
        Vector3 currentPosition = transform.position;
        bool moveReturn = moveCallback();
        yield return new WaitForSeconds(0.5f);
        anim.animation.timeScale = 1;
        yield return new WaitForSeconds(0.33f);
        attackCallback?.Invoke();
        yield return new WaitUntil(() => state.isCompleted);
        anim.animation.Play("Idle", 0);
        if (moveReturn)
        {
            moveReturnCallback();
            yield return new WaitForSeconds(0.5f);
            /*bool moving = true;
            transform.DOMove(currentPosition, 0.5f).onComplete = () => moving = false;
            while (moving) yield return null;*/
        }
        SkillCastEnd();
    }
    public void Skill_3(UnityAction callback)
    {
        StartCoroutine(Skill_3Routine(callback));
    }
    IEnumerator Skill_3Routine(UnityAction callback)
    {
        DragonBones.AnimationState state = anim.animation.Play(ANIM_ATTACK_C, 1);
        anim.animation.timeScale = 0.2f;
        yield return new WaitForSeconds(1.63f);
        anim.animation.timeScale = 1;
        yield return new WaitForSeconds(0.33f);
        for (int i = 0; i < 6; i++)
        {
            callback();
            yield return new WaitForSeconds(0.8f);
            if (i == 0)
            {
                anim.animation.Play("Idle", 0);
                SkillCastEnd();
            }
        }
    }
    public void Skill_4(System.Func<int, Vector2> getDestination, UnityAction<int> attackCallback, UnityAction skillEndCallback)
    {
        coroutine = StartCoroutine(Skill_4Routine(getDestination, attackCallback, skillEndCallback));
    }
    IEnumerator Skill_4Routine(System.Func<int, Vector2> getDestination, UnityAction<int> attackCallback, UnityAction skillEndCallback)
    {
        DragonBones.AnimationState state = anim.animation.Play(ANIM_ATTACK_D, 1);
        anim.animation.timeScale = 0.2f;
        yield return new WaitForSeconds(1.3f);
        
        for (int i = 0; i < 4; i++)
        {
            Vector2 destination;
            if (i == 0)
            {
                destination = new(getDestination(i).x, transform.position.y);
            }
            else
            {
                destination = getDestination(i);
                transform.DOMoveY(destination.y, 0);
            }

            yield return new WaitForSeconds(0.5f);
            if (i == 0)
            {
                anim.animation.timeScale = 1f;
                StartCoroutine(Skill_4AnimRoutnie());
            }
            attackCallback(i);
            transform.DOMove(destination, 0.5f).SetEase(Ease.Linear).onComplete = () => {
                if (dir == eDirection.left) Flip(true);
                else if (dir == eDirection.right) Flip(false);
            };

            yield return new WaitForSeconds(1f);
        }
        anim.animation.timeScale = 1f;
        yield return new WaitUntil(() => state.isCompleted);
        anim.animation.Play("Idle", 0);
        skillEndCallback();
        yield return new WaitForSeconds(0.5f);
        SkillCastEnd();
    }
    IEnumerator Skill_4AnimRoutnie()
    {
        yield return new WaitForSeconds(0.3f);
        anim.animation.timeScale = 0f;
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

    public void Die(UnityAction callback)
    {
        Debug.Log("chpater 3 phase 1 die");
        StopAllCoroutines();
        isSkillCasting = true;
        anim.animation.Stop();
        anim.animation.Play(ANIM_IDLE, 0);
        StartCoroutine(DieCoroutine(callback));
    }
    IEnumerator DieCoroutine(UnityAction callback)
    {
        dieAnim.enabled = true;
        yield return new WaitForSeconds(4f);
        Debug.Log("phase 1 die callback");
        callback();
    }
}
