using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PercentCounter : MonoBehaviour
{
    private Text percentText;

    [SerializeField]
    private PlayerChar player;

    void OnEnable()
    {
        percentText = GetComponent<Text>();
    }

    void LateUpdate()
    {
        percentText.text = player.damageStat + "%";
    }
}
