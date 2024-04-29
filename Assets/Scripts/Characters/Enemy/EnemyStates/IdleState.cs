using Characters.Player;
using Extensions.Reactive;
using UniRx;
using UnityEngine;
using UnityEngine.Serialization;

namespace Characters.Enemy
{
    public class IdleState : EnemyStateBase
    {
        public struct Ctx
        {
            public Transform Pivot;
            public float ChaseRange;
            public IReadOnlyReactiveProperty<PlayerCharacter> CurrentPlayerCharacter;
            public ReactiveEvent TargetInRange;
            public ReactiveEvent ToPatrolState;
        }

        private Ctx _ctx;
        private float _stateTimer = 0f;
        private float _maximumStateTime = 2f;
        
        public IdleState(Ctx ctx)
        {
            _ctx = ctx;
        }
        
        public override void Enter()
        {
            _stateTimer = Random.Range(.1f,_maximumStateTime);
        }

        public override void Execute()
        {
            var targetDistance = Vector3.Distance(_ctx.Pivot.position,_ctx.CurrentPlayerCharacter.Value.transform.position);
            if (targetDistance <= _ctx.ChaseRange)
            {
                _ctx.TargetInRange.Notify();
            }
            _stateTimer -= Time.deltaTime;
            if (_stateTimer < 0)
            {
                _ctx.ToPatrolState.Notify();
            }
        }
    }
}