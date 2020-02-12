using UnityEngine;
using System.Collections;

public class HighlightPiece : BasePiece {
	
	[SerializeField] public Sprite cell_highlighted;
	[SerializeField] public Sprite cell_next;
	[SerializeField] public Sprite cell_current;
	[SerializeField] public Sprite cell_attack;

	public override void Setup(Vector2 pos)
	{
		base.Setup (pos);
	}
	
	public override void RefreshPos()
	{
		//transform.localPosition = new Vector3 (pos.x * Game.POS_TO_COORDS, pos.y * Game.POS_TO_COORDS, 0);
		positionTo = new Vector3 (pos.x * Game.POS_TO_COORDS, pos.y * Game.POS_TO_COORDS, 0);
	}
	
	public override void setBoardPosition(Vector2 pos, bool animate = false)
	{
		this.pos = pos;
		if(!animate) transform.localPosition = new Vector3 (pos.x * Game.POS_TO_COORDS, pos.y * Game.POS_TO_COORDS, 0);
	}

}
