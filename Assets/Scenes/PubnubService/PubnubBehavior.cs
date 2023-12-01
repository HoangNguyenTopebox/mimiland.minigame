using System;
using PubnubApi;
using UnityEngine;

public class PubnubBehavior : MonoBehaviour
{
    private string _channelName = "Test";

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        PubnubService.Instance.Subscribe(_channelName);
        PubnubService.Instance.AddReceivedSignal(OnReceivedSignal);
    }

    [ContextMenu("Send Publish")]
    public void SendPublish()
    {
        PubnubService.Instance.Publish(_channelName, SignalTypeToString(PNSignalType.SendJoinWorld),
            new PNPublishResultExt(OnPublish));
    }

    [ContextMenu("Send Signal")]
    public void SendSignal()
    {
        PubnubService.Instance.SendSignal(_channelName, SignalTypeToString(PNSignalType.SendJoinWorld),
            new PNPublishResultExt(OnSignal));
    }

    private void OnPublish(PNPublishResult result, PNStatus status)
    {
        Debug.Log($"[PUBNUB] OnPublish: {result}");
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
        PubnubService.Instance.Unsubscribe(_channelName);
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