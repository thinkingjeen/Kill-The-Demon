using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInfo : RawInfo
{
    public int dia;
    /// <summary>
    /// 0: Hp, 1: Damage, 2: GoldDrop, 3: DiaDrop
    /// </summary>
    public int[] stats = new int[4];
    public List<int> unlockWaeponIds = new();
    public List<int> unlockSkillIds = new();
    
    
}
