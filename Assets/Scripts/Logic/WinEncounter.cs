using System;
using Extensions.Reactive;
using Tools.Extensions;
using UnityEngine;

namespace Logic
{
    public class WinEncounter : BaseMonoBehaviour
    {
        public struct Ctx
        {
            public ReactiveEvent PlayerReachWinEncounter;
        }

        private Ctx _ctx;

        public void SetCtx(Ctx ctx)
        {
            _ctx = ctx;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                _ctx.PlayerReachWinEncounter.Notify();
            }
        }
    }
}