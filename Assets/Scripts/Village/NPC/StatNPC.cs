using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatNPC : NPCBase
{
    
    void Start()
    {
        npcData = DataManager.instance.dicNPC[13];
    }

}
