using System;
using System.Collections.Generic;
using System.Linq;
using MasterServer.ClientSide;
using MasterServer.ServerSide.GameServer;
using MasterServer.ServerSide.MasterServer;

namespace MasterServer.ServerSide.Lobby;

public class LobbyManager
{
    private static LobbyManager _instance;
    private readonly List<Lobby> _lobbies;
    
    private LobbyManager()
    {
        _lobbies = new List<Lobby>();
    }
    
    public static LobbyManager GetInstance()
    {
        _instance ??= new LobbyManager();
        return _instance;
    }
    
    public Lobby GetOrCreateLobby()
    {
        foreach (var _lobby in _lobbies.Where(_lobby => _lobby.Available()))
            return _lobby;
        _lobbies.Add(new Lobby(2));
        GameServerSend.GameServerNotifyLobby(_lobbies[^1]);
        return _lobbies[^1];
    }

    private void OnLobbyFull(Lobby _lobby)
    {
        var _players = _lobby.GetPlayers();
        foreach (var _player in _players)
            ClientSend.JoinToLobby(_player, _lobby.GetId());
        _lobbies.Remove(_lobby);
    }

    public Lobby GetLobbyById(string _lobbyId)
    {
        return _lobbies.FirstOrDefault(_lobby => _lobby.GetId() == _lobbyId);
    }
    
    public class Lobby
    {
        private readonly string _lobbyId;
        private readonly List<int> _players;
        public readonly int Capacity;

        public Lobby(int _maxPlayers)
        {
            _players = new List<int>();
            _lobbyId = Guid.NewGuid().ToString();
            Capacity = _maxPlayers;
        }

        public bool Available()
        {
            return Capacity > _players.Count;
        }
        
        public void AddPlayer(int _playerId)
        {
            _players.Add(_playerId);
            if (Capacity == _players.Count)
                _instance.OnLobbyFull(this);
        }

        public string GetId()
        {
            return _lobbyId;
        }
        
        public List<int> GetPlayers()
        {
            return _players;
        }
    }
}