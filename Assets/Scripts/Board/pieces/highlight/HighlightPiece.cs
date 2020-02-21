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
}
