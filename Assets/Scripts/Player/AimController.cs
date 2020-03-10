using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimController : MonoBehaviour
{
    RectTransform _rectTransform;
    SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        _spriteRenderer.enabled = false;
    }

    public void SetAim(bool show)
    {
        _spriteRenderer.enabled = show;
    }

    public void SetRotation(float rotation)
    {
        _rectTransform.rotation = Quaternion.Euler(0, 0, rotation);
    }
}
