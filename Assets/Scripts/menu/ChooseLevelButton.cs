using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class ChooseLevelButton : MonoBehaviour , IPointerClickHandler{

	public int level = 0;

	public void OnPointerClick(PointerEventData eventData)
    {

       	GameData.choosedLevel = level;
		Application.LoadLevel("Game");
    }

}
