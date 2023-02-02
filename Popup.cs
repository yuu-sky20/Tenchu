using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using TMPro;

public class Popup : MonoBehaviour
{
    public GameObject panel;
    public TextMeshProUGUI popupText;

    public void ClosePopupWindow()
    {
        panel.SetActive(false);
        popupText.text = "";
    }

    public void OpenPopupWindow(GameObject gameObject)
    {
        panel.SetActive(true);
        string discription = "";
        popupText.fontSize = 12;
        switch (gameObject.name)
        {
            case "lowAttack":
                discription = "相手に15%のダメージを与える";
                break;
            case "hijyutsu":
                discription = "自身に忍術を付与する";
                break;
            case "stan1":
            case "stan2":
                discription = "25%のダメージを与え、5秒間敵をスタンさせる";
                break;    
            case "highAttack":
                discription = "相手に40%のダメージを与える";
                break;
            case "hissatsu":
                discription = "HP50%以下の敵を戦闘不能状態にする";
                break;
            default:
                break;
        }
        popupText.text = discription;
    }
}
