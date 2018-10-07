﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthController : MonoBehaviour, IDamage {

    public int maxHP = 100;
    [HideInInspector]
    public Game_Manager gameManager;
    public GameObject ragdoll;
    public Image bloodyScreen;

    public Text logs;
    private int currentHP;
    private Slider sliderHealthBar;
    private Animator sliderHealthBarAnim;
    void Start()
    {
        currentHP = maxHP;
        GameObject gObj = GameObject.FindGameObjectWithTag("UI");
        gameManager = GameObject.FindObjectOfType<Game_Manager>();
        sliderHealthBar = gObj.GetComponentInChildren<Slider>();
        sliderHealthBarAnim = gObj.GetComponentInChildren<Animator>();
        logs = gObj.transform.GetChild(0).GetComponent<Text>();
        Image[] imgs = gObj.GetComponentsInChildren<Image>(true);
        for (int i = 0; i < imgs.Length; i++)
        {
            if (imgs[i].name == "BloodScreen")
            {
                bloodyScreen = imgs[i];
                break;
            }
        }
        logs.text = gObj.name + " " + sliderHealthBar.name + " " + sliderHealthBarAnim.name;
        sliderHealthBar.maxValue = maxHP;
        UpdateHealthBar();
    }

    public void ApplyDamage(int damage, out bool killed)
    {
        killed = false;
        currentHP -= damage;
        currentHP = currentHP <= 0 ? 0 : currentHP;
        UpdateHealthBar();
        if (currentHP <= 0)
            Die();
    }

    public void Die()
    {
        Instantiate(ragdoll, transform.position, transform.rotation);
        bloodyScreen.gameObject.SetActive(true);
        //gameManager.StopAllCoroutines();
        gameManager.StopGenerateZombie();
        Destroy(gameObject);
    }

    public void Heal(int heal)
    {
        if (currentHP == maxHP)
            return;

        int fact_heal = maxHP - currentHP < heal ? maxHP - currentHP : heal;

        if(((float)currentHP + fact_heal) / maxHP > 0.2f)
            sliderHealthBarAnim.SetBool("LowHP", false);

        StartCoroutine(Healing(fact_heal));
        currentHP += fact_heal;
        //UpdateHealthBar();
    }

    private void UpdateHealthBar()
    {
        sliderHealthBar.value = currentHP;

        if ((float)currentHP / maxHP <= 0.2f)
            sliderHealthBarAnim.SetBool("LowHP", true);
        else
            sliderHealthBarAnim.SetBool("LowHP", false);
    }

    IEnumerator Healing(int heal)
    {
        int c = heal;
        while(heal >= 0)
        {
            yield return new WaitForSeconds((float)3/c);
            sliderHealthBar.value++;
            heal--;
        }
    }

    public void ApplyDamage(int damage, out bool killed, out GameObject obj)
    {
        throw new System.NotImplementedException();
    }
}
