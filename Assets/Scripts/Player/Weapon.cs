using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Weapon : MonoBehaviour
{
    public bool AttackStart = false;
    public UnityAction onAttackAnimCompleteAction;

    public void AnimAttackStart()
    {
        this.AttackStart = true;
    }

    public void AnimAttackComplete()
    {
        onAttackAnimCompleteAction();
    }
}
