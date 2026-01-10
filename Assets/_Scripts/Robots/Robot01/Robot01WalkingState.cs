using UnityEngine;
using UnityEngine.AI;

public class Robot01WalkingState : Robot01BaseState
{
    private readonly NavMeshAgent agent;
    private readonly Transform[] waypoints;
    private readonly float waitAtPoint;

    private int currentIndex = 0;

    private CountDownTimer waitTimer;

    public Robot01WalkingState(Enemy enemy,Animator animator,NavMeshAgent agent,Transform[] waypoints,float waitAtPoint): base(enemy, animator)
    {
        this.agent = agent;
        this.waypoints = waypoints;
        this.waitAtPoint = waitAtPoint;

        waitTimer = new CountDownTimer(waitAtPoint);
    }

    public override void OnEnter()
    {
        Debug.Log("Walking");
        agent.speed = 1.2f;
        agent.angularSpeed = 150;
        agent.acceleration = 7;
        animator.CrossFade(WalkAnimation, 0.15f);
        currentIndex = GetClosestWaypointIndex();
        agent.SetDestination(waypoints[currentIndex].position);
    }

    private int GetClosestWaypointIndex()
    {
        int closestIndex = 0;
        float closestDistance = float.MaxValue;

        Vector3 enemyPos = enemy.root.position;

        for (int i = 0; i < waypoints.Length; i++)
        {
            float dist = Vector3.Distance(enemyPos, waypoints[i].position);
            if (dist < closestDistance)
            {
                closestDistance = dist;
                closestIndex = i;
            }
        }

        return closestIndex;
    }

    public override void Update()
    {
        if (agent.pathPending) return;

        if (agent.remainingDistance > agent.stoppingDistance + 0.05f)
        {
            waitTimer.Reset();
            return;
        }

        if (!waitTimer.IsRunning && !waitTimer.IsFinished)
            waitTimer.Start();

        waitTimer.Tick();

        if (waitTimer.IsFinished)
        {
            float chance = Random.Range(0f, 1f); 
            if (chance <= 0.01f) 
            {
                enemy.isIdle = true;
            }
            else
            {
                MoveToNextPoint();
            }
            waitTimer.Reset();
        }
    }

    public override void OnExit()
    {
        waitTimer.Reset();
    }

    private void MoveToNextPoint()
    {
        currentIndex++;
        if (currentIndex >= waypoints.Length)
            currentIndex = 0;

        agent.SetDestination(waypoints[currentIndex].position);
    }
}
