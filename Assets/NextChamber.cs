using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NextChamber : MonoBehaviour
{
    public int nextScene;
    // ESSE CÓDIGO PODE FICAR DENTRO DO GAMECONTROLLER
    private void OnTriggerEnter(Collider collision)
    {
        GameObject collisionGameObject = collision.gameObject;
        if(collisionGameObject.tag == "Player")
        {
            SceneManager.LoadScene(nextScene);
        }
    }
}
