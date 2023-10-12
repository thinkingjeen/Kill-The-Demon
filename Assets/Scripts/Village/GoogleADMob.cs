using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;
using System;

public class GoogleADMob : MonoBehaviour
{
    private RewardedAd rewardedAd;

    public static GoogleADMob instance;

    public System.Action<Reward> onHandleUserEarnedReward;
    public System.Action<AdFailedToLoadEventArgs> onHandleRewardedAdFailedToLoad;

    public System.Action onHandleRewardedAdFailedToShow;
    public System.Action onHandleRewardedAdClosed;

    private Coroutine loadingRoutine;

    private void Awake()
    {
        instance = this;
    }

    public void Init()
    {//string adUnitId = "ca-app-pub-1759079710954365/5459573256";
        //adUnitId 설정
#if UNITY_EDITOR
        string adUnitId = "unused";
#elif UNITY_ANDROID
        string adUnitId = "ca-app-pub-1759079710954365/5962970280";
        
#elif UNITY_IPHONE
        string adUnitId = "ca-app-pub-3940256099942544/1712485313";
#else
        string adUnitId = "unexpected_platform";
#endif

        // 모바일 광고 SDK를 초기화함.
        MobileAds.Initialize(initStatus => { });

        //광고 로드 : RewardedAd 객체의 loadAd메서드에 AdRequest 인스턴스를 넣음
        AdRequest request = new AdRequest.Builder().Build();
        this.rewardedAd = new RewardedAd(adUnitId);
        this.rewardedAd.LoadAd(request);

        this.rewardedAd.OnAdLoaded += HandleRewardedAdLoaded; // 광고 로드가 완료되면 호출
        this.rewardedAd.OnAdFailedToLoad += HandleRewardedAdFailedToLoad; // 광고 로드가 실패했을 때 호출
        this.rewardedAd.OnAdOpening += HandleRewardedAdOpening; // 광고가 표시될 때 호출(기기 화면을 덮음)
        this.rewardedAd.OnAdFailedToShow += HandleRewardedAdFailedToShow; // 광고 표시가 실패했을 때 호출
        this.rewardedAd.OnUserEarnedReward += HandleUserEarnedReward;// 광고를 시청한 후 보상을 받아야할 때 호출
        this.rewardedAd.OnAdClosed += HandleRewardedAdClosed; // 닫기 버튼을 누르거나 뒤로가기 버튼을 눌러 동영상 광고를 닫을 때 호출
    }
    public void HandleRewardedAdLoaded(object sender, EventArgs args)
    {
        Debug.Log("HandleRewardedAdLoaded");
    }

    public void HandleRewardedAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
        Debug.Log("HandleRewardedAdFailedToLoad");
        this.onHandleRewardedAdFailedToLoad(args);

    }

    public void HandleRewardedAdOpening(object sender, EventArgs args)
    {
        Debug.Log("HandleRewardedAdOpening");
    }

    public void HandleRewardedAdFailedToShow(object sender, EventArgs args)
    {
        Debug.Log("HandleRewardedAdFailedToShow");
        this.onHandleRewardedAdFailedToShow();
    }

    public void HandleRewardedAdClosed(object sender, EventArgs args)
    {
        Debug.Log("HandleRewardedAdClosed");
        this.onHandleRewardedAdClosed();
    }

    public void HandleUserEarnedReward(object sender, Reward args)
    {
        Debug.Log("HandleUserEarnedReward");
        this.onHandleUserEarnedReward(args);
    }

    public void LoadAds()
    {
        if (loadingRoutine == null)
        {
            this.loadingRoutine = StartCoroutine(this.WaitRoutine());
        }
    }

    private IEnumerator WaitRoutine()
    {
        yield return new WaitUntil(() => this.rewardedAd.IsLoaded());
        this.rewardedAd.Show();
        this.loadingRoutine = null;
    }
}
