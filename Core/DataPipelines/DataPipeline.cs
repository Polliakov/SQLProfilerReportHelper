using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TraceKnife.Core.Abstractions;

namespace TraceKnife.Core.DataPipelines
{
    public class DataPipeline
    {
        private readonly List<IDataPipelineJob> _jobs = new List<IDataPipelineJob>();
        private readonly IDataPipelineContext _context;

        public DataPipeline(IDataPipelineContext context)
        {
            _context = context;
        }

        public DataPipeline AddJob(IDataPipelineJob job)
        {
            _jobs.Add(job);
            return this;
        }

        public async Task Execute()
        {
            foreach (var job in _jobs)
            {
                await job.RunAsync(_context);
            }
        }
    }
}
