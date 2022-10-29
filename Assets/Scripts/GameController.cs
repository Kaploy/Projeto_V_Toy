using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
   public StateMachine<GameController> StateMachine { get; private set; }

    private void Start()
    {
        StateMachine = new StateMachine<GameController>(this);
        StateMachine.ChangeState(FreeRoamState.instance);
    }

    private void Update()
    {
        StateMachine.Execute();
    }
}
