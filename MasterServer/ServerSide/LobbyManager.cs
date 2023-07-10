using System;
using System.Collections.Generic;
using System.Linq;

namespace MasterServer.ServerSide;

public class LobbyManager
{
    private static LobbyManager _instance;
    private readonly List<Lobby> _lobbies;
    public static event EventHandler<Lobby> GameServerNotifyLobby;
    public static event EventHandler<Lobby> LobbyIsFulled;

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
        OnGameServerNotifyLobby(_lobbies[^1]);
        return _lobbies[^1];
    }
    
    protected virtual void OnGameServerNotifyLobby(Lobby _e)
    {
        GameServerNotifyLobby?.Invoke(this, _e);
    }

    private static void OnLobbyIsFulled(Lobby _e)
    {
        LobbyIsFulled?.Invoke(null, _e);
    }
    
    public class Lobby
    {
        private readonly string _lobbyId;
        private readonly List<int> _players;
        private readonly int _capacity;

        public Lobby(int _maxPlayers)
        {
            _players = new List<int>();
            _lobbyId = Guid.NewGuid().ToString();
            _capacity = _maxPlayers;
        }

        public bool Available()
        {
            return _capacity > _players.Count;
        }
        
        public void AddPlayer(int _playerId)
        {
            _players.Add(_playerId);
            if (_capacity == _players.Count)
                OnLobbyIsFulled(this);
        }

        public void RemovePlayer(int _playerId)
        {
            _players.Remove(_playerId);
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