namespace TraceKnife.ViewKit.Calculations
{
    public static class ProgressFunctions
    {
        public record Progress(
            double Percent,
            DateTime? ExpectedTime
        );

        public static Func<int, Progress> GetPercentByCountCalc(
            DateTime startTime,
            int totalCount)
        {
            var done = 0.0;
            Progress handler(int processed)
            {
                done += processed;
                if (done <= 0.0)
                    return new Progress(0.0, null);

                var timePass = DateTime.Now.Subtract(startTime).TotalSeconds;
                var expectedTime = startTime.AddSeconds(timePass / done * totalCount);

                return new Progress(totalCount / done, expectedTime);
            }
            return handler;
        }
    }
}
