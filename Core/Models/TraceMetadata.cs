namespace TraceKnife.Core.Models
{
    public class TraceMetadata
    {
        public int CpuSum { get; set; }
        public long DurationSumUs { get; set; }
        public long WritesSum { get; set; }
        public long ReadsSum { get; set; }
        public int RowsCount { get; set; }
        public int QueriesCount { get; set; }
    }
}
