using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FightUI : MonoBehaviour
{
    public static float TIME_SCALE_SELECTION_CIRCLE = 0.3f;
    public static float TIME_SHOW_ICONS_ANIMATION = 0.3f;
    public static float TIME_SHOW_ICONS_DELAY = 0.2f;
    
    public static float TIME_HIDE_ICONS_ANIMATION = 0.2f;
    public static float TIME_HIDE_CIRCLE_ANIMATION = 0.2f;
    public static float TIME_HIDE_CIRCLE_ANIMATION_DELAY = 0.1f;
    
    public static int STATE_NONE = 0;
    public static int STATE_ICONS = 1;
    
    private Game game;

    private List<FightActionIcon> renderedIcons;
    private List<string> fightActionList = new List<string>()
    {
        InteractionIcon.ICON_CANCEL,
        InteractionIcon.ICON_ATTACK
    };

    private GameObject iconPrefab;
    private GameObject canvasPrefab;
    
    private GameObject canvasObj;
    private Canvas canvas;
    private GameObject selectionCircle;

    private string lastActionSprite;
    private Cell actionCell;

    private int state = STATE_NONE;
        
    void Start()
    {
        game = GetComponent<Game>();
    }
    
    public void displayFightInput(Cell fightCell)
    {
        if (state != STATE_NONE) return;

        state = STATE_ICONS;
        actionCell = fightCell;
        
        renderedIcons = new List<FightActionIcon>();

        iconPrefab = Resources.Load("prefabs/ui/icons/CanvasIcon") as GameObject;
        canvasPrefab = Resources.Load("prefabs/WorldCanvas") as GameObject;

        canvasObj = Instantiate(canvasPrefab, transform, true) as GameObject;
        canvas = canvasObj.GetComponent<Canvas>();
        canvas.sortingOrder = ZIndex.FIGHT_UI;
        canvasObj.transform.localPosition = fightCell.transform.localPosition;
        Debug.Log("create at " +fightCell.transform.localPosition);

        createSelectionCircle();

        float angleStep = 360f / fightActionList.Count;
        float curAngle = -180f;
        float R = 100*Game.TO_UNITS;
        
        for (int i = 0; i < fightActionList.Count; i++)
        {
            GameObject iconObj = Instantiate(iconPrefab, canvasObj.transform) as GameObject;
            FightActionIcon icon = iconObj.GetComponent<FightActionIcon>();
            icon.Setup();
            icon.sprite = fightActionList[i];

            icon.onIconClick.AddListener(onIconClick);

            icon.transform.localScale = Vector3.one * 0.8f;
            icon.transform.localPosition = Vector3.zero;

            icon.transform.DOScale(1,TIME_SHOW_ICONS_ANIMATION).SetDelay(TIME_SHOW_ICONS_DELAY);
            icon.transform.DOLocalMove(
                new Vector3(Mathf.Cos(curAngle * Mathf.PI / 180.0f) * R, Mathf.Sin(curAngle * Mathf.PI / 180.0f) * R),
                TIME_SHOW_ICONS_ANIMATION).SetDelay(TIME_SHOW_ICONS_DELAY);

            renderedIcons.Add(icon);
            
            curAngle += angleStep;
        }
    }

    private void createSelectionCircle()
    {
        selectionCircle = Instantiate(iconPrefab, canvasObj.transform) as GameObject;
        selectionCircle.GetComponent<FightActionIcon>().Setup();
        selectionCircle.GetComponent<FightActionIcon>().sprite = InteractionIcon.UI_SELECT_CIRCLE;
        
        Image circleImage = selectionCircle.GetComponent<Image>();
        float c_scale = 250f / 70f;
        selectionCircle.transform.localScale = Vector3.zero;
        selectionCircle.transform.DOScale(c_scale, TIME_SCALE_SELECTION_CIRCLE);
        
        Color circleColor = circleImage.color;
        circleColor.a = 0;
        circleImage.color = circleColor;

        circleColor.a = 1;
        circleImage.DOColor(circleColor, TIME_SCALE_SELECTION_CIRCLE);
    }

    private void hideFightUI()
    {
        for (int i = 0; i < renderedIcons.Count; i++)
        {
            FightActionIcon icon = renderedIcons[i];
           // icon.transform.DOScale(0, TIME_HIDE_ICONS_ANIMATION);
            //icon.transform.DOLocalMove(Vector3.zero, TIME_HIDE_ICONS_ANIMATION);

            Image iconImage = icon.GetComponent<Image>();
            Color imageColor = iconImage.color;
            imageColor.a = 0;
            iconImage.DOColor(imageColor, TIME_HIDE_ICONS_ANIMATION);
        }

        if (selectionCircle)
        {
           // selectionCircle.transform.DOScale(0, TIME_HIDE_CIRCLE_ANIMATION)
            //    .SetDelay(TIME_HIDE_CIRCLE_ANIMATION_DELAY);
            
            Image circleImage = selectionCircle.GetComponent<Image>();
            Color circleColor = circleImage.color;
            circleColor.a = 0;
            circleImage.DOColor(circleColor, TIME_HIDE_CIRCLE_ANIMATION)
                .SetDelay(TIME_HIDE_CIRCLE_ANIMATION_DELAY).OnComplete(applyActionAndRemoveUI);
        }
    }

    private void applyActionAndRemoveUI()
    {
        processAction();

        state = STATE_NONE;
        
        if (selectionCircle)
        {
            selectionCircle.transform.SetParent(null);
            Destroy(selectionCircle);
        }

        for (int i = 0; i < renderedIcons.Count; i++)
        {
            FightActionIcon icon = renderedIcons[i];
            icon.transform.SetParent(null);
            Destroy(icon.gameObject);
        }
        renderedIcons = new List<FightActionIcon>();
    }
    
    private void onIconClick(BaseEventData eventData)
    {
        FightActionIcon iconClicked = eventData.selectedObject.GetComponent<FightActionIcon>();
        if (iconClicked)
        {
            lastActionSprite = iconClicked.sprite;
            hideFightUI();
        }
    }

    private void processAction()
    {
        Debug.Log("PROCESS ACTION "+ lastActionSprite);
        if (lastActionSprite == InteractionIcon.ICON_ATTACK)
        {
            game.board.continueFight(actionCell);
        }

        game.gameInput.setUIInput(false);
        
        lastActionSprite = null;
        actionCell = null;
    }
}
