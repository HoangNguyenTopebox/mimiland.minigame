using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GamePreparation : MonoBehaviour
{
    [SerializeField] private SpyPlayer player;

    public IEnumerator Initialize(DateTime timeTillNextState) 
    {
        bool _isSpy = FakeRole();
        string keyword; 
        keyword = GetKeyword(_isSpy);
        player.Initialize(_isSpy,keyword);
        while (true)
        {
            if (DateTime.Now <= timeTillNextState)
            {
                yield return new WaitForSeconds(1f);
            }
            else
            {
                break;
            }
        }
        SpyGameplayManager.instance.OnUpdateState(GamestateEnum.CONVERSATION);
    }

    private bool FakeRole()
    {
        return Random.Range(0,2)==0?true:false;
    }

    private string GetKeyword(bool _isSpy)
    {
        if (_isSpy)
        {
            return "table";
        }
        else
        {
            return "chair";
        }
            
    }
}
