using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


namespace ChessRun.GUI
{

    public class FightActionIcon : MonoBehaviour, IPointerClickHandler
    {
        public EventTrigger.TriggerEvent onIconClick;

        private string _sprite;

        public string sprite
        {
            get { return _sprite; }
            set
            {
                if (_sprite == value) return;
                _sprite = value;
                Sprite sprite = Resources.Load<Sprite>(value) as Sprite;
                imageRenderer.sprite = sprite;
            }
        }

        private Image imageRenderer;

        private bool _visible = true;

        public bool visible
        {
            get { return _visible; }
            set
            {
                if (_visible == value) return;
                _visible = value;
                imageRenderer.enabled = _visible;
            }
        }

        public void Setup()
        {
            onIconClick = new EventTrigger.TriggerEvent();
            imageRenderer = GetComponent<Image>();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (onIconClick != null)
            {
                BaseEventData eventDataCallback = new BaseEventData(EventSystem.current);
                eventData.selectedObject = this.gameObject;
                onIconClick.Invoke(eventDataCallback);
            }
        }

    }

}