using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayText : MonoBehaviour
{
    // Sets the text boxes to be visible.
    public GameObject uiObject;
    void Start()
    {
        uiObject.SetActive(false);
    }
    void OnTriggerEnter2D(Collider2D col)
    {
        if ((col.gameObject.tag == "Player" ) || (col.gameObject.tag == "Coin"))
        {
            uiObject.SetActive(true);
        }
    }
}
