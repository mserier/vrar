using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

    public static UIManager Instance { get; private set; }

    public Transform hitsplat;
    public Text hitsplatText;

    public Text healthbarText;
    public Slider healthbar;

    void Awake()
    {
        if (Instance == null) { Instance = this; } else { Debug.Log("Warning: multiple " + this + " in scene!"); }
    }

    public void SpawnHitSplat(Transform target, int damage)
    {
        hitsplat.gameObject.SetActive(true);
        hitsplat.position = Camera.main.WorldToScreenPoint(target.position);


        hitsplatText.text = damage.ToString();
        StartCoroutine(HitSplatTimer(1));
    }

    public void UpdateHealthBar(int currentHealth, int maxHealth)
    {
        healthbarText.text = currentHealth + " / " + maxHealth;
        healthbar.value = (float) currentHealth / (float) maxHealth;
    }

    private IEnumerator HitSplatTimer(float duration)
    {
        yield return new WaitForSeconds(duration);
        hitsplat.gameObject.SetActive(false);
    }
}
