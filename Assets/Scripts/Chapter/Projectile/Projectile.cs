using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum eProjectileDirection
{
    Up = 1 << 7,
    UpRight = 1 << 6,
    Right = 1 << 5,
    DownRight = 1 << 4,
    Down = 1 << 3,
    DownLeft = 1 << 2,
    Left = 1 << 1,
    UpLeft = 1 << 0
}
public class Projectile : MonoBehaviour
{
    public float renewalTime = 9999999;
    public int renewalCnt = 1;
    public eDirection startDir;
    public int target;
    public Vector2Int location;
    public int speed;
    public int damage;
    public Vector2Int finalDir; //발사체의 방향까지 고려한 투사체의 최종 방향
    public bool isBomb;
    public int skillId;
    public bool isRemain;
    public List<Vector2Int> projectileRange;
    public int preHittedObjId;

    public void Init(int skillId, int damage, float instantiateTime, eDirection startDir, int target, Vector2Int location, Vector2Int realDir, float renewalTime, int size = 0)
    {
        this.skillId = skillId;
        if (skillId >= 2400)
        {
            var skillData = DataManager.instance.dicMonsterSkill[skillId];
            this.speed = skillData.speed;
            this.isBomb = skillData.isArea;
        }
        else 
        {
            var skillData = DataManager.instance.dicActiveSkill[skillId];
            this.speed = skillData.speed;
            this.isBomb = skillData.isArea;
            this.isRemain = skillData.isRemain;
        }
        this.damage = damage;
        this.renewalTime = instantiateTime;
        this.startDir = startDir;
        this.target = target;
        this.location = location;
        this.finalDir = realDir;
        this.renewalTime = renewalTime;
        this.renewalCnt = 1;
        this.projectileRange = new List<Vector2Int>();
        for (int i = -size; i <= size; ++i)
        {
            Vector2Int attackCoord1;
            attackCoord1 = this.finalDir * i;
            Vector2Int attackCoord2 = new Vector2Int(Mathf.RoundToInt(attackCoord1.x * Mathf.Cos(Mathf.PI / 2) - attackCoord1.y * Mathf.Sin(Mathf.PI / 2)), Mathf.RoundToInt(attackCoord1.x * Mathf.Sin(Mathf.PI / 2) + attackCoord1.y * Mathf.Cos(Mathf.PI / 2)));
            projectileRange.Add(attackCoord2);
        }
    }
}
