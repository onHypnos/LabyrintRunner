using System;
using Extensions.Reactive;
using UniRx;
using UnityEngine;

namespace Services
{
    public class UpdateRunner : MonoBehaviour
    {
        private event Action _fixedUpdateEvent;
        private event Action _lateUpdateEvent;
        private event Action _updateEvent;
        
        private float _characterGroupTimeScale = 1f;
        private bool _gamePause = false;
        public float GetGroupTimeScale => !_gamePause?_characterGroupTimeScale:0;
        
        public void Subscribe<T>(T instance) where T : IExecutable
        {
            if (instance is IFixedExecutable fixedUpdatable) _fixedUpdateEvent += fixedUpdatable.FixedExecute;
            if (instance is ILateExecutable lateUpdatable) _lateUpdateEvent += lateUpdatable.LateExecute;//.AddTo(disposable);
            if (instance is IExecutablePerFrame updatable) _updateEvent += updatable.Execute;//.AddTo(disposable);
        }

        public void Unsubscribe<T>(T instance) where T : IExecutable
        {
            if (instance is IFixedExecutable fixedUpdatable) _fixedUpdateEvent -= fixedUpdatable.FixedExecute;
            if (instance is ILateExecutable lateUpdatable) _lateUpdateEvent -= lateUpdatable.LateExecute;//.AddTo(disposable);
            if (instance is IExecutablePerFrame updatable) _updateEvent -= updatable.Execute;
        }
        
        private void FixedUpdate() => _fixedUpdateEvent?.Invoke();
        private void LateUpdate() => _lateUpdateEvent?.Invoke();
        private void Update() => _updateEvent?.Invoke();

        public void Pause()
        {
            _gamePause = true;
        }

        public void Unpause()
        {
            _gamePause = false;
        }
    }

    public interface IExecutablePerFrame : IExecutable
    {
        public void Execute();
    }

    public interface ILateExecutable : IExecutable
    {
        public void LateExecute();
    }

    public interface IFixedExecutable : IExecutable
    {
        public void FixedExecute();
    }

    public interface IExecutable
    {
    }
}