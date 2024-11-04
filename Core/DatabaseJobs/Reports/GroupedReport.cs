using System.Threading.Tasks;
using TraceKnife.Common;
using TraceKnife.Core.Abstractions;

namespace TraceKnife.Core.DatabaseJobs.Reports
{
    public class GroupedReport : IDataPipelineJob
    {
        private readonly Sql _sql;

        public GroupedReport(Sql sql)
        {
            _sql = sql;
        }

        public async Task RunAsync(IDataPipelineContext context)
        {
            var scale = 4;
            await _sql.ExecuteNonQueryAsync(120 * 60,
$@"
select
	  [DatabaseName]
	, [TextKey]
    , [ObjectName]

	, [% CPU]
	, [sum(CPU)]
	, [min(CPU)]
	, [avg(CPU)]
	, [max(CPU)]

	, [% Duration]
	, [sum(Duration)]
	, [min(Duration)]
	, [avg(Duration)]
	, [max(Duration)]

	, [% Reads]
	, [sum(Reads)]
	, [min(Reads)]
	, [avg(Reads)]
	, [max(Reads)]

	, [% Writes]
	, [sum(Writes)]
	, [min(Writes)]
	, [avg(Writes)]
	, [max(Writes)]

	, [% Count]
	, [Count]

	, [min(Duration)us]
    , [avg(Duration)us]
	, [max(Duration)us]

into [dbo].[{context.GroupedReportTable}]" +
$@"
from
(
	select 
		  *
		, round(cast([sum(CPU)] as float) / {context.TraceMetadata.CpuSum} * 100, {scale}) as [% CPU]

		, [min(Duration)us]/1000 as [min(Duration)]
		, [avg(Duration)us]/1000 as [avg(Duration)]
		, [max(Duration)us]/1000 as [max(Duration)] 
		, [sum(Duration)us]/1000 as [sum(Duration)]

		, case when {context.TraceMetadata.DurationSumUs} > 0 
			then round(cast([sum(Duration)us] as float) / {context.TraceMetadata.DurationSumUs} * 100, {scale})
			else 0
		  end as [% Duration]

		, case when {context.TraceMetadata.ReadsSum} > 0
			then round(cast([sum(Reads)] as float) / {context.TraceMetadata.ReadsSum} * 100, {scale})
			else 0
		  end as [% Reads]

		, case when {context.TraceMetadata.WritesSum} > 0
			then round(cast([sum(Writes)] as float) / {context.TraceMetadata.WritesSum} * 100, {scale})
			else 0
		  end as [% Writes]

		, case when {context.TraceMetadata.QueriesCount} > 0
			then round(cast([Count] as float) / {context.TraceMetadata.QueriesCount} * 100, {scale})
			else 0
		  end as [% Count]
	from
	(
		select
			[DatabaseName],
			[TextKey],
			[ObjectName],
  
			avg(CPU) as [avg(CPU)], 
			min(CPU) as [min(CPU)], 
			max(CPU) as [max(CPU)], 
			sum(CPU) as [sum(CPU)], 

			avg(Duration) as [avg(Duration)us], 
			min(Duration) as [min(Duration)us], 
			max(Duration) as [max(Duration)us], 
			sum(Duration) as [sum(Duration)us],

			avg(Reads) as [avg(Reads)],
			min(Reads) as [min(Reads)], 
			max(Reads) as [max(Reads)], 
			sum(Reads) as [sum(Reads)], 

			avg(Writes) as [avg(Writes)],
			min(Writes) as [min(Writes)], 
			max(Writes) as [max(Writes)], 
			sum(Writes) as [sum(Writes)], 

			count(*) as [Count]
		from
			[dbo].[{context.ProcessingTable}] as [RawTable]
		where
			EventClass in (10, 12)
		group by
			[DatabaseName], [TextKey], [ObjectName]
	) as [Groups]
) as [FieldSelector]");
        }
    }
}
