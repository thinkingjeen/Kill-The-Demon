using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInfo : RawInfo
{
    public bool isPlayingSaved; // 챕터 진행중에 게임이 종료되었을 때 true, 다른 상황에선 false
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
