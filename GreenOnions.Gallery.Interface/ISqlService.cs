using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace GreenOnions.Gallery.Interface
{
    public interface ISqlService
    {
        public Task<DataSet> Select(string sql, params SqlParameter[] parameters);

        public Task<DataSet> SelectMultiTable(string sql, IDictionary<string, string> tableMappings, params SqlParameter[] parameters);

        public Task<int> InsertUpdateDelete(string sql, params SqlParameter[] parameters);

        public Task<long> InsertUpdateDeleteOutputIdentity(string sql, params SqlParameter[] parameters);
    }
}
