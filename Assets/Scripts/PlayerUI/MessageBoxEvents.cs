using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[System.Serializable]
public class MessageBoxEvents
{
    [SerializeField] TMP_Text m_messageLogText = null;

    public void AddEventsToMessageLog(MessageLog messageLog)
    {
        messageLog.AddOnAddMessageEvent(ShowMessage);
        messageLog.AddOnRemoveMessageEvent(RemoveMessageLength);
    }

    public void RemoveEventsFromMessageLog(MessageLog messageLog)
    {
        messageLog.RemoveOnAddMessageEvent(ShowMessage);
        messageLog.RemoveOnRemoveMessageEvent(RemoveMessageLength);
    }

    public void ShowMessage(string message)
    {
        m_messageLogText.text += "\n" + message;
    }

    public void RemoveMessageLength(string message)
    {
        m_messageLogText.text = m_messageLogText.text.Remove(0, message.Length + 1);
    }

    public void ClearText()
    {
        m_messageLogText.text = "";
    }
}
