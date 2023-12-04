using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class SpyPlayer : MonoBehaviour
{
    private bool isSpy;
    private string keyword;
    

    public void Initialize(bool _isSpy,string _keyword)
    {
        isSpy = _isSpy;
        keyword = _keyword;
        Debug.Log("[PHAT] your keyword is: "+keyword+ "=>"+isSpy);
    }
    
    
}
