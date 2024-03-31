using System.Collections.Generic;
using System;
using System.Linq;
using System.Text;
using UnityEngine;

/// <summary>
/// ��������
/// </summary>
public sealed class EvalateConst
{
    public const int WIN = 10000;
    //����Ϊo������Ϊx, �Ӻ������
    //���� xoox
    public const int DEEP_DEATH2 = 2;

    //�ͼ����� xoo
    public const int LOWER_DEATH2 = 4;
    //���� xooox
    public const int DEEP_DEATH3 = 3;
    //�ͼ����� xooo
    public const int LOWER_DEATH3 = 6;

    //���� xoooox
    public const int DEEP_DEATH4 = 4;
    //�ͼ����� xoooo
    public const int LOWER_DEATH4 = 32;

    //��� oo
    public const int ALIVE2 = 10;
    //����� o o
    public const int JUMP_ALIVE2 = 2;
    //���� ooo
    public const int ALIVE3 = 100;
    //������ oo o
    public const int JUMP_ALIVE3 = 10;
    //���� oooo
    public const int ALIVE4 = 5000;
    //������ ��1��3����3��1��2��2�� o ooo || ooo o || oo oo
    public const int JUMP_ALIVE4 = 90;
}

/// <summary>
/// ���壨��¼������ε�����״̬��
/// </summary>
/// <typeparam name="V">����ƫ��</typeparam>
public class ChessBufferMap
{
    //���û�����Ϊ3
    private int maxCount = 3;
    public ChessBufferMap(int maxCount)
    {
        this.maxCount = maxCount;
    }

    private Dictionary<int, ChessPos> buffer = new Dictionary<int, ChessPos>();

    public ChessPos this[int key] {
        get { return buffer[key]; }
        set { buffer[key] = value; }
    
    }


    // ���Ԫ�أ�key�����ÿ�����ӵķ�����value��ÿ�����ӵ�offset��
    public void Put(int key, ChessPos value)
    {
        if(!buffer.ContainsKey(key))
        {
            buffer[key] = value;
        }
        buffer[key] = value;
        _CheckSize();
    }

    // �������Ԫ��
    public void PutAll(ChessBufferMap map)
    {
        foreach(var entry in map.buffer)
        {
            Put(entry.Key, entry.Value);
        }
    }

    // ��鲢������������С
    void _CheckSize()
    {
        //�������������м�ת�����б�����ֵ������ list�����մӴ�С����
        var list = buffer.Keys.ToList();
        list.Sort((a, b) => {
            return b.CompareTo(a);
        });
        while (buffer.Count > maxCount)
        {
            buffer.Remove(list.Last());
        }
    }

    // ��������תΪMap
    public Dictionary<int, ChessPos> ToMap()
    {
        return buffer;
    }

    // ��ȡ����Ԫ�ص�ֵ
    IEnumerable<ChessPos> Values()
    {
        return buffer.Values;
    }

    public List<ChessPos> ToList()
    {
        return buffer.Values.ToList();
    }

    // ��ȡ����Ԫ�ظ���
    public int Size()
    {
        return buffer.Count;
    }

    // תΪ�ַ�����ʾ
    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("{");
        var keys = buffer.Keys.ToList();
        keys.Sort((a, b) => {
            return b.CompareTo(a);
        });

        foreach (var i in keys)
        {
            sb.Append($"[{i} , {buffer[i]}] ,");
        }

        return $"{sb.ToString().Substring(0, sb.ToString().Length - 2)}";
     }

    // ��ȡ��һ��Ԫ�ص�ֵ
    public ChessPos GetFirst()
    {
        var keys = buffer.Keys.ToList();
        keys.Sort((a, b) => {
            return b.CompareTo(a);
        });
        return buffer[keys.First()];
    }

    // ��ȡ������Сֵ
    public int MinKey()
    {
        if (buffer.Count==0)
        {
            return int.MinValue;
        }
        var list = buffer.Keys.ToList();
        list.Sort((a, b) => {
            return b.CompareTo(a);
        });
        return list.Last();
    }

    // ��ȡ��ֵ��С��Ԫ��
    public ChessPos Min()
    {
        int min_key = MinKey();
        if (!buffer.ContainsKey(min_key))
        {
            return ChessPos.none;
        }
        return buffer[min_key];
    }

    // ��ȡ���м����б�
    public List<int> GetkeySet()
    {
        if (buffer.Count==0) return new List<int>();

        var sortedKeys = buffer.Keys.ToList();
        sortedKeys.Sort((a, b) => {
            return (b - a);
        });

        return sortedKeys;
    }

    // ��ȡ�������ֵ
    // ����λ�õ÷�
    public int MaxKey()
    {
        if (buffer.Count==0)
        {
            return int.MinValue;
        }
        var list = buffer.Keys.ToList();
        list.Sort((a, b) => {
            return b.CompareTo(a);
        });
        return list.First();
      }


    // ��ȡ��ֵ����Ԫ��
    public ChessPos Max()
    {
        int max_key = MaxKey();
        if (!buffer.ContainsKey(max_key))
        {
            return ChessPos.none;
        }
        return buffer[max_key];
    }
}

public class BufferChessmanList
{
    List<ChessInfo> buffer = new List<ChessInfo>();
    int maxCount = 5;

    public void Add(ChessInfo chessman)
    {
        buffer.Add(chessman);
        _checkSize();
    }

    public BufferChessmanList(int maxCount)
    {
        this.maxCount = maxCount;
    }

    void _checkSize()
    {
        buffer.Sort((a, b) => {
            return b.score - a.score;
        });

        while (buffer.Count > maxCount)
        {
            buffer.Remove(buffer.Last());
        }
    }

    public List<ChessPos> ToList()
    {
        List<ChessPos> list = new List<ChessPos>();
        foreach (ChessInfo c in buffer)
        {
            list.Add(c.chess_pos);
        }
        return list;
    }

    public List<int> ToScoreList()
    {
        List<int> list = new List<int>();
        foreach (ChessInfo c in buffer)
        {
            list.Add(c.score);
        }
        return list;
    }

    public int MinScore()
    {
        if (buffer.Count == 0)
        {
            return int.MinValue;
        }
        buffer.Sort((a, b) => {
            return b.score - a.score;
        });
        return buffer.Last().score;
    }
}

/// <summary>
/// �����ж�������������
/// </summary>
public static class CheckCurrentStateHelper
{

