namespace Producer.Interfaces;
public interface IBaseProducer
{
    void PublishMessage(string routingKey, object message);
}
