using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Game : MonoBehaviour {

	public static float TO_UNITS = 0.01f;
	public static float TO_PIXELS = 1/TO_UNITS;
	public static float CELL_SIZE = 150;
	public static float POS_TO_COORDS = TO_UNITS * CELL_SIZE;
	public static float COORDS_TO_POS = 1/(POS_TO_COORDS);

	public Camera camera;

	[HideInInspector] public Engine engine;
	public static GameController gameController;


	[HideInInspector] public Board board;

	[SerializeField] public GameUI gameUI;

	private Vector3 cameraLerp;


	List <string[]> levels = new List<string[]>();

	private int currentLevel = 0;


	private void setupLevels()
	{
		levels.Add (new string[]
		{
			"1EO",
			"1O11",
			"O1O",
			"OKO"
		});

		levels.Add (new string[] {"K#OOOE", 
			                            "11#11111"});

		levels.Add (new string[] {"K#OOE", 
			                            "H##"});
		
		levels.Add (new string[] {"##K#O#O", 
			                            "O#H#O##",
			                            "#####O",
			                            "EO####"
		});

	}

	// Use this for initialization
	void Start () {

		engine = GetComponent<Engine> () as Engine;
		board = GetComponent<Board>() as Board;
		gameController = GetComponent<GameController>() as GameController;

		PieceInteraction.game = this;

		setupLevels ();
		board.Setup ();
		
		//GameData.choosedLevel = 0;
		setLevel(GameData.choosedLevel);	

	}


	public void setLevel(int level)
	{

		currentLevel = level;	

		Debug.Log ("Setup level "+currentLevel);
		board.CreateByMap (levels [currentLevel]);
		gameController.startLevel();

		gameUI.Txt_level.text = "Уровень: "+ (currentLevel+1);
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
