using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum GamestateEnum
{
    START,
    PREPARE,
    CONVERSATION,
    WAIT_FOR_INPUT,
    CHECK_IS_END,
    END
}
public class SpyGameplayManager : MonoBehaviour
{
    public static SpyGameplayManager instance;
    private GamestateEnum state;

    [SerializeField] private GamePreparation preparation;
    private DateTime timeTillNextState;
    public GamestateEnum State
    {
        get => state;
        set => state = value;
    }

    private void Awake()
    {
        instance = this;
    }

    public void OnStartGame()
    {
        timeTillNextState = DateTime.Now;
        OnUpdateState(GamestateEnum.PREPARE);
    }


    public void OnUpdateState(GamestateEnum _state)
    {
        State = _state;
        switch (State)
        {
            case GamestateEnum.START:
                break;
            case GamestateEnum.PREPARE:
                Debug.Log("prepare state");
                timeTillNextState= timeTillNextState.AddSeconds(60f);
                StartCoroutine(preparation.Initialize(timeTillNextState));                
                break;
            case GamestateEnum.CONVERSATION:
                Debug.Log("conversation state");
                break;
            case GamestateEnum.WAIT_FOR_INPUT:
                Debug.Log("vote state");
                break;
            case GamestateEnum.CHECK_IS_END:
                Debug.Log("check end state");
                break;
            case GamestateEnum.END:
                Debug.Log("end state");
                break;
        }
    }
}
