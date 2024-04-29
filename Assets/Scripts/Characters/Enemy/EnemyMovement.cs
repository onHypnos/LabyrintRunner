using Extensions.Reactive;
using Services;
using Tools.Extensions;
using UnityEngine;

namespace Characters.Enemy
{
    public class EnemyMovement : BaseMonoBehaviour
    {
        public struct Ctx
        {
            public IReadOnlyReactiveEvent<Vector3> MoveTo;
            public UpdateRunner UpdateRunner;
            public float BaseEnemySpeed;
            public ReactiveEvent OnReachWall;
        }

        private Ctx _ctx;

        public void SetCtx(Ctx ctx)
        {
            _ctx = ctx;
            _ctx.MoveTo.SubscribeWithSkip(MoveTo);
        }
        
        private void MoveTo(Vector3 direction)
        {
            transform.rotation = Quaternion.LookRotation(direction);
            transform.position += direction * Time.deltaTime * _ctx.UpdateRunner.GetGroupTimeScale * _ctx.BaseEnemySpeed;
        }
    }
}