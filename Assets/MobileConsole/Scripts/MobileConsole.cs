using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MobileConsole
{
	public class MobileConsole : MonoBehaviour
	{
		[SerializeField]
		private Skin skin = Skin.Light;
		
		[SerializeField]
		private bool showOnError = false;

		[SerializeField]
		private Corner tapCorner = Corner.TopLeft;
		
		[SerializeField]
		private bool disableInReleaseBuild = false;
		
		[Range(50, 500)]
		[SerializeField]
		private int maxLogEntries = 100;


		[SerializeField]
		private StyleSettings lightSkinStyleSettings = null;

		[SerializeField]
		private StyleSettings darkSkinStyleSettings = null;

		[SerializeField]
		private GameObject console = null;

		[SerializeField]
		private Transform canvasTransform = null;

		[SerializeField]
		private CanvasScaler canvasScaler = null;
		
		[SerializeField]
		private LogEntry logEntryPrefab = null;
		
		[SerializeField]
		private ScrollRect logScrollRect = null;

		[SerializeField]
		private GameObject logScrollbar = null;

		[SerializeField]
		private RectTransform logEntriesContainer = null;
		
		[SerializeField]
		private Toggle infoToggle = null;
		
		[SerializeField]
		private Toggle warningToggle = null;
		
		[SerializeField]
		private Toggle errorToggle = null;
		
		[SerializeField]
		private TMP_Text infoCountText = null;
		
		[SerializeField]
		private TMP_Text warningCountText = null;
		
		[SerializeField]
		private TMP_Text errorCountText = null;
		
		[SerializeField]
		private Button topLeftOpenButton = null;
		
		[SerializeField]
		private Button topRightOpenButton = null;
		
		[SerializeField]
		private Button bottomLeftOpenButton = null;
		
		[SerializeField]
		private Button bottomRightOpenButton = null;
		
		[SerializeField]
		private Button closeButton = null;
		
		[SerializeField]
		private Button clearButton = null;

		[SerializeField]
		private Toggle collapseToggle = null;

		[SerializeField]
		private LayoutElement stackTraceLayoutElement = null;
		
		[SerializeField]
		private ScrollRect stackTraceScrollRect = null;
		
		[SerializeField]
		private TMP_Text stackTraceText = null;
		
		[SerializeField]
		private RectTransform poolTransform = null;

		[SerializeField]
		private GameObject coverGameObject = null;

		private Button openButton;
		private readonly Queue<LogCall> pooledLogCalls = new Queue<LogCall>();
		private readonly Queue<LogCall> logCalls = new Queue<LogCall>();
		private readonly Queue<LogEntry> pooledLogEntries = new Queue<LogEntry>();
		private readonly Queue<LogEntry> usedLogEntries = new Queue<LogEntry>();
		private Dictionary<int, LogEntry> collapsedLogEntries;
		private LogEntry selectedLogEntry;
		private readonly Dictionary<LogType, bool> filter = new Dictionary<LogType, bool>();
		private readonly Dictionary<LogType, int> counts = new Dictionary<LogType, int>();

		private bool scrollToBottom;
		private bool scrollToBottomOnShow;
		private int clickCount;
		private float lastClickTime;
		
		private int resolutionX;
		private int resolutionY;
	
		void Awake()
		{
			if ((disableInReleaseBuild && IsReleaseBuild()) || IsDuplicate())
			{
				Destroy(gameObject);
			}
			else
			{
				DontDestroyOnLoad(gameObject);
				CreateEventSystemIfRequired();
				AddListeners();
				FillPool();
				ResizeStackTracePanel();
				ApplySkin();
				SetupFilters();
				ResizeSafeAreaAndButtons();
				AddLogHandler();
				
				resolutionX = Screen.width;
				resolutionY = Screen.height;
			}
		}
		
		private bool IsReleaseBuild()
		{
			return !Application.isEditor && !Debug.isDebugBuild;
		}
		
		private bool IsDuplicate()
		{
			return FindObjectsOfType(GetType()).Length > 1;
		}
		
		private void CreateEventSystemIfRequired()
		{
			if (FindObjectOfType<EventSystem>() == null)
			{
				var eventSystemGameObject = new GameObject();
				eventSystemGameObject.name = "EventSystem";
				eventSystemGameObject.AddComponent<EventSystem>();
				eventSystemGameObject.AddComponent<StandaloneInputModule>();
				eventSystemGameObject.transform.SetParent(canvasTransform);
			}
		}

		private void AddListeners()
		{
			infoToggle.onValueChanged.AddListener(value => FilterChanged(LogType.Log, value));
			errorToggle.onValueChanged.AddListener(value => FilterChanged(LogType.Error, value));
			warningToggle.onValueChanged.AddListener(value => FilterChanged(LogType.Warning, value));
			switch (tapCorner)
			{
				default:
					openButton = topLeftOpenButton;
					break;
				case Corner.TopRight:
					openButton = topRightOpenButton;
					break;
				case Corner.BottomLeft:
					openButton = bottomLeftOpenButton;
					break;
				case Corner.BottomRight:
					openButton = bottomRightOpenButton;
					break;
			}
			topLeftOpenButton.gameObject.SetActive(tapCorner == Corner.TopLeft);
			topRightOpenButton.gameObject.SetActive(tapCorner == Corner.TopRight);
			bottomLeftOpenButton.gameObject.SetActive(tapCorner == Corner.BottomLeft);
			bottomRightOpenButton.gameObject.SetActive(tapCorner == Corner.BottomRight);
			openButton.onClick.AddListener(Open);
			closeButton.onClick.AddListener(Close);
			clearButton.onClick.AddListener(Clear);
			collapseToggle.onValueChanged.AddListener(ToggleCollapse);
		}

		private void FillPool()
		{
			for (var i = 0; i < maxLogEntries; i++)
			{
				var logEntry = Instantiate(logEntryPrefab, poolTransform);
				pooledLogEntries.Enqueue(logEntry);
			}
		}
		
		private void ResizeStackTracePanel()
		{
			stackTraceLayoutElement.preferredHeight = Mathf.Floor(Screen.height / (4.5f * canvasTransform.localScale.y));
		}
		
		private void ApplySkin()
		{
			var styleSettings = skin == Skin.Light ? lightSkinStyleSettings : darkSkinStyleSettings;
			
			foreach (var style in GetComponentsInChildren<Style>(true))
			{
				style.SetStyleSettings(styleSettings);
			}
		}
		
		private void SetupFilters()
		{
			filter[LogType.Log] = true;
			filter[LogType.Warning] = true;
			filter[LogType.Error] = true;
		}

		private void ResizeSafeAreaAndButtons()
		{
			var consoleRectTransform = console.gameObject.GetComponent<RectTransform>();
			var canvasScale = canvasTransform.localScale;
			
			var bottom = Screen.safeArea.y;
			var top = Screen.height - Screen.safeArea.height - Screen.safeArea.y;
			var left = Screen.safeArea.x;
			var right = Screen.width - Screen.safeArea.width - Screen.safeArea.x;
			
			var sizeDelta = new Vector2(-Math.Abs(right + left) / canvasScale.x, -Math.Abs(top + bottom) / canvasScale.y);
			consoleRectTransform.sizeDelta = sizeDelta;

			var anchoredPosition = new Vector2(((left - right) / 2) / canvasScale.x, ((bottom - top) / 2) / canvasScale.y);
			consoleRectTransform.anchoredPosition = anchoredPosition;

			topLeftOpenButton.GetComponent<RectTransform>().localScale = Vector2.one / canvasScale;
			topRightOpenButton.GetComponent<RectTransform>().localScale = Vector2.one / canvasScale;
			bottomLeftOpenButton.GetComponent<RectTransform>().localScale = Vector2.one / canvasScale;
			bottomRightOpenButton.GetComponent<RectTransform>().localScale = Vector2.one / canvasScale;

			canvasScaler.matchWidthOrHeight = Screen.height > Screen.width ? 0.3f : 0.5f;
			
			coverGameObject.SetActive(Screen.safeArea.width < Screen.width || Screen.safeArea.height < Screen.height);
		}

		private void AddLogHandler()
		{
			Application.logMessageReceivedThreaded += HandleLog;
		}

		private void HandleLog(string logString, string stackTrace, LogType type)
		{
			LogCall logCall;
			lock (pooledLogCalls)
			{
				logCall = pooledLogCalls.Count > 0 ? pooledLogCalls.Dequeue() : new LogCall();
			}
			logCall.Type = type == LogType.Exception || type == LogType.Assert ? LogType.Error : type;
			logCall.LogString = logString;
			logCall.StackTrace = stackTrace;
			lock (logCalls)
			{
				logCalls.Enqueue(logCall);
			}
		}
	
		void Start()
		{
			UpdateCountTexts();
			ClearStackTrace();
			Close();
		}

		void Update()
		{
			CheckScrollToBottom();
			HandleLogCalls();
			CheckChangedResolution();
		}
		
		private void CheckChangedResolution()
		{
			if (resolutionX != Screen.width || resolutionY != Screen.height)
			{
				ResizeSafeAreaAndButtons();
				ResizeStackTracePanel();
				
				resolutionX = Screen.width;
				resolutionY = Screen.height;
			}
		}

		private void CheckScrollToBottom()
		{
			if (scrollToBottom)
			{
				ScrollLogToBottom();
				scrollToBottom = false;
			}
		}

		private void HandleLogCalls()
		{
			lock (logCalls)
			{
				if (logCalls.Count == 0)
				{
					return;
				}
			}
			
			var wasLogAtBottom = IsLogAtBottom();
			var shouldShowDueToError = false;
			
			lock (logCalls)
			{
				while (logCalls.Count > 0)
				{
					EnsureFreeLogEntryInPool();
					
					var logCall = logCalls.Dequeue();
					HandleLogCall(logCall);
					lock (pooledLogCalls)
					{
						pooledLogCalls.Enqueue(logCall);
					}

					if (showOnError && logCall.Type == LogType.Error)
					{
						shouldShowDueToError = true;
					}
				}
			}
			
			if (IsShown())
			{
				scrollToBottom = wasLogAtBottom;
			}
			else
			{
				scrollToBottomOnShow = shouldShowDueToError || wasLogAtBottom;
				
				if (shouldShowDueToError)
				{
					Show();
				}
			}
		}
		
		private void EnsureFreeLogEntryInPool()
		{
			while (usedLogEntries.Count >= maxLogEntries)
			{
				var firstLogEntry = usedLogEntries.Dequeue();
				
				pooledLogEntries.Enqueue(firstLogEntry);
				firstLogEntry.SetParent(poolTransform);

				DecrementCount(firstLogEntry.Type);
				
				if (firstLogEntry == selectedLogEntry)
				{
					ClearSelection();
				}

				if (firstLogEntry.IsCollapsedRepresentative())
				{
					collapsedLogEntries.Remove(firstLogEntry.GetHashCode());
					
					var collapsedLinkedLogEntries = firstLogEntry.GetCollapsedLinkedLogEntries();
					if (collapsedLinkedLogEntries.Count > 1)
					{
						var newCollapsedRepresentative = collapsedLinkedLogEntries[1];

						for (var i = 1; i < collapsedLinkedLogEntries.Count; i++)
						{
							newCollapsedRepresentative.AddCollapsedLinkedLogEntry(collapsedLinkedLogEntries[i]);
						}

						collapsedLogEntries.Add(newCollapsedRepresentative.GetHashCode(), newCollapsedRepresentative);
						
						newCollapsedRepresentative.Refresh();
						newCollapsedRepresentative.SetVisible(firstLogEntry.IsVisible());
					}
					
					firstLogEntry.ClearCollapsedLinkedLogEntries();
				}
			}
		}

		private void HandleLogCall(LogCall logCall)
		{
			var logEntry = pooledLogEntries.Dequeue();
			logEntry.Populate(logCall);
			usedLogEntries.Enqueue(logEntry);
				
			logEntry.SetParent(logEntriesContainer);
			logEntry.gameObject.GetComponent<Button>().onClick.RemoveAllListeners();
			logEntry.gameObject.GetComponent<Button>().onClick.AddListener(() => SelectLogEntry(logEntry));

			if (IsCollapsed())
			{
				var hashCode = logCall.GetHashCode();
				if (collapsedLogEntries.ContainsKey(hashCode))
				{
					logEntry.Refresh();
					logEntry.Hide();
					
					var collapsedLogEntry = collapsedLogEntries[hashCode];
					collapsedLogEntry.AddCollapsedLinkedLogEntry(logEntry);
					collapsedLogEntry.Refresh();
				}
				else
				{
					logEntry.AddCollapsedLinkedLogEntry(logEntry);
					logEntry.Refresh();
					
					if (filter[logEntry.Type])
					{
						logEntry.Show();
					}
					else
					{
						logEntry.Hide();
					}
					
					collapsedLogEntries.Add(hashCode, logEntry);
				}
			}
			else
			{
				logEntry.Refresh();
				
				if (filter[logEntry.Type])
				{
					logEntry.Show();
				}
				else
				{
					logEntry.Hide();
				}
			}

			IncrementCount(logCall.Type);
		}
		
		private bool IsLogAtBottom()
		{
			return !logScrollbar.activeSelf || Math.Abs(logScrollRect.verticalNormalizedPosition) < 0.015f;
		}

		private void ScrollLogToBottom()
		{
			logScrollRect.verticalNormalizedPosition = 0f;
		}

		private void Close()
		{
			console.SetActive(false);
			openButton.gameObject.SetActive(true);
		}

		private void Open()
		{
			if (!IsShown())
			{
				if (Time.time - lastClickTime < 0.4f)
				{
					clickCount++;
					if (clickCount >= 2)
					{
						Show();
					}
				}
				else
				{
					clickCount = 0;
				}
				lastClickTime = Time.time;
			}
		}

		private void Show()
		{
			openButton.gameObject.SetActive(false);
			console.SetActive(true);
			
			if (scrollToBottomOnShow)
			{
				ScrollLogToBottom();
				scrollToBottomOnShow = false;
			}
		}

		private bool IsShown()
		{
			return console.activeSelf;
		}

		private void FilterChanged(LogType type, bool value)
		{
			filter[type] = value;

			foreach (var logEntry in usedLogEntries)
			{
				var isVisible = logEntry.IsVisible();
				var shouldBeVisible = filter[logEntry.Type];

				if (shouldBeVisible && IsCollapsed() && !logEntry.IsCollapsedRepresentative())
				{
					shouldBeVisible = false;
				}
				
				if (isVisible && !shouldBeVisible)
				{
					logEntry.Hide();
				}
				else if (!isVisible && shouldBeVisible)
				{
					logEntry.Show();
				}
			}
			
			if (!value && selectedLogEntry != null && selectedLogEntry.Type == type)
			{
				ClearSelection();
			}
		}

		private bool IsCollapsed()
		{
			return collapsedLogEntries != null;
		}

		private void ToggleCollapse(bool value)
		{
			ClearSelection();
			
			if (!IsCollapsed() && value)
			{
				Collapse();
			}
			else if (IsCollapsed() && !value)
			{
				Expand();
			}
			
			Canvas.ForceUpdateCanvases();
			
			scrollToBottom = true;
		}

		private void Collapse()
		{
			collapsedLogEntries = new Dictionary<int, LogEntry>();
			
			foreach (var logEntry in usedLogEntries)
			{
				var hashCode = logEntry.GetHashCode();
				if (collapsedLogEntries.ContainsKey(hashCode))
				{
					logEntry.Hide();
					
					var collapsedLogEntry = collapsedLogEntries[hashCode];
					collapsedLogEntry.AddCollapsedLinkedLogEntry(logEntry);
					collapsedLogEntry.Refresh();
				}
				else
				{
					logEntry.AddCollapsedLinkedLogEntry(logEntry);
					logEntry.Refresh();
					
					collapsedLogEntries.Add(hashCode, logEntry);
				}
			}
		}

		private void Expand()
		{
			foreach (var logEntry in usedLogEntries)
			{
				var shouldBeVisible = filter[logEntry.Type];
				logEntry.SetVisible(shouldBeVisible);

				if (logEntry.IsCollapsedRepresentative())
				{
					logEntry.ClearCollapsedLinkedLogEntries();
					logEntry.Refresh();
				}
			}
			
			collapsedLogEntries = null;
		}
		
		private void IncrementCount(LogType type)
		{
			counts[type] = counts.ContainsKey(type) ? counts[type] + 1 : 1;
			UpdateCountTexts();
		}
		
		private void DecrementCount(LogType type)
		{
			counts[type] = counts.ContainsKey(type) ? counts[type] - 1 : 0;
			UpdateCountTexts();
		}
		
		private void ResetCounts()
		{
			counts.Clear();
			UpdateCountTexts();
		}

		private void UpdateCountTexts()
		{
			infoCountText.text = "" + (counts.ContainsKey(LogType.Log) ? counts[LogType.Log] : 0);
			warningCountText.text = "" + (counts.ContainsKey(LogType.Warning) ? counts[LogType.Warning] : 0);
			errorCountText.text = "" + (counts.ContainsKey(LogType.Error) ? counts[LogType.Error] : 0);
		}

		private void Clear()
		{
			ClearSelection();
			CleanUpLogEntries();
			ResetCounts();
		}

		private void CleanUpLogEntries()
		{
			foreach (var logEntry in usedLogEntries)
			{
				logEntry.ClearCollapsedLinkedLogEntries();
				logEntry.SetParent(poolTransform);
				pooledLogEntries.Enqueue(logEntry);
				
			}
			usedLogEntries.Clear();
			collapsedLogEntries?.Clear();
		}

		private void SelectLogEntry(LogEntry logEntry)
		{
			var alreadySelected = selectedLogEntry == logEntry;
			
			ClearSelection();

			if (!alreadySelected)
			{
				selectedLogEntry = logEntry;
				selectedLogEntry.SetSelected(true);
				selectedLogEntry.Refresh();
				ShowStackTrace(logEntry);
			}
		}

		private void ClearSelection()
		{
			if (selectedLogEntry != null)
			{
				selectedLogEntry.SetSelected(false);
				selectedLogEntry.Refresh();
				selectedLogEntry = null;
				ClearStackTrace();
			}
		}

		private void ClearStackTrace()
		{
			stackTraceText.text = "\n";
		}
	
		private void ShowStackTrace(LogEntry logEntry)
		{
			stackTraceText.text = logEntry.LogString + "\n" + logEntry.StackTrace;
			stackTraceScrollRect.verticalNormalizedPosition = 1.0f;
		}
	}
}
