using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LifeCounter : MonoBehaviour
{
    private Text lifeText;

    [SerializeField]
    private PlayerChar player;

    void OnEnable()
    {
        lifeText = GetComponent<Text>();
    }

    void LateUpdate()
    {
        lifeText.text = "Lives: " + player.currentLives;
    }
}
