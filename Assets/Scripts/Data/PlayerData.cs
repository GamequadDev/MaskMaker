using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData", menuName = "Scriptable Objects/PlayerData")]
public class PlayerData : ScriptableObject
{
    public int money = 0;
    public int day = 1;
    public int diamondsCount = 5;
    public int feathersCount = 5;
    public int flowersCount = 5;
    public int leavesCount = 5;
    public int starsCount = 5;
}
