using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : MonoBehaviour
{
    public Rigidbody2D rb;
    public SpriteRenderer renderer;
    // Opens when the button is hit.
    public GameObject[] door;
    // Opposite of door gameobject.
    public GameObject[] doorClose;
    // When activated, cannot be closed again by this button.
    public GameObject[] permaLock;
    // Set some doors to be open by default.
    public GameObject[] startOpen;
    Color temp;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        renderer = GetComponent<SpriteRenderer>();
        // Red.
        renderer.color = new Color(1, 0, 0, 1);
        // Base colour of the door.
        temp = door[0].GetComponent<SpriteRenderer>().color;
        DoorSet(true, door);
        DoorSet(false, startOpen);
    }
    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.tag == "Coin" || col.gameObject.tag == "Player")
        {
            if (renderer.color == new Color(1, 0, 0, 1))
            {
                // Button set to green, neccesary actions taken.
                renderer.color = new Color(0, 1, 0, 1);
                DoorSet(false, door);
                DoorSet(true, startOpen);
                DoorSet(true, doorClose);
                DoorSet(true, permaLock);
                // The permalocks are turned pink.
                for (int i = 0; i <= (permaLock.Length - 1); i++)
                {
                    permaLock[i].GetComponent<SpriteRenderer>().color = new Color(1, 0, 1, 1);
                }
            }  
            else
            {
                // Button set to red, neccesary actions taken.
                renderer.color = new Color(1, 0, 0, 1);
                DoorSet(true, door);
                DoorSet(false, startOpen);
                DoorSet(false, doorClose);
                DoorSet(true, permaLock);
                // The permalocks are turned pink.
                for (int i = 0; i <= (permaLock.Length - 1); i++)
                {
                    permaLock[i].GetComponent<SpriteRenderer>().color = new Color(1, 0, 1, 1);
                }
            }
        }
    }
    // Cycles through gameObject arrays to action on each index.
    void DoorSet(bool active, GameObject[] door)
    {
        for (int i = 0; i <= (door.Length - 1); i++)
        {
            if (door[i].GetComponent<SpriteRenderer>().color == temp)
            {
                door[i].SetActive(active);
            }          
        }
    }
}
