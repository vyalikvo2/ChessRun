using UnityEngine;
using System.Collections.Generic;

namespace ChessRun.Board.Pieces
{
	public class Home : BasePiece
	{
		public override void Setup(Vector2 pos)
		{
			Type = TypePiece.BUILDING_HOME;

			Moves = new List<Vector2>()
			{

			};

			MovesAttack = new List<Vector2>()
			{
			};

			base.Setup(pos);
		}

		public override bool IsInteractableWith(BasePiece piece)
		{
			return piece.Type == TypePiece.KING_HORSE || piece.Type == TypePiece.KING;
		}

	}
}
