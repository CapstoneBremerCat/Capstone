public interface IMediator
{
    void Notify(object sender, GameEvent eventName, object eventData);
}
