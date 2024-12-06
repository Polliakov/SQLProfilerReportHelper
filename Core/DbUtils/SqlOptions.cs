using System;

namespace TraceKnife.Common
{
    public class SqlOptions
    {
        private int _timeout = -1;

        public int Timeout
        {
            get => _timeout;
            set
            {
                if (value < 1)
                    throw new ArgumentOutOfRangeException(nameof(Timeout), "Must be greater than 0.");
                _timeout = value;
            }
        }

        public void SetDefaultTimeout() => _timeout = -1;
    }
}
