using Characters.Player;
using Extensions.Reactive;
using UniRx;
using UnityEngine;
using UnityEngine.Serialization;

namespace Characters.Enemy
{
    public class ChaseState : EnemyStateBase
    {
        public struct Ctx
        {
            public Transform Pivot;
            public IReadOnlyReactiveProperty<PlayerCharacter> CurrentPlayerCharacter;
            public ReactiveEvent<Vector3> MoveTo;
            public float ChaseDistance;
            public float ReachDistance;
            public ReactiveEvent OnLoseTarget;
            public ReactiveEvent OnReachTarget;
        }

        private Ctx _ctx;

        public ChaseState(Ctx ctx)
        {
            _ctx = ctx;
        }

        public override void Enter()
        {
            
        }

        public override void Execute()
        {
            var targetDistance = Vector3.Distance(_ctx.Pivot.position,_ctx.CurrentPlayerCharacter.Value.transform.position);
            if (targetDistance < _ctx.ReachDistance)
            {
                _ctx.OnReachTarget.Notify();
            }
            else if (targetDistance < _ctx.ChaseDistance)
            {
                _ctx.MoveTo.Notify((_ctx.CurrentPlayerCharacter.Value.transform.position - _ctx.Pivot.position).normalized);
            }
            else
            {
                _ctx.OnLoseTarget.Notify();
            }
        }

        public override void Exit()
        {
            
        }
    }
}