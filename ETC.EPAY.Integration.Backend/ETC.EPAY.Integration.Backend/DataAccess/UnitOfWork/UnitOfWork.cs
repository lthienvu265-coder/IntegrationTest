using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETC.EPAY.Integration.DataAccess.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        #region Properties
        public IUnitOfWorkContext UnitOfWorkContext { get; init; }
        #endregion

        #region Constructor
        public UnitOfWork(IUnitOfWorkContext unitOfWorkContext)
        {
            this.UnitOfWorkContext = unitOfWorkContext;
        }
        #endregion

        #region Method
        public void Dispose()
        {
            if (UnitOfWorkContext.Transaction != null)
            {
                UnitOfWorkContext.Transaction.Dispose();
            }

            if (UnitOfWorkContext.Context != null)
            {
                UnitOfWorkContext.Context.Close();
                UnitOfWorkContext.Context.Dispose();
            }

            GC.SuppressFinalize(this);
        }

        public void SaveChanges() =>
                UnitOfWorkContext.Transaction.Commit();

        public async Task SaveChangesAsync() =>
                await UnitOfWorkContext.Transaction.CommitAsync();
        #endregion
    }
}
