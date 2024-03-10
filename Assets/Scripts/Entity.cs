
using UnityEngine;

public interface IEntity
{

    public string EntityID
    {
        get
        {
            return "";
        }
    }
    public void SetID(string entity_id);
}

public abstract class Entity : IEntity
{
    protected string entity_id;
    public string EntityID
    {
        get
        {
            return entity_id;
        }
    }

    public void SetID(string chess_id)
    {
        this.entity_id = chess_id;
    }

}

public abstract class MonoEntity : MonoBehaviour, IEntity
{
    //Æå×Óid
    protected string entity_id;
    public string EntityID
    {
        get
        {
            return entity_id;
        }
    }

    public void SetID(string chess_id)
    {
        this.entity_id = chess_id;
    }

}