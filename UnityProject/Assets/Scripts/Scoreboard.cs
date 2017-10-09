using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scoreboard : MonoBehaviour {

	void OnEnable()
    {
        Player[] players = GameManager.GetAllPlayers();
        foreach (Player player in players)
        {

        }
    }

    void OnDisable()
    {

    }
}
