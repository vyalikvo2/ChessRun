using UnityEngine;
using System.Collections.Generic;

namespace ChessRun.Board.Pieces
{
	public class Home : BasePiece
	{
		public override void Setup(Vector2 pos)
		{
			type = TypePiece.BUILDING_HOME;

			moves = new List<Vector2>()
			{

			};

			movesAttack = new List<Vector2>()
			{
			};

			base.Setup(pos);
		}

		public override bool isInteractableWith(BasePiece piece)
		{
			return piece.type == TypePiece.KING_HORSE || piece.type == TypePiece.KING;
		}

	}
}
