using UnityEngine;
using ChessRun.Board;

public class InteractiveMove
{
    public Vector2 Move;
    public InteractionType InteractionType;
    
    public  InteractiveMove(Vector2 move, InteractionType interactionType)
    {
        Move = move;
        InteractionType = interactionType;
    }
}