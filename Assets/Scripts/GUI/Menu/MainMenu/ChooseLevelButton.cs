using UnityEngine;
using UnityEngine.EventSystems;

namespace ChessRun.GUI.Menu
{
	public class ChooseLevelButton : MonoBehaviour, IPointerClickHandler
	{

		public int Level = 0;

		public void OnPointerClick(PointerEventData eventData)
		{

			GameData.ChoosedLevel = Level;
			Application.LoadLevel("Game");
		}

	}

}
