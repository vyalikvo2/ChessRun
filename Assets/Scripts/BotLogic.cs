using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class BotMove
{
    public const int MOVE = 0;
    public const int MOVE_DEFEND_ATTACK = 1;
    public const int MOVE_ATTACK = 2;
    public const int MOVE_ATTACK_HELP = 3;
    
    public Cell cellFrom;
    public Cell cellTo;
    public int type = MOVE;
    public int score = 0;

    public BotMove(Cell cellFrom, Cell cellTo, int type, int score = 0)
    {
        this.cellFrom = cellFrom;
        this.cellTo = cellTo;
        this.type = type;
        this.score = score;
    }
}
public class BotLogic : MonoBehaviour
{
    private Board board;

    private Game game;
    // Start is called before the first frame update
    void Start()
    {
        board = GetComponent<Board>();
        game = GetComponent<Game>();
    }

    public void makeBotMove()
    {
        List<BotMove> moveList = new List<BotMove>();
        
        for (int i = 0; i < Board.H; i++)
        {
            for (int j = 0; j < Board.W; j++)
            {
                Cell cell = board.cells[i, j];
                if (!cell) continue;
                if (!cell.piece) continue;
                if (cell.piece.relation != Relation.ENEMY) continue;

                BasePiece piece = cell.piece;

                for (int k = 0; k < piece.movesAttack.Count; k++)
                {
                    Vector2 attackDir = piece.movesAttack[k];
                    attackDir.y = -attackDir.y; // ENEMY ATTACK DIRECTION
                    Vector2 attackPos = piece.pos + attackDir;
                    if (attackPos.x < 0 || attackPos.y < 0 || attackPos.x >= Board.W || attackPos.y > Board.H) continue;
                    Cell attackCell = board.getCellAt(attackPos);
                    
                    if (!attackCell) continue;
                    
                    if (attackCell.hasFight)
                    {
                        if (attackCell.piece.relation == Relation.ENEMY)
                        {
                            BotMove defendAttackMove = new BotMove(cell, attackCell, BotMove.MOVE_DEFEND_ATTACK, 5);
                            moveList.Add(defendAttackMove);
                        }
                        else
                        {
                            BotMove attackHelpMove = new BotMove(cell, attackCell, BotMove.MOVE_ATTACK_HELP, 4);
                            moveList.Add(attackHelpMove);
                        }
                        
                    }
                    else if(attackCell.piece && attackCell.piece.relation == Relation.SELF)
                    {
                        int score = 3;
                        if (attackCell.piece.type == TypePiece.KING || attackCell.piece.type == TypePiece.KING_HORSE)
                        {
                            score = 10 + piece.stats.attack;
                        }
                        BotMove attackMove = new BotMove(cell, attackCell, BotMove.MOVE_ATTACK, score);
                        moveList.Add(attackMove);
                    }
                }
            }
        }

        if (moveList.Count > 0)
        {
            moveList.Sort(delegate(BotMove x, BotMove y)
            {
                if (x.score > y.score) return -1;
                else if (x.score <= y.score) return 1;
                else return 0;
            });
            
            BotMove nextMove = moveList[0];
            processBotMove(nextMove);

        }
        else
        {
            Game.gameController.endMove(); 
        }
    }

    private void processBotMove(BotMove move)
    {
        Game.gameController.beginBotMove();
        Debug.Log("Process bot move " + move.type);
        if (move.type == BotMove.MOVE_DEFEND_ATTACK)
        {
            board.fight.animateAllyHelpDefend(move.cellFrom, move.cellTo);
        } 
        else if (move.type == BotMove.MOVE_ATTACK_HELP)
        {
            Debug.Log(move.cellFrom.pos+ " -> "+move.cellTo.pos);
            board.fight.animateAllyHelpAttack(move.cellFrom, move.cellTo);
        }
        else if (move.type == BotMove.MOVE_ATTACK)
        {
            Debug.Log(move.cellFrom.pos+ " -> "+move.cellTo.pos);
            board.fight.beginAttack(move.cellFrom, move.cellTo);
        }
        
    }

}
