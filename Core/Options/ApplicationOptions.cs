using TraceKnife.Core.Abstractions;

namespace TraceKnife.Core.Configuration
{
    public class ApplicationOptions : IApplicationOptions
    {
        public string NormalizationFunctionName { get; set; }
        public string NormalizedTextDataColumn { get; set; }
        public string TableDraftPostfix { get; set; }
        public string TableDetailPostfix { get; set; }
        public string TableDeadlockPostfix { get; set; }
        public string TableErrorPostfix { get; set; }
    }
}
