using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using DragonBones;
using DG.Tweening;
using System.Runtime.InteropServices.WindowsRuntime;

public class Chapter1Boss : Monster
{
    //UnityArmatureComponent modelGO;

    const string ANIM_IDLE = "Idle";
    const string ANIM_ATTACK_A = "Attack A";
    const string ANIM_ATTACK_B = "Attack B";
    const string ANIM_ATTACK_C = "Attack C";
    const string ANIM_ATTACK_D = "Attack D";
    const string ANIM_TELEPORT = "Damage";
    
    //2601 A 2602 B 2603 B 2604 C 2605 D
    float[] skillCoolTimes = new float[5];
    float[] coolTimeCheck = new float[5];
    bool isSkillCasting = false;
    int skillNo = -1;

    public Material mat;

   // public UnityEngine.Transform model;

    Coroutine coroutine;
    public override void MonsterStart()
    {
        modelGO.animation.Play(ANIM_IDLE, 0);
        modelGO.animation.timeScale = 1f;

        dir = eDirection.left;

        for(int i = 0; i < skillCoolTimes.Length; i++)
        {
            skillCoolTimes[i] = DataManager.instance.dicMonsterSkill[2601 + i].coolTime;
            coolTimeCheck[i] = 4;
        }
        mat.color = Color.white;
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
                CastSkill(skillNo);
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
    void CastSkill(int index)
    {
        switch (index)
        {
            case 0:
                coroutine = StartCoroutine(Pattern_1());
                break;
            case 1:
            case 2:
            case 3:
            case 4:
                onMonsterGenerateMoveAction();
                break;
        }
    }
    int ReturnResult(int index, Vector2Int playerLocation)
    {
        
        int result = -1;
        int near = PlayerNearCheck(playerLocation);
        int line = PlayerLineCheck(playerLocation);
        switch (index)
        {
            case 0:
                result = 10;
                break;
            case 1:
                if (near == 0)
                {
                    if (isSkillCasting)
                    {
                        isSkillCasting = false;
                        return -1;
                    }
                    //SkillCastWait();
                    //break;
                }
                else if (near == -1) Flip(false);
                else if (near == 1) Flip(true);

                if (coroutine == null) coroutine = StartCoroutine(Pattern_2());
                else result = 11; 
                break;
            case 2:
                if (coroutine != null) break;
                if (near != 0)
                {
                    isSkillCasting = false;
                    return -1;
                }
                else 
                {
                    result = 12;
                }
                break;
            case 3:
            case 4:
                if (coroutine != null) break;
                if (line == 1)
                {
                    Flip(true);
                    result = 10 + index;
                }
                else if (line == -1)
                {
                    Flip(false);
                    result = 10 + index;
                }
                else SkillCastWait();
                break;
        }
        return result;
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
        if (Mathf.Abs(yDistnace) > 4 || Mathf.Abs(xDistance) > 4) return 0;
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
        if (0 <= yDistance && yDistance <= 2)
        {
            xDistance = playerLocation.x - location.x;
            if (xDistance >= 3) result = 1;
            else if (xDistance <= -3) result = -1;
        }
        return result;
    }

    IEnumerator Pattern_1()
    {
        DragonBones.AnimationState animState = modelGO.animation.Play(ANIM_ATTACK_A, 1);
        yield return new WaitForSeconds(0.7f);
        
        int count = 0;
        float timeCheck = 0;
        float attackInterval = 0.5f;
        while (count < 8)
        {
            timeCheck += Time.deltaTime;
            if (timeCheck >= attackInterval)
            {
                if (count == 2)
                {
                    modelGO.animation.Play(ANIM_IDLE, 0);
                }
                onMonsterGenerateMoveAction();
                timeCheck = 0;
                count++;
            }
            yield return null;
        }

        yield return new WaitForSeconds(1f);
        SkillCastEnd();
    }

    IEnumerator Pattern_2()
    {
        modelGO.animation.Play(ANIM_ATTACK_B, 1);
        yield return new WaitForSeconds(0.33f);
        modelGO.animation.timeScale = 0;
        onMonsterGenerateMoveAction();
        yield return new WaitForSeconds(1f);
        modelGO.animation.timeScale = 1;
        yield return new WaitForSeconds(1f);
        modelGO.animation.Play(ANIM_IDLE, 0);
        SkillCastEnd();
    }

    public void Pattern_3(UnityAction fadeOutCallback, UnityAction fadeInCallback)
    {
        coroutine = StartCoroutine(Pattern_3Routine(fadeOutCallback, fadeInCallback));
    }
    IEnumerator Pattern_3Routine(UnityAction fadeOutCallback, UnityAction fadeInCallback)
    {
        DragonBones.AnimationState state = modelGO.animation.Play(ANIM_TELEPORT, 1);
        modelGO.animation.timeScale = 0.5f;
        float fadeTime = state.totalTime / modelGO.animation.timeScale; // 2sec
        float t = fadeTime;
        while (!state.isCompleted)
        {
            float alpha = t / fadeTime;
            mat.color = new Color(1, 1, 1, alpha);
            t -= Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(1f);

        fadeOutCallback();

        state = modelGO.animation.Play(ANIM_TELEPORT, 1);
        modelGO.animation.timeScale = -1.5f;
        fadeTime = state.totalTime / Mathf.Abs(modelGO.animation.timeScale);
        while (!state.isCompleted)
        {
            float alpha = t / fadeTime;
            mat.color = new Color(1, 1, 1, alpha);
            t += Time.deltaTime;
            yield return null;
        }

        modelGO.animation.timeScale = 1f;
        state = modelGO.animation.Play(ANIM_ATTACK_B, 1);

        yield return new WaitForSeconds(0.5f);

        fadeInCallback();

        while (!state.isCompleted)
        {
            yield return null;
        }
        modelGO.animation.Play(ANIM_IDLE, 0);
        SkillCastEnd();
    }


    public void Pattern_4(System.Func<int, bool> callback)
    {
        StartCoroutine(Pattern_4Routine(callback));
    }
    IEnumerator Pattern_4Routine(System.Func<int, bool> callback)
    {
        DragonBones.AnimationState state = modelGO.animation.Play(ANIM_ATTACK_C, 1);
        yield return new WaitUntil(() => { return state.isCompleted; });
        modelGO.animation.Play(ANIM_IDLE, 0);
        int x = 0;
        bool pattenEnd = false;
        while (!pattenEnd)
        {
            pattenEnd = callback(x++);
            yield return new WaitForSeconds(0.1f);
        }
        SkillCastEnd();
    }

    public void Pattern_5(System.Func<bool> validCheck, System.Func<bool> callback)
    {
        if (!validCheck()) return;
        coroutine = StartCoroutine(Pattern_5Routine(callback));
    }
    IEnumerator Pattern_5Routine(System.Func<bool> callback)
    {
        DragonBones.AnimationState animState = modelGO.animation.Play(ANIM_ATTACK_D, 1);
        yield return new WaitForSeconds(1.17f);
        Coroutine animRoutine = StartCoroutine(Pattern_5AnimRotine());

        while (true)
        {
            if (callback()) yield return new WaitForSeconds(0.1f);
            else break;
        }

        StopCoroutine(animRoutine);
        modelGO.animation.timeScale = 1;
        yield return new WaitUntil(() => animState.isCompleted);

        modelGO.animation.Play(ANIM_IDLE, 0);
        SkillCastEnd();
    }
    IEnumerator Pattern_5AnimRotine()
    {
        int s = 1;
        while(true)
        {
            yield return new WaitForSeconds(0.66f);
            s = s == 1 ? -1 : 1;
            modelGO.animation.timeScale = s;
        }
    }
    void SkillCastEnd()
    {
        isSkillCasting = false;
        if(skillNo != -1) coolTimeCheck[skillNo] = 0;
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
            modelGO.armature.flipX = true;
            dir = eDirection.right;
        }
        else
        {
            modelGO.armature.flipX = false;
            dir = eDirection.left;
        }
    }
}
