using ETC.EPAY.Integration.DataAccess;
using ETC.EPAY.Integration.Extensions;
using ETC.EPAY.Integration.Models;
using ETC.EPAY.Integration.Request;
using ETC.EPAY.Integration.Resources;
using ETC.EPAY.Integration.Resources.DTO.Payment.Request;
using ETC.EPAY.Integration.Resources.DTO.Payment.Response;
using ETC.EPAY.Integration.Resources.Enums;
using ETC.EPAY.Integration.Response;
using ETC.EPAY.Integration.Results;
using ETC.EPAY.Integration.Services.PaymentGateway;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ETC.EPAY.Integration.Services.Payment
{
    public class PaymentService : IPaymentService
    {
        private IPaymentLogDAO _paymentLogDAO;
        private IPayGwService _payGwService;
        private HttpContext _httpContext => new HttpContextAccessor().HttpContext;
        private string _currencyCode = "VND";
        private long _expiredTime = 60;
        private long _feeSum = 120;
        private string _merchantCode;
        private string _merchantPassword;
        private long _appExpiredSeconds;
        private long _kioskExpiredSeconds;
        public PaymentService(IPaymentLogDAO paymentLogDAO, IPayGwService payGwService)
        {
            _paymentLogDAO = paymentLogDAO;
            _payGwService = payGwService;
        }

        private BaseResult<Inner> GetBaseResult<Inner>(CodeMessage statusCode, Inner data = default, StatusEnum status = StatusEnum.Success, string message = "")
        {
            string nameStatusCode = statusCode.GetElementNameCodeMessage();
            string tempCode = string.IsNullOrEmpty(nameStatusCode) ? "217" : nameStatusCode.RemoveSpaceCharacter();
            return new BaseResult<Inner>()
            {
                StatusCode = tempCode,
                Data = data,
                Status = status,
            };
        }
        public Task<BaseResult<CheckPaymentResponse>> CheckPaymentAsync(CheckPaymentRequest request)
        {
            throw new NotImplementedException();
        }

        public async Task<BaseResult<PayGwOrderDataResponse>> GeneratePaymentAsync(PayGwCreateOrderDataRequest request, string token)
        {
            DateTime utcNow = DateTime.UtcNow;
            var paymentLogModel = new PaymentLog
            {
                Id = Guid.NewGuid().ToString(),
                client_ip = "127.0.0.1",
                trace_id = "127.0.0.1",
                request_datetime_utc = utcNow,
                partner_payment_status = PaymentStatus.None,
                isofh_payment_status = PaymentStatus.None,
                pos_id = request.PosRefId,
                pos_ip = request.PosSerial,
                device_id = request.DeviceId,
                device_name = "Example",
                order_id = request.OrderCode,
                expired_datetime_utc = utcNow.AddSeconds(_expiredTime),
                payment_flow = PaymentFlow.Checkin,
                payment_type = (PaymentType)request.PaymentType,
                currency_code = _currencyCode
            };
            var paymentLog = await _paymentLogDAO.CreateAsync(paymentLogModel);

            if ((PaymentType)request.PaymentType is PaymentType.QrEpay or PaymentType.QrMobileBanking or PaymentType.CardReader
                   or PaymentType.DomesticCard or PaymentType.CreditAndDebitCard)
            {
                var payGwOrderDataResponse = await GenerateQrCodeGatewayAsync(paymentLogModel, request, _feeSum, token);
                return payGwOrderDataResponse;
            }
            return null;
        }

        private async Task<BaseResult<PayGwOrderDataResponse>> GenerateQrCodeGatewayAsync(PaymentLog paymentLog, PayGwCreateOrderDataRequest request, decimal totalAmount, string token)
        {
            BaseResult<PayGwOrderDataResponse> generateQrCode = await _payGwService.CreateOrderAsync(request, default, token);

            if (generateQrCode.Status == StatusEnum.Success)
            {
                paymentLog.qr = generateQrCode.Data.QrCode;
                paymentLog.payment_url = paymentLog.payment_type == PaymentType.QrEpay
                    ? generateQrCode.Data.DeepLink
                    : generateQrCode.Data.PaymentUrl;
                paymentLog.partner_payment_status = PaymentStatus.Init;
                paymentLog.partner_transaction_id = generateQrCode.Data.TransId;
            }
            else
                paymentLog.partner_payment_status = PaymentStatus.Fail;
            return generateQrCode;
        }
    }
}
