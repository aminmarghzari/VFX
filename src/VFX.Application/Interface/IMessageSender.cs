namespace VFX.Application.Interface;

public interface IMessageSender
{
    Task SendMessageAsync<T>(string topic, T message);
}
