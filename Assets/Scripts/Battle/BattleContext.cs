using System.Collections.Generic;

namespace Battle
{
    public class BattleContext
    {
        private IBattleState _currentState;
        private IBattleState _previousState;

        private Dictionary<BattleList, IBattleState> _stateTable;

        public void Init(BattleStateController battle, BattleList initState)
        {
            if (_stateTable is not null) return;

            Dictionary<BattleList, IBattleState> table = new()
            {
                { BattleList.Start, new BattleStart(battle) },
                { BattleList.PlayerCommand, new BattlePlayerCommand(battle) },
                { BattleList.Battle, new BttleStep(battle) },
                { BattleList.End, new BattleEnd(battle) },
            };
            _stateTable = table;
            _currentState = _stateTable[initState];
            BattleManager.Instance.State = initState;
            _currentState.Entry();
            ChangeState(initState);
        }

        public void ChangeState(BattleList next)
        {
            if (_stateTable is null) return;
            if (_currentState is null || _currentState.State == next)
            {
                return;
            }
            BattleManager.Instance.State = next;
            var nextState = _stateTable[next];
            _previousState = _currentState;
            _previousState?.Exit();
            _currentState = nextState;
            _currentState.Entry();
            
        }
        public void Update() => _currentState?.Update();
    }
}