    #region ����״̬�ж�
    /// <summary>
    /// ���� xoo
    /// </summary>
    /// <param name="board"></param>
    /// <param name="list"></param>
    /// <returns></returns>
    static bool IsLowerDeath2(ChessBoard board, List<ChessPos> list)
    {
        if (list.Count != 2) return false;
        ChessPos offset1 = NextChessman(list[1], list[0]);
        ChessPos offset2 = NextChessman(list[0], list[1]);
        return (IsEffectivePosition(offset1) && IsBlankPosition(board, offset1)) ||
            (IsEffectivePosition(offset2) && IsBlankPosition(board, offset2));
    }

    /// <summary>
    /// ��� oo
    /// </summary>
    /// <param name="list"></param>
    /// <returns></returns>
    static bool IsAlive2(ChessBoard board, List<ChessPos> list)
    {
        if (list.Count != 2) return false;
        //���������Ӵ���
        ChessPos offset1 = NextChessman(list[1], list[0]);
        ChessPos offset2 = NextChessman(list[0], list[1]);

        return IsEffectivePosition(offset1) &&
            IsEffectivePosition(offset2) &&
            IsBlankPosition(board, offset1) &&
            IsBlankPosition(board, offset2);
    }

    /// <summary>
    /// ���� xooo
    /// </summary>
    /// <param name="list"></param>
    /// <returns></returns>
    static bool IsLowerDeath3(ChessBoard board, List<ChessPos> list)
    {
        if (list.Count != 3) return false;
        ChessPos offset1 = NextChessman(list[1], list[0]);
        ChessPos offset2 = NextChessman(list[1], list[2]);
        return (IsEffectivePosition(offset1) && IsBlankPosition(board, offset1)) ||
            (IsEffectivePosition(offset2) && IsBlankPosition(board, offset2));
    }

    /// <summary>
    /// ���� ooo
    /// </summary>
    /// <param name="list"></param>
    /// <returns></returns>
    static bool IsAlive3(ChessBoard board, List<ChessPos> list)
    {
        if (list.Count != 3) return false;

        ChessPos offset1 = NextChessman(list[1], list[0]);
        ChessPos offset2 = NextChessman(list[1], list[2]);
        return (IsEffectivePosition(offset1) && IsBlankPosition(board, offset1)) &&
            (IsEffectivePosition(offset2) && IsBlankPosition(board, offset2));
    }

    /// <summary>
    /// ���� xoooo
    /// </summary>
    /// <param name="list"></param>
    /// <returns></returns>
    static bool IsLowerDeath4(ChessBoard board, List<ChessPos> list)
    {
        if (list.Count != 4) return false;

        ChessPos offset1 = NextChessman(list[1], list[0]);
        ChessPos offset2 = NextChessman(list[2], list[3]);
        return (IsEffectivePosition(offset1) && IsBlankPosition(board, offset1)) ||
            (IsEffectivePosition(offset2) && IsBlankPosition(board, offset2));
    }

    /// <summary>
    /// ���� oooo
    /// </summary>
    /// <param name="list"></param>
    /// <returns></returns>
    static bool IsAlive4(ChessBoard board, List<ChessPos> list)
    {
        if (list.Count != 4) return false;

        ChessPos offset1 = NextChessman(list[1], list[0]);
        ChessPos offset2 = NextChessman(list[2], list[3]);
        return (IsEffectivePosition(offset1) && IsBlankPosition(board, offset1)) &&
            (IsEffectivePosition(offset2) && IsBlankPosition(board, offset2));
    }

    /// <summary>
    /// ��ȡ����2����
    /// </summary>
    /// <param name="board"></param>
    /// <param name="list"></param>
    /// <param name="chessType"></param>
    /// <returns></returns>
    static int GetJumpAlive2Count(ChessBoard board, List<ChessPos> list, ChessType chessType)
    {
        if (list.Count != 1)
        {
            return 0;
        }
        int count = 0;

        if (list.First().x >= 3)
        {
            //�߽磬 ˮƽ���� ��
            ChessPos left = new ChessPos(list.First().x - 2, list.First().y);
            count += ExistSpecificChessman(board, left, chessType) &&
            IsAllBlankPosition(board, new List<ChessPos>() {
                new ChessPos(list.First().x + 1, list.First().y),
                new ChessPos(left.x - 1, left.y)
            }) ? 1 : 0;
        }

        int LINE_COUNT = board.board_y_size;
        if (list.First().x <= LINE_COUNT - 4)
        {
            //�߽磬 ˮƽ���� ��
            ChessPos right = new ChessPos(list.First().x + 2, list.First().y);
            count += ExistSpecificChessman(board, right, chessType) &&
            IsAllBlankPosition(board, new List<ChessPos>(){
                new ChessPos(list.First().x - 1, list.First().y),
                new ChessPos(right.x + 1, right.y)
            }) ? 1 : 0;
        }

        if (list.First().y >= 3)
        {
            //�߽磬 ��ֱ���� ��
            ChessPos top = new ChessPos(list.First().x, list.First().y - 2);
            count += ExistSpecificChessman(board, top, chessType) &&
            IsAllBlankPosition(board, new List<ChessPos>() {
                new ChessPos(list.First().x, list.First().y + 1),
                new ChessPos(top.x, top.y - 1)
            }) ? 1 : 0;
        }

        int COL_COUNT = board.board_x_size;
        if (list.First().y <= COL_COUNT - 4)
        {
            //�߽磬 ��ֱ���� ��
            ChessPos bottom = new ChessPos(list.First().x, list.First().y + 2);
            count += ExistSpecificChessman(board, bottom, chessType) &&
            IsAllBlankPosition(board, new List<ChessPos>() {
                new ChessPos(list.First().x, list.First().y - 1),
                new ChessPos(bottom.x, bottom.y + 1)
            }) ? 1 : 0;
        }

        if (list.First().x >= 3 && list.First().y >= 3)
        {
            //�߽磬 ���Ϸ��� ��
            ChessPos leftTop = new ChessPos(list.First().x - 2, list.First().y - 2);
            count += ExistSpecificChessman(board, leftTop, chessType) &&
            IsAllBlankPosition(board, new List<ChessPos>(){
                new ChessPos(list.First().x + 1, list.First().y + 1),
                new ChessPos(leftTop.x - 1, leftTop.y - 1)
            }) ? 1 : 0;
        }

        if (list.First().x >= 3 && list.First().y <= COL_COUNT - 4)
        {
            //�߽磬 ���·��� ��
            ChessPos leftBottom = new ChessPos(list.First().x - 2, list.First().y + 2);
            count += ExistSpecificChessman(board, leftBottom, chessType) &&
            IsAllBlankPosition(board, new List<ChessPos>(){
                new ChessPos(list.First().x + 1, list.First().y - 1),
                new ChessPos(leftBottom.x - 1, leftBottom.y + 1)
            }) ? 1 : 0;
        }

        if (list.First().x <= LINE_COUNT - 4 && list.First().y >= 3)
        {
            //�߽磬 ���Ϸ��� ��
            ChessPos rightTop = new ChessPos(list.First().x + 2, list.First().y - 2);
            count += ExistSpecificChessman(board, rightTop, chessType) &&
            IsAllBlankPosition(board, new List<ChessPos>(){
                new ChessPos(list.First().x - 1, list.First().y + 1),
                new ChessPos(rightTop.x + 1, rightTop.y - 1)
            }) ? 1 : 0;
        }

        if (list.First().x <= LINE_COUNT - 4 && list.First().y <= LINE_COUNT - 4)
        {
            //�߽磬 ���·��� ��
            ChessPos rightBottom = new ChessPos(list.First().x + 2, list.First().y + 2);
            count += ExistSpecificChessman(board, rightBottom, chessType) &&
            IsAllBlankPosition(board, new List<ChessPos>(){
                new ChessPos(list.First().x - 1, list.First().y - 1),
                new ChessPos(rightBottom.x + 1, rightBottom.y + 1)
            }) ? 1 : 0;
        }
        return count;
    }

