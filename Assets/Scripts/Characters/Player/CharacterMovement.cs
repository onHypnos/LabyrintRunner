using Services;
using Tools.Extensions;
using UniRx;
using UnityEngine;

namespace Characters.Player
{
    public class CharacterMovement : BaseMonoBehaviour
    {
        public struct Ctx
        {
            public CompositeDisposable Disposable;
            public IReadOnlyReactiveProperty<Vector3> UiTouchInput;
            public UpdateRunner UpdateRunner;
            public float CharacterSpeed;
        }

        private Ctx _ctx;
        
        public void SetCtx(Ctx ctx)
        {
            _ctx = ctx;
            _ctx.UiTouchInput.Subscribe(Move).AddTo(_ctx.Disposable);
        }

        private void Move(Vector3 direction)
        {
            if (direction != Vector3.zero)
            {
                var movementDirection = new Vector3(direction.x, 0, direction.y);
                transform.rotation = Quaternion.LookRotation(movementDirection);
                transform.position += movementDirection * Time.deltaTime * _ctx.UpdateRunner.GetGroupTimeScale * _ctx.CharacterSpeed;
            }
        }
    }
}