using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    public PowerUpEffects powerupEffect;
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.name == "Player")
        {
            Destroy(gameObject);
            powerupEffect.Apply(collision.gameObject);
        }
    }
}
