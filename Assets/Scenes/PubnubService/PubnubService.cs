using System;
using System.Collections.Generic;
using PubnubApi;
using PubnubApi.Unity;
using UnityEngine;

public class PubnubService
{
    private static PubnubService _instance;

    public static PubnubService Instance
    {
        get { return _instance ??= new PubnubService(); }
    }

    private PubnubService()
    {
        Initialize();
    }

    private event Action<PNSignalResult<object>> OnSignalCallback;

    private Pubnub _pubnub;
    private PNConfiguration _pnConfiguration;
    private SubscribeCallbackListener _pnSubscribeCallbackListener;
    private bool _isInitialized;
    private const string PublishKey = "pub-c-a221cc05-1cba-44cf-837d-9e684a6bb244";
    private const string SubscribeKey = "sub-c-719c2465-e21a-44f9-856d-3f3b9eb88e44";
    private const string SecretKey = "sec-c-MDIzN2Y2ZjktODg3Ny00MGZlLTk2YjYtMzJmY2JjNTFmNzhm";

    private void Initialize()
    {
        if (_isInitialized)
        {
            Debug.LogWarning("PubnubService is already initialized.");
            return;
        }

        _isInitialized = true;
        _pnConfiguration = new PNConfiguration(new UserId("pub-c-9b0b9c7c-9c7a-4b7a-9e1a-2b9b8b0b0b0b"))
        {
            PublishKey = PublishKey,
            SubscribeKey = SubscribeKey,
            SecretKey = SecretKey,
            Secure = true
        };

        _pubnub = new Pubnub(_pnConfiguration);

        _pnSubscribeCallbackListener = new SubscribeCallbackListener();
        _pubnub.AddListener(_pnSubscribeCallbackListener);

        _pnSubscribeCallbackListener.onStatus += OnPnStatus;
        _pnSubscribeCallbackListener.onSignal += OnPnSignal;
        _pnSubscribeCallbackListener.onMessage += OnPnMessage;
        /*pnSubscribeCallbackListener.onMessage += OnPnMessage;
        pnSubscribeCallbackListener.onPresence += OnPnPresence;
        pnSubscribeCallbackListener.onFile += OnPnFile;
        pnSubscribeCallbackListener.onObject += OnPnObject;
        pnSubscribeCallbackListener.onMessageAction += OnPnMessageAction;*/
    }

    public void Subscribe(List<string> channels)
    {
        _pubnub.Subscribe<string>()
            .Channels(channels)
            .Execute();
    }

    public void Unsubscribe(List<string> channels)
    {
        _pubnub.Unsubscribe<string>()
            .Channels(channels)
            .Execute();
    }

    public void Publish(string channel, string message, PNCallback<PNPublishResult> callback = null)
    {
        if (callback == null)
            _pubnub.Publish()
                .Channel(channel)
                .Message(message)
                .ExecuteAsync();
        else
            _pubnub.Publish()
                .Channel(channel)
                .Message(message)
                .Execute(callback);
    }

    //send signal
    public void SendSignal(string channel, string message, PNCallback<PNPublishResult> callback)
    {
        _pubnub.Signal()
            .Channel(channel)
            .Message(message)
            .Execute(callback);
    }

    public void FetchHistory(string chatChannel, int i, PNCallback<PNFetchHistoryResult> callback)
    {
        _pubnub.FetchHistory().Channels(new List<string> { chatChannel }).MaximumPerChannel(i).Execute(callback);
    }

    private void OnPnStatus(Pubnub pubnub, PNStatus pnStatus)
    {
        Debug.Log($"[PUBNUB] OnPnStatus: {pnStatus.Category}");
    }

    private void OnPnSignal(Pubnub pubnub, PNSignalResult<object> signalResult)
    {
        if (signalResult.Publisher == _pnConfiguration.UserId) return;
        Debug.Log($"[PUBNUB] OnPnSignal: {signalResult.Message}");
        OnSignalCallback?.Invoke(signalResult);
    }

    public void AddReceivedSignal(Action<PNSignalResult<object>> callback)
    {
        OnSignalCallback += callback;
    }

    public void RemoveSignalCallback(Action<PNSignalResult<object>> callback)
    {
        OnSignalCallback -= callback;
    }

    private void OnPnMessage(Pubnub pubnub, PNMessageResult<object> messageResult)
    {
        Debug.Log($"[PUBNUB] OnPnMessage: {messageResult.Message}");
    }

    public void ClearUp()
    {
        Pubnub.CleanUp();
    }


    public void ClearAll()
    {
        _pubnub.UnsubscribeAll<string>();
        _pubnub.UnsubscribeAll<object>();
        _pnSubscribeCallbackListener.onStatus -= OnPnStatus;
        _pnSubscribeCallbackListener.onSignal -= OnPnSignal;
        _pnSubscribeCallbackListener.onMessage -= OnPnMessage;
        OnSignalCallback = null;
    }
}