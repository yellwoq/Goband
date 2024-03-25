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

            if (CheckIsSuccessByCount(chessType, chessPos) || chess_map_dic.Count >= board_x_size * board_y_size)
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

    //判断棋子某个方向是否满足指定数量的棋子
    private bool CheckByStep(
        ChessPos p, List<ChessPos> chess_array, int xdiff, int ydiff, 
        int cu_check_count=5, int ani_check_count=5, int check_count=5)
    {
        ChessPos tmp = new ChessPos(0, 0);
        int i;
        int cnt = 0;

        //向反方向找到颜色相同的点
        for (i = 1; i < ani_check_count; i++)
        {
            tmp.x = p.x - xdiff * i;
            tmp.y = p.y - ydiff * i;
            if (!chess_array.Contains(tmp))
                break;
            cnt++;
        }

        for (i = 1; i < cu_check_count; i++)
        {
            tmp.x = p.x + xdiff * i;
            tmp.y = p.y + ydiff * i;
            if (!chess_array.Contains(tmp))
                break;
            cnt++;
        }

        if (cnt >= check_count-1)
            return true;
        return false;
    }

    private List<ChessPos> GetPosListByStepOtherChess(
        ChessType m_ChessType, ChessPos p, List<ChessPos> chess_array, int xdiff, int ydiff, int check_count = 5)
    {
        List<ChessPos> chessPosList = new List<ChessPos>();
        if (check_count <= 1)
        {
            return chessPosList;
        }
        ChessPos tmp = new ChessPos(0, 0);
        int i;

        int cnt = 0;
        int cnt_other = 0;
        List<ChessPos> cnt_blank_pos_list = new List<ChessPos>();

        int rev_cnt = 0;
        int rev_cnt_other = 0;
        List<ChessPos> rev_cnt_blank_pos_list = new List<ChessPos>();

        bool cnt_can_break = false;
        bool rev_can_break = false;
        // 1、从当前正反方向出发，直到遇到第一个对方棋子或者空格数加上当前计量总数等于check_count-1
        for (i = -check_count + 1; i < check_count; i++)
        {
            if (i == 0)
            {
                continue;
            }
            tmp.x = p.x + xdiff * i;
            tmp.y = p.y + ydiff * i;

            if ((i < 0 && !rev_can_break) || (i > 0 && !cnt_can_break))
                if (!chess_array.Contains(tmp))
                {
                    string chess_uid;
                    chess_map_dic.TryGetValue(tmp, out chess_uid);
                    bool has_entity;
                    Chess chess = ChessFactory.Instance.GetEntity<Chess>(chess_uid, out has_entity);
                    cnt_can_break = i > 0 && has_entity;
                    rev_can_break = i < 0 && has_entity;
                    if (has_entity)
                    {
                        if (i < 0 && !rev_can_break)
                        {
                            rev_cnt_other++;
                        }
                        else if (i > 0 && !cnt_can_break)
                        {
                            cnt_other++;
                        }
                    }
                    else
                    {
                        if (i < 0) rev_cnt_blank_pos_list.Add(tmp);
                        else cnt_blank_pos_list.Add(tmp);
                    }
                    cnt_can_break |= cnt + cnt_blank_pos_list.Count >= check_count - 1;
                    rev_can_break |= rev_cnt + rev_cnt_blank_pos_list.Count >= check_count - 1;
                    continue;
                }

            if (i > 0 && !cnt_can_break)
            {
                cnt++;
            }
            else if (i < 0 && !rev_can_break)
            {
                rev_cnt++;
            }
        }

        //2、根据空格数和计数计算位置，分情况讨论
        //1）某一边的计数总数加另一边的计数等于 check_count-1，此时没有空格
        if (cnt + rev_cnt == check_count - 1)
        {
            ChessPos rev_before_pos = new ChessPos(p.x + xdiff * (-check_count), p.y + ydiff * (-check_count));
            ChessPos cn_after_pos = new ChessPos(p.x + xdiff * check_count, p.y + ydiff * check_count);
            if(!chess_map_dic.ContainsKey(rev_before_pos))
            {
                chessPosList.Add(rev_before_pos);
            }
            if (!chess_map_dic.ContainsKey(cn_after_pos))
            {
                chessPosList.Add(cn_after_pos);
            }
            return chessPosList;
        }
        //2）某一边的计数总数加另一边的计数总数小于 check_count-1，说明中间有空格 
        //3）直接检测所有空格位置是否满足CheckByStep，检测范围就是当前的范围， 满足则加进去

        else if (cnt + rev_cnt < check_count - 1)
        {
            cnt_blank_pos_list.AddRange(rev_cnt_blank_pos_list);
            for (int b = 0; b < cnt_blank_pos_list.Count; b++)
            {
                ChessPos blank_chessPos = cnt_blank_pos_list[b];
                int cur_check_count = Mathf.Clamp(
                    p.x + (check_count - 1) * xdiff - blank_chessPos.x, 0, check_count - 1);
                int ani_check_count = Mathf.Clamp(
                    p.x + (-check_count + 1) * xdiff - blank_chessPos.x, 0, check_count - 1);
                if (CheckByStep(
                    blank_chessPos, chess_array, xdiff, ydiff, cur_check_count, ani_check_count, check_count))
                {
                    chessPosList.Add(blank_chessPos);
                }
            }
        }
        return chessPosList;
    }

    private List<ChessPos> GetPosListByStep(ChessPos p, List<ChessPos> chess_array, int xdiff, int ydiff, int check_count = 5)
    {
        List<ChessPos> chessPosList = new List<ChessPos>();
        if(check_count <= 1)
        {
            return chessPosList;
        }
        ChessPos tmp = new ChessPos(0, 0);
        int i;

        int cnt = 0;
        int cnt_other = 0;
        List<ChessPos> cnt_blank_pos_list = new List<ChessPos>();

        int rev_cnt = 0;
        int rev_cnt_other = 0;
        List<ChessPos> rev_cnt_blank_pos_list = new List<ChessPos>();

        bool cnt_can_break = false;
        bool rev_can_break = false;
        // 1、从当前正反方向出发，直到遇到第一个对方棋子或者空格数加上当前计量总数等于check_count-1
        for (i = -check_count + 1; i < check_count; i++)
        {
            if(i == 0)
            {
                continue;
            }
            tmp.x = p.x + xdiff * i;
            tmp.y = p.y + ydiff * i;

            if((i < 0 && !rev_can_break) || (i > 0 && !cnt_can_break))
                if (!chess_array.Contains(tmp))
                {
                    string chess_uid;
                    chess_map_dic.TryGetValue(tmp, out chess_uid);
                    bool has_entity;
                    Chess chess = ChessFactory.Instance.GetEntity<Chess>(chess_uid, out has_entity);
                    cnt_can_break = i > 0 && has_entity;
                    rev_can_break = i < 0 && has_entity;
                    if (has_entity)
                    {
                        if(i < 0 && !rev_can_break)
                        {
                            rev_cnt_other++;
                        }
                        else if(i > 0 && !cnt_can_break)
                        {
                            cnt_other++;
                        }
                    }
                    else
                    {
                        if (i < 0) rev_cnt_blank_pos_list.Add(tmp);
                        else cnt_blank_pos_list.Add(tmp);
                    }
                    cnt_can_break |= cnt + cnt_blank_pos_list.Count >= check_count - 1;
                    rev_can_break |= rev_cnt + rev_cnt_blank_pos_list.Count >= check_count - 1;
                    continue;
                }

                if (i > 0 && !cnt_can_break)
                {
                    cnt++;
                }
                else if(i < 0 && !rev_can_break)
                {
                    rev_cnt++;
                }
        }

        //2、根据空格数和计数计算位置，分情况讨论
        //1）某一边的计数总数加另一边的计数等于 check_count-1，此时已经没有位置可以下了
        if(cnt + rev_cnt == check_count - 1)
        {
            return chessPosList;
        }
        //2）某一边的计数总数加另一边的计数总数小于 check_count-1，说明才有可能有下的地方满足下了之后满足check_count个的连接 
        //3）直接检测所有空格位置是否满足CheckByStep，检测范围就是当前的范围， 满足则加进去

        else if(cnt + rev_cnt < check_count - 1)
        {
            cnt_blank_pos_list.AddRange(rev_cnt_blank_pos_list);
            for (int b = 0; b < cnt_blank_pos_list.Count; b++)
            {
                ChessPos blank_chessPos = cnt_blank_pos_list[b];
                int cur_check_count = Mathf.Clamp(
                    p.x + (check_count - 1) * xdiff - blank_chessPos.x, 0, check_count-1);
                int ani_check_count = Mathf.Clamp(
                    p.x + (-check_count + 1) * xdiff - blank_chessPos.x, 0, check_count - 1);
                if (CheckByStep(
                    blank_chessPos, chess_array, xdiff, ydiff, cur_check_count, ani_check_count, check_count))
                {
                    chessPosList.Add(blank_chessPos);
                }
            }
        }
        return chessPosList;
    }


    public bool GetNextCanPlacePosByChessCount(ChessType chessType, ChessPos p, int chessCount, out List<ChessPos> winChessPos)
    {
        List<ChessPos> array = chess_type_pos_map[chessType];
        winChessPos = new List<ChessPos>();
        winChessPos.AddRange(GetPosListByStep(p, array, 0, 1, chessCount));
        winChessPos.AddRange(GetPosListByStep(p, array, 1, 0, chessCount));
        winChessPos.AddRange(GetPosListByStep(p, array, 1, 1, chessCount));
        winChessPos.AddRange(GetPosListByStep(p, array, -1, 1, chessCount));
        return winChessPos.Count > 0;
    }

    public bool CheckIsSuccessByCount(ChessType chessType, ChessPos p, int check_count=5)
    {
        List<ChessPos> array = chess_type_pos_map[chessType];

        if (CheckByStep(p, array, 0, 1, check_count - 1, check_count - 1, check_count))    //上下直线判断
            return true;
        if (CheckByStep(p, array, 1, 0, check_count - 1, check_count - 1, check_count))    //左右直线判断
            return true;
        if (CheckByStep(p, array, 1, 1, check_count - 1, check_count - 1, check_count))    //右朝上直线判断
            return true;
        if (CheckByStep(p, array, -1, 1, check_count - 1, check_count - 1, check_count))   //右朝下直线判断
            return true;
        return false;
    }

    public List<ChessPos> GetChessPosListByChessType(ChessType chessType)
    {
        if (!chess_type_pos_map.ContainsKey(chessType))
        {
            return null;
        }
        return chess_type_pos_map[chessType];
    }

    void OnScreenSizeChanged(int x, int y, Vector2 referenceResolution)
    {
        //在屏幕发生改变时调用
        Debug.Log("屏幕，发生了改变");
    }

}
