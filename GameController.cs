using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
using Random = System.Random;

interface IEnemy
{
    public int GetPivotHP();
    public int GetHP();
    public void SetHP(int afterHP);
    public bool IsStun();
    public void Stun();
    public int GetMutexStun();
    public void AwakeStun();
    public void Damage(int damage);
    public abstract int InitHP();
}

class Enemy : IEnemy
{
    private int hp;
    private int pivotHP;
    private bool isStun;
    private int mutexStun;

    public Enemy()
    {
        Random r = new Random();
        hp = InitHP();
        pivotHP = hp;
        isStun = false;
        mutexStun = 0;
    }

    public int GetPivotHP()
    {
        return pivotHP;
    }

    public int GetHP()
    {
        return hp;
    }

    public void SetHP(int afterHP)
    {
        hp = afterHP;
    }

    public bool IsStun()
    {
        return isStun;
    }
    
    public void Stun()
    {
        isStun = true;
        mutexStun++;
    }

    public void AwakeStun()
    {
        isStun = false;
        mutexStun--;
    }

    public int GetMutexStun()
    {
        return mutexStun;
    }

    public void Damage(int damage)
    {
        if (hp <= damage)
        {
            hp = 0;
        }
        else
        {
            hp -= damage;
        }
    }

    public virtual int InitHP()
    {
        return 100;
    }
}

class DPS : Enemy
{
    public DPS() : base(){}
    public override int InitHP()
    {
        Random r = new Random();
        return r.Next(40, 80);
    }
}

class Tank : Enemy
{
    public Tank() : base(){}

    public override int InitHP()
    {
        Random r = new Random();
        return r.Next(40, 70);
    }
}

class Healer : Enemy
{
    public Healer() : base(){}

    public override int InitHP()
    {
        Random r = new Random();
        return r.Next(40, 71);
    }
}

public class GameController : MonoBehaviour
{
    public GameObject[] enemies;
    private IEnemy[] enemiesData = new IEnemy[10];
    public Sprite[] sprites;
    public TextMeshProUGUI killedText;
    private int killedCount;

    private int targetIndex;
    private IEnemy targetEnemy;
    
    public AudioClip soundDamage1;
    public AudioClip soundDamage2;
    public AudioClip soundStan;
    public AudioClip soundKilled;
    public AudioClip soundFailed;
    private AudioSource audioSource;
    
    private bool isGameEnd = false;
    public GameObject retryMessage;

    public GameObject actionController;

    private void Initialize()
    {
        SetEnemy();
        SetHPSlider();
        killedText.text = "";
        killedCount = 0;
        retryMessage.SetActive(false);
        targetEnemy = enemiesData[targetIndex];
        actionController.SendMessage("Initialize");
    }

