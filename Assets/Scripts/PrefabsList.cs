using UnityEngine;
using System.Collections;

public class PrefabsList : MonoBehaviour {


	public static GameObject emptySprite;
	[SerializeField]
	public GameObject _emptySprite;
	
	public static GameObject emptyObject;
	[SerializeField]
	public GameObject _emptyObject;

	public static GameObject king;
	[SerializeField]
	public GameObject _king;

	public static GameObject pawn;
	[SerializeField]
	public GameObject _pawn;

	public static GameObject horse;
	[SerializeField]
	public GameObject _horse;

	public static GameObject home;
	[SerializeField]
	public GameObject _home;

	public static GameObject king_horse;
	[SerializeField]
	public GameObject _king_horse;

	public static GameObject cell_highlighted;
	[SerializeField]
	public GameObject _cell_highlighted;

	public static GameObject UI_Enemy_Healthbar;
	[SerializeField]
	public GameObject _UI_Enemy_Healthbar;


	
	void Start()
	{
		emptySprite = _emptySprite;
		emptyObject = _emptyObject;
		king = _king;
		cell_highlighted = _cell_highlighted;
		pawn = _pawn;
		home = _home;
		horse = _horse;
		king_horse = _king_horse;
		UI_Enemy_Healthbar = _UI_Enemy_Healthbar;

	}


}
