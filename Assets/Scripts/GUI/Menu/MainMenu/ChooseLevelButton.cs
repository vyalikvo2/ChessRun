using UnityEngine;
using UnityEngine.EventSystems;

namespace ChessRun.GUI.Menu
{
	public class ChooseLevelButton : MonoBehaviour, IPointerClickHandler
	{

		public int level = 0;

		public void OnPointerClick(PointerEventData eventData)
		{

			GameData.choosedLevel = level;
			Application.LoadLevel("Game");
		}

	}

}
