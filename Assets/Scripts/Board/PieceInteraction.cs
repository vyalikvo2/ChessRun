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
		public static Game game;

		public static InteractionType getType(Cell c1, Cell c2)
		{
			if (c2.piece)
			{
				if (c1.piece.type == TypePiece.KING && c2.piece.type == TypePiece.HORSE)
				{
					return InteractionType.KING_TO_HORSE;
				}
				else if (c1.piece.type == TypePiece.KING && c2.piece.type == TypePiece.BUILDING_HOME)
				{
					return InteractionType.END_LEVEL;
				}
				else if (c1.piece.type == TypePiece.KING_HORSE && c2.piece.type == TypePiece.BUILDING_HOME)
				{
					return InteractionType.END_LEVEL_FROM_HORSE;
				}
			}
			else // NO PIECE IN NEXT CELL INTERACTIONS
			{
				if (c1.piece.type == TypePiece.KING_HORSE)
				{
					for (int i = 0; i < c1.piece.interactiveMoves.Count; i++)
					{
						if (c2.pos - c1.pos == c1.piece.interactiveMoves[i].move)
						{
							return c1.piece.interactiveMoves[i].interactionType;
						}
					}
				}
			}

			return InteractionType.NONE;
		}

		public static void interact(Cell c1, Cell c2)
		{
			InteractionType type = getType(c1, c2);

			if (type == InteractionType.KING_TO_HORSE)
			{
				c1.piece.zIndex = ZIndex.UI_ANIMATING_PIECE_MOVE;
				c1.piece.stats.visible = false;
				Game.gameController.moveAndScalePieceCallback(c1.piece, c2.pos, 0.6f, Vector3.zero, delegate
				{
					BasePiece newPiece = game.board.createPieceFromChar(TypePiece.KING_HORSE);
					newPiece.stats.attack = c1.piece.stats.attack + c2.piece.stats.attack;
					newPiece.stats.health = c1.piece.stats.health + c2.piece.stats.health;

					destroyPiece(c1.piece);
					destroyPiece(c2.piece);
					c1.piece = null;
					c2.piece = null;

					newPiece.setBoardPosition(c2.pos);
					c2.piece = newPiece;

					newPiece.relation = Relation.SELF;

					Game.gameController.momentInteractionComplete();

				});
			}
			else if (type == InteractionType.KING_FROM_HORSE)
			{
				BasePiece newPiece2 = game.board.createPieceFromChar(TypePiece.KING);
				BasePiece newPiece1 = game.board.createPieceFromChar(TypePiece.HORSE);

				int health = c1.piece.stats.health;

				if (health == 1)
				{
					newPiece2.stats.health = 0;
					newPiece1.stats.health = 1;
				}
				else
				{
					int maxHealth = newPiece1.stats.health;
					newPiece1.stats.health = (int) health / 2;
					if (newPiece1.stats.health > maxHealth)
					{
						newPiece1.stats.health = maxHealth;
					}

					newPiece2.stats.health = health - newPiece1.stats.health;
				}


				destroyPiece(c1.piece);

				c1.piece = newPiece1;
				c2.piece = newPiece2;

				newPiece1.pos = c1.pos;
				newPiece2.pos = c1.pos;

				Game.gameController.movePieceTo(newPiece2, c2.pos);

				newPiece1.relation = Relation.SELF;
				newPiece2.relation = Relation.SELF;
			}
		}

		public static void destroyPiece(BasePiece p)
		{
			p.gameObject.transform.SetParent(null);
			p.Destructor();
			Destroy(p.gameObject);
		}
	}
	
}