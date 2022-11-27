using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : MonoBehaviour
{
    public Rigidbody2D rb;
    public SpriteRenderer renderer;
    public GameObject[] door;
    public GameObject[] doorClose;
    public GameObject[] permaLock;
    public GameObject[] startOpen;
    Color temp;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        renderer = GetComponent<SpriteRenderer>();
        renderer.color = new Color(1, 0, 0, 1);
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
                renderer.color = new Color(0, 1, 0, 1);
                DoorSet(false, door);
                DoorSet(true, startOpen);
                DoorSet(true, doorClose);
                DoorSet(true, permaLock);
                for (int i = 0; i <= (permaLock.Length - 1); i++)
                {
                    permaLock[i].GetComponent<SpriteRenderer>().color = new Color(1, 0, 1, 1);
                }
            }  
            else
            {
                renderer.color = new Color(1, 0, 0, 1);
                DoorSet(true, door);
                DoorSet(false, startOpen);
                DoorSet(false, doorClose);
                DoorSet(true, permaLock);
                for (int i = 0; i <= (permaLock.Length - 1); i++)
                {
                    permaLock[i].GetComponent<SpriteRenderer>().color = new Color(1, 0, 1, 1);
                }
            }
        }
    }

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
