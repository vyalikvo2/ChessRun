using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class Relation {
	public static int SELF = 0;
	public static int ENEMY = 1;
	public static int NEURAL = 2;
	public static int BUILDING = 3;
}

public class BasePiece : MonoBehaviour
{
	public int relation = Relation.SELF;
	public char type	= TypePiece.NONE;

	private Vector2 _pos;
	public Vector2 pos {
		get { return _pos; }
		set { _pos = value; RefreshPos();}
	}

	private Sprite _currentSprite;
	[HideInInspector] public Sprite currentSprite { 
		get { return _currentSprite; } 
		set { 
			if((GetComponent<SpriteRenderer> () as SpriteRenderer).sprite != value){
				(GetComponent<SpriteRenderer> () as SpriteRenderer).sprite = value;
				_currentSprite = value;
			}
		}
	}

	public List<Vector2> moves;
	public List<Vector2> movesAttack;

	private int _zIndex = 0;
	public int zIndex { get { return _zIndex; } set { _zIndex = value; GetComponent<SpriteRenderer>().sortingOrder = _zIndex; } }

	public virtual void Setup(Vector2 pos)
	{
		zIndex = ZIndex.PIECES_MY;
		this.pos = pos;
	}

	// recalculate setter (cause of mouse dragging)
	public void RefreshPos()
	{
		transform.position = new Vector3 (pos.x * Game.POS_TO_COORDS, pos.y * Game.POS_TO_COORDS, 0);
	}

	// need to be overrided ( returns interactiontype )
	public virtual bool isInteractableWith(BasePiece piece)
	{
		return false;
	}

	public void Destructor()
	{
	}

}
