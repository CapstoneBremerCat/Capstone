using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Game;
public enum GameEvent
{
    INPUT_UPDATED,
    SKILL_ACTIVATED,
    SKILL_AVAILABLE,
    EQUIPPED_WEAPON,
    REFRESH_STATUS,
    EQUIPPED_PASSIVE,
    EQUIPPED_ACTIVE,
    EQUIPPED_SKILL,
    ITEM_PICKED_UP,
    RESTART
}
public class Mediator : MonoBehaviour, IMediator
{
    #region instance
    private static Mediator instance = null;
    public static Mediator Instance { get { return instance; } }

    private void Awake()
    {
        // Scene에 이미 인스턴스가 존재 하는지 확인 후 처리
        if (instance)
        {
            Destroy(this.gameObject);
            return;
        }
        // instance를 유일 오브젝트로 만든다
        instance = this;

        // Scene 이동 시 삭제 되지 않도록 처리
        DontDestroyOnLoad(this.gameObject);
    }
    #endregion

    private Dictionary<GameEvent, List<Action<object>>> _eventHandlers 
        = new Dictionary<GameEvent, List<Action<object>>>();

    public void Notify(object sender, GameEvent eventName, object eventData)
    {
        if (_eventHandlers.TryGetValue(eventName, out List<Action<object>> handlers))
        {
            foreach (var handler in handlers)
            {
                handler(eventData);
            }
        }
    }

    public void RegisterEventHandler(GameEvent eventName, Action<object> handler)
    {
        if (!_eventHandlers.TryGetValue(eventName, out List<Action<object>> handlers))
        {
            handlers = new List<Action<object>>();
            _eventHandlers[eventName] = handlers;
        }

        handlers.Add(handler);
    }

    public void UnregisterEventHandler(GameEvent eventName, Action<object> handler)
    {
        if (_eventHandlers.TryGetValue(eventName, out List<Action<object>> handlers))
        {
            handlers.Remove(handler);
        }
    }


}