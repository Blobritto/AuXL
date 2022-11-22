using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class resetcoin : MonoBehaviour
{
    public bool wasNice;
    
    // Start is called before the first frame update
    void Start()
    {
        wasNice = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "Player" && wasNice == false)
        {
            if (GameObject.FindWithTag("Coin") != null)
            {
                GameObject.Destroy(GameObject.FindWithTag("Coin").gameObject);
                wasNice = true;
            }
        }
    }
}
