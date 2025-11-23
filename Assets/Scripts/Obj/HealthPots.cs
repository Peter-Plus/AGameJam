using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPots : MonoBehaviour
{
    //血瓶

    //当玩家碰到血瓶时，增加玩家的生命值
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerCore playerCore = collision.GetComponent<PlayerCore>();
            if (playerCore != null)
            {
                playerCore.PickupHealthPotion();
                Destroy(gameObject);
            }
        }
    }
}
