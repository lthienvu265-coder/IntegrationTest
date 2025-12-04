using Dapper;
using ETC.EPAY.Integration.DataAccess.PaymentLog;
using ETC.EPAY.Integration.DataAccess.UnitOfWork;
using ETC.EPAY.Integration.Resources;
using ETC.EPAY.Integration.Resources.DTO.Payment.Request;
using ETC.EPAY.Integration.Resources.Enums;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace ETC.EPAY.Integration.DataAccess
{
    public partial class PaymentLogDAO : BaseDAO, IPaymentLogDAO
    {
        #region Constructor
        public PaymentLogDAO(IUnitOfWorkContext unitOfWorkContext)
        {
            this.Context = unitOfWorkContext.Context;
            this.Transaction = unitOfWorkContext.Transaction;
        }
        #endregion

        #region Method

        public Task<Models.PaymentLog?> GetByIdAsync(string id)
        {
            var query = PaymentLogQuery.GetByIdQuery(id);
            return Context.QueryFirstOrDefaultAsync<Models.PaymentLog?>(query.sql, query.param, Transaction, Constant.TimeOutCancelDAO);
        }

        public async Task<(bool hasValue, IEnumerable<Models.PaymentLog> data)> GetReportMonthlyAsync(DateOnly date)
        {
            // Excute
            var query = PaymentLogQuery.GetReportMonthlyQuery(date);
            var result = await Context.QueryAsync<Models.PaymentLog>(query.sql, query.param, Transaction, Constant.TimeOutCancelDAO);

            // Process result
            if (result.GetEnumerator().MoveNext())
                return (true, result);

            return (false, default);
        }

        public async Task<(bool hasValue, IEnumerable<Models.PaymentLog> data)> GetReportDailyAsync(DateOnly date)
        {
            // Excute
            var query = PaymentLogQuery.GetReportDailyQuery(date);
            var result = await Context.QueryAsync<Models.PaymentLog>(query.sql, query.param, Transaction, Constant.TimeOutCancelDAO);

            // Process result
            if (result.GetEnumerator().MoveNext())
                return (true, result);

            return (false, default);
        }

        public async Task<(bool isSuccess, Models.PaymentLog data)> CreateAsync(Models.PaymentLog model)
        {
            // Excute
            var query = PaymentLogQuery.CreateQuery(model);
            var result = await Context.ExecuteAsync(query.sql, query.param, Transaction, Constant.TimeOutCancelDAO);
            await Transaction.CommitAsync();

            // Process result
            if (result > 0)
            {
                model.transaction_id = query.param.Get<int>("transactionId");
                return (true, model);
            }
            else
            {
                return (false, model);
            }
            ;
        }

        public async Task<(bool hasValue, Models.PaymentLog data)> GetPaidOrderAsync(string orderId)
        {
            // Excute
            var query = PaymentLogQuery.GetPaidOrderQuery(orderId);
            var result = await Context.QueryFirstOrDefaultAsync<Models.PaymentLog>(query.sql, query.param, Transaction, Constant.TimeOutCancelDAO);

            // Process result
            return result != null ? (true, result) : (false, default);
        }

        public async Task<(bool hasValue, Models.PaymentLog data)> GetByOrderAsync(string orderId)
        {
            // Excute
            var query = PaymentLogQuery.GetByOrderQuery(orderId);
            var result = await Context.QueryFirstOrDefaultAsync<Models.PaymentLog>(query.sql, query.param, Transaction, Constant.TimeOutCancelDAO);

            // Process result
            return result != null ? (true, result) : (false, default);
        }

        public async Task<bool> UpdateLastRetryAsync(Models.PaymentLog model)
        {
            var query = PaymentLogQuery.UpdateLastRetry(model);
            var result = await Context.ExecuteAsync(query.sql, query.param, Transaction, Constant.TimeOutCancelDAO);
            return result > 0;
        }

        public Task<IEnumerable<Models.PaymentLog>> GetUncompletedAsync(DateOnly inDate, params PaymentFlow[] flows)
        {
            var query = PaymentLogQuery.GetUncompletedQuery(inDate, flows);
            return Context.QueryAsync<Models.PaymentLog>(query.sql, query.param, Transaction, Constant.TimeOutCancelDAO);
        }

        public async Task<(bool isSuccess, Models.PaymentLog data)> UpdatePartnerStatusAsync(Models.PaymentLog model, bool checkFinalStatus = true)
        {
            if (checkFinalStatus && await AlreadyHasFinalStatusAsync(model.Id))
            {
                return (false, model);
            }
            // Excute
            var query = PaymentLogQuery.UpdatePartnerStatusQuery(model);
            var result = await Context.ExecuteAsync(query.sql, query.param, Transaction, Constant.TimeOutCancelDAO);

            // Insert TBL_PARTNER_PAYMENT_STATUS
            var queryInsert = PaymentLogQuery.InsertPartnerStatusQuery(model);
            var resultInsert = await Context.ExecuteAsync(queryInsert.sql, queryInsert.param, Transaction, Constant.TimeOutCancelDAO);

            // Process result
            return result > 0 ? (true, model) : (false, model);
        }

        private async Task<bool> AlreadyHasFinalStatusAsync(string id)
        {
            var payment = await GetByIdAsync(id);
            if (payment == null)
            {
                return false;
            }

            return payment.partner_payment_status is PaymentStatus.Success or PaymentStatus.Fail;
        }

        public async Task<(bool isSuccess, Models.PaymentLog data)> CheckPaymentAsync(CheckPaymentRequest request)
        {
            // Excute
            var query = PaymentLogQuery.CheckPaymentQuery(request);
            var result = await Context.QuerySingleOrDefaultAsync<Models.PaymentLog>(query.sql, query.param, Transaction, Constant.TimeOutCancelDAO);

            // Process result
            return result != null ? (true, result) : (false, result);
        }

        public async Task<(bool isSuccess, Models.PaymentLog data)> CheckPaymentEpayAsync(CheckPaymentEpayRequest request)
        {
            // Excute
            var query = PaymentLogQuery.CheckPaymentEpayQuery(request);
            var result = await Context.QuerySingleOrDefaultAsync<Models.PaymentLog>(query.sql, query.param, Transaction, Constant.TimeOutCancelDAO);

            // Process result
            return result != null ? (true, result) : (false, result);
        }

        public async Task<(bool isSuccess, Models.PaymentLog data)> GetByPartnerIdAsync(string partnerTransId)
        {
            // Excute
            var query = PaymentLogQuery.GetByPartnerIdQuery(partnerTransId);
            var result = await Context.QuerySingleOrDefaultAsync<Models.PaymentLog>(query.sql, query.param, Transaction, Constant.TimeOutCancelDAO);

            // Process result
            return result != null ? (true, result) : (false, result);
        }

        public async Task<(bool hasValue, Models.PaymentLog data)> GetForScanAgainAsync(string maHoSo, string maNb, string maPhieuThu)
        {
            // Excute
            var query = PaymentLogQuery.GetForScanAgainQuery(maHoSo, maNb, maPhieuThu);
            var result = await Context.QueryFirstOrDefaultAsync<Models.PaymentLog>(query.sql, query.param, Transaction, Constant.TimeOutCancelDAO);

            // Process result
            return result != null ? (true, result) : (false, result);
        }
        #endregion
    }
}
