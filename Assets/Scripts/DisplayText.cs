using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayText : MonoBehaviour
{
    public GameObject uiObject;
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
        if ((col.gameObject.tag == "Player" ) || (col.gameObject.tag == "Coin"))
        {
            uiObject.SetActive(true);
        }
    }
}
