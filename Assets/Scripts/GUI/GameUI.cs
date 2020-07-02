using UnityEngine;
using UnityEngine.UI;

using ChessRun.GUI.Menu;

namespace ChessRun.GUI
{

	public enum MenuType
	{
		NONE,
		REPLAY,
		LEVEL_COMPLETE,
	}

	public class GameUI : MonoBehaviour
	{
		[SerializeField] public GameObject levelText;
		[SerializeField] public GameObject menuContainer;

		[HideInInspector] public Game game;

		[SerializeField] public GameObject replayMenuObj;
		[HideInInspector] public ReplayMenu replayMenu;

		[SerializeField] public GameObject levelCompleteMenuObj;
		[HideInInspector] public LevelCompleteMenu levelCompleteMenu;

		[HideInInspector] private MenuType currentMenu = MenuType.NONE;
		[HideInInspector] private bool _menuVisible = true;

		[HideInInspector] private bool _inited = false;

		void init()
		{
			game = GetComponent<Game>();
			replayMenu = replayMenuObj.GetComponent<ReplayMenu>();
			replayMenu.gameUI = this;

			levelCompleteMenu = levelCompleteMenuObj.GetComponent<LevelCompleteMenu>();
			levelCompleteMenu.gameUI = this;

			_inited = true;
		}

		public void setMenu(MenuType menuType)
		{
			if (!_inited) init();
			switch (menuType)
			{
				case MenuType.REPLAY:
					replayMenu.setVisible(true);
					break;

				case MenuType.LEVEL_COMPLETE:
					levelCompleteMenu.setVisible(true);
					break;
			}

			if (menuType != MenuType.NONE)
			{
				game.gameInput.setUIInput(true);
				_setMenuVisible(true);
			}
			else
			{
				game.gameInput.setUIInput(false);
				_hideAllMenu();
				_setMenuVisible(false);
			}

			currentMenu = menuType;
		}

		private void _hideAllMenu()
		{
			replayMenu.setVisible(false);
			levelCompleteMenu.setVisible(false);
		}


		private void _setMenuVisible(bool visible)
		{
			_menuVisible = visible;
			menuContainer.GetComponent<Image>().enabled = visible;
		}

	}
}
