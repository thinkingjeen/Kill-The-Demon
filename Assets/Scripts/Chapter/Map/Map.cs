using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum eMapKind
{
    battle, elite, shop, question, boss
}
public class Map
{
    public int x;
    public int y;
    public int id;
    public eMapKind mapKind;
    public List<int> nextMapIds;

    public Map(int x, int y)
    {
        this.nextMapIds = new List<int>();
        this.x = x;
        this.y = y;
        this.id = 10000 + x * 100 + y;
    }
}
