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
    UPGRADED_WEAPON,
    REFRESH_STATUS,
    EQUIPPED_PASSIVE,
    EQUIPPED_ACTIVE,
    EQUIPPED_SKILL,
    ITEM_PICKED_UP,
    ACHIEVEMENT_UNLOCKED,
    NFTTICKET_EARNED,
    RESTART
}
public class Mediator : MonoBehaviour, IMediator
{
    #region instance
    private static Mediator instance = null;
    public static Mediator Instance { get { return instance; } }

    private void Awake()
    {
        // Scene�� �̹� �ν��Ͻ��� ���� �ϴ��� Ȯ�� �� ó��
        if (instance)
        {
            Destroy(this.gameObject);
            return;
        }
        // instance�� ���� ������Ʈ�� �����
        instance = this;

        // Scene �̵� �� ���� ���� �ʵ��� ó��
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