using Characters.Player;
using Extensions.Reactive;
using UniRx;
using UnityEngine;

namespace Characters.Enemy
{
    public class PatrolState : EnemyStateBase
    {
        public struct Ctx
        {
            public Transform Pivot;
            public ReactiveEvent<Vector3> MoveTo;
            public float ChaseRange;
            public IReadOnlyReactiveProperty<PlayerCharacter> CurrentPlayerCharacter;
            public ReactiveEvent TargetInRange;
            public ReactiveEvent ToIdleState;
        }
        
        private Ctx _ctx;
        private float _stateTimer = 0f;
        private float _minStateTime = 1f; //todo refactor move to config
        private float _maximumStateTime = 2f; //todo refactor move to config
        private float _startPointPatrolMaximumDelta = 3f; //todo refactor move to config
        private Vector3 _startPosition;
        private Vector3 _currentPatrolDirection;
        public PatrolState(Ctx ctx)
        {
            _ctx = ctx;
            _startPosition = _ctx.Pivot.position;
            
        }

        public override void Enter()
        {
            _stateTimer = Random.Range(_minStateTime,_maximumStateTime);
            _currentPatrolDirection =(new Vector3(
                _startPosition.x + Random.Range(-1, 1) * _startPointPatrolMaximumDelta,
                0,
                _startPosition.z + Random.Range(-1, 1) * _startPointPatrolMaximumDelta) - _ctx.Pivot.transform.position).normalized;
        }

        public override void Execute()
        {
            var targetDistance = Vector3.Distance(_ctx.Pivot.position,_ctx.CurrentPlayerCharacter.Value.transform.position);
            if (targetDistance <= _ctx.ChaseRange)
            {
                _ctx.TargetInRange.Notify();
            }
            else
            {
                _ctx.MoveTo.Notify(_currentPatrolDirection);
            }

            _stateTimer -= Time.deltaTime;
            if (_stateTimer < 0)
            {
                _ctx.ToIdleState.Notify();
            }
        }
    }
}