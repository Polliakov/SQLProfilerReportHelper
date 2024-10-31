namespace TraceKnife.Core.Abstractions
{
    public interface IApplicationOptions
    {
        string NormalizationFunctionName { get; }
        string NormalizedTextDataColumn { get; }
        string TableDraftPostfix { get; }
        string TableDetailPostfix { get; }
        string TableDeadlockPostfix { get; }
        string TableErrorPostfix { get; set; }
    }
}
