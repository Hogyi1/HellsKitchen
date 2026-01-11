using UnityEngine;


public enum EnemyType
{
    TypeA,
    TypeB,
    Both
}

[CreateAssetMenu(fileName = "EnemyData", menuName = "Scriptable Objects/EnemyData")]

public class EnemyData : ScriptableObject
{
    public EnemyType[] Days;
}
