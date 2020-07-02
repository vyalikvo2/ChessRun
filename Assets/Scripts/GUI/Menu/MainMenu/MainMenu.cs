using UnityEngine;

namespace ChessRun.GUI.Menu
{
	public class MainMenu : MonoBehaviour
	{
		public void playGame()
		{
			Application.LoadLevel("ChooseLevel");
		}

		public void BTN_BackToMainMenu()
		{
			Application.LoadLevel("MainMenu");
		}
	}

}
