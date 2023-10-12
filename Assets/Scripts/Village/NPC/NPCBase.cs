using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class NPCBase : MonoBehaviour
{
    public Vector2Int loacation;
    public NpcData npcData;
    void Start()
    {
        
    }

    /// <summary>
    /// npc id�� �ش��ϴ� ��� ��ȭ ��ũ��Ʈ �ܼ� â�� ���
    /// </summary>
    public void PrintDialogueScripts()
    {
        Debug.Log("Print scripts");
        Debug.LogFormat("id : {0} | name : {1}", npcData.id,npcData.name);

        IEnumerable<string> dialog =
            from KeyValuePair<int, NpcScriptData> pair in DataManager.instance.dicScript
            where pair.Value.npcId == npcData.id
            select pair.Value.script;

        foreach (string s in dialog)
        {
            Debug.Log(s);
        }
    }
}
