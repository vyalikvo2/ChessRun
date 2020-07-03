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
		[SerializeField] public GameObject LevelText;
		[SerializeField] private GameObject _menuContainer;

		[HideInInspector] public GameEngine GameEngine;

		[SerializeField] private GameObject _replayMenuObj;
		[HideInInspector] private ReplayMenu replayMenu;

		[SerializeField] private GameObject _levelCompleteMenuObj;
		[HideInInspector] private LevelCompleteMenu _levelCompleteMenu;

		[HideInInspector] private MenuType _currentMenu = MenuType.NONE;
		[HideInInspector] private bool _menuVisible = true;

		[HideInInspector] private bool _inited = false;

		private void _init()
		{
			GameEngine = GetComponent<GameEngine>();
			replayMenu = _replayMenuObj.GetComponent<ReplayMenu>();
			replayMenu.GameUI = this;

			_levelCompleteMenu = _levelCompleteMenuObj.GetComponent<LevelCompleteMenu>();
			_levelCompleteMenu.GameUI = this;

			_inited = true;
		}

		public void SetMenu(MenuType menuType)
		{
			if (!_inited) _init();
			switch (menuType)
			{
				case MenuType.REPLAY:
					replayMenu.SetVisible(true);
					break;

				case MenuType.LEVEL_COMPLETE:
					_levelCompleteMenu.SetVisible(true);
					break;
			}

			if (menuType != MenuType.NONE)
			{
				GameEngine.GameInput.SetUiInput(true);
				_setMenuVisible(true);
			}
			else
			{
				GameEngine.GameInput.SetUiInput(false);
				_hideAllMenu();
				_setMenuVisible(false);
			}

			_currentMenu = menuType;
		}

		private void _hideAllMenu()
		{
			replayMenu.SetVisible(false);
			_levelCompleteMenu.SetVisible(false);
		}


		private void _setMenuVisible(bool visible)
		{
			_menuVisible = visible;
			_menuContainer.GetComponent<Image>().enabled = visible;
		}

	}
}