    /// <summary>
    /// ��ȡ����3����
    /// </summary>
    /// <param name="board"></param>
    /// <param name="list"></param>
    /// <param name="chessType"></param>
    /// <returns></returns>
    static int GetJumpAlive3Count(ChessBoard board, List<ChessPos> list, ChessType chessType)
    {
        if (list.Count != 1 && list.Count != 2)
            return 0;
        int count = 0;
        if (list.Count == 1)
        {
            //1��2 ��3
            /// leftBlank left2 left1 blank list.First() rightBlank
            if (list.First().x >= 4)
            {
                //���̱߽�, ˮƽ�� ��
                ChessPos left1 = new ChessPos(list.First().x - 2, list.First().y);
                ChessPos left2 = new ChessPos(left1.x - 1, list.First().y);
                ChessPos blank = new ChessPos(list.First().x - 1, list.First().y);
                ChessPos leftBlank = new ChessPos(left2.x - 1, list.First().y);
                ChessPos rightBlank = new ChessPos(list.First().x + 1, list.First().y);

                count += ExistSpecificChessmanAll(board, new List<ChessPos>() { left1, left2 }, chessType) &&
                IsAllBlankPosition(board, new List<ChessPos>() { blank, leftBlank, rightBlank })
                ? 1 : 0;
            }

            int LINE_COUNT = board.board_y_size;
            ///leftBlank list.First()  blank right1  right2 rightBlank
            if (list.First().x <= LINE_COUNT - 5)
            {
                //���̱߽�, ˮƽ�� ��
                ChessPos leftBlank = new ChessPos(list.First().x - 1, list.First().y);
                ChessPos blank = new ChessPos(list.First().x + 1, list.First().y);
                ChessPos right1 = new ChessPos(blank.x + 1, blank.y);
                ChessPos right2 = new ChessPos(right1.x + 1, blank.y);
                ChessPos rightBlank = new ChessPos(right2.x + 1, blank.y);
                count += ExistSpecificChessmanAll(board, new List<ChessPos>() { right1, right2 }, chessType) &&
                IsAllBlankPosition(board, new List<ChessPos>() { leftBlank, blank, rightBlank })
                ? 1 : 0;
            }

            /// topBlank
            /// top2
            /// top1
            /// blank
            /// list.First()
            /// bottomBlank

            if (list.First().y >= 4)
            {
                //���̱߽�, ��ֱ���� ��
                ChessPos blank = new ChessPos(list.First().x, list.First().y - 1);
                ChessPos top1 = new ChessPos(list.First().x, blank.y - 1);
                ChessPos top2 = new ChessPos(list.First().x, top1.y - 1);
                ChessPos topBlank = new ChessPos(list.First().x, top2.y - 1);
                ChessPos bottomBlank = new ChessPos(list.First().x, list.First().y + 1);
                count += ExistSpecificChessmanAll(board, new List<ChessPos>() { top1, top2 }, chessType) &&
                IsAllBlankPosition(board, new List<ChessPos>() { topBlank, blank, bottomBlank })
                ? 1 : 0;
            }

            int COL_COUNT = board.board_x_size;
            /// topBlank
            /// list.First()
            /// blank
            /// top1
            /// top2
            /// bottomBlank
            if (list.First().y <= COL_COUNT - 5)
            {
                //���̱߽�, ��ֱ���� ��
                ChessPos topBlank = new ChessPos(list.First().x, list.First().y - 1);
                ChessPos blank = new ChessPos(list.First().x, list.First().y + 1);
                ChessPos top1 = new ChessPos(list.First().x, blank.y + 1);
                ChessPos top2 = new ChessPos(list.First().x, top1.y + 1);
                ChessPos bottomBlank = new ChessPos(list.First().x, top2.y + 1);
                count += ExistSpecificChessmanAll(board, new List<ChessPos>() { top1, top2 }, chessType) &&
                IsAllBlankPosition(board, new List<ChessPos>() { topBlank, blank, bottomBlank })
                ? 1 : 0;
            }

            ///����
            /// |leftTopBlank
            /// |          leftTop2
            /// |                   leftTop1
            /// |                           blank
            /// |                                 list.First()
            /// |                                          rightBottomBlank
            if (list.First().x >= 4 && list.First().y >= 4)
            {
                //���̱߽�, ���Ϸ��� ��
                ChessPos rightBottomBlank = new ChessPos(list.First().x + 1, list.First().y + 1);
                ChessPos blank = new ChessPos(list.First().x - 1, list.First().y - 1);
                ChessPos leftTop1 = new ChessPos(blank.x - 1, blank.y - 1);
                ChessPos leftTop2 = new ChessPos(leftTop1.x - 1, leftTop1.y - 1);
                ChessPos leftTopBlank = new ChessPos(leftTop2.x - 1, leftTop2.y - 1);
                count += ExistSpecificChessmanAll(board, new List<ChessPos>() { leftTop1, leftTop2 }, chessType) &&
                IsAllBlankPosition(board, new List<ChessPos>() { rightBottomBlank, blank, leftTopBlank })
            ? 1 : 0;
            }

            ///����
            ///  |                                                 rightTopBlank1
            ///  |                                       list.First()
            ///  |                                   blank
            ///  |                        leftBottom1
            ///  |            leftBottom2
            ///  |leftBottomBlank
            if (list.First().x >= 4 && list.First().y <= COL_COUNT - 5)
            {
                ChessPos rightTopBlank = new ChessPos(list.First().x + 1, list.First().y - 1);
                ChessPos blank = new ChessPos(list.First().x - 1, list.First().y + 1);
                ChessPos leftBottom1 = new ChessPos(blank.x - 1, blank.y + 1);
                ChessPos leftBottom2 = new ChessPos(leftBottom1.x - 1, leftBottom1.y + 1);
                ChessPos leftBottomBlank = new ChessPos(leftBottom2.x - 1, leftBottom2.y + 1);
                count += ExistSpecificChessmanAll(board, new List<ChessPos>() { leftBottom1, leftBottom2 }, chessType) &&
                        IsAllBlankPosition(board, new List<ChessPos>() { rightTopBlank, blank, leftBottomBlank })
                ? 1 : 0;
            }

            ///����
            ///                                             rightTopBlank|
            ///                                     rightTop2            |
            ///                             rightTop1                    |
            ///                         blank                            |
            ///               list.First()                                 |
            /// leftBottomBlank                                          |
            if (list.First().x <= LINE_COUNT - 5 && list.First().y >= 4)
            {
                ChessPos leftBottomBlank = new ChessPos(list.First().x - 1, list.First().y + 1);
                ChessPos blank = new ChessPos(list.First().x + 1, list.First().y - 1);
                ChessPos rightTop1 = new ChessPos(blank.x - 1, blank.y + 1);
                ChessPos rightTop2 = new ChessPos(rightTop1.x - 1, rightTop1.y + 1);
                ChessPos rightTopBlank = new ChessPos(rightTop2.x - 1, rightTop2.y + 1);
                count += ExistSpecificChessmanAll(board, new List<ChessPos>() { rightTop1, rightTop2 }, chessType) &&
                IsAllBlankPosition(board, new List<ChessPos>() { leftBottomBlank, blank, rightTopBlank })
                ? 1 : 0;
            }

            ///����
            /// leftTopBlank                                       |
            ///             list.First()                             |
            ///                       blank                        |
            ///                          leftTop1                  |
            ///                                 leftTop2           |
            ///                                        rightBottom |
            ///
            if (list.First().x <= LINE_COUNT - 4 && list.First().y <= COL_COUNT - 4)
            {
                ChessPos leftTopBlank = new ChessPos(list.First().x - 1, list.First().y - 1);
                ChessPos blank = new ChessPos(list.First().x + 1, list.First().y + 1);
                ChessPos leftTop1 = new ChessPos(blank.x + 1, blank.y + 1);
                ChessPos leftTop2 = new ChessPos(leftTop1.x + 1, leftTop1.y + 1);
                ChessPos rightBottom = new ChessPos(leftTop2.x + 1, leftTop2.y + 1);
                count += ExistSpecificChessmanAll(board, new List<ChessPos>() { leftTop1, leftTop2 }, chessType) &&
                IsAllBlankPosition(board, new List<ChessPos>() { leftTopBlank, blank, rightBottom })
                ? 1 : 0;
            }
        }
        else if (list.Count == 2)
        {
            //2��1 ��3
            /// next1Next1Blank next1 next1blank list[0] list[1] next2Blank next2 next2Next2Blank
            ChessPos next1blank = NextChessman(list[1], list[0]);
            ChessPos next1 = NextChessman(list[0], next1blank);
            ChessPos next1Next1Blank = NextChessman(next1blank, next1);
            ChessPos next2Blank = NextChessman(list[0], list[1]);
            ChessPos next2 = NextChessman(list[1], next2Blank);
            ChessPos next2Next2Blank = NextChessman(next2Blank, next2);

            count += ExistSpecificChessman(board, next1, chessType) &&
                    IsAllBlankPosition(board, new List<ChessPos>() { next1Next1Blank, next1blank, next2Blank })
          ? 1 : 0;
            count += ExistSpecificChessman(board, next2, chessType) &&
                    IsAllBlankPosition(board, new List<ChessPos>() { next1blank, next2Blank, next2Next2Blank })
          ? 1 : 0;
        }
        return count;
    }

