using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using Random = UnityEngine.Random;

// ����AI
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
    /// ��ȡ��Ҫʤ��������λ�ü���
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
        //1���ж��ҷ��Ƿ���λ�ÿ���ֱ��ʤ�����߿�Ҫʤ���ˣ� ��������ʤ����λ�ã�û�������2
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
        //2���жϵ�ǰ�Է����Ӱڷ�����û�У��������������������һ��λ�ã� �н���3
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
        //3���жϵ�ǰ�Է�����ǰ���µ�����λ�õĸ����������ҡ����¡����ϡ����ϣ� �ж��ǲ����Ѿ���3�ż����������ˣ�����ǣ���ô����4
        if (other_last_place_pos != ChessPos.none)
        {
            List<ChessPos> other_next_Pos;
            //4����ʱ��Ҫ��ס�Է������ӣ������м��������
            //1���Է������Ŀţ�����AI�������أ� �м�û�п�λ����ʱ������õ������������λ��
            if (m_chessBoard.CheckIsSuccessByCount(other_chess_type, other_last_place_pos, 4))
            {

            }
            //2���Է��������ţ���û��AI�������أ� �м�û�п�λ����ʱ������õ��Ŀ���������λ�ã�
            //***���ף� ���ȡ���������˵�λ�á�
            // �У��ж�������һ����Ӯ�ĸ��ʸ���    
            //�ѣ��ж�������һ����Ӯ�ĸ��ʸ��������Ϳ�   ��
            //3���Է������Ŀţ���û��AI�������أ� �м��Ƿ��п�λ��
            //	�У��жϸ��Ŀ������м��λ���� ��λ��ԽС��Խ����
            //��
            //	***���ף� ��λ��ԽС��Խ���� ��
            // 	�У�ֻ��1�ŵ�ʱ�򷵻��м����λ�û��������������һ�����ж�����Ӯ�ĸ��ʴ� �����ӿ�λ������ж�����Ӯ�ĸ��ʴ�
            //	�ѣ�ֻ��1�ŵ�ʱ�򷵻��м����λ�û��������������һ�����ж�����Ӯ�ĸ��ʴ������ӿ�λ������ж�����Ӯ�ĸ��ʴ�
            //	���ж��ĸ�����������   ��

            //	û�У�ֱ���ж�Ϊ��
            //4���Է������Ŀţ�����AI�������أ� �м��п�λ��
            //	�У���
            //	***���ף� ��λ��ԽС��Խ���� ��
            // 	�У�ֻ��1�ŵ�ʱ�򷵻��м����λ�û�������û���ص����һ�������ż������ж�����Ӯ�ĸ��ʴ�
            //	�ѣ�ֻ��1�ŵ�ʱ�򷵻��м����λ�û�������û���ص����һ�������ż������ж�����Ӯ�ĸ��ʴ����ж��ĸ�����������   ��

            //5���Է��������ţ���û��AI�������أ� �м��п�λ
            //    �У���

            //    ***���ף� ��λ��ԽС��Խ���� ��
            // 	�У�ֻ��1�ŵ�ʱ�򷵻��м����λ�û��������������һ�����ж�����Ӯ�ĸ��ʴ� �����ӿ�λ������ж�����Ӯ�ĸ��ʴ�
            //	�ѣ�ֻ��1�ŵ�ʱ�򷵻��м����λ�û��������������һ�����ж�����Ӯ�ĸ��ʴ������ӿ�λ������ж�����Ӯ�ĸ��ʴ�
            //	���ж��ĸ�����������   ��

        }


        //    ���ǣ� ����5
        //5�����������***���ף� ���ȡ������û�б��¹���λ�á�    �У�������ǰ�������Ѿ��µ�λ�ó����� �ж��ĸ������ϵ�������ӽ�ʤ������������ѡȡһ�� �ѣ�������ǰ�������Ѿ��µ�λ�ó����� �ж��ĸ������ϵ�������ӽ�ʤ������������ѡȡһ�� �� ������ͣ�ÿ�����ѡһ������ȥ���֡�
        x_index = Random.Range(0, m_chessBoard.board_x_size);
        y_index = Random.Range(0, m_chessBoard.board_y_size);
        return new ChessPos(x_index, y_index);
    }
}
