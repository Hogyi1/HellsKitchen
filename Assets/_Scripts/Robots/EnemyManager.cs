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

    private bool CanSpawnEnemyOne = true;
    private bool CanSpawnEnemyTwo = true;

    public event Action<EnemyType> PlayerDied;

    private void Start()
    {
        random = new System.Random();
        enemyCheck = new LoopTimer(5,9999);
        enemyCheck.OnLoop += UpdatingEnemyStatus;
        enemyCheck.Start();
        countDownEnemyOne = new CountDownTimer(25);
        robotOne.Despawned += OnRobotOneDespawned;
        robotTwo.Despawned += OnRobotTwoDespawned;
        robotTwo.HidingSpotArrival += RobotTwoHidingSpotArrival;
        robotOne.OnDestroy += RobotOneOnDestroy;
        robotTwo.OnDestroy += RobotTwoOnDestroy;
        wardrobe.OnHide += WardrobeOnHide;
        robotOne.OnKillPlayer += RobotOneOnKillPlayer;
        robotTwo.OnKillPlayer += RobotTwoOnKillPlayer;
    }

    private void RobotTwoOnKillPlayer(EnemyType obj)
    {
        PlayerDied?.Invoke(obj);
    }

    private void RobotOneOnKillPlayer(EnemyType obj)
    {
        PlayerDied?.Invoke(obj);
    }

    private void RobotTwoOnDestroy()
    {
        CanSpawnEnemyTwo = false;
    }

    private void RobotOneOnDestroy()
    {
        CanSpawnEnemyOne = false;
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
        robotOne.isPlayerHiding = hidingParam;
        robotTwo.isPlayerHiding = hidingParam;
    }

    private void UpdatingEnemyStatus(int obj)
    {
        if (EnemyOneIsOnField && !HasStartedRetreating && CanSpawnEnemyOne)
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
        if (countDownEnemyOne.IsFinished && randValue < 0.4 && CanSpawnEnemyOne)
        {
            HasStartedRetreating = true;
            robotOne.isRetreating = true;
        }
    }

    public IEnumerator Spawn()
    {
        //var index = GameManager.Instance.CurrentDays;

        var type = enemyData.Days[4];

        switch (type)
        {
            case EnemyType.TypeA:
                Debug.Log("A");
                if (!EnemyOneIsOnField)
                {
                    SpawnEnemyOne();
                }

                break;
            case EnemyType.TypeB:
                Debug.Log("B");
                if (!EnemyTwoIsOnField)
                {
                    SpawnEnemyTwo();
                }
                break;
            case EnemyType.Both:
                randValue = random.NextDouble();
                Debug.Log("C");
                if (randValue < 0.002 && !EnemyOnField)
                {
                   
                    
                    SpawnEnemyOne();
                    
                    yield return new WaitForSeconds(7);
                    
                    SpawnEnemyTwo();
                    
                }
                else if (randValue > 0.002 && randValue <= 0.5)
                {
                    if (!EnemyOneIsOnField)
                    {
                        SpawnEnemyOne();
                    }
                }
                else if (randValue > 0.5)
                {
                    if (!EnemyTwoIsOnField)
                    {
                        SpawnEnemyTwo();
                    }
                }
                break;
        }
            

    }

    private void SpawnEnemyOne()
    {
        if(CanSpawnEnemyOne)
        {
            AudioManager.Instance.PlaySFX(spawnConfig, spawnSource);
            EnemyOneIsOnField = true;
            EnemyOnField = true;
            robotOne.MoveUp(spawnPoint);
        }
    }

    private void SpawnEnemyTwo()
    {
        if (CanSpawnEnemyTwo)
        {
            AudioManager.Instance.PlaySFX(spawnConfig, spawnSource);
            EnemyTwoIsOnField = true;
            EnemyOnField = true;
            robotTwo.MoveUp(spawnPoint);
        }       
    }
}

