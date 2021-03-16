using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int _dmg;
    public bool _isRotate;

    void Update()
    {
        if (_isRotate)
            transform.Rotate(Vector3.forward * 10);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "BorderBullet")
        {
            gameObject.SetActive(false);
            //Destroy(gameObject);
        }
    }
}