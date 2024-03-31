using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


public class ChessFactory: Singleton<ChessFactory>
{
    public Dictionary<string, object> unit_maps = new Dictionary<string, object>();

    // 创建实体
    public T CreatEntity<T>(EntityType entityType, out string entity_id, params object[] init_infos) where T : Entity, new()
    {
        T entity = new T();
        entity_id = Guid.NewGuid().ToString();
        entity.SetID(entity_id);
        switch (entityType)
        {
            case EntityType.Chess:
                break;
            case EntityType.ChessBoard:
                break;
            default:
                break;
        }
        unit_maps.Add(entity_id, entity);
        return entity;
    }

    public T CreatEntity<T>(
        EntityType entityType, out string entity_id, GameObject m_entity=null) where T : MonoEntity, new()
    {
        if(m_entity == null)
        {
            m_entity = new GameObject();
        }
 
        T entity = m_entity.GetComponent<T>();
        if (entity == null)
        {
            m_entity.AddComponent<T>();
        }
        entity_id = Guid.NewGuid().ToString();
        entity.SetID(entity_id);
        switch (entityType)
        {
            case EntityType.Chess:
                break;
            case EntityType.ChessBoard:
                break;
            default:
                break;
        }
        unit_maps.Add(entity_id, entity);
        return entity;
    }

    public T GetEntity<T>(string unit_id, out bool has_entity) where T: IEntity, new()
    {
        object entity;
        if (unit_maps.TryGetValue(unit_id, out entity))
        {
            has_entity = true;
            if (entity is T)
            {
                return (T)entity;
            }
            return default;
        }
        has_entity = false;
        return default;
    }

    public void RemoveEntity(string unit_id)
    {
        object entity;
        if (unit_maps.TryGetValue(unit_id, out entity))
        {
            unit_maps.Remove(unit_id);
        }
    }
}

// 游戏进度控制
public class ProcessController : MonSingleton<ProcessController>
{
    public CanvasScaler canvas_scaler;
    public ChessBoard board;
    public MyChessAI _m_ai;
    
    public bool is_white_first = false;

    public bool use_ai = false;
    [HideInInspector]
    public bool is_white_start = false;

    private Vector2 current_screen_size = Vector2.zero;

    private bool is_game_start = false;
    public bool IsGameStart
    {
        get { return is_game_start;}
    }

    private bool wait_ai = false;
    private void OnEnable()
    {
        EventManager.Instance.RegisterEvent<bool>(
            EventType.GameStateChanged, SetGameState);
        EventManager.Instance.RegisterEvent<ChessPos>(
            EventType.ON_AI_FINISHED, OnAIChessFinished);
        is_white_start = is_white_first;
        _m_ai.SetBelongChessboard(board);
    }

    private void SetGameState(bool state)
    {
        is_game_start = state;
    }

    void Start()
    {
        Vector2 reference_resolution = canvas_scaler.referenceResolution;
        current_screen_size = reference_resolution;
    }

    void Update()
    {
        if (is_game_start)
        {
            if (Screen.width != current_screen_size.x || Screen.height != current_screen_size.y)
            {
                EventManager.Instance.PublicEvent(
                    EventType.OnScreenChanged, Screen.width, Screen.height, canvas_scaler.referenceResolution);
                current_screen_size.x = Screen.width;
                current_screen_size.y = Screen.height;
            }
            ChessType chess_type = ChessType.BLACK;
            if (is_white_start)
            {
                chess_type = ChessType.WHITE;
            }
            if (use_ai && chess_type == _m_ai.AIChessType)
            {
                if(!wait_ai)
                {
                    EventManager.Instance.PublicEvent(EventType.ON_AI_START);
                    wait_ai = true;
                }
                return;
            }
            if (Input.GetMouseButtonDown(0))
            {
                bool is_on_right_pos;
                ChessPos chessPos = board.CaculateAnchorPosIndex(Input.mousePosition, out is_on_right_pos);
                if (is_on_right_pos)
                {
                    if(board.PlaceChess(chess_type, chessPos.x, chessPos.y))
                    {
                        is_white_start = !is_white_start;
                    }
                    wait_ai = false;
                }
            }

        }

    }

    void OnAIChessFinished(ChessPos next_pos)
    {
        ChessType chess_type = ChessType.BLACK;
        if (is_white_start)
        {
            chess_type = ChessType.WHITE;
        }
        Debug.Log(next_pos);
        if (board.PlaceChess(chess_type, next_pos.x, next_pos.y))
        {
            is_white_start = !is_white_start;
        }
        wait_ai = false;
        Debug.Log("OnAIChessFinished");
    }
}
