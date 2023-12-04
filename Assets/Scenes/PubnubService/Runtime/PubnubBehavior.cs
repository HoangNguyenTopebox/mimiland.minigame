using System;
using System.Collections.Generic;
using PubnubApi;
using UnityEngine;

public class PubnubBehavior : MonoBehaviour
{
    private string _worldChannel = "w-1";
    private string _chatChannel = "g-1";
    private PubnubService _pubnubService;

    private string[] Channels
    {
        get => new string[]
        {
            _worldChannel,
            _chatChannel,
        };
    }

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        _pubnubService = PubnubService.Instance;

        //_pubnubService.Subscribe(Channels);
        _pubnubService.OnSignalCallback += OnReceivedSignal;
        _pubnubService.OnStatusCallback += OnStatusCallback;
        _pubnubService.OnMessageCallback += OnMessageCallback;
    }

    #region Methods

    public void JoinChannel(string[] channels = null)
    {
        channels ??= Channels;
        _pubnubService.Subscribe(channels);
    }

    public void LeaveChannel(string[] channels = null)
    {
        channels ??= Channels;
        _pubnubService.Unsubscribe(channels);
    }

    private string _message = "Hello World";
    private Queue<string> _messages = new Queue<string>();

    public void SendMessage()
    {
        _pubnubService.SendMessage(_chatChannel, _message,
            new PNPublishResultExt(OnMessage));
    }


    public void SendSignal()
    {
        _pubnubService.SendSignal(_worldChannel, SignalTypeToString(PNSignalType.SendJoinWorld),
            null, new PNPublishResultExt(OnSignal));
    }

    public void FetchHistory()
    {
        _pubnubService.FetchHistory(_chatChannel, 10, new PNFetchHistoryResultExt(OnFetchHistory));
    }

    public void Disconnect()
    {
        _pubnubService.Disconnect();
    }

    public void Reconnect()
    {
        _pubnubService.Reconnect();
    }

    #endregion

    #region Callbacks

    private void OnMessage(PNPublishResult result, PNStatus status)
    {
        if (status.Error)
        {
            Debug.LogError($"[PUBNUB] OnMessage: {status.ErrorData.Information}");
        }
    }

    private void OnSignal(PNPublishResult result, PNStatus status)
    {
        if (status.Error)
        {
            Debug.LogError($"[PUBNUB] OnSignal: {status.ErrorData.Information}");
        }
    }

    private void OnReceivedSignal(PNSignalResult<object> result)
    {
        //Debug.Log($"[PUBNUB] OnReceivedSignal: {result}");
        switch (result.Message)
        {
            case PNSignal.SendJoinWorld:
                Debug.Log($"[PUBNUB] OnReceivedSignal: {PrintSignalResult(result)}");
                break;
            case PNSignal.SendCreateLobby:
                Debug.Log($"[PUBNUB] OnReceivedSignal: {PrintSignalResult(result)}");
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void OnMessageCallback(PNMessageResult<object> messageResult)
    {
        _messages.Enqueue($"<color=#000>{messageResult.Publisher} : {messageResult.Message}</color>");
    }

    private void OnStatusCallback(PNStatusCategory status)
    {
        if (status == PNStatusCategory.PNConnectedCategory)
        {
            FetchHistory();
        }
        else if (status == PNStatusCategory.PNDisconnectedCategory)
        {
        }
        else if (status == PNStatusCategory.PNReconnectedCategory)
        {
        }
        else if (status == PNStatusCategory.PNUnexpectedDisconnectCategory)
        {
            //_pubnubService.ClearAll();
            Debug.LogWarning("[PUBNUB] Unexpected Disconnect Try Again");
        }

        //Debug.Log($"[PUBNUB] OnStatusCallback: {status}");
    }

    private void OnFetchHistory(PNFetchHistoryResult fetchHistoryResult, PNStatus status)
    {
        if (status.Error)
        {
            Debug.LogError($"[PUBNUB] OnFetchHistory: {status.ErrorData.Information}");
        }
        else
        {
            foreach (var messages in fetchHistoryResult.Messages)
            {
                foreach (var message in messages.Value)
                {
                    Debug.Log(
                        $"[PUBNUB] OnFetchHistory: {message.Entry} - {message.Timetoken} - {message.Meta} - {message.Uuid}");

                    _messages.Enqueue($"<color=#000>{message.Uuid} : {message.Entry}</color>");
                }
            }
        }
    }

    #endregion


    private string PrintSignalResult(PNSignalResult<object> result)
    {
        return
            $"[PUBNUB] OnReceivedSignal: {result.Message} - {result.Subscription} - {result.Timetoken} - {result.UserMetadata} - {result.Publisher}";
    }

    private string SignalTypeToString(PNSignalType type)
    {
        switch (type)
        {
            case PNSignalType.SendJoinWorld:
                return PNSignal.SendJoinWorld;
            case PNSignalType.SendCreateLobby:
                return PNSignal.SendCreateLobby;
            case PNSignalType.SendJoinLobby:
                return PNSignal.SendJoinLobby;
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }
    }


    private void OnGUI()
    {
//#if UNITY_EDITOR
        if (GUI.Button(new Rect(10, 10, 100, 50), "Send Message"))
        {
            SendMessage();
        }

        if (GUI.Button(new Rect(200, 70, 100, 50), "Join Channel"))
        {
            JoinChannel();
        }

        if (GUI.Button(new Rect(200, 130, 100, 50), "Leave Channel"))
        {
            LeaveChannel();
        }

        _message = GUI.TextField(new Rect(200, 10, 200, 50), _message);

        GUI.Label(new Rect(Screen.width / 2, Screen.height / 2, 1000, Screen.height), string.Join("\n", _messages));

        if (GUI.Button(new Rect(10, 70, 100, 50), "Send Signal"))
        {
            SendSignal();
        }

        if (GUI.Button(new Rect(10, 130, 100, 50), "Fetch History"))
        {
            FetchHistory();
        }

        if (GUI.Button(new Rect(10, 190, 100, 50), "Disconnect"))
        {
            Disconnect();
        }

        if (GUI.Button(new Rect(10, 250, 100, 50), "Reconnect"))
        {
            Reconnect();
        }

        GUIText(PubnubService.Instance.GetUuid());
//#endif
    }


    private void GUIText(string text)
    {
//#if UNITY_EDITOR
        GUI.Label(new Rect(Screen.width / 2, 10, 1000, 50), text);
//#endif
    }

    private void OnDestroy()
    {
        _pubnubService.Unsubscribe(Channels);
    }

    private void OnApplicationQuit()
    {
        _pubnubService.ClearAll();
    }
}

public static class PNSignal
{
    public const string SendJoinWorld = "W-1";
    public const string SendCreateLobby = "C-1";
    public const string SendJoinLobby = "J-1";
}

public enum PNSignalType
{
    SendJoinWorld,
    SendCreateLobby,
    SendJoinLobby,
}