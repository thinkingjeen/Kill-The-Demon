using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DragonBones;
using DG.Tweening;

public class Test : MonoBehaviour
{
    UnityArmatureComponent anim;
    float t = 0;
    public UnityEngine.Transform model;

    List<Vector2Int> square3x3 = new();
    List<Vector2Int> square5x5 = new();
    void Start()
    {
        anim = model.GetComponent<UnityArmatureComponent>();
        Skill_1();
    }
    public void Skill_1()
    {
        StartCoroutine(a());
    }
    IEnumerator a()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            anim.armature.flipX = !anim.armature.flipX;
            //anim.animation.timeScale = anim.animation.timeScale == 1f ? 0.5f : 1f;
        }
    }

}
