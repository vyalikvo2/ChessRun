using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class Game : MonoBehaviour {

	public static float TO_UNITS = 0.01f;
	public static float TO_PIXELS = 1/TO_UNITS;
	public static float CELL_SIZE = 150;
	public static float POS_TO_COORDS = TO_UNITS * CELL_SIZE;
	public static float COORDS_TO_POS = 1/(POS_TO_COORDS);

	public Camera camera;

	[HideInInspector] public Engine engine;
	public static GameController gameController;
	
	[HideInInspector] public GameInput gameInput;


	[HideInInspector] public Board board;

	[SerializeField] public GameUI gameUI;
	[HideInInspector] public FightUI fightUI;
	
	private Vector3 cameraLerp;


	List <string[]> levels = new List<string[]>();

	private int currentLevel = 0;


	private void setupLevels()
	{
		levels.Add (new string[]
		{
			"E",
			"O",
			"1",
			"K"
		});

		levels.Add (new string[] {"EO", 
			                            "1O1",
			                            "O1O",
			                            "OKO"});
		
		levels.Add (new string[] {"##E", 
										"##O", 
										"##O", 
			                            "111",
			                            "1111",
			                            "1111",
			                            "1111",
			                            "1111",
			                            "1111",
			                            "OOOO",
			                            "KH"
		});
		
		levels.Add (new string[] {"##OOOOOO#",
			"O#EO#####", 
			"K#######O",
			"H#####O#O",
			"O#O#####O",
			"####OOH"
		});


	}

	// Use this for initialization
	void Start () {

		engine = GetComponent<Engine> ();
		board = GetComponent<Board>();
		fightUI = GetComponent<FightUI>();
		gameController = GetComponent<GameController>();
		gameInput = GetComponent<GameInput>();
		gameUI = GetComponent<GameUI>();

		PieceInteraction.game = this;

		setupLevels ();
		board.Setup ();
		
		GameData.choosedLevel = 2;
		setLevel(GameData.choosedLevel);	
		
		gameUI.setMenuVisible(false);
	}


	public void setLevel(int level)
	{
		board.clearBoard();
		
		currentLevel = level;	

		Debug.Log ("Setup level "+currentLevel);
		board.CreateByMap (levels [currentLevel]);
		gameController.startLevel();

		gameUI.levelText.GetComponent<Text>().text = "Уровень: "+ (currentLevel+1);
	}

	public void replayLevel()
	{
		setLevel(currentLevel);
	}

	public void nextLevel()
	{
		currentLevel++;
		setLevel(currentLevel);
	}

	public void centerCamera(Vector3 pos, bool animate = true)
	{
		cameraLerp = pos + new Vector3(1, 1, -1);
		if (!animate)
		{
			camera.transform.position = cameraLerp;
		}
	}
	// Update is called once per frame
	void Update () {
		if(cameraLerp != null){
			camera.transform.position = Vector3.Lerp (camera.transform.position, cameraLerp, 0.1f);
		}
	}
}
