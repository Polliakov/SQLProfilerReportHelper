using System;
using System.Threading.Tasks;

namespace TraceKnife.Core.Abstractions
{
    public interface IDataPipelineJob<TReturn, TProgress> : IDataPipelineJob<TProgress>
    {
        Task<T> RunAsync<T>(IDataPipelineContext context)
            where T : TReturn;
    }

    public interface IDataPipelineJob<TProgress> : IDataPipelineJob
    {
        event Action<TProgress> Progress;
    }

    public interface IDataPipelineJob
    {
        Task RunAsync(IDataPipelineContext context);
    }
}
