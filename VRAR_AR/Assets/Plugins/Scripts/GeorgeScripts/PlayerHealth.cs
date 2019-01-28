using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour {

    public const int maxHealth = 100;
    public int currentHealth = maxHealth;
    public RectTransform healthBar; 

   public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            currentHealth = maxHealth;
            RpcRespawn();
        }

        healthBar.sizeDelta = new Vector2(currentHealth, healthBar.sizeDelta.y);
   

    }
         
    void RpcRespawn()
    {
        transform.position = Vector3.zero;

    }

}
