using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreeRoamState : State<GameController>
{
    public static FreeRoamState instance { get; private set; }

    private void Awake()
    {
        instance = this;
    }
    /*public override void Execute()
    {
        PlayerController.instance.HandleUpdate();
    }
    */
}