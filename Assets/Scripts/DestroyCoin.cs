using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyCoin : MonoBehaviour
{
    // Only be nice once, not every frame.
    public bool wasNice;
    void Start()
    {
        wasNice = false;
    }
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "Player" && wasNice == false)
        {
            if (GameObject.FindWithTag("Coin") != null)
            {
                // Destroys the coin on screen.
                GameObject.Destroy(GameObject.FindWithTag("Coin").gameObject);
                wasNice = true;
            }
        }
    }
}
