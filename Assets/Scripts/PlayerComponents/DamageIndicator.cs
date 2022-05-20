using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using DG.Tweening.Core.Easing;
using UnityEngine;
using UnityEngine.UI;

public class DamageIndicator : MonoBehaviour
{
    private GameObject flashView;
    private CanvasRenderer _panel;
    private GameManager _manager;

    private void Awake()
    {
        _panel = GetComponent<CanvasRenderer>();
        _panel.SetAlpha(0f);
    }

    void Start () {
        _manager = GameManager.GetInstance();
        _manager.playerDamage.AddListener(Damage);
    }

    private void Damage(float dmg)
    {
         StartCoroutine("FlashPanel");
    }

    IEnumerator FlashPanel()
    {
        _panel.SetAlpha(0.5f);
        
        yield return new WaitForSeconds(0.1f);
        
        if(_manager.GetPlayer()._alive) _panel.SetAlpha(0f);
    }
}
