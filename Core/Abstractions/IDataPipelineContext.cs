using TraceKnife.Core.Models;

namespace TraceKnife.Core.Abstractions
{
    public interface IDataPipelineContext
    {
        string ProcessingTable { get; }
        int PreferredParallelism { get; }

        string MetadataTable { get; }
        string GroupedReportTable { get; }
        string ErrorsReportTable { get; }
        string DeadlocksReportTable { get; }

        TraceMetadata TraceMetadata { get; set; }

        IDbObjectsOptions DbObjectsOptions { get; }
    }
}
