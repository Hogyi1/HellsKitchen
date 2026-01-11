using System;
using System.Collections;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    //Az enemy-k legyenek a scene-ben csak pálya alatt vagy le akarjuk õket spwanolni egy prefab-bõl?
    //egyenlõre az elõbbit csinálom meg, de változtatni bármikor tudok
    [SerializeField] private Enemy robotOne;
    [SerializeField] private Enemy2 robotTwo;

    [SerializeField] private Transform spawnPoint;

    [SerializeField] private EnemyData enemyData;
    private System.Random random;
    private double randValue = 0;
    private CountDownTimer countDownTimer;
    private bool EnemyOneIsOnField = false;
    private bool EnemyTwoIsOnField = false;
    private bool EnemyOnField = false;

    private CountDownTimer countDownEnemyOne;

    public bool IsEnabled {get;set;}

    private void Start()
    {
        IsEnabled = false;
        countDownTimer = new CountDownTimer(5);
        countDownTimer.Start();
        countDownEnemyOne = new CountDownTimer(20);
        robotOne.Despawned += OnRobotOneDespawned;
        robotTwo.Despawned += OnRobotTwoDespawned;
    }

    private void OnRobotTwoDespawned()
    {
        EnemyTwoIsOnField = false;
    }

    private void OnRobotOneDespawned()
    {
        EnemyOneIsOnField = false;
    }

    private void Update()
    {
        while(IsEnabled && countDownTimer.IsFinished)
        {
            countDownTimer.Start();
            randValue = random.NextDouble();
            if (randValue <= 0.01)
            {
                StartCoroutine(Spawn());
            }


            EnemyCountDownStart();

            RobotRetreatCalc();
           
        }
    }

    private void EnemyCountDownStart()
    {
        if (EnemyOneIsOnField)
        {
            countDownEnemyOne.Start();
        }
    }

    private void RobotRetreatCalc()
    {
        randValue = random.NextDouble();
        if (countDownEnemyOne.IsFinished && randValue < 0.4)
        {
            robotOne.isRetreating = true;
        }
    }

    private IEnumerator Spawn()
    {
        //var index = GameManager.Instance.CurrentDays;

        var type = enemyData.Days[1]; // index kell


        switch (type)
        {
            case EnemyType.TypeA:

                if (!EnemyOneIsOnField)
                {
                    //playsound
                    robotOne.MoveUp(spawnPoint);
                }

                break;
            case EnemyType.TypeB:

                if (!EnemyTwoIsOnField)
                {
                    //playsound
                    robotTwo.MoveUp(spawnPoint);
                }
                break;
            case EnemyType.Both:
                randValue = random.NextDouble();
                if (randValue < 0.002 && !EnemyOnField)
                {
                    
                    if (!EnemyOneIsOnField)
                    {
                        //playsound
                        robotOne.MoveUp(spawnPoint);
                        EnemyOnField = true;
                    }
                    yield return new WaitForSeconds(15);
                    if (!EnemyTwoIsOnField)
                    {
                        //playsound
                        robotTwo.MoveUp(spawnPoint);
                        EnemyOnField = true;
                    }
                }
                else if (randValue > 0.002 && randValue < 0.5)
                {
                    if (!EnemyOneIsOnField)
                    {
                        //playsound
                        robotOne.MoveUp(spawnPoint);
                        EnemyOnField = true;
                    }
                }
                else if (randValue > 0.5)
                {
                    if (!EnemyTwoIsOnField)
                    {
                        //playsound
                        robotTwo.MoveUp(spawnPoint);
                        EnemyOnField = true;
                    }
                }
                else
                {
                    if(EnemyOneIsOnField)
                    {
                        //playsound
                        robotTwo.MoveUp(spawnPoint);
                    }
                    else
                    {
                        //playsound
                        robotOne.MoveUp(spawnPoint);
                    }
                }
                break;
        }
            

    }
}

