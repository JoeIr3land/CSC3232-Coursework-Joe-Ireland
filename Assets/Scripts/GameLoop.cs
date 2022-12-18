using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameLoop : MonoBehaviour
{
    //Characters and properties
    [SerializeField]
    private PlayerChar player_1;
    [SerializeField]
    private PlayerChar player_2;

    [SerializeField]
    private GameObject p1WinScreen;
    [SerializeField]
    private GameObject p2WinScreen;

    private bool gameOver;
    [SerializeField]
    int resetTime = 180; //Wait 3 seconds after end of game before restarting
    int resetCounter;

    private int p1_livesRemaining;
    private int p2_livesRemaining;
    
    void OnEnable()
    {
        p1_livesRemaining = player_1.maxLifeCount;
        p2_livesRemaining = player_2.maxLifeCount;
        gameOver = false;
        resetCounter = 0;
    }

    void FixedUpdate()
    {
        if (gameOver)
        {
            if(resetCounter == resetTime)
            {
                SceneManager.LoadScene("SampleScene");
            }
            resetCounter++;
        }

        updateLivesRemaining();
        if(p1_livesRemaining <= 0)
        {
            p2WinScreen.GetComponent<Text>().enabled = true;
            gameOver = true;
        }
        if (p2_livesRemaining <= 0)
        {
            p1WinScreen.GetComponent<Text>().enabled = true;
            gameOver = true;
        }
    }

    void updateLivesRemaining()
    {
        p1_livesRemaining = player_1.GetLivesRemaining();
        p2_livesRemaining = player_2.GetLivesRemaining();
    }


}
