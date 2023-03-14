using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NextChamber : MonoBehaviour
{
    int nextScene; 

    //vers�o tempor�ria do c�digo para teste
    public int nextRoom;
    // ESSE C�DIGO PODE FICAR DENTRO DO GAMECONTROLLER
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
