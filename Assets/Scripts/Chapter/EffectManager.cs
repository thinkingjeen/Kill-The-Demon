using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EffectManager : MonoBehaviour
{
    public GameObject[] effectPrefabs;
    public GameObject playerGo;

    public void PlayEffect(int index, Vector3 position, float waitTime = 0)
    {
        StartCoroutine(PlayEffectRoutine(index, position, waitTime));
    }

    private IEnumerator PlayEffectRoutine(int index, Vector3 position, float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        GameObject effectGo = Instantiate(effectPrefabs[index], playerGo.transform);
        effectGo.transform.position = position;
        if (!playerGo.GetComponent<Player>().rightDirection && index != 2)
        {
            effectGo.transform.localScale = new Vector3(effectGo.transform.localScale.x, -effectGo.transform.localScale.y, effectGo.transform.localScale.z);
        }
        yield return null;
        yield return new WaitForSeconds(effectGo.GetComponent<ParticleSystem>().main.duration);
        Destroy(effectGo);
        yield return null;
    }

    public GameObject ContinuePlayEffect(int index, Vector3 position)
    {
        GameObject effectGo = Instantiate(effectPrefabs[index], playerGo.transform);
        effectGo.transform.position = position;
        if (!playerGo.GetComponent<Player>().rightDirection)
        {
            effectGo.transform.localScale = new Vector3(effectGo.transform.localScale.x, -effectGo.transform.localScale.y, effectGo.transform.localScale.z);
        }
        return effectGo;
    }

    public GameObject ContinueIndependentPlayEffect(int index, Vector3 position)
    {
        GameObject effectGo = Instantiate(effectPrefabs[index]);
        effectGo.transform.position = position;
        return effectGo;
    }

    public void IndependentEffect(int index, Vector3 position,float during = -1)
    {
        StartCoroutine(IndependentEffectRoutine(index, position, during));    
    }

    private IEnumerator IndependentEffectRoutine(int index, Vector3 position,float during)
    {
        GameObject effectGo = Instantiate(effectPrefabs[index]);
        effectGo.transform.position = position;
        yield return null;
        if (during == -1)
        {
            yield return new WaitForSeconds(effectGo.GetComponent<ParticleSystem>().main.duration);
        }
        else
        {
            yield return new WaitForSeconds(during);
        }
        Destroy(effectGo);
        yield return null;
    }
}