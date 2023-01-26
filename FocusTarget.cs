using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class FocusTarget : MonoBehaviour
{
    public GameObject[] enemies;
    private int focusTarget;
    
    public AudioClip soundSelect;
    private AudioSource audioSource;

    public GameObject gameController;

    private void Start()
    {
        focusTarget = 0;
        UpdateFocusTarget(0);

        audioSource = GetComponent<AudioSource>();
    }


    public void OnClickedImage(int i)
    {
        if (enemies.Length <= i) return;
        focusTarget = i;
        UpdateFocusTarget(i);
        audioSource.PlayOneShot(soundSelect);
    }

    private void UpdateFocusTarget(int x)
    {
        for (int i = 0; i < enemies.Length; i++)
        {
            GameObject focus = enemies[i].transform.Find("Focus").gameObject;
            if (i == x)
            {
                focus.SetActive(true);   
            }
            else
            {
                focus.SetActive(false);
            }
        }
        
        gameController.SendMessage("SetTargetEnemy", focusTarget);
    }
}
