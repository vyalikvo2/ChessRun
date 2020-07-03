using UnityEngine;
using System.Collections;

public class PrefabsList : MonoBehaviour {


	public static GameObject EmptySprite;
	[SerializeField]
	public GameObject _emptySprite;
	
	public static GameObject EmptyObject;
	[SerializeField]
	public GameObject _emptyObject;

	public static GameObject King;
	[SerializeField]
	public GameObject _king;

	public static GameObject pawn;
	[SerializeField]
	public GameObject _pawn;

	public static GameObject Horse;
	[SerializeField]
	public GameObject _horse;

	public static GameObject Home;
	[SerializeField]
	public GameObject _home;

	public static GameObject KingHorse;
	[SerializeField]
	public GameObject _king_horse;

	public static GameObject CellHightlighted;
	[SerializeField]
	public GameObject _cell_highlighted;

	public static GameObject UiEnemyHealthbar;
	[SerializeField]
	public GameObject _UI_Enemy_Healthbar;

	
	void Start()
	{
		EmptySprite = _emptySprite;
		EmptyObject = _emptyObject;
		King = _king;
		CellHightlighted = _cell_highlighted;
		pawn = _pawn;
		Home = _home;
		Horse = _horse;
		KingHorse = _king_horse;
		UiEnemyHealthbar = _UI_Enemy_Healthbar;
	}


}
