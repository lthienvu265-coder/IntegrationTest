using Dapper;
using ETC.EPAY.Integration.Extensions;
using ETC.EPAY.Integration.Resources.DTO.Payment.Request;
using ETC.EPAY.Integration.Resources.Enums;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETC.EPAY.Integration.DataAccess.PaymentLog
{
    public static class PaymentLogQuery
    {
        public static (string sql, DynamicParameters param) GetByIdQuery(string id)
        {
            var param = new DynamicParameters();
            param.Add("@id", id, dbType: DbType.String,
                direction: ParameterDirection.Input);
            string query = @"SELECT * FROM TBL_PAYMENT_LOG WHERE ID = @id";

            return (query, param);
        }

        public static (string sql, DynamicParameters param) GetUncompletedQuery(DateOnly date, PaymentFlow[] flows)
        {
            // Param component
            var param = new DynamicParameters();
            param.Add("@dateRequest", date.ToString("dd-MMM-yy"), dbType: DbType.String,
                direction: ParameterDirection.Input);

            // Add flows to parameters
            string flowList = String.Empty;

            if (flows.Length > 0)
            {
                flowList = string.Join(",", flows.Select(f => (int)f));
                param.Add("@flows", flowList, dbType: DbType.String, direction: ParameterDirection.Input);
            }

            // SQL component
            string query = $@"SELECT * FROM TBL_PAYMENT_LOG WHERE 
                                  PAYMENT_TYPE IN (1, 2, 3, 6, 7) AND 
                                  (TO_DATE(TO_CHAR(FROM_TZ( CAST(CREATED_DATETIME_UTC AS TIMESTAMP ), 'UTC' ) AT TIME ZONE 'ASIA/BANGKOK', 'MM/DD/YYYY'), 'MM/DD/YYYY') = @dateRequest) AND
                                  (
                                      (PARTNER_PAYMENT_STATUS = 1 AND ISOFH_PAYMENT_STATUS NOT IN (0, 1)) OR
                                      PARTNER_PAYMENT_STATUS IN (2, 4, 7)
                                    )";

            if (!string.IsNullOrEmpty(flowList))
            {
                query += $"AND PAYMENT_FLOW IN ({flowList})";
            }

            return (query, param);
        }

        public static (string sql, DynamicParameters param) GetReportMonthlyQuery(DateOnly date)
        {
            // Param component
            var param = new DynamicParameters();
            param.Add("@dateRequest", date.ToString("yyyy/MM"), dbType: DbType.String,
                direction: ParameterDirection.Input);

            // SQL component
            string query =
                @"SELECT * FROM TBL_PAYMENT_LOG WHERE (TO_CHAR(FROM_TZ( CAST(CREATED_DATETIME_UTC AS TIMESTAMP ), 'UTC' ) AT TIME ZONE 'ASIA/BANGKOK', 'YYYY/MM') = @dateRequest)";

            return (query, param);
        }

        public static (string sql, DynamicParameters param) GetReportDailyQuery(DateOnly date)
        {
            // Param component
            var param = new DynamicParameters();
            param.Add("@dateRequest", date.ToString("dd-MMM-yy").ToUpper(), dbType: DbType.String,
                direction: ParameterDirection.Input);

            // SQL component
            string query =
                @"SELECT * FROM TBL_PAYMENT_LOG WHERE (TO_DATE(TO_CHAR(FROM_TZ( CAST(CREATED_DATETIME_UTC AS TIMESTAMP ), 'UTC' ) AT TIME ZONE 'ASIA/BANGKOK', 'MM/DD/YYYY'), 'MM/DD/YYYY') = @dateRequest)";

            return (query, param);
        }

        public static (string sql, DynamicParameters param) CreateQuery(Models.PaymentLog model)
        {
            // Param component
            var param = new DynamicParameters();
            param.Add("@id", model.Id, dbType: DbType.String, direction: ParameterDirection.Input);
            param.Add("@clientIp", model.client_ip, dbType: DbType.String, direction: ParameterDirection.Input);
            param.Add("@traceId", model.trace_id, dbType: DbType.String, direction: ParameterDirection.Input);
            param.Add("@createdDatetimeUtc", DateTime.UtcNow, dbType: DbType.DateTime,
                direction: ParameterDirection.Input);
            param.Add("@updatedDatetimeUtc", DateTime.UtcNow, dbType: DbType.DateTime,
                direction: ParameterDirection.Input);

            param.Add("@paymentType", model.payment_type, dbType: DbType.Int32, direction: ParameterDirection.Input);
            param.Add("@requestDatetimeUtc", model.request_datetime_utc, dbType: DbType.DateTime,
                direction: ParameterDirection.Input);
            param.Add("@partnerTransactionId", model.partner_transaction_id, dbType: DbType.String,
                direction: ParameterDirection.Input);
            param.Add("@partnerPaymentStatus", model.partner_payment_status, dbType: DbType.Int32,
                direction: ParameterDirection.Input);
            param.Add("@isofhPaymentStatus", model.isofh_payment_status, dbType: DbType.Int32,
                direction: ParameterDirection.Input);
            param.Add("@posId", model.pos_id ?? string.Empty, dbType: DbType.String,
                direction: ParameterDirection.Input);
            param.Add("@posIp", model.pos_ip ?? string.Empty, dbType: DbType.String,
                direction: ParameterDirection.Input);
            param.Add("@deviceId", model.device_id, dbType: DbType.String, direction: ParameterDirection.Input);
            param.Add("@deviceName", model.device_name, dbType: DbType.String, direction: ParameterDirection.Input);
            param.Add("@orderId", model.order_id, dbType: DbType.String, direction: ParameterDirection.Input);
            param.Add("@newTotalAmount", model.new_total_amount, dbType: DbType.Decimal,
                direction: ParameterDirection.Input);
            param.Add("@oldTotalAmount", model.old_total_amount, dbType: DbType.Decimal,
                direction: ParameterDirection.Input);
            param.Add("@responseDatetimeUtc", model.response_datetime_utc, dbType: DbType.DateTime,
                direction: ParameterDirection.Input);
            param.Add("@expiredDatetimeUtc", model.expired_datetime_utc, dbType: DbType.DateTime,
                direction: ParameterDirection.Input);
            param.Add("@qr", model.qr ?? string.Empty, dbType: DbType.String, direction: ParameterDirection.Input);
            param.Add("@paymentFlow", model.payment_flow, dbType: DbType.Int32, direction: ParameterDirection.Input);
            param.Add("@phoneNumber", model.phone_number ?? string.Empty, dbType: DbType.String,
                direction: ParameterDirection.Input);
            param.Add("@currencyCode", model.currency_code ?? string.Empty, dbType: DbType.String,
                direction: ParameterDirection.Input);

            param.Add("@maHoSo", model.ma_ho_so, dbType: DbType.String, direction: ParameterDirection.Input);
            param.Add("@maNb", model.ma_nb, dbType: DbType.String, direction: ParameterDirection.Input);
            param.Add("@maPhieuThu", model.ma_phieu_thu, dbType: DbType.String, direction: ParameterDirection.Input);
            param.Add("@hisRegister", model.his_register, dbType: DbType.String, direction: ParameterDirection.Input);
            param.Add("@paymentUrl", model.payment_url, dbType: DbType.String, direction: ParameterDirection.Input);

            param.Add("@transactionId", dbType: DbType.Int32, direction: ParameterDirection.Output);

            // SQL component
            string query =
                @"insert into public.tbl_payment_log(
                    id,
                    client_ip,
                    trace_id,
                    created_datetime_utc,
                    updated_datetime_utc,
                    payment_type,
                    request_datetime_utc,
                    partner_transaction_id,
                    partner_payment_status,
                    isofh_payment_status,
                    pos_id,
                    pos_ip,
                    device_id,
                    device_name,
                    order_id,
                    response_datetime_utc,
                    expired_datetime_utc,
                    qr,
                    payment_flow,
                    phone_number,
                    currency_code,
                    ma_ho_so,
                    ma_nb,
                    ma_phieu_thu,
                    new_total_amount,
                    old_total_amount,
                    his_register,
                    payment_url
                ) 
                values (@id, @clientIp, @traceId, @createdDatetimeUtc, @updatedDatetimeUtc,
                @paymentType, @requestDatetimeUtc, @partnerTransactionId, @partnerPaymentStatus,@isofhPaymentStatus, @posId, @posIp, @deviceId,@deviceName, @orderId,
                @responseDatetimeUtc, @expiredDatetimeUtc, @qr, @paymentFlow, @phoneNumber, @currencyCode,
                @maHoSo, @maNb, @maPhieuThu, @newTotalAmount, @oldTotalAmount, @hisRegister, @paymentUrl) RETURNING @transaction_id;";

            return (query, param);
        }

        public static (string sql, DynamicParameters param) GetPaidOrderQuery(string orderId)
        {
            // Param component
            var param = new DynamicParameters();
            param.Add("@orderId", orderId, dbType: DbType.String, direction: ParameterDirection.Input);

            // SQL component
            string query = @"SELECT * FROM TBL_PAYMENT_LOG WHERE ORDER_ID = @orderId 
                    FETCH FIRST 1 ROW ONLY";

            return (query, param);
        }

        public static (string sql, DynamicParameters param) GetByOrderQuery(string orderId)
        {
            // Param component
            var param = new DynamicParameters();
            param.Add("@orderId", orderId, dbType: DbType.String, direction: ParameterDirection.Input);

            // SQL component
            string query = @"SELECT * FROM TBL_PAYMENT_LOG WHERE ORDER_ID = @orderId
                    FETCH FIRST 1 ROW ONLY";

            return (query, param);
        }

        public static (string sql, DynamicParameters param) UpdatePartnerStatusQuery(Models.PaymentLog model)
        {
            // Param component
            var param = new DynamicParameters();
            param.Add("@id", model.Id, dbType: DbType.String, direction: ParameterDirection.Input);
            param.Add("@partnerPaymentStatus", model.partner_payment_status, dbType: DbType.Int32,
                direction: ParameterDirection.Input);
            param.Add("@isofhPaymentStatus", model.isofh_payment_status, dbType: DbType.Int32,
                direction: ParameterDirection.Input);
            param.Add("@updatedDatetimeUtc", DateTime.UtcNow, dbType: DbType.DateTime,
                direction: ParameterDirection.Input);
            param.Add("@transactionId", model.transaction_id, dbType: DbType.Int32, direction: ParameterDirection.Input);
            param.Add("@invoice", model.invoice, dbType: DbType.String, direction: ParameterDirection.Input);
            param.Add("@partnerTransactionId", model.partner_transaction_id, dbType: DbType.String,
                direction: ParameterDirection.Input);

            // SQL component
            string query = @"UPDATE TBL_PAYMENT_LOG
                SET PARTNER_PAYMENT_STATUS = @partnerPaymentStatus,
                ISOFH_PAYMENT_STATUS = @isofhPaymentStatus,
                INVOICE = @invoice,
                UPDATED_DATETIME_UTC = @updatedDatetimeUtc,
                PARTNER_TRANSACTION_ID = @partnerTransactionId
                WHERE TRANSACTION_ID = @transactionId";

            return (query, param);
        }

        public static (string sql, DynamicParameters param) UpdateLastRetry(Models.PaymentLog model)
        {
            var param = new DynamicParameters();
            param.Add("@id", model.Id, dbType: DbType.String, direction: ParameterDirection.Input);
            param.Add("@lastRetryDatetimeUtc", DateTime.UtcNow, dbType: DbType.DateTime,
                direction: ParameterDirection.Input);
            param.Add("@totalRetry", model.total_retry, dbType: DbType.Int32, direction: ParameterDirection.Input);

            string query = @"UPDATE TBL_PAYMENT_LOG
                SET LAST_RETRY_DATETIME_UTC = @lastRetryDatetimeUtc,
                    TOTAL_RETRY = @totalRetry
                WHERE ID = @id";
            return (query, param);
        }

        public static (string sql, DynamicParameters param) InsertPartnerStatusQuery(Models.PaymentLog model)
        {
            // Param component
            var param = new DynamicParameters();
            param.Add("@partnerPaymentStatus", model.partner_payment_status, dbType: DbType.Int32,
                direction: ParameterDirection.Input);
            param.Add("@id", model.Id, dbType: DbType.String, direction: ParameterDirection.Input);
            param.Add("@createdDatetimeUtc", DateTime.UtcNow, dbType: DbType.DateTime,
                direction: ParameterDirection.Input);
            param.Add("@updatedDatetimeUtc", DateTime.UtcNow, dbType: DbType.DateTime,
                direction: ParameterDirection.Input);

            // SQL component
            string query =
                @"INSERT INTO TBL_PARTNER_PAYMENT_STATUS(STATUS, PAYMENT_LOG_ID, CREATED_DATETIME_UTC, UPDATED_DATETIME_UTC)
                        VALUES (@partnerPaymentStatus, @id, @createdDatetimeUtc, @updatedDatetimeUtc)";

            return (query, param);
        }

        public static (string sql, DynamicParameters param) GetByIdQuery(int id)
        {
            // Param component
            var param = new DynamicParameters();
            param.Add("@id", id, dbType: DbType.String, direction: ParameterDirection.Input);

            // SQL component
            string query = @"SELECT * FROM TBL_PAYMENT_LOG WHERE ID = @id";

            return (query, param);
        }

        public static (string sql, DynamicParameters param) CheckPaymentQuery(CheckPaymentRequest request)
        {
            // Param component
            var transId = request.TransactionId.ConvertEpayIdToTransactionId();
            var param = new DynamicParameters();
            param.Add("@transactionId", transId, dbType: DbType.Int32,
                direction: ParameterDirection.Input);
            param.Add("@orderId", request.OrderId, dbType: DbType.String, direction: ParameterDirection.Input);

            // SQL component
            string query =
                @"SELECT * FROM TBL_PAYMENT_LOG WHERE TRANSACTION_ID = @transactionId AND ORDER_ID = @orderId";

            return (query, param);
        }

        public static (string sql, DynamicParameters param) CheckPaymentEpayQuery(CheckPaymentEpayRequest request)
        {
            // Param component
            var param = new DynamicParameters();
            param.Add("@maNb", request.MaNb, dbType: DbType.String, direction: ParameterDirection.Input);
            param.Add("@maHoSo", request.MaHoSo, dbType: DbType.String, direction: ParameterDirection.Input);
            param.Add("@phieuThuId", request.PhieuThuId, dbType: DbType.String, direction: ParameterDirection.Input);

            // SQL component
            string query =
                @"SELECT * FROM TBL_PAYMENT_LOG WHERE MA_HO_SO = @maHoSo AND MA_NB = @maNb AND MA_PHIEU_THU = @phieuThuId";

            return (query, param);
        }

        public static (string sql, DynamicParameters param) GetByPartnerIdQuery(string partnerTransId)
        {
            // Param component
            var param = new DynamicParameters();
            param.Add("@partnerTransId", partnerTransId, dbType: DbType.String, direction: ParameterDirection.Input);

            // SQL component
            string query = @"SELECT * FROM TBL_PAYMENT_LOG WHERE PARTNER_TRANSACTION_ID = @partnerTransId";

            return (query, param);
        }

        public static (string sql, DynamicParameters param) GetForScanAgainQuery(string maHoSo, string maNb,
            string maPhieuThu)
        {
            // Param component
            var param = new DynamicParameters();
            param.Add("@maHoSo", maHoSo, dbType: DbType.String, direction: ParameterDirection.Input);
            param.Add("@maNb", maNb, dbType: DbType.String, direction: ParameterDirection.Input);
            param.Add("@maPhieuThu", maPhieuThu, dbType: DbType.String, direction: ParameterDirection.Input);

            // SQL component
            string query = @"SELECT * 
                             FROM TBL_PAYMENT_LOG 
                             WHERE 
                             MA_HO_SO = @maHoSo
                             AND MA_NB = @maNb
                             AND MA_PHIEU_THU = @maPhieuThu
                             --AND PARTNER_PAYMENT_STATUS = 1 
                             --AND ISOFH_PAYMENT_STATUS = 1
                             ORDER BY UPDATED_DATETIME_UTC DESC
                             FETCH FIRST 1 ROW ONLY";

            return (query, param);
        }
    }
}
