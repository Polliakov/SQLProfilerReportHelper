namespace TraceKnife.Core.Abstractions
{
    public interface IDataPipelineContext
    {
        string ProcessingTable { get; }
        int PreferredParallelism { get;  }
    }
}
