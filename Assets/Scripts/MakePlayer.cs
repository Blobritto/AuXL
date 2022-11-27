using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MakePlayer : MonoBehaviour
{
    // Spawn the player at the start of the scene.
    public GameObject player;
    void Start()
    {
        Instantiate(player, transform.position, Quaternion.identity);   
    }
}
