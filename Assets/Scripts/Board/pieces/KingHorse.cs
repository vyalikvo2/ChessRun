﻿using UnityEngine;
using System.Collections.Generic;

namespace ChessRun.Board.Pieces
{
	public class KingHorse : BasePiece
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

			InteractiveMoves = new List<InteractiveMove>()
			{
				new InteractiveMove(new Vector2(-1, 1), InteractionType.KING_FROM_HORSE), // top left
				new InteractiveMove(new Vector2(1, -1), InteractionType.KING_FROM_HORSE), // bottom right
				new InteractiveMove(new Vector2(-1, -1), InteractionType.KING_FROM_HORSE), // bottom left
				new InteractiveMove(new Vector2(1, 1), InteractionType.KING_FROM_HORSE), // top right
				new InteractiveMove(new Vector2(0, 1), InteractionType.KING_FROM_HORSE), // top
				new InteractiveMove(new Vector2(0, -1), InteractionType.KING_FROM_HORSE), // bottom
				new InteractiveMove(new Vector2(-1, 0), InteractionType.KING_FROM_HORSE), // left
				new InteractiveMove(new Vector2(1, 0), InteractionType.KING_FROM_HORSE) // right
			};

			Type = TypePiece.KING_HORSE;
			CanJump = true;

			base.Setup(pos);

			CreateStats();
			Stats.health = 2;
			Stats.attack = 2;
		}
	}
}