    /// <summary>
    /// ��ȡ����4����
    /// </summary>
    /// <param name="board"></param>
    /// <param name="list"></param>
    /// <param name="chessType"></param>
    /// <returns></returns>
    static int GetJumpAlive4Count(ChessBoard board, List<ChessPos> list, ChessType chessType)
    {
        if (list.Count <= 0 || list.Count >= 4)
        {
            return 0;
        }
        int count = 0;
        int LINE_COUNT = board.board_y_size;
        int COL_COUNT = board.board_x_size;

        if (list.Count == 1)
        {
            ///��
            ///leftBlank left3 left2 left1 blank list.First() rightBlank
            if (list.First().x >= 5)
            {
                ChessPos rightBlank = new ChessPos(list.First().x + 1, list.First().y);
                ChessPos blank = new ChessPos(list.First().x - 1, list.First().y);
                ChessPos left1 = new ChessPos(blank.x - 1, list.First().y);
                ChessPos left2 = new ChessPos(left1.x - 1, list.First().y);
                ChessPos left3 = new ChessPos(left2.x - 1, list.First().y);
                ChessPos leftBlank = new ChessPos(left3.x - 1, list.First().y);
                count += ExistSpecificChessmanAll(board, new List<ChessPos>() { left1, left2, left3 }, chessType) &&
                IsAllBlankPosition(board, new List<ChessPos>() { rightBlank, blank, leftBlank })
                ? 1 : 0;
            }

            ///��
            ///leftBlank list.First() blank right1 right2 right3 rightBlank
            if (list.First().x <= LINE_COUNT - 6)
            {
                ChessPos leftBlank = new ChessPos(list.First().x - 1, list.First().y);
                ChessPos blank = new ChessPos(list.First().x + 1, list.First().y);
                ChessPos right1 = new ChessPos(blank.x + 1, blank.y);
                ChessPos right2 = new ChessPos(right1.x + 1, blank.y);
                ChessPos right3 = new ChessPos(right2.x + 1, blank.y);
                ChessPos rightBlank = new ChessPos(right3.x + 1, blank.y);
                count += ExistSpecificChessmanAll(board, new List<ChessPos>() { right1, right2, right3 }, chessType) &&
                IsAllBlankPosition(board, new List<ChessPos>() { leftBlank, blank, rightBlank })
                ? 1 : 0;
            }

            ///��
            /// topBlank
            /// top3
            /// top2
            /// top1
            /// blank
            /// list.First()
            /// bottomBlank
            if (list.First().y >= 5)
            {
                ChessPos bottomBlank = new ChessPos(list.First().x, list.First().y + 1);
                ChessPos blank = new ChessPos(list.First().x, list.First().y - 1);
                ChessPos top1 = new ChessPos(blank.x, blank.y - 1);
                ChessPos top2 = new ChessPos(top1.x, blank.y - 1);
                ChessPos top3 = new ChessPos(top2.x, blank.y - 1);
                ChessPos topBlank = new ChessPos(top3.x, blank.y - 1);
                count += ExistSpecificChessmanAll(board, new List<ChessPos>() { top1, top2, top3 }, chessType) &&
                IsAllBlankPosition(board, new List<ChessPos>() { bottomBlank, blank, topBlank })
                ? 1 : 0;
            }

            /// ��
            /// topBlank
            /// list.First()
            /// blank
            /// bottom1
            /// bottom2
            /// bottom3
            /// bottomBlank
            if (list.First().y <= LINE_COUNT - 5)
            {
                ChessPos topBlank = new ChessPos(list.First().x, list.First().y - 1);
                ChessPos blank = new ChessPos(list.First().x, list.First().y + 1);
                ChessPos bottom1 = new ChessPos(blank.x, blank.y + 1);
                ChessPos bottom2 = new ChessPos(bottom1.x, bottom1.y + 1);
                ChessPos bottom3 = new ChessPos(bottom2.x, bottom2.y + 1);
                ChessPos bottomBlank = new ChessPos(bottom3.x, bottom3.y + 1);
                count +=
                ExistSpecificChessmanAll(board, new List<ChessPos>() { bottom1, bottom2, bottom3 }, chessType) &&
                IsAllBlankPosition(board, new List<ChessPos>() { topBlank, blank, bottomBlank })
                ? 1 : 0;
            }

            /// ����
            /// leftTopBlank
            ///             leftTop3
            ///                    leftTop2
            ///                          leftTop1
            ///                                 blank
            ///                                     list.First()
            ///                                             rightBottom

            if (list.First().x >= 5 && list.First().y >= 5)
            {
                ChessPos rightBottom = new ChessPos(list.First().x + 1, list.First().y + 1);
                ChessPos blank = new ChessPos(list.First().x - 1, list.First().y - 1);
                ChessPos leftTop1 = new ChessPos(blank.x - 1, blank.y - 1);
                ChessPos leftTop2 = new ChessPos(leftTop1.x - 1, leftTop1.y - 1);
                ChessPos leftTop3 = new ChessPos(leftTop2.x - 1, leftTop2.y - 1);
                ChessPos leftTopBlank = new ChessPos(leftTop3.x - 1, leftTop3.y - 1);
                count +=
                ExistSpecificChessmanAll(board, new List<ChessPos>() { leftTop1, leftTop2, leftTop3 }, chessType) &&
                IsAllBlankPosition(board, new List<ChessPos>() { rightBottom, blank, leftTopBlank })
                ? 1 : 0;
            }

            ///����
            ///                                                 rightTopBlank
            ///                                           list.First()
            ///                                       blank
            ///                              leftBottom1
            ///                       leftBottom2
            ///               leftBottom3
            /// leftBottomBlank
            if (list.First().x >= 5 && list.First().y <= LINE_COUNT - 5)
            {
                ChessPos rightTopBlank = new ChessPos(list.First().x + 1, list.First().y - 1);
                ChessPos blank = new ChessPos(list.First().x - 1, list.First().y + 1);
                ChessPos leftBottom1 = new ChessPos(blank.x - 1, blank.y + 1);
                ChessPos leftBottom2 = new ChessPos(leftBottom1.x - 1, leftBottom1.y + 1);
                ChessPos leftBottom3 = new ChessPos(leftBottom2.x - 1, leftBottom2.y + 1);
                ChessPos leftBottomBlank = new ChessPos(leftBottom3.x - 1, leftBottom3.y + 1);
                count += ExistSpecificChessmanAll(
                board, new List<ChessPos>() { leftBottom1, leftBottom2, leftBottom3 }, chessType) &&
                IsAllBlankPosition(board, new List<ChessPos>() { rightTopBlank, blank, leftBottomBlank })
                ? 1 : 0;
            }

            /// ����
            ///                                             rightTopBlank
            ///                                       rightTop3
            ///                                 rightTop2
            ///                         rightTop1
            ///                     blank
            ///             list.First()
            /// leftBottomBlank
            if (list.First().x <= LINE_COUNT - 5 && list.First().y >= 5)
            {
                ChessPos leftBottomBlank = new ChessPos(list.First().x - 1, list.First().y + 1);
                ChessPos blank = new ChessPos(list.First().x + 1, list.First().y - 1);
                ChessPos rightTop1 = new ChessPos(blank.x + 1, blank.y - 1);
                ChessPos rightTop2 = new ChessPos(rightTop1.x + 1, rightTop1.y - 1);
                ChessPos rightTop3 = new ChessPos(rightTop2.x + 1, rightTop2.y - 1);
                ChessPos rightTopBlank = new ChessPos(rightTop3.x + 1, rightTop3.y - 1);
                count += ExistSpecificChessmanAll(
                board, new List<ChessPos>() { rightTop1, rightTop2, rightTop3 }, chessType) &&
                IsAllBlankPosition(board, new List<ChessPos>() { leftBottomBlank, blank, rightTopBlank })
                ? 1 : 0;
            }

            /// ����
            /// leftTopBlank
            ///           list.First()
            ///                    blank
            ///                        rightBottom1
            ///                               rightBottom2
            ///                                      rightBottom3
            ///                                            rightBottomBlank
            if (list.First().x <= LINE_COUNT - 5 && list.First().y <= LINE_COUNT - 5)
            {
                ChessPos leftTopBlank = new ChessPos(list.First().x - 1, list.First().y - 1);
                ChessPos blank = new ChessPos(list.First().x + 1, list.First().y + 1);
                ChessPos rightBottom1 = new ChessPos(blank.x + 1, blank.y + 1);
                ChessPos rightBottom2 = new ChessPos(rightBottom1.x + 1, rightBottom1.y + 1);
                ChessPos rightBottom3 = new ChessPos(rightBottom2.x + 1, rightBottom2.y + 1);
                ChessPos rightBottomBlank =
                    new ChessPos(rightBottom3.x + 1, rightBottom3.y + 1);
                count += ExistSpecificChessmanAll(
                board, new List<ChessPos>() { rightBottom1, rightBottom2, rightBottom3 }, chessType) &&
                IsAllBlankPosition(board, new List<ChessPos>() { leftTopBlank, blank, rightBottomBlank })
                ? 1 : 0;
            }
        }
        else if (list.Count == 2)
        {
            //2��2
            /// next2Blank next2 next1 next1Blank list[0] list[1]  next3Blank next3 next4 next4Blank
            ChessPos next1Blank = NextChessman(list[1], list[0]);
            ChessPos next1 = NextChessman(list[0], next1Blank);
            ChessPos next2 = NextChessman(next1Blank, next1);
            ChessPos next2Blank = NextChessman(next1, next2);
            ChessPos next3Blank = NextChessman(list[0], list[1]);
            ChessPos next3 = NextChessman(list[1], next3Blank);
            ChessPos next4 = NextChessman(next3Blank, next3);
            ChessPos next4Blank = NextChessman(next3, next4);

            count += ExistSpecificChessmanAll(board, new List<ChessPos>() { next2, next1 }, chessType) &&
            IsAllBlankPosition(board, new List<ChessPos>() { next2Blank, next1Blank, next3Blank })
            ? 1 : 0;
            count += ExistSpecificChessmanAll(board, new List<ChessPos>() { next3, next4 }, chessType) &&
            IsAllBlankPosition(board, new List<ChessPos>() { next1Blank, next3Blank, next4Blank })
            ? 1 : 0;
        }
        else if (list.Count == 3)
        {
            //3��1
            ///next1Next1Blank next1 next1Blank list[0] list[1] list[2] next2Blank next2 next2Next2Blank
            ChessPos next1Blank = NextChessman(list[1], list[0]);
            ChessPos next1 = NextChessman(list[0], next1Blank);
            ChessPos next1Next1Blank = NextChessman(next1Blank, next1);
            ChessPos next2Blank = NextChessman(list[1], list[2]);
            ChessPos next2 = NextChessman(list[2], next2Blank);
            ChessPos next2Next2Blank = NextChessman(next2Blank, next2);

            count += ExistSpecificChessman(board, next1, chessType) &&
            IsAllBlankPosition(board, new List<ChessPos>() { next1Next1Blank, next1Blank, next2Blank })
            ? 1 : 0;
            count += ExistSpecificChessman(board, next2, chessType) &&
            IsAllBlankPosition(board, new List<ChessPos>() { next1Blank, next2Blank, next2Next2Blank })
            ? 1 : 0;
        }
        return count;
    }
    #endregion

