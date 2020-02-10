using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionIcon : MonoBehaviour
{
    public static string PREFIX = "images/board/interaction/";
    public static string ICON_INTERACTION = PREFIX+"interaction";

    private string _sprite;
    public string sprite
    {
        get { return _sprite; }
        set
        {
            if (_sprite == value) return;
            Sprite sprite = Resources.Load(value) as Sprite;
            spriteRenderer.sprite = sprite;
        }
    }
    private SpriteRenderer spriteRenderer;

    private bool _visible = true;

    public bool visible
    {
        get { return _visible; }
        set
        {
            if (_visible == value) return;
            _visible = value;
            spriteRenderer.enabled = _visible; 
        }
    }

    public void Setup()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sortingOrder = 100;
    }
    
}
