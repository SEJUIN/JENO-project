using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Background : MonoBehaviour
{
    public float _speed;
    public int _startIndex;
    public int _endIndex;
    public Transform[] _sprites;

    float _viewHeight;

    void Awake()
    {
        _viewHeight = Camera.main.orthographicSize * 2;
    }

    void Update()
    {
        Move();
        Scrolling();
    }

    void Move()
    {
        Vector3 curPos = transform.position;
        Vector3 nextPos = Vector3.down * _speed * Time.deltaTime;
        transform.position = curPos + nextPos;
    }

    void Scrolling()
    {
        if (_sprites[_endIndex].position.y < _viewHeight * (-1))
        {
            Vector3 backSpritePos = _sprites[_startIndex].localPosition;
            //Vector3 frontSpritePos = _sprites[_endIndex].localPosition;
            _sprites[_endIndex].localPosition = backSpritePos + Vector3.up * _viewHeight;

            int startIndexSave = _startIndex;
            _startIndex = _endIndex;
            _endIndex = (startIndexSave - 1 == -1) ? _sprites.Length - 1 : startIndexSave - 1;
        }
    }
}
