using System;
using SimpleInputNamespace;
using Tools.Extensions;
using UniRx;
using UnityEngine;

namespace UI
{
    public class SimpleInputJoystickWrapper : BaseMonoBehaviour
    {
        [SerializeField] private Joystick _joystick;
        
        public struct Ctx
        {
            public ReactiveProperty<Vector3> UiTouchInput;
        }

        private Ctx _ctx;        
        public void SetCtx(Ctx ctx)
        {
            _ctx = ctx;
            SimpleInput.OnUpdate += Execute;
        }

        /// <summary>
        /// Говнокод
        /// </summary>
        public void Execute()
        {
            _ctx.UiTouchInput?.SetValueAndForceNotify(_joystick.Value);
        }
    }
}