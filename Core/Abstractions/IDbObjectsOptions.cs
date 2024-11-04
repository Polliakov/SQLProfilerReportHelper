namespace TraceKnife.Core.Abstractions
{
    public interface IDbObjectsOptions
    {
        string NormalizationFunctionName { get; }
        string NormalizedTextDataColumn { get; }
        string TableMetadataPostfix { get; }
        string TableDraftPostfix { get; }
        string TableGroupedPostfix { get; }
        string TableDeadlockPostfix { get; }
        string TableErrorsPostfix { get; }
    }
}
