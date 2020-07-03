using UnityEngine;
using System.Collections.Generic;

namespace ChessRun.Board.Pieces
{
	public class Horse : BasePiece
	{
		public override void Setup(Vector2 pos)
		{
			Moves = new List<Vector2>()
			{
				new Vector2(-1, -2), // bottom left
				new Vector2(-1, 2), // top left
				new Vector2(2, -1), // bottom right
				new Vector2(2, 1), // top right
				new Vector2(-2, 1), // top
				new Vector2(-2, -1), // bottom
				new Vector2(1, -2), // left
				new Vector2(1, 2) // right
			};

			MovesAttack = Moves;

			Type = TypePiece.HORSE;
			CanJump = true;

			base.Setup(pos);

			CreateStats();
			Stats.health = 2;
			Stats.attack = 2;
		}

		public override bool IsInteractableWith(BasePiece piece)
		{
			return piece.Type == TypePiece.KING;
		}

	}
}
