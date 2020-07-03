using UnityEngine;

namespace ChessRun.Board.Pieces.Highlight
{
	public class HighlightPiece : BasePiece
	{

		[SerializeField] public Sprite CellHighlighted;
		[SerializeField] public Sprite CellNext;
		[SerializeField] public Sprite CellCurrent;
		[SerializeField] public Sprite CellAttack;

		public override void Setup(Vector2 pos)
		{
			base.Setup(pos);
		}
	}
}