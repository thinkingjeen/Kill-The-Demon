using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MobileConsole
{
    internal class LogEntry : MonoBehaviour
    {
        private const string DateFormat = "HH:mm:ss";

        [SerializeField]
        private GameObject selectionGameObject = null;
        
        [SerializeField]
        private TMP_Text logText = null;

        [SerializeField]
        private TMP_Text timeText = null;

        [SerializeField]
        private Image infoIcon = null;

        [SerializeField]
        private Image warningIcon = null;

        [SerializeField]
        private Image errorIcon = null;

        [SerializeField]
        private GameObject countGameObject = null;
        
        [SerializeField]
        private TMP_Text countText = null;
        
        public LogType Type => type;
        public string LogString => logString;
        public string StackTrace => stackTrace;
        public DateTime TimeStamp => timeStamp;
        
        private LogType type;
        private string logString;
        private string stackTrace;
        private DateTime timeStamp;
        private List<LogEntry> collapsedLinkedLogEntries;
        private bool isSelected;
        private int hashCode;
        private RectTransform cachedRectTransform;

        private Style logTextStyle;
        private Style timeTextStyle;
        
        public void Populate(LogCall logCall)
        {
            logString = logCall.LogString;
            stackTrace = logCall.StackTrace;
            type = logCall.Type;
            timeStamp = DateTime.Now;
            
            RecalculateHashCode();
            
            logText.text = logString;
            infoIcon.gameObject.SetActive(type == LogType.Log);
            warningIcon.gameObject.SetActive(type == LogType.Warning);
            errorIcon.gameObject.SetActive(type == LogType.Error);
            
            logTextStyle = logText.gameObject.GetComponent<Style>();
            timeTextStyle = timeText.gameObject.GetComponent<Style>();
        }

        public void SetParent(RectTransform parent)
        {
            if (cachedRectTransform == null)
            {
                cachedRectTransform = gameObject.GetComponent<RectTransform>();
            }
            
            cachedRectTransform.SetParent(parent);
        }

        public void SetSelected(bool selected)
        {
            isSelected = selected;
            selectionGameObject.SetActive(selected);
        }

        public void AddCollapsedLinkedLogEntry(LogEntry logEntry)
        {
            if (collapsedLinkedLogEntries == null)
            {
                collapsedLinkedLogEntries = new List<LogEntry>();
            }
            
            collapsedLinkedLogEntries.Add(logEntry);
        }

        public List<LogEntry> GetCollapsedLinkedLogEntries()
        {
            return collapsedLinkedLogEntries;
        }

        public void ClearCollapsedLinkedLogEntries()
        {
            collapsedLinkedLogEntries = null;
        }

        public bool IsCollapsedRepresentative()
        {
            return collapsedLinkedLogEntries != null;
        }

        public void Refresh()
        {
            if (isSelected)
            {
                logTextStyle.SetStyle(StyleType.SelectedText);
                timeTextStyle.SetStyle(StyleType.SelectedText);
            }
            else
            {
                logTextStyle.SetStyle(StyleType.Text);
                timeTextStyle.SetStyle(StyleType.Text);
            }
            
            if (collapsedLinkedLogEntries != null)
            {
                countGameObject.SetActive(true);
                countText.text = $"{collapsedLinkedLogEntries.Count}";
            }
            else
            {
                countGameObject.SetActive(false);
            }
            
            if (isSelected)
            {
                if (collapsedLinkedLogEntries != null)
                {
                    var lastCollapsedLinkedLogEntry = collapsedLinkedLogEntries[collapsedLinkedLogEntries.Count - 1];
                    timeText.text = lastCollapsedLinkedLogEntry.TimeStamp.ToString(DateFormat);
                }
                else
                {
                    timeText.text = timeStamp.ToString(DateFormat);
                }
                
                timeText.gameObject.SetActive(true);
            }
            else
            {
                timeText.gameObject.SetActive(false);
            }
        }

        public void Show()
        {
            SetVisible(true);
        }

        public void Hide()
        {
            SetVisible(false);
        }

        public void SetVisible(bool visible)
        {
            gameObject.SetActive(visible);
        }

        public bool IsVisible()
        {
            return gameObject.activeSelf;
        }

        private void RecalculateHashCode()
        {
            hashCode = Utils.CalculateHashCode(type, logString, stackTrace);
        }

        public override int GetHashCode()
        {
            return hashCode;
        }
    }
}