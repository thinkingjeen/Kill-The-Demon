using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecordInfo : RawInfo
{
    public int NormalMonsters;
    public int EliteMonsters;
    public int BossMonsters;
    public int winCombat;
    public int ClearChapters;

    public int Moves;
    public int Death;

    public int Gold;
    public int CompleteGames;

    public int[] previous;

    public RecordInfo()
    {
        NormalMonsters = 0;
        EliteMonsters = 0;
        ClearChapters = 0;
        Moves = 0;
        Death = 0;
        Gold = 0;
        CompleteGames = 0;
    }

    public void SavePreviousStatus()
    {
        previous = new int[5];
        previous[0] = NormalMonsters;
        previous[1] = EliteMonsters;
        previous[2] = BossMonsters;
        previous[3] = winCombat;
        previous[4] = Gold;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns>[0]: normal monsters, [1]: elite monsters, [2]: boss monsters,[3]: win combat, [4]: gold</returns>
    public int[] GetPreviousStatus()
    {
        return previous;
    }
    public int[] GetCurrentStatus()
    {
        int[] current = new int[5];
        current[0] = NormalMonsters;
        current[1] = EliteMonsters;
        current[2] = BossMonsters;
        current[3] = winCombat;
        current[4] = Gold;
        return current;
    }

}
