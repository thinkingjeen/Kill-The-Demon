using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Alert : MonoBehaviour
{
    private float alertTime;
    private float judgeTime;
    public Monster mon;

    public UnityAction onEndAction;
    public void Init(float judgeTime, Monster mon)
    {
        this.alertTime = judgeTime;
        this.judgeTime = this.alertTime;
        this.mon = mon;
    }

    // Update is called once per frame
    void Update()
    {
        this.judgeTime -= Time.deltaTime;
        if (this.judgeTime < 0)
        {
            this.judgeTime = 0;
            this.onEndAction();
            //Destroy(this.gameObject);
            this.gameObject.SetActive(false);
        }
        this.transform.localScale = new Vector3(1, 1, 1) * (1 + (judgeTime/ alertTime));
    }
}
