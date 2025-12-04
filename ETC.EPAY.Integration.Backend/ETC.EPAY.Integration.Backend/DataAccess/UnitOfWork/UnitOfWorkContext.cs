using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETC.EPAY.Integration.DataAccess.UnitOfWork
{
    public sealed class UnitOfWorkContext : IUnitOfWorkContext
    {
        public NpgsqlConnection Context { get; init; }
        public NpgsqlTransaction Transaction { get; init; }

        public UnitOfWorkContext(string connectionString)
        {
            this.Context = new NpgsqlConnection(connectionString);

            try
            {
                this.Context.Open();
                this.Transaction = Context.BeginTransaction();
            }
            catch (Exception)
            {
                NpgsqlConnection.ClearAllPools();

                this.Context.Open();
                this.Transaction = Context.BeginTransaction();
            }
        }
        public void Dispose()
        {
            if (Transaction != null)
            {
                Transaction.Dispose();
            }

            if (Context != null)
            {
                Context.Close();
                Context.Dispose();
            }

            GC.SuppressFinalize(this);
        }
    }
}
