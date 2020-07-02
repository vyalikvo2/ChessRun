using UnityEngine;
using ChessRun.Board;

public class InteractiveMove
{
    public Vector2 move;
    public InteractionType interactionType;
    
    public  InteractiveMove(Vector2 move, InteractionType interactionType)
    {
        this.move = move;
        this.interactionType = interactionType;
    }
}