using System;
using UniRx;

namespace Extensions.Reactive
{
  public class ReactiveTrigger : IReadOnlyReactiveTrigger, IDisposable
  {
    private readonly Subject<bool> _subject;

    public ReactiveTrigger()
    {
      _subject = new Subject<bool>();
    }

    public void Dispose()
    {
      _subject.Dispose();
    }

    public IDisposable Subscribe(Action action)
    {
      return _subject.Subscribe(_ => action?.Invoke());
    }

    public IDisposable SubscribeOnce(Action action)
    {
      return _subject.Take(1).Subscribe(_ => action?.Invoke());
    }

    public void Notify()
    {
      _subject.OnNext(true);
    }
  }
}