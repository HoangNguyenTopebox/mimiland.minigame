using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class SpyFakePlayer : MonoBehaviour
{
    private bool isSpy;
    private string keyword;
    
    
    // Update is called once per frame

    private void Start()
    {
        isSpy = FakeRole();
        keyword = GetKeyword(isSpy);
        Debug.Log(gameObject.name+"keyword is "+keyword+"=>"+isSpy);

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

    private bool FakeRole()
    {
        return Random.Range(0,1)==0?true:false;
    }
    void Update()
    {
        if (SpyGameplayManager.instance.State == GamestateEnum.WAIT_FOR_INPUT)
        {
            
        }
    }
}
