using TraceKnife.Core.Abstractions;

namespace TraceKnife.Core.Configuration
{
    public class DbObjectsOptions : IDbObjectsOptions
    {
        public string NormalizationFunctionName { get; set; }
        public string NormalizedTextDataColumn { get; set; }
        public string TableMetadataPostfix { get; set; }
        public string TableDraftPostfix { get; set; }
        public string TableGroupedPostfix { get; set; }
        public string TableDeadlockPostfix { get; set; }
        public string TableErrorsPostfix { get; set; }
    }
}
