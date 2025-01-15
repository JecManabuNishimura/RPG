using System.Collections.Generic;

public class PlayerContext 
{
    private IPlayerState _currentState; // 現在の状態
    private IPlayerState _previousState; // 直前の状態

    private Dictionary<PlayerStatus, IPlayerState> _stateTable;

    public void Init(Player player, PlayerStatus initState)
    {
        if (_stateTable is not null) return;

        Dictionary<PlayerStatus, IPlayerState> table = new()
        {
            { PlayerStatus.Field, new PlayerStateField(player) },
        };
        _stateTable = table;
        
        ChangeState(initState);
        
    }

    public void ChangeState(PlayerStatus next)
    {
        if (_stateTable is null) return;
        if (_currentState is null || _currentState.State == next)
        {
            var nextState = _stateTable[next];
            _previousState = _currentState;
            _previousState?.Exit();
            _currentState = nextState;
            _currentState.Entry();
        }
    }

    public void Update() => _currentState?.Update();
}
