using System.Data.SqlClient;
using System.Threading.Tasks;

namespace TraceKnife.Common
{
    public interface ISqlConnectionFactory
    {
        Task<SqlConnection> CreateAsync();
        SqlConnection Create();
    }
}
