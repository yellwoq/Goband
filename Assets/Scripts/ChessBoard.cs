using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;

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
    private Dictionary<ChessType, List<ChessPos>> chess_type_pos_map = new Dictionary<ChessType, List<ChessPos>>();

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
    public void PlaceChess(ChessType chessType, int xIndex, int yIndex)
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
                chess_type_pos_map.Add(chessType, new List<ChessPos>());
            }
            chess_type_pos_map[chessType].Add(chessPos);

            if (CheckIsSuccess(chessType, chessPos) || chess_map_dic.Count >= board_x_size * board_y_size)
            {
                Debug.Log(chessType + "YING LE");
                EventManager.Instance.PublicEvent(EventType.GameStateChanged, false);
                string win_chess = chessType == ChessType.WHITE ? "白棋" : "黑棋";
                string tip_str = $"恭喜!! {win_chess}赢了,点击按钮再来一局!!";
                if(chess_map_dic.Count >= board_x_size * board_y_size)
                {
                    win_chess = "无";
                    tip_str = $"和棋了,点击按钮再来一局!!";
                }
                ControllerUI controller_ui = UIManager.Instance.ShowPopup("ControllerUI") as ControllerUI;
                controller_ui.SetShowTxt(tip_str);
                controller_ui.ClickAddListener(OnFinish);
            }
        }
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
    private bool CheckByStep(ChessPos p, List<ChessPos> chess_array, int xdiff, int ydiff)
    {
        ChessPos tmp = new ChessPos(0, 0);
        int i;
        int cnt = 0;

        //向反方向找到颜色相同的点
        for (i = 1; i < 5; i++)
        {
            tmp.x = p.x - xdiff * i;
            tmp.y = p.y - ydiff * i;
            if (!chess_array.Contains(tmp))
                break;
            cnt++;
        }

        for (i = 1; i < 5; i++)
        {
            tmp.x = p.x + xdiff * i;
            tmp.y = p.y + ydiff * i;
            if (!chess_array.Contains(tmp))
                break;
            cnt++;
        }

        if (cnt >= 4)
            return true;
        return false;
    }

    public bool CheckIsSuccess(ChessType chessType, ChessPos p)
    {
        List<ChessPos> array = chess_type_pos_map[chessType];

        if (CheckByStep(p, array, 0, 1))    //上下直线判断
            return true;
        if (CheckByStep(p, array, 1, 0))    //左右直线判断
            return true;
        if (CheckByStep(p, array, 1, 1))    //右朝上直线判断
            return true;
        if (CheckByStep(p, array, -1, 1))   //右朝下直线判断
            return true;
        return false;
    }

    void OnScreenSizeChanged(int x, int y, Vector2 referenceResolution)
    {
        //在屏幕发生改变时调用
        Debug.Log("屏幕，发生了改变");
    }
}
