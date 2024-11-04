using TraceKnife.Core.Abstractions;
using TraceKnife.Core.Models;

namespace TraceKnife.Core.DataPipelines
{
    public class DataPipelineContext : IDataPipelineContext
    {
        public string ProcessingTable { get; set; }
        public int PreferredParallelism { get; set; }
        public IDbObjectsOptions DbObjectsOptions { get; set; }

        public string MetadataTable => ProcessingTable + DbObjectsOptions.TableMetadataPostfix;
        public string GroupedReportTable => ProcessingTable + DbObjectsOptions.TableGroupedPostfix;
        public string ErrorsReportTable => ProcessingTable + DbObjectsOptions.TableErrorsPostfix;
        public string DeadlocksReportTable => ProcessingTable + DbObjectsOptions.TableDeadlockPostfix;

        public TraceMetadata TraceMetadata { get; set; }
    }
}
