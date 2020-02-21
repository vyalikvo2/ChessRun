using Assets.Scripts.UI;
using UnityEngine;
using UnityEngine.UI;


public class GameUI : MonoBehaviour
{
	public const int MENU_NONE = 0;
	public const int MENU_REPLAY = 1;
	public const int MENU_LEVEL_COMPLETE = 2;
	
	[SerializeField] public GameObject levelText;
	[SerializeField] public GameObject menuContainer;

	[HideInInspector] public Game game;

	[SerializeField] public GameObject replayMenuObj;
	[HideInInspector] public ReplayMenu replayMenu;
	
	[SerializeField] public GameObject levelCompleteMenuObj;
	[HideInInspector] public LevelCompleteMenu levelCompleteMenu;

	[HideInInspector] private int currentMenu = MENU_NONE;
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
	public void setMenu(int menuId)
	{
		if (!_inited) init();
		switch (menuId)
		{
			case MENU_REPLAY:
				replayMenu.setVisible(true);
				break;
			
			case MENU_LEVEL_COMPLETE:
				levelCompleteMenu.setVisible(true);
				break;
		}

		if (menuId != MENU_NONE)
		{
			game.gameInput.setUIInput(true);
			setMenuVisible(true);
		} 
		else 
		{
			game.gameInput.setUIInput(false);
			hideAllMenu();
			setMenuVisible(false);
		}
		
		currentMenu = menuId;
	}

	private void hideAllMenu()
	{
		replayMenu.setVisible(false);
		levelCompleteMenu.setVisible(false);
	}
	
	
	private void setMenuVisible(bool visible)
	{
		_menuVisible = visible;
		menuContainer.GetComponent<Image>().enabled = visible;
	}

}
