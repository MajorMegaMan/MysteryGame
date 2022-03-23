using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessageLog
{
    int m_maxCapacity;
    Queue<string> m_messages;

    public delegate void MessageAction(string message);
    MessageAction m_onMessageAdd = null;
    MessageAction m_onMessageRemove = null;

    public MessageLog(int maxCapacity)
    {
        m_maxCapacity = maxCapacity;
        m_messages = new Queue<string>(maxCapacity);
    }

    public void AddMessage(string message)
    {
        if(m_messages.Count == m_maxCapacity)
        {
            // Remove first message as the queue is too big
            m_onMessageRemove?.Invoke(m_messages.Dequeue());
        }
        // Add to queue
        m_messages.Enqueue(message);
        m_onMessageAdd?.Invoke(message);
    }

    public string[] GetMessages()
    {
        return m_messages.ToArray();
    }

    public void AddOnAddMessageEvent(MessageAction messageAction)
    {
        m_onMessageAdd += messageAction;
    }

    public void RemoveOnAddMessageEvent(MessageAction messageAction)
    {
        m_onMessageAdd -= messageAction;
    }

    public void ClearOnAddMessageEvent()
    {
        m_onMessageAdd = null;
    }

    public void AddOnRemoveMessageEvent(MessageAction messageAction)
    {
        m_onMessageRemove += messageAction;
    }

    public void RemoveOnRemoveMessageEvent(MessageAction messageAction)
    {
        m_onMessageRemove -= messageAction;
    }

    public void ClearOnRemoveMessageEvent()
    {
        m_onMessageRemove = null;
    }
}
