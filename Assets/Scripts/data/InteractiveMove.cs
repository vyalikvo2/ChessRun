using UnityEngine;

public class InteractiveMove
{
    public Vector2 move;
    public string interactionType;
    
    public  InteractiveMove(Vector2 move, string interactionType)
    {
        this.move = move;
        this.interactionType = interactionType;
    }
}