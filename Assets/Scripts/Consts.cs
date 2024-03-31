public enum ChessType
{
    NONE,
    WHITE,
    BLACK
}

public enum AIType
{
    EASY,
    MIDDLE,
    DIFFICULT
}

public struct ChessPos
{
    public int x;
    public int y;

    public static ChessPos zero = new ChessPos(0, 0);
    public static ChessPos none = new ChessPos(-1, -1);
    public ChessPos(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public static bool operator ==(ChessPos c1, ChessPos c2)
    {
        return c1.x == c2.x && c1.y == c2.y;
    }

    public static bool operator !=(ChessPos c1, ChessPos c2)
    {
        return c1.x != c2.x || c1.y != c2.y;
    }

    public static ChessPos operator +(ChessPos c1, ChessPos c2)
    {
        return new ChessPos(c1.x + c2.x, c1.y + c2.y);
    }

    public override bool Equals(object obj)
    {
        return base.Equals(obj);
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public override string ToString()
    {
        return $"[{x}-{y}]";
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
    ON_AI_START,
    ON_AI_FINISHED,
}



