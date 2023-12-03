using System;
using System.Collections.Generic;
using PubnubApi;
using UnityEngine;

public class PubnubBehavior : MonoBehaviour
{
    private string _worldChannel = "w-1";
    private string _chatChannel = "g-1";
    private List<string> Channels
    {
        get => new List<string>
        {
            _worldChannel,
            _chatChannel,
        };
    }

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        PubnubService.Instance.Subscribe(Channels);
        PubnubService.Instance.AddReceivedSignal(OnReceivedSignal);
    }


    [ContextMenu("Send Publish")]
    public void SendPublish()
    {
        PubnubService.Instance.Publish(_chatChannel, "Hello World",
            new PNPublishResultExt(OnPublish));
    }

    [ContextMenu("Fetch History")]
    public void FetchHistory()
    {
        PubnubService.Instance.FetchHistory(_chatChannel, 10, new PNFetchHistoryResultExt(OnFetchHistory));
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
                    Debug.Log($"[PUBNUB] OnFetchHistory: {message.Entry} - {message.Timetoken} - {message.Meta} - {message.Uuid}");
                }
            }
        }
    }

    [ContextMenu("Send Signal")]
    public void SendSignal()
    {
        PubnubService.Instance.SendSignal(_worldChannel, SignalTypeToString(PNSignalType.SendJoinWorld),
            new PNPublishResultExt(OnSignal));
    }

    private void OnPublish(PNPublishResult result, PNStatus status)
    {
        if (status.Error)
        {
            Debug.LogError($"[PUBNUB] OnPublish: {status.ErrorData.Information}");
        }
        else
        {
            //Debug.Log($"[PUBNUB] OnPublish: {result}");
        }
    }

    private void OnSignal(PNPublishResult result, PNStatus status)
    {
        Debug.Log($"[PUBNUB] OnSignal: {result}");
    }

    private void OnReceivedSignal(PNSignalResult<object> result)
    {
        //Debug.Log($"[PUBNUB] OnReceivedSignal: {result}");
        switch (result.Message)
        {
            case PNSignal.SendJoinWorld:

                Debug.Log($"[PUBNUB] OnReceivedSignal: {result.Message}");
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private string SignalTypeToString(PNSignalType type)
    {
        switch (type)
        {
            case PNSignalType.SendJoinWorld:
                return PNSignal.SendJoinWorld;
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }
    }

    private void OnDestroy()
    {
        PubnubService.Instance.Unsubscribe(Channels);
        PubnubService.Instance.ClearAll();
    }
}

public static class PNSignal
{
    public const string SendJoinWorld = "W-1";
}

public enum PNSignalType
{
    SendJoinWorld,
}