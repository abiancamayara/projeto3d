using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class moeda : MonoBehaviour
{
    public float Coin;
    private void OnTriggerEnter(Collider collision)
    {
        if(collision.gameObject.tag == "Player") 
        {
            GameController.instance.UpdateQuantidade(Coin);
            Destroy(gameObject);
        }
    }
}
