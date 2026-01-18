using System;
using System.Collections;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] private Enemy robotOne;
    [SerializeField] private Enemy2 robotTwo;
    
    [SerializeField] private Transform spawnPoint;

    [SerializeField] private EnemyData enemyData;
    [SerializeField] private AudioSource spawnSource;
    [SerializeField] private AudioSO spawnConfig;
    [SerializeField] private Wardrobe wardrobe;


    private System.Random random;
    private double randValue = 0;
    private bool EnemyOneIsOnField = false;
    private bool EnemyTwoIsOnField = false;
    private bool EnemyOnField = false;
    private bool HasStartedRetreating = false;
    private CountDownTimer countDownEnemyOne;
    private LoopTimer enemyCheck;
    

    public bool IsEnabled {get;set;}

    private void Start()
    {
        random = new System.Random();
        IsEnabled = false;
        enemyCheck = new LoopTimer(5,9999);
        enemyCheck.OnLoop += UpdatingEnemyStatus;
        enemyCheck.Start();
        countDownEnemyOne = new CountDownTimer(25);
        robotOne.Despawned += OnRobotOneDespawned;
        robotTwo.Despawned += OnRobotTwoDespawned;
        robotTwo.HidingSpotArrival += RobotTwoHidingSpotArrival;
        wardrobe.OnHide += WardrobeOnHide;
    }

    private void RobotTwoHidingSpotArrival()
    {
        StartCoroutine(OpenAndKill());
    }

    private IEnumerator OpenAndKill()
    {
        wardrobe.Eject();
        yield return new WaitForSeconds(1.5f);

        robotTwo.isAtKillingPos = true;
    }


    private void WardrobeOnHide(bool hidingParam)
    {
        robotTwo.isPlayerHiding = hidingParam;
    }

    private void UpdatingEnemyStatus(int obj)
    {
        if (EnemyOneIsOnField && !HasStartedRetreating)
        {
            countDownEnemyOne.Start();
        }

        RobotRetreatCalc();
    }

    private void OnRobotTwoDespawned()
    {
        EnemyTwoIsOnField = false;
    }

    private void OnRobotOneDespawned()
    {
        EnemyOneIsOnField = false;
    }
    private void RobotRetreatCalc()
    {
        randValue = random.NextDouble();
        if (countDownEnemyOne.IsFinished && randValue < 0.4)
        {
            HasStartedRetreating = true;
            robotOne.isRetreating = true;
        }
    }

    public IEnumerator Spawn()
    {
        //var index = GameManager.Instance.CurrentDays;

        var type = enemyData.Days[5]; // index kell

        switch (type)
        {
            case EnemyType.TypeA:

                if (!EnemyOneIsOnField)
                {
                    AudioManager.Instance.PlaySFX(spawnConfig,spawnSource);
                    
                    robotOne.MoveUp(spawnPoint);
                }

                break;
            case EnemyType.TypeB:

                if (!EnemyTwoIsOnField)
                {
                    AudioManager.Instance.PlaySFX(spawnConfig, spawnSource);
                    robotTwo.MoveUp(spawnPoint);
                }
                break;
            case EnemyType.Both:
                randValue = random.NextDouble();
                if (randValue < 0.002 && !EnemyOnField)
                {
                    
                    if (!EnemyOneIsOnField)
                    {
                        AudioManager.Instance.PlaySFX(spawnConfig, spawnSource);
                        robotOne.MoveUp(spawnPoint);
                        EnemyOnField = true;
                    }
                    yield return new WaitForSeconds(15);
                    if (!EnemyTwoIsOnField)
                    {
                        AudioManager.Instance.PlaySFX(spawnConfig, spawnSource);
                        robotTwo.MoveUp(spawnPoint);
                        EnemyOnField = true;
                    }
                }
                else if (randValue > 0.002 && randValue < 0.5)
                {
                    if (!EnemyOneIsOnField)
                    {
                        AudioManager.Instance.PlaySFX(spawnConfig, spawnSource);
                        robotOne.MoveUp(spawnPoint);
                        EnemyOnField = true;
                    }
                }
                else if (randValue > 0.5)
                {
                    if (!EnemyTwoIsOnField)
                    {
                        AudioManager.Instance.PlaySFX(spawnConfig, spawnSource);
                        robotTwo.MoveUp(spawnPoint);
                        EnemyOnField = true;
                    }
                }
                else
                {
                    if(EnemyOneIsOnField)
                    {
                        AudioManager.Instance.PlaySFX(spawnConfig, spawnSource);
                        robotTwo.MoveUp(spawnPoint);
                    }
                    else
                    {
                        AudioManager.Instance.PlaySFX(spawnConfig, spawnSource);
                        robotOne.MoveUp(spawnPoint);
                    }
                }
                break;
        }
            

    }
}

