using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class GameManager : Singleton<GameManager>
{
    public CharacterStats playerStats;

    private CinemachineVirtualCamera followCamera;
    List<IEndGameObserver> endGameObservers = new List<IEndGameObserver>();
    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }
    public void RigisterPlayer(CharacterStats player) 
    {
        playerStats = player ;
        followCamera = FindObjectOfType<CinemachineVirtualCamera>();
        if (followCamera != null)
        {
            //Debug.Log("wwwwwwwww");
            followCamera.Follow = playerStats.transform.GetChild(2);
            followCamera.LookAt= playerStats.transform.GetChild(2);
            //Debug.Log(playerStats.transform);
        }
    }

    public void AddObserver(IEndGameObserver observer)
    {
        endGameObservers.Add(observer);
    }
    public void RemoveObserver(IEndGameObserver observer)
    {
        endGameObservers.Remove(observer);
    }
    public void NotifyObserver()
    {
        foreach (var observer in endGameObservers)
        {
            observer.EndNotify();
        } 
    }
    public Transform GetEntrance()
    {
        foreach (var item in FindObjectsOfType<TransitionDestination>())
        {
            if (item.destinationTag == TransitionDestination.DestinationTag.ENTER)
            {
                return item.transform;
            }
           
        }
        return null;
    }
}
