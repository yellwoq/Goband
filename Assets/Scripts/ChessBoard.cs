using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

// 棋盘
public class ChessBoard : MonoEntity
{
    public int board_x_size = 16;
    public int board_y_size = 15;
    public GameObject chess_prefab;

    private Transform touchBoard;
    private Transform chessParent;
    private Vector2 refrenceSolution;

    public Vector2 RefrenceSolution
    {
        get
        {
            return refrenceSolution;
        }
        set
        {
            refrenceSolution = value;
        }
    }

    //棋子数据集合
    internal Dictionary<ChessPos, string> chess_map_dic = new Dictionary<ChessPos, string>();

    //检验游戏胜利相关
    public Dictionary<ChessType, List<ChessInfo>> chess_type_pos_map = new Dictionary<ChessType, List<ChessInfo>>();

    private void Awake()
    {
        chessParent = transform.Find("Chesses");
        touchBoard = transform.Find("TouchBoard");

    }

    private void OnEnable()
    {
        EventManager.Instance.RegisterEvent<int, int, Vector2>(
            EventType.OnScreenChanged, OnScreenSizeChanged);
    }

    private void OnDisable()
    {
        EventManager.Instance.UnRegisterEvent<int, int, Vector2>(
            EventType.OnScreenChanged, OnScreenSizeChanged);
    }

    private void Update()
    {
        //Vector2 loc_pos;
        //RectTransformUtility.ScreenPointToLocalPointInRectangle(
        //  chessParent.GetComponent<RectTransform>(), Input.mousePosition, null, out loc_pos);

        //Debug.Log("棋子的屏幕坐标：" + loc_pos);
    }

    //绘制棋盘
    public void DrawChessboard()
    {
        //foreach (var item in chess_map_dic)
        //{
        //    for (int i = 0; i < item.Value.Count; i++)
        //    {
        //        item.Value
        //    }
        //}
    }

    //在位置放置一枚棋子
    public bool PlaceChess(ChessType chessType, int xIndex, int yIndex)
    {
        ChessPos chessPos = new ChessPos(xIndex, yIndex);
        if (!chess_map_dic.ContainsKey(chessPos))
        {
            string entity_id;
            GameObject chess_obj = Instantiate(chess_prefab, chessParent);
            chess_obj.name = chessType.ToString() + $"({xIndex}, {yIndex})";
            chess_obj.SetActive(true);
            Chess chess = ChessFactory.Instance.CreatEntity<Chess>(EntityType.Chess, out entity_id, chess_obj);
            chess.SetInfo(chessType, xIndex, yIndex, CaculateAnchorPos(xIndex, yIndex));
            chess_map_dic.Add(chessPos, entity_id);

            if (!chess_type_pos_map.ContainsKey(chessType))
            {
                chess_type_pos_map.Add(chessType, new List<ChessInfo>());
            }
            chess_type_pos_map[chessType].Add(chess.m_chessInfo);

            if (CheckResultIsSuccess(chessType, chessPos) || CheckIsHeChess())
            {
                Debug.Log(chessType + "YING LE");
                EventManager.Instance.PublicEvent(EventType.GameStateChanged, false);
                string win_chess = chessType == ChessType.WHITE ? "白棋" : "黑棋";
                string tip_str = $"恭喜!! {win_chess}赢了,点击按钮再来一局!!";
                if(CheckIsHeChess())
                {
                    win_chess = "无";
                    tip_str = $"和棋了,点击按钮再来一局!!";
                }
                ControllerUI controller_ui = UIManager.Instance.ShowPopup("ControllerUI") as ControllerUI;
                controller_ui.SetShowTxt(tip_str);
                controller_ui.ClickAddListener(OnFinish);
            }
            return true;
        }
        return false;
    }

    private void OnFinish()
    {
        ClearBoard();
    }

    private Vector2 CaculateAnchorPos(int xIndex, int yIndex)
    {
        float xoffset = UIConsts.DEFAULT_X_OFFSET_POS;
        float yoffset = UIConsts.DEFAULT_Y_OFFSET_POS;
        float xPos = UIConsts.DEFAULT_LEFT_X_POS + xoffset * xIndex;
        float yPos = UIConsts.DEFAULT_UP_Y_POS + yoffset * yIndex;
        return new Vector2(xPos, yPos);
    }

    // 检测鼠标是否点击在了一个棋子上
    public ChessPos CaculateAnchorPosIndex(Vector2 inputPos, out bool is_on_right_pos)
    {
        float xoffset = UIConsts.DEFAULT_X_OFFSET_POS;
        float yoffset = UIConsts.DEFAULT_Y_OFFSET_POS;
        RectTransform chess_RectTrans = chess_prefab.GetComponent<RectTransform>();
        Vector2 screen_ui_anchor_point;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            chess_RectTrans, Input.mousePosition, null, out screen_ui_anchor_point);
        screen_ui_anchor_point.x += chess_RectTrans.offsetMax.x - chess_RectTrans.rect.width / 2;
        screen_ui_anchor_point.y += chess_RectTrans.offsetMax.y - chess_RectTrans.rect.height / 2;

