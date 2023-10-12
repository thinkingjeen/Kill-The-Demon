using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageRest : StageBase
{
    public Transform bonfirePosition;
    bool isUsed = false;
    public AudioClip healAudio;
    public override void Init(Player player)
    {
        base.Init(player);
        Vector2Int bonfireLocation = Vector2Int.RoundToInt(bonfirePosition.position) - bl;
        tileInfoArray[bonfireLocation.x, bonfireLocation.y].objId = 15;
        tileInfoArray[Mathf.RoundToInt(portal.position.x) - bl.x, Mathf.RoundToInt(portal.position.y) - bl.y].objId = ConstantIDs.OPENED_PORTAL;
    }

    public void PlayerHeal()
    {
        if (!isUsed)
        {
            SoundManager.PlaySFX(this.healAudio);
            Debug.Log("회복");
            InfoManager.instance.gameInfo.hp += (int)(InfoManager.instance.gameInfo.maxHp * 0.2f);
            this.isUsed = true;
            if (InfoManager.instance.gameInfo.hp > InfoManager.instance.gameInfo.maxHp)
            {
                InfoManager.instance.gameInfo.hp = InfoManager.instance.gameInfo.maxHp;
            }
        }
        else
        {
            Debug.Log("안회복");
        }
    }
}