    //�����first��second������һ�����ӵ�λ��ƫ������
    static ChessPos NextChessman(ChessPos first, ChessPos second)
    {
        //���first��second��yֵ�Ƿ���ȡ�
        //�����ȣ���ʾ������ˮƽ�������ƶ�����ô��һ�����ӵ�λ��ƫ��������ˮƽ���������һ������ƶ�һ��ȡ����first��x�Ƿ����second��x��
        //���first.x > second.x���������ƶ�һ�񣬼�second.x - 1�����������ƶ�һ�񣬼�second.x + 1�������걣�ֲ��䣬��Ϊfirst.y
        if (first.y == second.y)
        {
            return new ChessPos(
                first.x > second.x ? second.x - 1 : second.x + 1, first.y);
        }
        //���first.x��second.x��ȣ���ʾ�����ڴ�ֱ�������ƶ�����ô��һ�����ӵ�λ��ƫ�������ڴ�ֱ���������ϻ������ƶ�һ��
        //ȡ����first��y�Ƿ����second��y�����first.y > second.y���������ƶ�һ�񣬼�second.y - 1�����������ƶ�һ��
        //��second.y + 1�������걣�ֲ��䣬��Ϊfirst.x��
        //���������������������㣬��ô��ʾ������б�Խ��߷������ƶ�������first.x��second.x�Ĵ�С��ϵ��
        //�Լ�first.y��second.y�Ĵ�С��ϵ��������һ�����ӵ�λ��ƫ������
        else if (first.x == second.x)
        {
            return new ChessPos(
                first.x, first.x > second.y ? second.y - 1 : second.y + 1);
        }
        else if (first.x > second.x)
        {
            // ���½�
            if (first.y > second.y)
            {
                return new ChessPos(second.x - 1, second.x - 1);
            }
            //���Ͻ�
            else
            {
                return new ChessPos(second.x - 1, second.y + 1);
            }
        }
        else
        {
            //���½�
            if (first.y > second.y)
            {
                return new ChessPos(second.x + 1, second.y - 1);
            }
            //���Ͻ�
            else
            {
                return new ChessPos(second.x + 1, second.y + 1);
            }
        }
    }