    private void Awake()
    {
        targetIndex = 0;
        Initialize();
        StartCoroutine(MoveHealerHP());
    }

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }
    
    private void Update()
    {
        if (isGameEnd)
        {
            retryMessage.SetActive(true);
        }
        else
        {
            retryMessage.SetActive(false);
        }
    }

    private void FixedUpdate()
    {
        if (!isGameEnd)
        {
            SetHPSlider();
        }
        if (0 < killedCount) killedText.text = killedCount + " KILLED!";
        if (5 <= killedCount) GameClear();
    }

    private void SetEnemy()
    {
        for (int i = 0; i < enemies.Length; i++)
        {
            Random r = new Random();
            int type = r.Next(0, 3);
            Image image = enemies[i].transform.Find("Image").GetComponent<Image>();
            switch (type)
            {
                case 0:
                    enemiesData[i] = new DPS();
                    image.sprite = sprites[0];
                    break;
                case 1:
                    enemiesData[i] = new Tank();
                    image.sprite = sprites[1];
                    break;
                case 2:
                    enemiesData[i] = new Healer();
                    image.sprite = sprites[2];
                    break;
                default:
                    enemiesData[i] = new Enemy();
                    image.sprite = sprites[0];
                    break;
            }
        }
    }

    private IEnumerator MoveHealerHP()
    {
        while (true)
        {
            if (!isGameEnd)
            {
                for (int i = 0; i < enemiesData.Length; i++)
                {
                    if (enemiesData[i] != null)
                    {
                        if (0 < enemiesData[i].GetHP())
                        {
                            if (!enemiesData[i].IsStun())
                            {
                                if (enemiesData[i].GetType().ToString() == "Healer")
                                {
                                    MoveHP(enemiesData[i].GetPivotHP(), i);
                                }
                            }
                        }
                    }
                }
                yield return new WaitForSeconds(3);
            }

            yield return null;
        }
    }
    private void MoveHP(int pivotHP, int i)
    {
        Random r = new Random(); 
        int hp = pivotHP + r.Next(0, 40) - 20;
        if (hp < 0)
        {
            hp = pivotHP;
        } else if (hp > 100)
        {
            hp = 100;
        }
        enemiesData[i].SetHP(hp);
    }

    private void SetHPSlider()
    {
        for (int i = 0; i < enemies.Length; i++)
        {
            Slider slider = enemies[i].transform.Find("Slider").GetComponent<Slider>();
            TextMeshProUGUI text = enemies[i].transform.Find("Per").GetComponent<TextMeshProUGUI>();
            slider.value = (float)enemiesData[i].GetHP() / 100;
            text.text = enemiesData[i].GetHP() + " %";
        }
    }

    public void SetTargetEnemy(int i)
    {
        targetEnemy = enemiesData[i];
        targetIndex = i;
    }

    public int LowAttack()
    {
        if (CheckTargetKilled() == 1) return 1;
        CalcDamage(15);
        if (CheckTargetKilled() == 1)
        {
            killedCount++;
            audioSource.PlayOneShot(soundKilled);
        }
        else
        {
            audioSource.PlayOneShot(soundDamage1);
        }
        return 0;
    }

    public int HighAttack()
    {
        if (CheckTargetKilled() == 1) return 1;
        CalcDamage(40);
        if (CheckTargetKilled() == 1)
        {
            killedCount++;
            audioSource.PlayOneShot(soundKilled);
        }
        else
        {
            audioSource.PlayOneShot(soundDamage2);
        }
        return 0;
    }

    public int StunAttack()
    {
        if (CheckTargetKilled() == 1) return 1;
        CalcDamage(25);
        if (CheckTargetKilled() == 1)
        {
            killedCount++;
            audioSource.PlayOneShot(soundKilled);
        }
        else
        {
            audioSource.PlayOneShot(soundStan);
            AsyncStun();
        }
        return 0;
    }

    private async void AsyncStun()
    {
        IEnemy enemy = targetEnemy;
        if (0 == enemy.GetMutexStun())
        {
            enemy.Stun();
        }
        
        await Task.Delay(5000);
        
        if (1 == enemy.GetMutexStun())
        {
            enemy.AwakeStun();
        }
    }

    public int Hissatsu()
    {
        if (CheckTargetKilled() == 1) return 1;
        int hp = targetEnemy.GetHP();
        if (hp <= 50)
        {
            killedCount++;
            audioSource.PlayOneShot(soundKilled);
            targetEnemy.Damage(50);
        }
        else
        {
            audioSource.PlayOneShot(soundFailed);
            GameOver();
        }
        return 0;
    }

    private void CalcDamage(int damage)
    {
        switch (targetEnemy.GetType().ToString())
        {
            case "Tank":
                targetEnemy.Damage((int)(damage * 0.6));
                break;
            case "Healer":
            case "DPS":
                targetEnemy.Damage(damage);
                break;
            default:
                break;
        }
    }

    private int CheckTargetKilled()
    {
        int hp = targetEnemy.GetHP();
        if (hp == 0) return 1;
        return 0;
    }

    private void GameOver()
    {
        TextMeshProUGUI retryText = retryMessage.transform.Find("Text").GetComponent<TextMeshProUGUI>();
        retryText.text = "GameOver.\nRetry?";
        isGameEnd = true;
    }

    private void GameClear()
    {
        
        TextMeshProUGUI retryText = retryMessage.transform.Find("Text").GetComponent<TextMeshProUGUI>();
        retryText.text = "GameClear!\nRetry?";
        isGameEnd = true;
    }

    public void OnClickedRetry()
    {
        isGameEnd = false;
        Initialize();
    }
}
