using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScalableArrow : MonoBehaviour
{
   [SerializeField] public GameObject body;
   [SerializeField] public GameObject head;

   [HideInInspector] private float body_w;
   [HideInInspector] private float head_w;
   [HideInInspector] private SpriteRenderer body_renderer;
   [HideInInspector] private SpriteRenderer head_renderer;

   [HideInInspector] public InteractionIcon icon;
   [HideInInspector] private bool _iconVisible = false;

   [HideInInspector]
   public bool iconVisible
   {
      get { return _iconVisible; }
      set
      {
         if (_iconVisible == value) return;
         _iconVisible = value;
         updateVisible();
      }
   }
   
   private int _width;
   public int width
   {
      set
      {
         if (_width == value) return;
         body.transform.localScale = new Vector3((value-head_w/2)/body_w,1,1);
         body.transform.localPosition = new Vector3(0,0,0);
         head.transform.localPosition = new Vector3((value-head_w/2)/100,0,0);
         if (icon)
         {
            icon.transform.localPosition = new Vector3(((value-head_w/2)/body_w) / 2, 0,0);
            icon.transform.localScale = new Vector3(0.4f,0.4f,0.4f);
         }
         _width = value;
      }
      get { return _width; }
   }

   private bool _visible = true;
   public bool visible
   {
      get { return _visible; }
      set
      {
         if (_visible == value) return;
         _visible = value;
         updateVisible();
      }
   }

   private void updateVisible()
   {
      body_renderer.enabled = _visible;
      head_renderer.enabled = _visible;
      if (icon)
      {
         icon.visible = _visible && iconVisible;
      }
   }
   public void Setup()
   {
      body_renderer = body.GetComponent<SpriteRenderer>();
      head_renderer = head.GetComponent<SpriteRenderer>();
      
      body_w = body_renderer.bounds.size.x * body_renderer.sprite.pixelsPerUnit;
      head_w = head_renderer.bounds.size.x * head_renderer.sprite.pixelsPerUnit;
      
      body_renderer.sortingOrder = ZIndex.UI_OVER_GAME_1;
      head_renderer.sortingOrder = ZIndex.UI_OVER_GAME_1;

      GameObject iconPrefab = Resources.Load("prefabs/InteractionIcon") as GameObject;
      GameObject iconObj = Instantiate(iconPrefab, Vector3.zero, Quaternion.identity);
      iconObj.transform.SetParent(transform);
      icon = iconObj.GetComponent<InteractionIcon>();
      icon.Setup();
      icon.visible = false;
   }

   public void setRotation(float angle)
   {
      Quaternion rot = Quaternion.AngleAxis(angle, Vector3.forward);
      transform.rotation = rot;
      icon.transform.rotation = Quaternion.AngleAxis(0, Vector3.back);
   }
}
