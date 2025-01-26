using UnityEngine;

public interface IEnemy
{
    void TakeDamage();
    void StartUP();
    GameObject gameObject { get; }

}