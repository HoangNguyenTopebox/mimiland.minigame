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

    public event Action<PNSignalResult<object>> OnSignalCallback;
    public event Action<PNStatusCategory> OnStatusCallback;

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

        _pnConfiguration = new PNConfiguration(new UserId(GetUuid()))
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
    }

    private string GetUuid()
    {
        return Guid.NewGuid().ToString();
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
        if (pnStatus.Error)
            Debug.LogError($"[PUBNUB] OnPnStatus: {pnStatus.ErrorData.Information}");
        else
        {
            if (pnStatus.Category == PNStatusCategory.PNConnectedCategory)
            {
                Debug.Log($"[PUBNUB] OnPnStatus: {pnStatus.Uuid} connected to {string.Join("|", pnStatus.AffectedChannels)} channel(s)");
            }
            OnStatusCallback?.Invoke(pnStatus.Category);
        }
    }

    private void OnPnSignal(Pubnub pubnub, PNSignalResult<object> signalResult)
    {
        if (signalResult.Publisher == _pnConfiguration.UserId) return;
        OnSignalCallback?.Invoke(signalResult);
    }

    private void OnPnMessage(Pubnub pubnub, PNMessageResult<object> messageResult)
    {
        Debug.Log($"[PUBNUB] OnPnMessage: {messageResult.Message}");
    }

    public void ClearUp()
    {
        Pubnub.CleanUp();
    }

    public List<string> GetChannels()
    {
        return _pubnub.GetSubscribedChannels();
    }

    public void ClearAll()
    {
        _pubnub.Disconnect<string>();
        _pubnub.UnsubscribeAll<string>();
        _pubnub.UnsubscribeAll<object>();
        _pnSubscribeCallbackListener.onStatus -= OnPnStatus;
        _pnSubscribeCallbackListener.onSignal -= OnPnSignal;
        _pnSubscribeCallbackListener.onMessage -= OnPnMessage;
        OnSignalCallback = null;
    }
}