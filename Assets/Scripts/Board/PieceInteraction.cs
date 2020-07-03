using UnityEngine;

using ChessRun.Board.Pieces;

namespace ChessRun.Board
{
	public enum InteractionType
	{
		NONE,
		END_LEVEL,
		KING_TO_HORSE,
		KING_FROM_HORSE,
		END_LEVEL_FROM_HORSE,
	}

	public class PieceInteraction : MonoBehaviour
	{
		public static GameEngine Game;

		public static InteractionType GetType(Cell c1, Cell c2)
		{
			if (c2.Piece)
			{
				switch (c1.Piece.Type)
				{
					case TypePiece.KING:
						if (c2.Piece.Type == TypePiece.HORSE) return InteractionType.KING_TO_HORSE;
						else if (c2.Piece.Type == TypePiece.BUILDING_HOME) return InteractionType.END_LEVEL;
						break;
					case TypePiece.KING_HORSE:
						if(c2.Piece.Type == TypePiece.BUILDING_HOME) return InteractionType.END_LEVEL_FROM_HORSE;
						break;
				}
			}
			else // NO PIECE IN NEXT CELL INTERACTIONS
			{
				if (c1.Piece.Type == TypePiece.KING_HORSE)
				{
					for (int i = 0; i < c1.Piece.InteractiveMoves.Count; i++)
					{
						if (c2.Pos - c1.Pos == c1.Piece.InteractiveMoves[i].Move)
						{
							return c1.Piece.InteractiveMoves[i].InteractionType;
						}
					}
				}
			}

			return InteractionType.NONE;
		}

		public static void Interact(Cell c1, Cell c2)
		{
			InteractionType type = GetType(c1, c2);

			if (type == InteractionType.KING_TO_HORSE)
			{
				c1.Piece.zIndex = ZIndex.UI_ANIMATING_PIECE_MOVE;
				c1.Piece.Stats.visible = false;
				GameEngine.GameController.MoveAndScalePieceCallback(c1.Piece, c2.Pos, 0.6f, Vector3.zero, delegate
				{
					BasePiece newPiece = Game.Board.CreatePieceFromChar(TypePiece.KING_HORSE);
					newPiece.Stats.attack = c1.Piece.Stats.attack + c2.Piece.Stats.attack;
					newPiece.Stats.health = c1.Piece.Stats.health + c2.Piece.Stats.health;

					_destroyPiece(c1.Piece);
					_destroyPiece(c2.Piece);
					c1.Piece = null;
					c2.Piece = null;

					newPiece.SetBoardPosition(c2.Pos);
					c2.Piece = newPiece;

					newPiece.Relation = PieceRelation.SELF;

					GameEngine.GameController.MomentInteractionComplete();

				});
			}
			else if (type == InteractionType.KING_FROM_HORSE)
			{
				BasePiece newPiece2 = Game.Board.CreatePieceFromChar(TypePiece.KING);
				BasePiece newPiece1 = Game.Board.CreatePieceFromChar(TypePiece.HORSE);

				int health = c1.Piece.Stats.health;

				if (health == 1)
				{
					newPiece2.Stats.health = 0;
					newPiece1.Stats.health = 1;
				}
				else
				{
					int maxHealth = newPiece1.Stats.health;
					newPiece1.Stats.health = (int) health / 2;
					if (newPiece1.Stats.health > maxHealth)
					{
						newPiece1.Stats.health = maxHealth;
					}

					newPiece2.Stats.health = health - newPiece1.Stats.health;
				}


				_destroyPiece(c1.Piece);

				c1.Piece = newPiece1;
				c2.Piece = newPiece2;

				newPiece1.pos = c1.Pos;
				newPiece2.pos = c1.Pos;

				GameEngine.GameController.MovePieceTo(newPiece2, c2.Pos);

				newPiece1.Relation = PieceRelation.SELF;
				newPiece2.Relation = PieceRelation.SELF;
			}
		}

		private static void _destroyPiece(BasePiece p)
		{
			p.Destructor();
			Destroy(p.gameObject);
		}
	}
	
}