using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UIResultPopUp : MonoBehaviour
{
    public RectTransform gameover;
    public Text resultText;
    public Text resultText2;
    public Button btnBackToVillage;
    
    void Start()
    {
        btnBackToVillage.onClick.AddListener(() => {
            App.instance.YesAudio();
            App.instance.LoadVillageScene(); 
        });
    }
}
