﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Player : NetworkBehaviour {

    /**
     * - IMPROVE CODE
     * - Checkpoints 2h
     * 
     * - Add elements to pause screen 30min
     * - Make death screen : background transparent, show score, buttons restart, menu, quit
     * 
     - Place everywhere 
     * - Death detection if speed is lower than x for y seconds
     * 
     * - Animations and details... : Countdown, more information on screen
     **/

    public bool gamePaused = false;

    public bool isSingleplayer = false;

    [SyncVar]
    public int gameState = 0;

    [SyncVar]
    public int score = 0;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void Pause()
    {
        CanvasGroup cg = GameObject.Find("Pause").GetComponent<CanvasGroup>();
        if (gamePaused)
        {
            gamePaused = false;
            cg.interactable = false;
            cg.alpha = 0;
            Time.timeScale = 1f;

        } else
        {
            gamePaused = true;
            cg.interactable = true;
            cg.alpha = 1;
            Time.timeScale = 0f;
        }
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Pause();
        }
    }

    public void Ready()
    {
        Debug.Log("Game can start!");
        if (GameObject.Find("Waiting"))
        {
            GameObject.Find("Waiting").SetActive(false);
        }
    }

    [Command]
    public void CmdAddScore()
    {
        score += 5;
        RpcUpdateScore(score);
    }
    [ClientRpc]
    void RpcUpdateScore(int newScore)
    {
        GameObject.Find("ScoreLabel").GetComponent<Text>().text = newScore.ToString();
    }

}
