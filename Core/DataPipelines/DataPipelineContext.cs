using TraceKnife.Core.Abstractions;

namespace TraceKnife.Core.DataPipelines
{
    public class DataPipelineContext : IDataPipelineContext
    {
        public string ProcessingTable { get; set; }
        public int PreferredParallelism { get; set; }
    }
}
