using UnityEngine;
using UnityEngine.UI;


public class GameUI : MonoBehaviour {
	
	[SerializeField] public GameObject levelText;
	[SerializeField] public GameObject menuContainer;
	
	[SerializeField] public GameObject gameOverText;
	[SerializeField] public GameObject replayButton;
	[SerializeField] public GameObject replayButtonText;
	
	[HideInInspector] private Game game;
	
	private bool _visible = true;

	void Start()
	{
		game = GetComponent<Game>();
	}
	
	public void setMenuVisible(bool visible)
	{
		_visible = visible;
		menuContainer.GetComponent<Image>().enabled = visible;
		replayButton.GetComponent<Image>().enabled = visible;
		replayButton.GetComponent<Button>().enabled = visible;
		replayButtonText.GetComponent<Text>().enabled = visible;
		gameOverText.GetComponent<Text>().enabled = visible;
	}

	public void btn_replayLevel()
	{
		game.gameInput.setUIInput(false);
		setMenuVisible(false);
		game.replayLevel();
	}
}
