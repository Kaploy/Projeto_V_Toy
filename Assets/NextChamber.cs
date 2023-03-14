using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NextChamber : MonoBehaviour
{
    int nextScene; 

    //versão temporária do código para teste
    public int nextRoom;
    // ESSE CÓDIGO PODE FICAR DENTRO DO GAMECONTROLLER
    private void Awake()
    {
        //int nextScene = Random.Range(2, 4);
        //Debug.Log(nextScene);
    }
    private void OnTriggerEnter(Collider collision)
    {
        GameObject collisionGameObject = collision.gameObject;
        if(collisionGameObject.tag == "Player")
        {
            SceneManager.LoadScene(nextRoom);
            //SceneManager.LoadScene(nextScene);
        }
    }
}
