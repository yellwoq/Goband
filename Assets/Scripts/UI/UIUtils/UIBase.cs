public delegate void OnReconnect();

public class UIBase: MonoEntity
{
    public string ui_name;

    public OnReconnect on_reconnect_callback;

    private void Awake()
    {
        ui_name = GetType().Name;
    }
    public void SetVisible(bool visible)
    {
        gameObject.SetActive(visible);
    }

    protected virtual void OnReconnect()
    {
        //断线重连处理
        on_reconnect_callback?.Invoke();
    }
}