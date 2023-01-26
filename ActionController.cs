using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class ActionController : MonoBehaviour
{
    public GameObject gameController;
    
    private int lowAttackCount;
    private int hijyutsuCount;
    public int maxlowAttackCount;
    public int maxhijyutsuCount;

    private bool hijyutsuEnabled;
    private bool stanEnabled;
    private bool highAttackEnabled;

    public GameObject lowAttack;
    public GameObject hijyutsu;
    public GameObject stan1;
    public GameObject stan2;
    public GameObject highAttack;
    public GameObject hissatsu;
    
    public TextMeshProUGUI lowAttackStack;
    public TextMeshProUGUI hijyutsuStack;
    
    public AudioClip soundHijyutsu;
    private AudioSource audioSource;

    public void Initialize()
    {
        lowAttackCount = maxlowAttackCount;
        hijyutsuCount = maxhijyutsuCount;
        hijyutsuEnabled = true;
        stanEnabled = true;
        highAttackEnabled = true;
        InitAppearance();
    }
    
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        Initialize();
    }

    void Update()
    {
        lowAttackStack.text = lowAttackCount.ToString();
        hijyutsuStack.text = hijyutsuCount.ToString();
    }

    private void InitAppearance()
    {
        lowAttack.SetActive(true);
        hijyutsu.SetActive(true);
        stan1.SetActive(false);
        stan2.SetActive(false);
        highAttack.SetActive(false);
        hissatsu.SetActive(true);

        lowAttackStack.text = lowAttackCount.ToString();
        hijyutsuStack.text = hijyutsuCount.ToString();
    }

    public void OnClickedLowAttackBtn()
    {
        if (lowAttackCount <= 0) return;
        if (gameController.GetComponent<GameController>().LowAttack() == 1) return;
        lowAttackCount--;
    }

    public void OnClickedHijyutsuBtn()
    {
        if (hijyutsuCount <= 0) return;
        if (!hijyutsuEnabled) return;
        hijyutsuCount--;
        hijyutsuEnabled = false;
        hijyutsu.SetActive(false);
        if (stanEnabled) stan1.SetActive(true);
        if (highAttackEnabled) highAttack.SetActive(true);
        audioSource.PlayOneShot(soundHijyutsu);
    }

    public void OnClickedStan1Btn()
    {
        if (gameController.GetComponent<GameController>().StunAttack() == 1) return;
        stan1.SetActive(false);
        highAttack.SetActive(false);
        stan2.SetActive(true);
    }

    public void OnClickedStan2Btn()
    {
        if (gameController.GetComponent<GameController>().StunAttack() == 1) return;
        stan2.SetActive(false);
        highAttack.SetActive(false);
        hijyutsu.SetActive(true);
        stanEnabled = false;
        highAttackEnabled = true;
        hijyutsuEnabled = true;
    }

    public void OnClickedHighAttackBtn()
    {
        if (gameController.GetComponent<GameController>().HighAttack() == 1) return;
        highAttack.SetActive(false);
        stan1.SetActive(false);
        stan2.SetActive(false);
        hijyutsu.SetActive(true);
        highAttackEnabled = false;
        stanEnabled = true;
        hijyutsuEnabled = true;
    }

    public void OnClickedHissatsuBtn()
    {
        if (gameController.GetComponent<GameController>().Hissatsu() == 1) return;
    }
}