    //�жϸ�λ���Ƿ���Ч��
    public static bool IsEffectivePosition(ChessPos offset)
    {
        return offset != ChessPos.none;
    }

    static bool ExistSpecificChessmanAll(ChessBoard board, List<ChessPos> positions, ChessType chessType)
    {
        if (positions.Count == 0)
        {
            return false;
        }

        bool flag = true;
        foreach (ChessPos of in positions)
        {
            flag &= ExistSpecificChessman(board, of, chessType);
        }
        return flag;
    }

    // ������λ���Ƿ�����ض�������
    public static bool ExistSpecificChessman(ChessBoard board, ChessPos position, ChessType chessType)
    {
        if (!board.chess_map_dic.ContainsKey(position))
        { return false; }

        string entity_id = board.chess_map_dic[position];
        bool has_entity;
        Chess chess = ChessFactory.Instance.GetEntity<Chess>(entity_id, out has_entity);

        return chess && chess.chess_type == chessType;
    }

    static bool IsAllBlankPosition(ChessBoard board, List<ChessPos> list)
    {
        foreach (ChessPos o in list)
        {
            if (!IsBlankPosition(board, o))
            {
                return false;
            }
        }
        return true;
    }

    //isBlankPosition�������ж�ĳ��λ�����Ƿ�û�����ӣ�д���߼����û������ܷ����Ӳ��
    static bool IsBlankPosition(ChessBoard board, ChessPos position)
    {
        return !board.chess_map_dic.ContainsKey(position);
    }

