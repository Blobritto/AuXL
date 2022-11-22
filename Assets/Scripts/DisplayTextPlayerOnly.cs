using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayTextPlayerOnly : MonoBehaviour
{
    public GameObject uiObject;
    public GameObject spawnPoint;
    // Start is called before the first frame update
    void Start()
    {
        uiObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }

    // Once the player enters the trigger, the text will display
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