        for (int i = 0; i < board_x_size; i++)
        {
            for (int j = 0; j < board_y_size; j++)
            {
                float xPos = UIConsts.DEFAULT_LEFT_X_POS + xoffset * i;
                float yPos = UIConsts.DEFAULT_UP_Y_POS + yoffset * j;
   
                if(Mathf.Abs(screen_ui_anchor_point.x - xPos) <= chess_RectTrans.rect.width / 2 &&
                    Mathf.Abs(screen_ui_anchor_point.y - yPos) <= chess_RectTrans.rect.height / 2)
                {
                    is_on_right_pos = true;
                    Debug.Log("找到了棋子位置！！！");
                    return new ChessPos(i, j);
                }
            }
        }
        is_on_right_pos = false;
        return ChessPos.zero;
    }

    //清空棋盘
    public void ClearBoard()
    {
        foreach (var item in chess_map_dic)
        {
            string entity_id = item.Value;
            bool has_entity;
            Chess chess = ChessFactory.Instance.GetEntity<Chess>(entity_id, out has_entity);
            if(has_entity)
            {
                ChessFactory.Instance.RemoveEntity(entity_id);
                Destroy(chess.gameObject);
            }
        }
        chess_map_dic.Clear();
        chess_type_pos_map.Clear();
    }

    //判断是否胜利
    bool CheckResultIsSuccess(ChessType chessType, ChessPos pos)
    {
        int currentX = pos.x;
        int currentY = pos.y;

        int count = 0;
        List<ChessPos> winResult = new List<ChessPos>();
        int LINE_COUNT = board_y_size;
        int COL_COUNT = board_x_size;

        ///横
        /// o o o o o
        /// o o o o o
        /// x x x x x
        /// o o o o o
        /// o o o o o
        winResult.Clear();
        // 循环遍历当前行的前后四个位置（如果存在），检查是否有特定的棋子连成五子相连
        //判断 currentX - 4 > 0 时，它的意思是判断左侧第 4 个位置是否在棋盘内。
        //如果 currentX - 4 >= 0，则表示左侧第 4 个位置在棋盘内；
        //否则，即 currentX - 4 < 0，表示左侧第 4 个位置已经超出了棋盘边界。
        for (int i = (currentX - 4 >= 0 ? currentX - 4 : 0);
            i <= (currentX + 4 < LINE_COUNT ? currentX + 4 : LINE_COUNT - 1);
            i++)
        {
           // 计算当前位置的坐标
           ChessPos position = new ChessPos(i, currentY);

            // 检查当前位置是否存在胜利的棋子
            if (CheckCurrentStateHelper.ExistSpecificChessman(this, position, chessType))
            {
                // 将该棋子添加到胜利结果列表中，并增加计数器
                winResult.Add(position);
                count++;
            }
            else
            {
                // 如果不存在特定的棋子，清空胜利结果列表，并将计数器重置为0
                winResult.Clear();
                count = 0;
            }

            // 解析：如果计数器达到5，表示有五子相连，输出胜利者信息并返回true
            if (count >= 5)
            {
                return true;
            }
        }

        ///竖
        /// o o x o o
        /// o o x o o
        /// o o x o o
        /// o o x o o
        /// o o x o o
        count = 0;
        winResult.Clear();
        for (int j = (currentY - 4 >= 0 ? currentY - 4 : 0);
            j <= (currentY + 4 > COL_COUNT ? COL_COUNT - 1 : currentY + 4);
            j++)
        {
            ChessPos position = new ChessPos(currentX, j);
            if (CheckCurrentStateHelper.ExistSpecificChessman(this, position, chessType))
            {
                winResult.Add(position);
                count++;
            }
            else
            {
                winResult.Clear();
                count = 0;
            }
            if (count >= 5)
            {
                return true;
            }
        }

        ///正斜
        /// o o o o x
        /// o o o x o
        /// o o x o o
        /// o x o o o
        /// x o o o o
        count = 0;
        winResult.Clear();
        ChessPos offset2 = new ChessPos(currentX - 4, currentY + 4);
        for (int i = 0; i < 10; i++)
        {
            ChessPos position = new ChessPos(offset2.x + i, offset2.y - i);
            if (CheckCurrentStateHelper.ExistSpecificChessman(this, position, chessType))
            {
                winResult.Add(position);
                count++;
            }
            else
            {
                winResult.Clear();
                count = 0;
            }
            if (count >= 5)
            {
                return true;
            }
        }

        ///反斜
        /// x o o o o
        /// o x o o o
        /// o o x o o
        /// o o o x o
        /// o o o o x
        count = 0;
        winResult.Clear();
        ChessPos offset = new ChessPos(currentX - 4, currentY - 4);
        for (int i = 0; i < 10; i++)
        {
            ChessPos position = new ChessPos(offset.x + i, offset.y + i);
            if (CheckCurrentStateHelper.ExistSpecificChessman(this, position, chessType))
            {
                winResult.Add(position);
                count++;
            }
            else
            {
                winResult.Clear();
                count = 0;
            }
            if (count >= 5)
            {
                return true;
            }
        }
        winResult.Clear();
        return false;
    }

    bool CheckIsHeChess()
    {
        return chess_map_dic.Count >= board_x_size * board_y_size;
    }

    public List<ChessInfo> GetChessPosListByChessType(ChessType chessType)
    {
        if (!chess_type_pos_map.ContainsKey(chessType))
        {
            return null;
        }
        return chess_type_pos_map[chessType];
    }

    public Chess GetChessByPos(ChessPos pos)
    {
        if (!chess_map_dic.ContainsKey(pos))
            return null;
        string entity_id = chess_map_dic[pos];
        bool has_entity;
        Chess chess = ChessFactory.Instance.GetEntity<Chess>(entity_id, out has_entity);
        return chess;
    }

    void OnScreenSizeChanged(int x, int y, Vector2 referenceResolution)
    {
        //在屏幕发生改变时调用
        Debug.Log("屏幕，发生了改变");
    }

}
