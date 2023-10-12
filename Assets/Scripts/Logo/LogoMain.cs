using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class LogoMain : MonoBehaviour
{
    public Image imgLogo;
    public UnityAction onLogoSceneEndAction;
    // Start is called before the first frame update
    private void Awake()
    {
        App.instance.logoMain = this;
    }
    void Start()
    {
        onLogoSceneEndAction();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