    //�������������������ֵΪ2�ķ�Χ��
    static int LimitMax(int num)
    {
        return num >= 2 ? 2 : num;
    }

    /// <summary>
    /// ��ÿһ��������й���
    /// </summary>
    /// <param name="first"></param>
    /// <param name="myChessman"></param>
    /// <param name="chessType"></param>
    /// <returns></returns>
    public static int GetScoring(ChessBoard board, List<ChessPos> myChessman, ChessType chessType) {
        if (myChessman.Count >= 5)
        {
            return EvalateConst.WIN;
        }
        int score = 0;
        switch (myChessman.Count)
        {
            case 1:
                break;
            case 2:
                if (IsAlive2(board, myChessman))
                {
                    score += EvalateConst.ALIVE2;
                    score +=
                        LimitMax(GetJumpAlive3Count(board, myChessman, chessType)) * EvalateConst.JUMP_ALIVE3;
                    score +=
                        LimitMax(GetJumpAlive4Count(board, myChessman, chessType)) * EvalateConst.JUMP_ALIVE4;

                    Debug.Log("$printMsg ��2����, �÷�+$ALIVE2");
                    
                }
                else if (IsLowerDeath2(board, myChessman))
                {
                    score += EvalateConst.LOWER_DEATH2;
                    Debug.Log("$printMsg �ͼ���2���� ,�÷�+$LOWER_DEATH2");
                }
                else
                {
                    score += EvalateConst.DEEP_DEATH2;
                    Debug.Log("$printMsg ��2���� ,�÷�+$DEEP_DEATH2");
                }
                break;
            case 3:
                if (IsAlive3(board, myChessman))
                {
                    score += EvalateConst.ALIVE3;
                    score +=
                        LimitMax(GetJumpAlive4Count(board, myChessman, chessType)) * EvalateConst.JUMP_ALIVE4;
                    Debug.Log("$printMsg ��3����, �÷�+$ALIVE3");
                }
                else if (IsLowerDeath3(board, myChessman))
                {
                    score += EvalateConst.LOWER_DEATH3;
                    Debug.Log("$printMsg �ͼ���3���� ,�÷�+$LOWER_DEATH3");
                }
                else
                {
                    score += EvalateConst.DEEP_DEATH3;
                    Debug.Log("$printMsg ��3���� ,�÷�+$DEEP_DEATH3");
                }
                break;

            case 4:
                if (IsAlive4(board, myChessman))
                {
                    score += EvalateConst.ALIVE4;
                    Debug.Log("$printMsg ��4����, �÷�+$ALIVE4");
                }
                else if (IsLowerDeath4(board, myChessman))
                {
                    score += EvalateConst.LOWER_DEATH4;
                    Debug.Log("$printMsg �ͼ���4���� ,�÷�+$LOWER_DEATH4");
                }
                else
                {
                    score += EvalateConst.DEEP_DEATH4;
                    Debug.Log("$printMsg ��4���� ,�÷�+$DEEP_DEATH4");
                }
                break;

            case 5:
            default:
                score += EvalateConst.WIN;
                break;
        }
        return score;
    }


    /// <summary>
    /// ���Ӽ�ֵ(������ܣ�
    /// </summary>
    /// <param name="offset"></param>
    /// <returns></returns>
    public static int GetAlongScoring(ChessBoard board, ChessPos offset, ChessType chessType)
    {
        int score = 0;
        List<ChessPos> list = new List<ChessPos>()
        { 
          new ChessPos(offset.x - 1, offset.y),
          new ChessPos(offset.x + 1, offset.y),
          new ChessPos(offset.x, offset.y + 1),
          new ChessPos(offset.x, offset.y - 1),
          new ChessPos(offset.x - 1, offset.y - 1),
          new ChessPos(offset.x - 1, offset.y + 1),
          new ChessPos(offset.x + 1, offset.y - 1),
          new ChessPos(offset.x + 1, offset.y + 1),

        };
        foreach(var pos in list)
        {
            if (IsEffectivePosition(pos) && IsBlankPosition(board, pos))
            {
                score++;
            }
        }

        return score + PositionScore(board, offset);
    }

    ///λ�õ÷�(Խ�������ĵ÷�Խ��)
    public static int PositionScore(ChessBoard board, ChessPos offset)
    {
        //���ֵ��ͨ����(offset.x - 7.5)^2 + (offset.y - 7.5)^2��������õ��ġ�
        //���У�^��ʾ�˷���������ȡƽ�������԰�������ÿ�����ӵ�λ�����һ��Բ׶��Խ��������λ��Խ��
        //�ο��㱻�趨Ϊ(7.5, 7.5)�����̵�����
        double mid_value = Mathf.Pow((float)(board.board_y_size * 0.5), 2) +
            Mathf.Pow((float)(board.board_x_size * 0.5), 2);
        double z = -(
            Mathf.Pow((float)(offset.x - board.board_y_size * 0.5), 2) +
            Mathf.Pow((float)(offset.y - board.board_x_size * 0.5), 2)) + mid_value;
        z /= 10;
        return (int)z;
    }

