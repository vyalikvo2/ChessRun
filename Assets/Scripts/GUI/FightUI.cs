using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

using ChessRun.Board;

namespace ChessRun.GUI
{
    public enum FightUIState
    {
        NONE,
        ICONS
    }

    public class FightUI : MonoBehaviour
    {
        public static float TIME_SCALE_SELECTION_CIRCLE = 0.3f;
        public static float TIME_SHOW_ICONS_ANIMATION = 0.3f;
        public static float TIME_SHOW_ICONS_DELAY = 0.2f;

        public static float TIME_HIDE_ICONS_ANIMATION = 0.2f;
        public static float TIME_HIDE_CIRCLE_ANIMATION = 0.2f;
        public static float TIME_HIDE_CIRCLE_ANIMATION_DELAY = 0.1f;

        private GameEngine _gameEngine;

        private List<FightActionIcon> _renderedIcons;

        private List<string> _fightActionList = new List<string>()
        {
            InteractionIcon.ICON_CANCEL,
            InteractionIcon.ICON_ATTACK
        };

        private GameObject _iconPrefab;
        private GameObject _canvasPrefab;

        private GameObject _canvasObj;
        private Canvas _canvas;
        private GameObject _selectionCircle;

        private string _lastActionSprite;
        private Cell _actionCell;

        private FightUIState _state = FightUIState.NONE;

        void Start()
        {
            _gameEngine = GetComponent<GameEngine>();
        }

        public void DisplayFightInput(Cell fightCell)
        {
            if (_state != FightUIState.NONE) return;

            _state = FightUIState.ICONS;
            _actionCell = fightCell;

            _renderedIcons = new List<FightActionIcon>();

            _iconPrefab = Resources.Load("prefabs/ui/icons/CanvasIcon") as GameObject;
            _canvasPrefab = Resources.Load("prefabs/WorldCanvas") as GameObject;

            _canvasObj = Instantiate(_canvasPrefab, transform, true) as GameObject;
            _canvas = _canvasObj.GetComponent<Canvas>();
            _canvas.sortingOrder = ZIndex.FIGHT_UI;
            _canvasObj.transform.localPosition = fightCell.transform.localPosition;

            _createSelectionCircle();

            float angleStep = 360f / _fightActionList.Count;
            float curAngle = -180f;
            float rlength = 100 * GameEngine.TO_UNITS;

            for (int i = 0; i < _fightActionList.Count; i++)
            {
                GameObject iconObj = Instantiate(_iconPrefab, _canvasObj.transform) as GameObject;
                FightActionIcon icon = iconObj.GetComponent<FightActionIcon>();
                icon.Setup();
                icon.sprite = _fightActionList[i];

                icon.OnIconClick.AddListener(_onIconClick);

                icon.transform.localScale = Vector3.one * 0.8f;
                icon.transform.localPosition = Vector3.zero;

                icon.transform.DOScale(1, TIME_SHOW_ICONS_ANIMATION).SetDelay(TIME_SHOW_ICONS_DELAY);
                icon.transform.DOLocalMove(
                    new Vector3(Mathf.Cos(curAngle * Mathf.PI / 180.0f) * rlength,
                        Mathf.Sin(curAngle * Mathf.PI / 180.0f) * rlength),
                    TIME_SHOW_ICONS_ANIMATION).SetDelay(TIME_SHOW_ICONS_DELAY);

                _renderedIcons.Add(icon);

                curAngle += angleStep;
            }
        }

        private void _createSelectionCircle()
        {
            _selectionCircle = Instantiate(_iconPrefab, _canvasObj.transform) as GameObject;
            _selectionCircle.GetComponent<FightActionIcon>().Setup();
            _selectionCircle.GetComponent<FightActionIcon>().sprite = InteractionIcon.UI_SELECT_CIRCLE;

            Image circleImage = _selectionCircle.GetComponent<Image>();
            float c_scale = 250f / 70f;
            _selectionCircle.transform.localScale = Vector3.zero;
            _selectionCircle.transform.DOScale(c_scale, TIME_SCALE_SELECTION_CIRCLE);

            Color circleColor = circleImage.color;
            circleColor.a = 0;
            circleImage.color = circleColor;

            circleColor.a = 1;
            circleImage.DOColor(circleColor, TIME_SCALE_SELECTION_CIRCLE);
        }

        private void _hideFightUI()
        {
            for (int i = 0; i < _renderedIcons.Count; i++)
            {
                FightActionIcon icon = _renderedIcons[i];
                // icon.transform.DOScale(0, TIME_HIDE_ICONS_ANIMATION);
                //icon.transform.DOLocalMove(Vector3.zero, TIME_HIDE_ICONS_ANIMATION);

                Image iconImage = icon.GetComponent<Image>();
                Color imageColor = iconImage.color;
                imageColor.a = 0;
                iconImage.DOColor(imageColor, TIME_HIDE_ICONS_ANIMATION);
            }

            if (_selectionCircle)
            {
                // selectionCircle.transform.DOScale(0, TIME_HIDE_CIRCLE_ANIMATION)
                //    .SetDelay(TIME_HIDE_CIRCLE_ANIMATION_DELAY);

                Image circleImage = _selectionCircle.GetComponent<Image>();
                Color circleColor = circleImage.color;
                circleColor.a = 0;
                circleImage.DOColor(circleColor, TIME_HIDE_CIRCLE_ANIMATION)
                    .SetDelay(TIME_HIDE_CIRCLE_ANIMATION_DELAY).OnComplete(_applyActionAndRemoveUI);
            }
        }

        private void _applyActionAndRemoveUI()
        {
            _processAction();

            _state = FightUIState.NONE;

            if (_selectionCircle)
            {
                Destroy(_selectionCircle);
            }

            for (int i = 0; i < _renderedIcons.Count; i++)
            {
                FightActionIcon icon = _renderedIcons[i];
                Destroy(icon.gameObject);
            }

            _renderedIcons = new List<FightActionIcon>();
        }

        private void _onIconClick(BaseEventData eventData)
        {
            FightActionIcon iconClicked = eventData.selectedObject.GetComponent<FightActionIcon>();
            if (iconClicked)
            {
                _lastActionSprite = iconClicked.sprite;
                _hideFightUI();
            }
        }

        private void _processAction()
        {
            if (_lastActionSprite == InteractionIcon.ICON_ATTACK)
            {
                _gameEngine.Board.ContinueFight(_actionCell);
            }

            _gameEngine.GameInput.SetUiInput(false);

            _lastActionSprite = null;
            _actionCell = null;
        }
    }
    
}