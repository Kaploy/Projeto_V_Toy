using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowBox : MonoBehaviour
{
    public PlayerController playerController;
    Collider playerCollider;
    // Start is called before the first frame update
    void Start()
    {
        playerController = FindObjectOfType<PlayerController>();
        playerCollider = playerController.GetComponent<BoxCollider>();
    }

    private void OnTriggerEnter(Collider playerCollider)
    {
        
        playerController.playerVisible = false;
    }

    private void OnTriggerExit(Collider playerCollider)
    {
        playerController.playerVisible = true;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
