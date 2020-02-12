using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

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
	public List<InteractiveMove> interactiveMoves;

	public PieceStats stats;
	protected Vector3 positionTo;

	private float scaleTo = -1; // animation parameter

	private int _zIndex = 0;

	public int zIndex
	{
		get { return _zIndex; } 
		set { 
			_zIndex = value; 
			GetComponent<SpriteRenderer>().sortingOrder = _zIndex;
			if(stats)
				stats.zIndex = _zIndex;
		}
	}

	public virtual void Setup(Vector2 pos)
	{
		setBoardPosition(pos);
		createStats();
		
		zIndex = ZIndex.PIECES_MY;
	} 
	public void createStats()
	{
		GameObject statsPrefab = Resources.Load("prefabs/PieceStats") as GameObject;
		GameObject statsObj = Instantiate(statsPrefab, gameObject.transform) as GameObject;
		stats = statsObj.GetComponent<PieceStats>();
		stats.health = 10;
		stats.attack = 3;
		stats.transform.localPosition = new Vector3(0,0,0);
	}

	// recalculate setter (cause of mouse dragging)
	public virtual void RefreshPos()
	{
		positionTo = new Vector3 (pos.x * Game.POS_TO_COORDS, pos.y * Game.POS_TO_COORDS, 0);
	}

	public virtual void setBoardPosition(Vector2 pos, bool animate = false)
	{
		this.pos = pos;
		if(!animate) transform.localPosition = new Vector3 (pos.x * Game.POS_TO_COORDS, pos.y * Game.POS_TO_COORDS, 0);
	}

	public virtual void animateScale(float from, float to)
	{
		transform.localScale = Vector3.one * from;
		scaleTo = to;
	}

	// need to be overrided ( returns interactiontype )
	public virtual bool isInteractableWith(BasePiece piece)
	{
		return false;
	}

	private void Update()
	{
		if (positionTo != null)
		{
			transform.localPosition = Vector3.Lerp(transform.localPosition, positionTo, 0.20f);
		}

		if (scaleTo != -1)
		{
			transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one * scaleTo, 0.20f);
		}
	}

	public void Destructor()
	{
	}

}
