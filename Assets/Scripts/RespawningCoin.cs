using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class RespawningCoin : Coin
{

    //Changed
    public event Action<RespawningCoin> OnCollected;

    public override int Collect()
    {

        if (!IsServer)
        {
            Show(false);
            return 0;
        }
        
        if(alreadyCollected)
        {
            return 0;
        }

        alreadyCollected = true;

        //Changed
        OnCollected?.Invoke(this);

        return coinValue;
    }


    //Changed
    public void Reset()
    {
        alreadyCollected = false;
    }
}
