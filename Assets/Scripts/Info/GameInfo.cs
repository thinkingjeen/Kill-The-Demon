using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInfo : RawInfo
{
    public bool isPlayingSaved; // é�� �����߿� ������ ����Ǿ��� �� true, �ٸ� ��Ȳ���� false
    public int[] skills = new int[4];
    public int weapon;
    public int chapter;
    public int stageId;
    public int rewardSeed;
    public int seedNextCnt;
    public Dictionary<int, Map> chapterMap;
    public int gold;
    public int maxHp;
    public int hp;
}
