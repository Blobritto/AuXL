using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayTextPlayerOnly : MonoBehaviour
{
    // Specific use case script to stop player from cheating on a challenge.
    public GameObject uiObject;
    public GameObject spawnPoint;
    void Start()
    {
        uiObject.SetActive(false);
    }
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "Player")
        {
            uiObject.SetActive(true);
            col.transform.position = spawnPoint.transform.position;
            if (GameObject.FindWithTag("Coin") != null)
            {
                GameObject.Destroy(GameObject.FindWithTag("Coin").gameObject);
            }
        }
    }
}
