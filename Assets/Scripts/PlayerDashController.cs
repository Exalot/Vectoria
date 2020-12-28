using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDashController : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;
    private Color _color;
    private float _transparency = 250f;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }
    
    void Update()
    {
        _spriteRenderer.material.color = new Color(0.0f, 1.0f, 1, _transparency/250);
        _transparency -= 10;
        if (_transparency <= 0)
        {
            Destroy(gameObject);
        }
    }

    public void begin(Sprite sprite, bool isFacingLeft)
    {
        _spriteRenderer.sprite = sprite;
        if (isFacingLeft)
            _spriteRenderer.flipX = true;
    }
    
}
