using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLoop : MonoBehaviour
{
    //Characters and properties
    [SerializeField]
    private PlayerChar player_1;
    [SerializeField]
    private PlayerChar player_2;

    private int p1_livesRemaining;
    private int p2_livesRemaining;

    private bool p1_lose;
    private bool p2_lose;
    
    void OnEnable()
    {
        p1_livesRemaining = player_1.maxLifeCount;
        p2_livesRemaining = player_2.maxLifeCount;
    }

    void Update()
    {
        updateLivesRemaining();
        if(p1_livesRemaining <= 0)
        {
            p1_lose = true;
            Debug.Log("Player 2 Wins");
        }
        if (p2_livesRemaining <= 0)
        {
            p2_lose = true;
            Debug.Log("Player 1 Wins");
        }
    }

    void updateLivesRemaining()
    {
        p1_livesRemaining = player_1.GetLivesRemaining();
        p2_livesRemaining = player_2.GetLivesRemaining();
    }


}
