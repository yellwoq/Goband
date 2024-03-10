public enum ChessType
{
    NONE,
    WHITE,
    BLACK
}

public struct ChessPos
{
    public int x;
    public int y;

    public static ChessPos zero = new ChessPos(0, 0);
    public ChessPos(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
}

public enum EntityType
{
    Chess, // Æå×Ó
    ChessBoard, //ÆåÅÌ
}

public class UIConsts
{
    public const int DEFAULT_LEFT_X_POS = 60;
    public const int DEFAULT_X_OFFSET_POS = 56;
    public const int DEFAULT_UP_Y_POS = -40;
    public const int DEFAULT_Y_OFFSET_POS = -43;
    public const float SCREEN_DETAL_CHECK = 5;
}

public enum EventType
{
    GetRefrenceSolution,
    OnScreenChanged,
    GameStateChanged,
}



