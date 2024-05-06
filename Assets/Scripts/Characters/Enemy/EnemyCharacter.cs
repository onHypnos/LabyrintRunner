using System;
using System.Collections.Generic;
using Characters.Player;
using Extensions.Reactive;
using Services;
using Services.SaveLoad;
using Tools.Extensions;
using UniRx;
using UnityEngine;

namespace Characters.Enemy
{
    public class EnemyCharacter : BaseMonoBehaviour, IExecutablePerFrame
    {
        public struct Ctx
        {
            public ReactiveEvent OnEnemyReachTarget;
            public float ChaseRange;
            public int ID;
            public ReactiveProperty<PlayerCharacter> CurrentPlayerCharacter;
            public UpdateRunner UpdateRunner;
            public float BaseEnemySpeed;
        }

        private Ctx _ctx;

        private Dictionary<Type, EnemyStateBase> _states = new();
        private EnemyStateBase _currentState = null;
        private CompositeDisposable _disposable = new();


        [SerializeField] private EnemyMovement _enemyMovement;

        public void SetCtx(Ctx ctx)
        {
            _ctx = ctx;
            var moveTo = new ReactiveEvent<Vector3>();
            var loseChasedTarget = new ReactiveEvent();
            var targetInRange = new ReactiveEvent();
            var toIdleState = new ReactiveEvent();
            var toPatrolState = new ReactiveEvent();
            _enemyMovement.SetCtx(new EnemyMovement.Ctx
            {
                MoveTo = moveTo,
                UpdateRunner = _ctx.UpdateRunner,
                BaseEnemySpeed = _ctx.BaseEnemySpeed,
                OnReachWall = toIdleState
            });
            
            _states.Add(typeof(IdleState), new IdleState(new IdleState.Ctx
            {
                Pivot = transform,
                ChaseRange = _ctx.ChaseRange,
                CurrentPlayerCharacter = _ctx.CurrentPlayerCharacter,
                TargetInRange = targetInRange,
                ToPatrolState = toPatrolState
            }));
            _states.Add(typeof(PatrolState), new PatrolState(new PatrolState.Ctx
            {
                Pivot = transform,
                MoveTo = moveTo,
                ChaseRange = _ctx.ChaseRange,
                CurrentPlayerCharacter = _ctx.CurrentPlayerCharacter,
                TargetInRange = targetInRange,
                ToIdleState = toIdleState,

            }));
            _states.Add(typeof(ChaseState), new ChaseState(new ChaseState.Ctx
            {
                Pivot = transform,
                CurrentPlayerCharacter = _ctx.CurrentPlayerCharacter,
                MoveTo = moveTo,
                ChaseDistance = _ctx.ChaseRange,
                ReachDistance = 1f,
                OnLoseTarget = loseChasedTarget,
                OnReachTarget = _ctx.OnEnemyReachTarget,

            }));
            
            toPatrolState.SubscribeWithSkip(EnterState<PatrolState>);
            toIdleState.SubscribeWithSkip(EnterState<IdleState>);
            targetInRange.SubscribeWithSkip(EnterState<ChaseState>);
            _ctx.OnEnemyReachTarget.SubscribeWithSkip(EnterState<IdleState>).AddTo(_disposable);
            loseChasedTarget.SubscribeWithSkip(EnterState<IdleState>);
            _ctx.UpdateRunner.Subscribe(this);
        }

        public void EnterState<T>() where T : EnemyStateBase
        {
            if (_currentState != null)
            {
                if(typeof(T) == _currentState.GetType())
                    return;
                _currentState.Exit();
            }
            _currentState = _states[typeof(T)];
            _currentState.Enter();
        }

        public void Execute()
        {
            _currentState.Execute();
        }

        public SaveData.EnemyData SaveData()
        {
            return new SaveData.EnemyData
            {
                ID = _ctx.ID,
                Position = transform.position,
                Rotation = transform.rotation
            };
        }

        public void LoadData(Vector3 position, Quaternion rotation)
        {
            transform.position = position;
            transform.rotation = rotation;
            EnterState<IdleState>();
        }
        
        protected override void OnDestroy()
        {
            _disposable.Dispose();
            _ctx.UpdateRunner.Unsubscribe(this);
            base.OnDestroy();
        }
    }
}
