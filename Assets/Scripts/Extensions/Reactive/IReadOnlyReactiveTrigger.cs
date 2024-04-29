using System;

namespace Extensions.Reactive
{
  public interface IReadOnlyReactiveTrigger
  {
    IDisposable Subscribe(Action action);
    IDisposable SubscribeOnce(Action action);
  }
}