    /// <summary>
    /// ����ĳ�����Ӷ��� ownerPlayer �ķ�ֵ
    /// </summary>
    /// <param name="chessmanPosition"></param>
    /// <param name=""></param>
    /// <returns></returns>
    public static int ChessmanGrade(ChessBoard board, ChessType ownerPlayer, ChessPos chessmanPosition)
    {
        int score = 0;
        List<ChessPos> myChenssman = new List<ChessPos>();
        ChessPos offset;
        ChessPos first = chessmanPosition;
        ChessType player = ownerPlayer;

        ///����
        //����(��)
        offset = new ChessPos(first.x - 1, first.y);
        myChenssman.Clear();
        myChenssman.Add(first);
        while (ExistSpecificChessman(board, offset, player))
        {
            myChenssman.Add(offset);
            offset = new ChessPos(offset.x - 1, offset.y);
        }

        //����(��)
        offset = new ChessPos(first.x + 1, first.y);
        while (ExistSpecificChessman(board, offset, player))
        {
            myChenssman.Add(offset);
            offset = new ChessPos(offset.x + 1, offset.y);
        }
        //Ҫ��x��С��������
        myChenssman.Sort((a, b) => {
            return (a.x - b.x);
        });
        score += GetScoring(board, myChenssman, player);

        ///����
        //����(��)
        myChenssman.Clear();
        myChenssman.Add(first);
        offset = new ChessPos(first.x, first.y - 1);
        while (ExistSpecificChessman(board, offset, player))
        {
            myChenssman.Add(offset);
            offset = new ChessPos(offset.x, offset.y - 1);
        }

        //����(��)
        offset = new ChessPos(first.x, first.y + 1);
        while (ExistSpecificChessman(board, offset, player))
        {
            myChenssman.Add(offset);
            offset = new ChessPos(offset.x, offset.y + 1);
        }
        myChenssman.Sort((a, b) => {
            return (a.y - b.y);
        });
        score += GetScoring(board, myChenssman, player);

        ///��б(�������� -> ��һ����)
        //��б(��������)
        myChenssman.Clear();
        myChenssman.Add(first);
        offset = new ChessPos(first.x - 1, first.y + 1);
        while (ExistSpecificChessman(board, offset, player))
        {
            myChenssman.Add(offset);
            offset = new ChessPos(offset.x - 1, offset.y + 1);
        }

        //��б(��һ����)
        offset = new ChessPos(first.x + 1, first.y - 1);
        while (ExistSpecificChessman(board, offset, player))
        {
            myChenssman.Add(offset);
            offset = new ChessPos(offset.x + 1, offset.y - 1);
        }
        myChenssman.Sort((a, b) => {
            return (a.x - b.x) + (a.y - b.y);
        });
        score += GetScoring(board, myChenssman, player);

        ///��б(�ڶ����� -> ��������)
        //��б(�ڶ�����)
        myChenssman.Clear();
        myChenssman.Add(first);
        offset = new ChessPos(first.x - 1, first.y - 1);
        while (ExistSpecificChessman(board, offset, player))
        {
            myChenssman.Add(offset);
            offset = new ChessPos(offset.x - 1, offset.y - 1);
        }

        //��б(��������)
        offset = new ChessPos(first.x + 1, first.y + 1);
        while (ExistSpecificChessman(board, offset, player))
        {
            myChenssman.Add(offset);
            offset = new ChessPos(offset.x + 1, offset.y + 1);
        }
        myChenssman.Sort((a, b) => {
            return (a.x - b.x) + (a.y + b.y);
        });
        score += GetScoring(board, myChenssman, player);

        int ss = score + GetAlongScoring(board, first, player);
        int jumpAlive4Count = GetJumpAlive4Count(board, new List<ChessPos>() { first}, player);
        int jumpAlive3Count = GetJumpAlive4Count(board, new List<ChessPos>() { first }, player);
        int jumpAlive2Count = GetJumpAlive4Count(board, new List<ChessPos>() { first }, player);
        score += LimitMax(jumpAlive4Count) * EvalateConst.JUMP_ALIVE4 +
                 LimitMax(jumpAlive3Count) * EvalateConst.JUMP_ALIVE3 +
                 LimitMax(jumpAlive2Count) * EvalateConst.JUMP_ALIVE2;

        //Debug.Log($"���ӷ�ֵΪ: ${ss} ,���е��ӵ÷�:${GetAlongScoring(board, first, player)}, ��ϵ÷�:${score}");

        return score + GetAlongScoring(board, first, player);
      }

    /// <summary>
    /// �����ҷ���һ���Ϻõ�λ��
    /// </summary>
    /// <returns></returns>
    public static ChessBufferMap MyBetterPosition(ChessBoard board, ChessType m_type, int max_count = 5)
    {
        ChessPos offset = ChessPos.zero;
        ChessBufferMap ourMap = new ChessBufferMap(max_count);
        int LINE_COUNT = board.board_y_size;
        int COL_COUNT = board.board_x_size;
        for (int i = 0; i <= LINE_COUNT; i++)
        {
            for (int j = 0; j <= COL_COUNT; j++)
            {
                offset = new ChessPos(i, j);
                if (IsBlankPosition(board, offset))
                {
                    int score = ChessmanGrade(board, m_type, offset);
                    if (ourMap.MinKey() < score)
                    {
                        ourMap.Put(score, offset);
                    }
                }
            }
        }
        return ourMap;
    }

    /// <summary>
    /// ����з���һ���Ϻõ�λ��
    /// </summary>
    /// <param name="board"></param>
    /// <param name="en_type"></param>
    /// <param name="max_count"></param>
    /// <returns></returns>
    public static ChessBufferMap EnemyBetterPosition(ChessBoard board, ChessType en_type, int max_count = 5) 
    {
        ChessPos offset = ChessPos.zero;
        ChessBufferMap enemyMap = new ChessBufferMap(max_count);

        int count = 0;
        int LINE_COUNT = board.board_y_size;
        int COL_COUNT = board.board_x_size;
        for (int i = 0; i <= LINE_COUNT; i++) 
        {
            for (int j = 0; j <= COL_COUNT; j++) 
            {
                offset = new ChessPos(i, j);
                if (IsBlankPosition(board, offset))
                {
                    DateTime start = DateTime.Now;
                    int score = ChessmanGrade(board, en_type, offset);
                    DateTime end = DateTime.Now;
                    count++;
                    int time = end.Millisecond - start.Millisecond;
                    if (time > 5)
                    {
                        Debug.Log($"���ҵз���������λ�ú�ʱ��{time}");
                    }
                    if (enemyMap.MinKey() < score)
                    {
                        enemyMap.Put(score, offset);
                    }
                }
            }
         }
        Debug.Log($"���ҵз���������λ�ô�����{count}");
        return enemyMap;
    }

}