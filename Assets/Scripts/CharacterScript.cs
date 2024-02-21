using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public abstract class CharacterScript : MonoBehaviour
{
    [Header("ParentClass")]
    public float maxHealth;
    public bool IsDeath;
    float target;
    public float currentHealth;
    [SerializeField] Image healthBar;
    public float healthLerpValue;
    // Start is called before the first frame update


    public void Awake()
    {
        currentHealth = maxHealth;
        target = currentHealth;
    }
    private void Update()
    {
        LerpHealthValue();
    }
    public void TakeDamage(float Damage)
    {
        currentHealth -= Damage;
        if (currentHealth <= 0)
        {
            IsDeath = true;
            OnDeath();
        }
        UpdateHealthUI();
    }
    public void HealHealth(float amount)
    {
        currentHealth += amount;
        if (currentHealth >= maxHealth) currentHealth = maxHealth;
        UpdateHealthUI();
    }

    public void UpdateHealthUI()
    {
        target = currentHealth / maxHealth;
    }
    public void LerpHealthValue()
    {
        healthBar.fillAmount = Mathf.MoveTowards(healthBar.fillAmount, target, 2 * Time.deltaTime);
    }
    public abstract void OnDeath();
}
