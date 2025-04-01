using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    [SerializeField] protected string enemyTag = "Enemy";

    [SerializeField] protected float fireRate = 1f;

    [SerializeField] private Sprite icon;
    protected int _curLvl = 1;
    protected float _fireCountdown = 0f;
    protected int _maxLvl = 5;

    public Sprite Icon
    {
        get => icon;
        private set => icon = value;
    }

    public abstract void Upgrade();
}