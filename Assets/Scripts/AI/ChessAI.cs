using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using Random = UnityEngine.Random;

// 棋子AI
public class ChessAI : MonoBehaviour
{
    private ChessType m_ChessType;
    public ChessType AIChessType
    {
        get
        {
            return m_ChessType;
        }

        set
        {
            m_ChessType = value;
        }
    }

    private ChessBoard m_chessBoard;

    public void SetBelongChessboard(ChessBoard chessBoard)
    {
        m_chessBoard = chessBoard;
    }

    private ChessPos GetRandomChessPos(List<ChessPos> chessPos)
    {
        if (chessPos.Count == 0) return ChessPos.none;

        int pos_index = Random.Range(0, chessPos.Count);
        return chessPos[pos_index];
    }

    /// <summary>
    /// 获取将要胜利的棋子位置集合
    /// </summary>
    /// <returns></returns>
    private List<ChessPos> GetWillWinPos()
    {
        List<ChessPos> current_has_place_chessPos = m_chessBoard.GetChessPosListByChessType(m_ChessType);
        if (current_has_place_chessPos != null || current_has_place_chessPos.Count == 0)
        {
            return null;
        }

        List<ChessPos>  win_pos_list = new List<ChessPos>();
        for (int i = 0; i < current_has_place_chessPos.Count; i++)
        {
            ChessPos chessPos = current_has_place_chessPos[i];
            if()
        }

    }

    public ChessPos GetNextChessPos(ChessPos other_last_place_pos)
    {
        List<ChessPos> cur_chessPosList = m_chessBoard.GetChessPosListByChessType(m_ChessType);
        //1、判断我方是否有位置可以直接胜利或者快要胜利了， 有则下在胜利的位置，没有则进入2
        List<ChessPos> can_win_pos_list = new List<ChessPos>();
        for (int i = 0; i < cur_chessPosList.Count; i++)
        {
            List<ChessPos> win_pos_list;
            if (m_chessBoard.GetNextCanPlacePosByChessCount(
                m_ChessType, cur_chessPosList[i], 4, out win_pos_list))
            {
                can_win_pos_list.AddRange(win_pos_list);
            }
            else continue;
        }
        
        if (can_win_pos_list.Count > 0) return GetRandomChessPos(can_win_pos_list);
        //2、判断当前对方棋子摆放数，没有，则随机放置在棋盘任意一个位置， 有进入3
        ChessType other_chess_type = m_ChessType == ChessType.WHITE ? ChessType.BLACK : ChessType.WHITE;
        List<ChessPos> other_chess_Pos_list = m_chessBoard.GetChessPosListByChessType(other_chess_type);
        int x_index = -1;
        int y_index = -1;
        if (other_chess_Pos_list == null || other_chess_Pos_list.Count == 0)
        {
            x_index = Random.Range(0, m_chessBoard.board_x_size);
            y_index = Random.Range(0, m_chessBoard.board_y_size);
            return new ChessPos(x_index, y_index);
        }
        //3、判断当前对方棋子前面下的棋子位置的各个方向，左右、上下、右上、左上， 判断是不是已经有3颗及以上棋子了，如果是，那么进入4
        if (other_last_place_pos != ChessPos.none)
        {
            List<ChessPos> other_next_Pos;
            //4、此时需要拦住对方的棋子，这里有几种情况：
            //1）对方棋子四颗，且有AI棋子拦截， 中间没有空位，此时必须放置第五颗棋子所在位置
            if (m_chessBoard.CheckIsSuccessByCount(other_chess_type, other_last_place_pos, 4))
            {

            }
            //2）对方棋子三颗，且没有AI棋子拦截， 中间没有空位，此时必须放置第四颗棋子所在位置（
            //***容易： 随机取棋盘中两端的位置。
            // 中：判断两边哪一边下赢的概率更大。    
            //难：判断两边哪一边下赢的概率更大。往阵型靠   ）
            //3）对方棋子四颗，且没有AI棋子拦截， 中间是否有空位：
            //	有：判断该四颗棋子中间空位数， 空位数越小的越优先
            //（
            //	***容易： 空位数越小的越优先 。
            // 	中：只有1颗的时候返回中间这颗位置或者其他两边随机一个，判定放哪赢的概率大， 其他从空位上逐个判定放哪赢的概率大。
            //	难：只有1颗的时候返回中间这颗位置或者其他两边随机一个，判定放哪赢的概率大。其他从空位上逐个判定放哪赢的概率大，
            //	再判定哪个对阵型有利   ）

            //	没有：直接判定为负
            //4）对方棋子四颗，且有AI棋子拦截， 中间有空位：
            //	有：（
            //	***容易： 空位数越小的越优先 。
            // 	中：只有1颗的时候返回中间这颗位置或者其他没拦截的随机一个，两颗及以上判定放哪赢的概率大。
            //	难：只有1颗的时候返回中间这颗位置或者其他没拦截的随机一个，两颗及以上判定放哪赢的概率大。再判定哪个对阵型有利   ）

            //5）对方棋子三颗，且没有AI棋子拦截， 中间有空位
            //    有：（

            //    ***容易： 空位数越小的越优先 。
            // 	中：只有1颗的时候返回中间这颗位置或者其他两边随机一个，判定放哪赢的概率大， 其他从空位上逐个判定放哪赢的概率大。
            //	难：只有1颗的时候返回中间这颗位置或者其他两边随机一个，判定放哪赢的概率大。其他从空位上逐个判定放哪赢的概率大，
            //	再判定哪个对阵型有利   ）

        }


        //    不是， 进入5
        //5、自身出发，***容易： 随机取棋盘中没有被下过的位置。    中：从自身前面所有已经下的位置出发， 判断哪个方向上的棋子最接近胜利，多个就随机选取一个 难：从自身前面所有已经下的位置出发， 判断哪个方向上的棋子最接近胜利，多个就随机选取一个 ， 设计阵型，每局随机选一个阵型去布局。
        x_index = Random.Range(0, m_chessBoard.board_x_size);
        y_index = Random.Range(0, m_chessBoard.board_y_size);
        return new ChessPos(x_index, y_index);
    }
}
