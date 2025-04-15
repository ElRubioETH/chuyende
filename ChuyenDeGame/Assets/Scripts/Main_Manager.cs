using System;
using System.Collections.Generic;
using Fusion;
using Fusion.Sockets;
using UnityEngine;

public class Main_Manager : NetworkBehaviour, INetworkRunnerCallbacks
{
    public NetworkPrefabRef  _malePlyerPrefab;
    public NetworkPrefabRef _femalePlayerPrefab;

    public NetworkRunner _runner;
    public NetworkSceneManagerDefault _sceneManager;
    
    // khoi tao cac gia tri
    void Awake()
    {
      if (_runner == null)
        {
            GameObject runnerObj = new GameObject("NetworkRunner");
            _runner = runnerObj.AddComponent<NetworkRunner>();
            _runner.AddCallbacks (this);
            _sceneManager = runnerObj.AddComponent<NetworkSceneManagerDefault>();


        }


        ConnectToFusion();

    }
    async void ConnectToFusion()
    {
        Debug.Log("Connecting to fusion Network...");
        _runner.ProvideInput = true;// cho phep ng chs vao
        string sessionName = ("MyGameSession");// tao ma phong


        var startGameArgs = new StartGameArgs()
        {
            GameMode = GameMode.Shared,   // Chế độ chơi shared
            SceneManager = _sceneManager,
            SessionName = sessionName,
            PlayerCount = 5,              // Số lượng người chơi
            IsVisible = true,             // Cho phép hiển thị số phòng
            IsOpen = true                 // Cho phép người chơi khác tham gia
        };

        // ket noi mang vao fusion
        var result = await _runner.StartGame(startGameArgs);
        if (result.Ok)
        {
            Debug.Log("Connectted to Fusion Network successfully");

        }
        else
        {
            Debug.LogError($"Failed to connect; {result.ShutdownReason}");

        }
    }
    

    

    public void OnConnectedToServer(NetworkRunner runner)
    { 
       
    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
        
    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {
        
    }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {
        
    }

    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
    {
        
    }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {
        
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        
    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {
        
    }

    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
        
    }

    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
        
    }
    // ham nay se dc goi sau khi ket noi thanh cong
    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        Debug.Log("....Player joiend: " + player);
        if (_runner.LocalPlayer != player) return; 
        //thuc hien swpan nhan vat
        var playerClass = PlayerPrefs.GetString("PlayerClass");
        var playerName = PlayerPrefs.GetString("PlayerName");

        var prefab = playerClass.Equals("Male") ? _malePlyerPrefab : _femalePlayerPrefab;

        var position = new Vector3(0,1,0);
        _runner.Spawn
            (
                prefab,
                position,
                Quaternion.identity,
                player,
                (r,o) =>
                {
                    Debug.Log("player spawned: " + 0);

                }
            );

    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        
    }

    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)
    {
        
    }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data)
    {
        
    }

    public void OnSceneLoadDone(NetworkRunner runner)
    {
        
    }

    public void OnSceneLoadStart(NetworkRunner runner)
    {
        
    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
        
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
        
    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {
        
    }
}
