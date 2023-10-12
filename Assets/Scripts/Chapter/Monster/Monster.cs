using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DragonBones;

public abstract class Monster : MonoBehaviour
{
    public Vector2Int location;
    public int id;
    public eDirection dir = eDirection.up;
    public MonsterData monData;
    public int hp;
    public float moveSpan;
    public List<int> skillIds;
    public float nAttackDelay = 0;

    public UnityArmatureComponent modelGO;

    public float movingSpan = 0;
    public System.Action onMonsterGenerateMoveAction;

    public UnityAction onDieAction;
    public UnityAction<int, Vector2> onHitAction;

    public UnityAction onAstarMoveAction;
    public UnityAction onProjectileMonsterMoveAction;
    public Action<float, float, float> onEliteMoveAction;
    public Action<float, float, float, float, float, float> onBossMoveAction;
    public int sizeX = 0;
    public int sizeY = 0;

    public Action<Vector2Int, eDirection, int> onRangeAttackAction; //자기 좌표, 보고 있는 방향, id
    public Action<Vector2Int, eDirection, int> onAlertAttackAction; //자기 좌표, 보고 있는 방향, id
    public Action<Vector2Int,eDirection, GameObject, int> onProjectileAttackAction; //자기 좌표, 보고있는 방향, 사용하는 프리팹, id
    public Action<Vector2Int, int> onMoveAction;//자기 좌표, 이동 거리(돌진 등을 할때)

    private void Start()
    {
        monData = DataManager.instance.dicMonster[id];
        hp = monData.maxHp;
        MonsterStart();
    }

    private void Update()
    {
        this.movingSpan -= Time.deltaTime;
        if (/*this.movingSpan >= this.monData.moveDelay*/this.movingSpan <= 0)
        {
            movingSpan = this.monData.moveDelay;
            onMonsterGenerateMoveAction();
        }
        MonsterUpdate();
    }

    public void Hit(int damage)
    {
        hp -= damage;
        Debug.LogFormat("hp : {0}/{1}", hp, monData.maxHp);
        onHitAction?.Invoke(damage, location);
        if (hp <= 0)
        {
            onDieAction();
            if (id != 403)
            {
                gameObject.SetActive(false);
            }
        }
    }

    public void AnimationPlay(string name)
    {
        this.modelGO.animation.Play(name, 1);
        this.modelGO.animationName = name;
    }

    public int MonsterGenerateMove(int situation, eDirection dir, float magnititude, Vector2Int playerLocation)
    {
        int resultBehavior = 0;
        if (sizeX != 0 && sizeY != 0 && this.id < 400)
        {
            if (playerLocation.x - this.location.x > 0 && !this.modelGO.armature.flipX)
            {
                this.modelGO.armature.flipX = true;
                Debug.LogFormat("flipX : {0}",this.modelGO.armature.flipX);
                this.location += new Vector2Int(this.sizeX - 1, 0);
                Debug.LogFormat("Location Change, Location : {0}", this.location);
                this.sizeX *= -1;
                Debug.LogFormat("SizeX Change, SizeX : {0}", this.sizeX);
            }
            else if (playerLocation.x - this.location.x < 0 && this.modelGO.armature.flipX)
            {
                this.modelGO.armature.flipX = false;
                Debug.LogFormat("flipX : {0}", this.modelGO.armature.flipX);
                this.location += new Vector2Int(this.sizeX + 1, 0);
                Debug.LogFormat("Location Change, Location : {0}", this.location);
                this.sizeX *= -1;
                Debug.LogFormat("SizeX Change, SizeX : {0}", this.sizeX);
            }
        }
        switch (situation)
        {
            case 0: resultBehavior = MonsterSituationMove0(dir, magnititude, playerLocation);
                break;
            case 1: resultBehavior = MonsterSituationMove1(dir, magnititude, playerLocation);
                break;
            case 2: resultBehavior = MonsterSituationMove2(dir, magnititude, playerLocation);
                break;
        }
        return resultBehavior;
    }

    /// <summary>
    /// situation : 0 => 일직선상 장애물X, 1 => 일직선상 장애물O, 2 => 일직선상 X
    /// </summary>
    /// <param name="dir">몬스터 기준으로 플레이어가 위치하고 있는 방향</param>
    /// <param name="magnititude">몬스터와 플레이어 사이거리, 소숫점 첫째자리까지 나옴</param>
    /// <param name="playerLocation"></param>
    /// <returns></returns>
    public abstract int MonsterSituationMove0(eDirection dir, float magnititude, Vector2Int playerLocation);
    /// <summary>
    /// situation : 0 => 일직선상 장애물X, 1 => 일직선상 장애물O, 2 => 일직선상 X
    /// </summary>
    /// <param name="dir">몬스터 기준으로 플레이어가 위치하고 있는 방향</param>
    /// <param name="magnititude">몬스터와 플레이어 사이거리, 소숫점 첫째자리까지 나옴</param>
    /// <param name="playerLocation"></param>
    /// <returns></returns>
    public abstract int MonsterSituationMove1(eDirection dir, float magnititude, Vector2Int playerLocation);
    /// <summary>
    /// situation : 0 => 일직선상 장애물X, 1 => 일직선상 장애물O, 2 => 일직선상 X
    /// </summary>
    /// <param name="dir">몬스터 기준으로 플레이어가 위치하고 있는 방향</param>
    /// <param name="magnititude">몬스터와 플레이어 사이거리, 소숫점 첫째자리까지 나옴</param>
    /// <param name="playerLocation"></param>
    /// <returns></returns>
    public abstract int MonsterSituationMove2(eDirection dir, float magnititude, Vector2Int playerLocation);

    public abstract void MonsterStart();
    public abstract void MonsterUpdate();
}