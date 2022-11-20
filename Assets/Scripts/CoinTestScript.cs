using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinTestScript : MonoBehaviour
{
    [SerializeField] public Rigidbody2D rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = new Vector2(20, 20);
        rb.angularVelocity = -1000f;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
