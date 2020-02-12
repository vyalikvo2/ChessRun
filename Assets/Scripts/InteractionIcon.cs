using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionIcon : MonoBehaviour
{
    public static string PREFIX = "images/board/interaction/";
    public static string ICON_INTERACTION = PREFIX + "action_interact";
    public static string ICON_ATTACK = PREFIX + "action_attack";
    
    private string _sprite;
    public string sprite
    {
        get { return _sprite; }
        set
        {
            if (_sprite == value) return;
            Sprite sprite = Resources.Load<Sprite>(value) as Sprite;
            Debug.Log("SPRITE "+sprite+ " | " +value);
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
        sprite = ICON_INTERACTION;
    }
    
}
