using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    [Header("Base Stats")]
    [SerializeField] protected float baseMaxHealth = 100f;
    [SerializeField] protected float baseAttack = 10f;
    [SerializeField] protected float baseDefense = 2f;

    [Header("Multipliers")]
    [SerializeField, Range(0.1f, 5f)] protected float healthMultiplier = 1.0f;
    [SerializeField, Range(0.1f, 5f)] protected float attackMultiplier = 1.0f;
    [SerializeField, Range(0.1f, 5f)] protected float defenseMultiplier = 1.0f;

    protected float currentHealth;
    protected bool isDead = false;
    
    public float MaxHealth => baseMaxHealth * healthMultiplier;
    public float FinalAttack => baseAttack * attackMultiplier;
    public float FinalDefense => baseDefense * defenseMultiplier;
    
    protected virtual void Start()
    {
        currentHealth = MaxHealth;
    }
    
    public abstract void Attack();
    
    public virtual void TakeDamage(float rawDamage)
    {
        if (isDead) return;

        float actualDamage = Mathf.Max(rawDamage - FinalDefense, 1f);
        currentHealth -= actualDamage;

        Debug.Log($"{gameObject.name} 鍙楀埌 {actualDamage} 浼ゅ");
        
        OnHurtEffect();

        if (currentHealth <= 0) Die();
    }
    
    protected virtual void OnHurtEffect() { }

    protected virtual void Die()
    {
        isDead = true;
        Destroy(gameObject);
    }
}
