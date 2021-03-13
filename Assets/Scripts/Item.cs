using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public string _type;
    Rigidbody2D _rigid;

    void Awake()
    {
        _rigid = GetComponent<Rigidbody2D>();
    }

    void OnEnable()
    {
        _rigid.velocity = Vector2.down * 1.5f;
    }
}
