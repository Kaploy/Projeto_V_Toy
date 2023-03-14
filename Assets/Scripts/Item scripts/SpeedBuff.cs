using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenuAttribute(menuName = "Items/Speed buff")]

public class SpeedBuff : PowerUpEffects
{
    public float speedIncrease;
    public override void Apply(GameObject player)
    {
        player.GetComponent<PlayerController>().speed += speedIncrease;
        player.GetComponent<TrailRenderer>().enabled = true;
    }
}
