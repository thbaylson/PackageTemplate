namespace BaylsonGames.Analytics
{
    public interface IAnalyticsEventSink
    {
        void Record(AnalyticsEventPayload payload);

        void Flush();
    }
}
