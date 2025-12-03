using AutoMapper;
using ETC.EPAY.Integration.Backend;
using ETC.EPAY.Integration.DataAccess;
using ETC.EPAY.Integration.DataAccess.UnitOfWork;
using ETC.EPAY.Integration.Extensions;
using ETC.EPAY.Integration.Models;
using ETC.EPAY.Integration.Request;
using ETC.EPAY.Integration.Resources;
using ETC.EPAY.Integration.Resources.DTO.Payment.Response;
using ETC.EPAY.Integration.Resources.Enums;
using ETC.EPAY.Integration.Response;
using ETC.EPAY.Integration.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Net.WebSockets;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ETC.EPAY.Integration.Services.PaymentGateway
{
    public class PayGwService : IPayGwService
    {
        private string _baseAddress;
        private string _merchantCode;
        private string _merchantUser;
        private string _merchantPassword;
        /// <summary>
        /// Private key dùng để giải mã dữ liệu từ Cổng Thanh Toán (Do BE Kiosk tạo ra)
        /// </summary>
        private string _privateKey;
        private string _publicKey;
        private string _secretKey;
        private IPaymentLogDAO _paymentLogDAO;
        private IUnitOfWork _unitOfWork;
        private int _kioskExpiredSeconds;
        private readonly WebSocketConnectionManager _wsManager;
        protected readonly ResponseMessage ResponseMessage = new ResponseMessage();

        public PayGwService(string baseAddress, string merchantCode, string merchantUser, string merchantPassword, string privateKey, string secretKey, IPaymentLogDAO paymentLogDAO, WebSocketConnectionManager wsManager)
        {
            _baseAddress = baseAddress;
            _merchantCode = merchantCode;
            _merchantUser = merchantUser;
            _merchantPassword = merchantPassword;
            _privateKey = privateKey;
            _secretKey = secretKey;
            _paymentLogDAO = paymentLogDAO;
            _wsManager = wsManager;
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

        public async Task<BaseResult<PayGwTokenDataResponse>> GetTokenAsync(CancellationToken cancellationToken)
        {
            var _httpClient = new HttpClient();
            try
            {
                string url = $"{_baseAddress}{Resources.PaymentGateway.Login}";
                var payloadData = PayGwLoginRequest.Create(_merchantUser, _merchantPassword, _merchantCode);
                PayGwRequest<PayGwLoginRequest> payload =
                    PayGwRequest<PayGwLoginRequest>.Create(payloadData, _merchantCode, _secretKey, _privateKey);

                _httpClient.DefaultRequestHeaders.Add("merchantCode", "IACV");
                _httpClient.DefaultRequestHeaders.Add("messageType", "token");
                _httpClient.DefaultRequestHeaders.Add("lang", "vi");
                _httpClient.DefaultRequestHeaders.Add("version", "1.0.0");
                _httpClient.DefaultRequestHeaders.Add("clientIp", "127.0.0.1");

                return await PostAsync<PayGwTokenDataResponse>(url, payload, cancellationToken, _httpClient);
            }
            catch (Exception e)
            {
                return GetBaseResult<PayGwTokenDataResponse>(CodeMessage._102, status: StatusEnum.Failed);
            }
        }

        public async Task<BaseResult<PayGwOrderDataResponse>> CreateOrderAsync(PayGwCreateOrderDataRequest request,
            CancellationToken cancellationToken, string token)
        {
            var _httpClient = new HttpClient();
            try
            {
                PayGwRequest<PayGwCreateOrderDataRequest> payload =
                    PayGwRequest<PayGwCreateOrderDataRequest>.Create(request, _merchantCode, _secretKey, _privateKey);

                string url = $"{_baseAddress}{Resources.PaymentGateway.CreateOrder}";

                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                _httpClient.DefaultRequestHeaders.Add("merchantCode", "IACV");
                _httpClient.DefaultRequestHeaders.Add("messageType", "token");
                _httpClient.DefaultRequestHeaders.Add("lang", "vi");
                _httpClient.DefaultRequestHeaders.Add("version", "1.0.0");
                _httpClient.DefaultRequestHeaders.Add("clientIp", "127.0.0.1");
                return await PostAsync<PayGwOrderDataResponse>(url, payload, cancellationToken, _httpClient);
            }
            catch (Exception e)
            {
                return GetBaseResult<PayGwOrderDataResponse>(CodeMessage._102, status: StatusEnum.Failed);
            }
        }

        public async Task<BaseResult<PayGwOrderStatusDataResponse>> CheckOrderStatusAsync(
            PayGwCheckOrderStatusDataRequest request, CancellationToken cancellationToken, string token)
        {
            var _httpClient = new HttpClient();
            try
            {
                PayGwRequest<PayGwCheckOrderStatusDataRequest> payload =
                    PayGwRequest<PayGwCheckOrderStatusDataRequest>.Create(request, _merchantCode, _secretKey, _privateKey);

                string url = $"{_baseAddress}{Resources.PaymentGateway.CheckStatus}";

                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                _httpClient.DefaultRequestHeaders.Add("merchantCode", "IACV");
                _httpClient.DefaultRequestHeaders.Add("messageType", "token");
                _httpClient.DefaultRequestHeaders.Add("lang", "vi");
                _httpClient.DefaultRequestHeaders.Add("version", "1.0.0");
                _httpClient.DefaultRequestHeaders.Add("clientIp", "127.0.0.1");
                return await PostAsync<PayGwOrderStatusDataResponse>(url, payload, cancellationToken, _httpClient);
            }
            catch (Exception e)
            {
                return GetBaseResult<PayGwOrderStatusDataResponse>(CodeMessage._102, status: StatusEnum.Failed);
            }
        }

        private async Task<BaseResult<T>> PostAsync<T>(string url, object payload, CancellationToken cancellationToken, HttpClient _httpClient) where T : PayGwDataResponseBase
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync(url, payload, cancellationToken: cancellationToken);
                response.EnsureSuccessStatusCode();

                PayGwResponse<T>? result = await response.Content.ReadFromJsonAsync<PayGwResponse<T>>(cancellationToken: cancellationToken);
                if (result is null)
                {
                    return GetBaseResult<T>(CodeMessage._102, status: StatusEnum.Failed);
                }

                var decryptedData = result.DecryptedData(_secretKey);
                if (decryptedData is null)
                {
                    return GetBaseResult<T>(CodeMessage._102, status: StatusEnum.Failed);
                }

                if (decryptedData.IsFailed)
                {
                    return GetBaseResult<T>(CodeMessage._102, status: StatusEnum.Failed, data: decryptedData);
                }
                return GetBaseResult(CodeMessage._200, data: decryptedData);
            }
            catch (Exception e)
            {
                return GetBaseResult<T>(CodeMessage._102, status: StatusEnum.Failed);
            }
        }

        public async Task<BaseResult<PayGwRefundResponse>> RefundAsync(PayGwRefundRequest request, CancellationToken cancellationToken, string token)
        {
            var _httpClient = new HttpClient();
            try
            {
                PayGwRequest<PayGwRefundRequest> payload =
                    PayGwRequest<PayGwRefundRequest>.Create(request, _merchantCode, _secretKey, _privateKey);

                string url = $"{_baseAddress}{Resources.PaymentGateway.Refund}";

                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                _httpClient.DefaultRequestHeaders.Add("merchantCode", "IACV");
                _httpClient.DefaultRequestHeaders.Add("messageType", "token");
                _httpClient.DefaultRequestHeaders.Add("lang", "vi");
                _httpClient.DefaultRequestHeaders.Add("version", "1.0.0");
                _httpClient.DefaultRequestHeaders.Add("clientIp", "127.0.0.1");
                return await PostAsync<PayGwRefundResponse>(url, payload, cancellationToken, _httpClient);
            }
            catch (Exception e)
            {
                return GetBaseResult<PayGwRefundResponse>(CodeMessage._102, status: StatusEnum.Failed);
            }
        }

        public async Task<BaseResult<PayGwCheckRefundStatusResponse>> CheckRefundStatusAsync(PayGwCheckRefundStatusRequest request, CancellationToken cancellationToken, string token)
        {
            var _httpClient = new HttpClient();
            try
            {
                PayGwRequest<PayGwCheckRefundStatusRequest> payload =
                    PayGwRequest<PayGwCheckRefundStatusRequest>.Create(request, _merchantCode, _secretKey, _privateKey);

                string url = $"{_baseAddress}{Resources.PaymentGateway.CheckRefundStatus}";

                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                _httpClient.DefaultRequestHeaders.Add("merchantCode", "IACV");
                _httpClient.DefaultRequestHeaders.Add("messageType", "token");
                _httpClient.DefaultRequestHeaders.Add("lang", "vi");
                _httpClient.DefaultRequestHeaders.Add("version", "1.0.0");
                _httpClient.DefaultRequestHeaders.Add("clientIp", "127.0.0.1");
                return await PostAsync<PayGwCheckRefundStatusResponse>(url, payload, cancellationToken, _httpClient);
            }
            catch (Exception e)
            {
                return GetBaseResult<PayGwCheckRefundStatusResponse>(CodeMessage._102, status: StatusEnum.Failed);
            }
        }

        public async Task<BaseResult<PayGwDeleteTokenResponse>> DeleteTokenAsync(PayGwDeleteTokenRequest request, CancellationToken cancellationToken, string token)
        {
            var _httpClient = new HttpClient();
            try
            {
                PayGwRequest<PayGwDeleteTokenRequest> payload =
                    PayGwRequest<PayGwDeleteTokenRequest>.Create(request, _merchantCode, _secretKey, _privateKey);

                string url = $"{_baseAddress}{Resources.PaymentGateway.DeleteToken}";

                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                _httpClient.DefaultRequestHeaders.Add("merchantCode", "IACV");
                _httpClient.DefaultRequestHeaders.Add("messageType", "token");
                _httpClient.DefaultRequestHeaders.Add("lang", "vi");
                _httpClient.DefaultRequestHeaders.Add("version", "1.0.0");
                _httpClient.DefaultRequestHeaders.Add("clientIp", "127.0.0.1");
                return await PostAsync<PayGwDeleteTokenResponse>(url, payload, cancellationToken, _httpClient);
            }
            catch (Exception e)
            {
                return GetBaseResult<PayGwDeleteTokenResponse>(CodeMessage._102, status: StatusEnum.Failed);
            }
        }

        public async Task<BaseResult<PayGwRetrieveTokenResponse>> RetrieveTokenAsync(PayGwRetrieveTokenRequest request, CancellationToken cancellationToken, string token)
        {
            var _httpClient = new HttpClient();
            try
            {
                PayGwRequest<PayGwRetrieveTokenRequest> payload =
                    PayGwRequest<PayGwRetrieveTokenRequest>.Create(request, _merchantCode, _secretKey, _privateKey);

                string url = $"{_baseAddress}{Resources.PaymentGateway.RetrieveToken}";

                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                _httpClient.DefaultRequestHeaders.Add("merchantCode", "IACV");
                _httpClient.DefaultRequestHeaders.Add("messageType", "token");
                _httpClient.DefaultRequestHeaders.Add("lang", "vi");
                _httpClient.DefaultRequestHeaders.Add("version", "1.0.0");
                _httpClient.DefaultRequestHeaders.Add("clientIp", "127.0.0.1");
                return await PostAsync<PayGwRetrieveTokenResponse>(url, payload, cancellationToken, _httpClient);
            }
            catch (Exception e)
            {
                return GetBaseResult<PayGwRetrieveTokenResponse>(CodeMessage._102, status: StatusEnum.Failed);
            }
        }

        public async Task<BaseResult<PayGwQrCusPaymentResponse>> QrCusPaymentAsync(PayGwQrCusPaymentRequest request, CancellationToken cancellationToken, string token)
        {
            var _httpClient = new HttpClient();
            try
            {
                PayGwRequest<PayGwQrCusPaymentRequest> payload =
                    PayGwRequest<PayGwQrCusPaymentRequest>.Create(request, _merchantCode, _secretKey, _privateKey);

                string url = $"{_baseAddress}{Resources.PaymentGateway.QrCusPayment}";

                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                _httpClient.DefaultRequestHeaders.Add("merchantCode", "IACV");
                _httpClient.DefaultRequestHeaders.Add("messageType", "token");
                _httpClient.DefaultRequestHeaders.Add("lang", "vi");
                _httpClient.DefaultRequestHeaders.Add("version", "1.0.0");
                _httpClient.DefaultRequestHeaders.Add("clientIp", "127.0.0.1");
                return await PostAsync<PayGwQrCusPaymentResponse>(url, payload, cancellationToken, _httpClient);
            }
            catch (Exception e)
            {
                return GetBaseResult<PayGwQrCusPaymentResponse>(CodeMessage._102, status: StatusEnum.Failed);
            }
        }

        public async Task<BaseResult<bool>> CreateOrderReturnUrlAsync(string epayResult, CancellationToken cancellationToken)
        {
            string epayResultJson = Encoding.UTF8.GetString(Convert.FromBase64String(epayResult));
            var payGwReturnUrlRequest = JsonConvert.DeserializeObject<PayGwReturnUrlRequest>(epayResultJson);
            if (string.IsNullOrEmpty(payGwReturnUrlRequest?.TransCode))
                return GetBaseResult<bool>(CodeMessage._98, status: StatusEnum.Failed);

            var paymentLog = await _paymentLogDAO.GetByIdAsync(payGwReturnUrlRequest.OrderCode);

            // Validate data
            if (paymentLog == null)
                return GetBaseResult<bool>(CodeMessage._211, status: StatusEnum.Failed);

            // Lưu trạng thái ban đầu trước khi kiểm tra
            var oldPartnerStatus = paymentLog.PartnerPaymentStatus;

            paymentLog.PartnerTransactionId = payGwReturnUrlRequest.TransCode;
            if (paymentLog.PartnerPaymentStatus is PaymentStatus.Fail or PaymentStatus.Success)
            {
                return GetBaseResult(CodeMessage._200, data: false);
            }

            // Update payment status
            paymentLog.PartnerPaymentStatus = payGwReturnUrlRequest.TransStatus switch
            {
                PayGwStatus.NotPaid => PaymentStatus.Pending,
                PayGwStatus.Paid => PaymentStatus.Success,
                PayGwStatus.Fail => PaymentStatus.Fail,
                PayGwStatus.Pending => PaymentStatus.Pending,
                PayGwStatus.UserCanceled => PaymentStatus.Cancel,
                PayGwStatus.WaitPartnerProcess => PaymentStatus.Pending,
                _ => throw new ArgumentOutOfRangeException()
            };

            var updateResult = await _paymentLogDAO.UpdatePartnerStatusAsync(paymentLog);
            if (!updateResult.isSuccess)
            {
                return GetBaseResult<bool>(CodeMessage._99, status: StatusEnum.Failed);
            }

            await _unitOfWork.SaveChangesAsync();

            // Process result
            return GetBaseResult(CodeMessage._200, data: paymentLog.PartnerPaymentStatus == PaymentStatus.Success ? true : false);
        }

        public async Task<BaseResult<bool>> CreateOrderCallbackAsync(PayGwResponse<PayGwIpnPayGatewayRequest> request, CancellationToken cancellationToken)
        {
            if (request.VerifySignature(_publicKey, _secretKey) == false)
            {
                return GetBaseResult<bool>(CodeMessage._98, status: StatusEnum.Failed);
            }
            if (string.IsNullOrEmpty(request.Data))
            {
                return GetBaseResult<bool>(CodeMessage._98, status: StatusEnum.Failed);
            }
            var createOrderCallbackRequest = request.DecryptedData(_secretKey);

            if (string.IsNullOrEmpty(createOrderCallbackRequest?.TransCode))
                return GetBaseResult<bool>(CodeMessage._98, status: StatusEnum.Failed);

            var paymentLog = await _paymentLogDAO.GetByIdAsync(createOrderCallbackRequest.OrderCode);

            // Validate data
            if (paymentLog == null)
                return GetBaseResult<bool>(CodeMessage._211, status: StatusEnum.Failed);

            // Lưu trạng thái ban đầu trước khi kiểm tra
            var oldPartnerStatus = paymentLog.PartnerPaymentStatus;

            paymentLog.PartnerTransactionId = createOrderCallbackRequest.TransCode;
            if (paymentLog.PartnerPaymentStatus is PaymentStatus.Fail or PaymentStatus.Success)
            {
                return GetBaseResult(CodeMessage._200, data: false);
            }

            // Update payment status
            paymentLog.PartnerPaymentStatus = createOrderCallbackRequest.TransStatus switch
            {
                PayGwStatus.NotPaid => PaymentStatus.Pending,
                PayGwStatus.Paid => PaymentStatus.Success,
                PayGwStatus.Fail => PaymentStatus.Fail,
                PayGwStatus.Pending => PaymentStatus.Pending,
                PayGwStatus.UserCanceled => PaymentStatus.Cancel,
                PayGwStatus.WaitPartnerProcess => PaymentStatus.Pending,
                _ => throw new ArgumentOutOfRangeException()
            };

            var updateResult = await _paymentLogDAO.UpdatePartnerStatusAsync(paymentLog);
            if (!updateResult.isSuccess)
            {
                return GetBaseResult<bool>(CodeMessage._99, status: StatusEnum.Failed);
            }

            await _unitOfWork.SaveChangesAsync();

            // 🔥 Send message to all WebSocket clients
            var payload = new
            {
                SocketId = paymentLog.PosId,
                OrderCode = paymentLog.OrderId,
                Status = paymentLog.PartnerPaymentStatus.ToString()
            };

            var json = System.Text.Json.JsonSerializer.Serialize(payload);
            var bytes = System.Text.Encoding.UTF8.GetBytes(json);

            foreach (var socket in _wsManager.GetAllSockets())
            {
                if (socket.Key == payload.SocketId)
                {
                    if (socket.Value.State == WebSocketState.Open)
                    {
                        await socket.Value.SendAsync(
                            bytes,
                            WebSocketMessageType.Text,
                            endOfMessage: true,
                            cancellationToken: CancellationToken.None
                        );
                    }
                }
            }

            // Process result
            return GetBaseResult(CodeMessage._200,
                data: paymentLog.PartnerPaymentStatus == PaymentStatus.Success ? true : false);
        }

        public async Task<BaseResult<bool>> RefundCallbackAsync(PayGwResponse<PayGwIpnPayGatewayRefundRequest> request, CancellationToken cancellationToken)
        {
            if (request.VerifySignature(_publicKey, _secretKey) == false)
            {
                return GetBaseResult<bool>(CodeMessage._98, status: StatusEnum.Failed);
            }
            if (string.IsNullOrEmpty(request.Data))
            {
                return GetBaseResult<bool>(CodeMessage._98, status: StatusEnum.Failed);
            }
            var createOrderCallbackRequest = request.DecryptedData(_secretKey);

            if (string.IsNullOrEmpty(createOrderCallbackRequest?.TransCode))
                return GetBaseResult<bool>(CodeMessage._98, status: StatusEnum.Failed);

            var paymentLog = await _paymentLogDAO.GetByIdAsync(createOrderCallbackRequest.OrderCode);

            // Validate data
            if (paymentLog == null)
                return GetBaseResult<bool>(CodeMessage._211, status: StatusEnum.Failed);

            // Lưu trạng thái ban đầu trước khi kiểm tra
            var oldPartnerStatus = paymentLog.PartnerPaymentStatus;

            paymentLog.PartnerTransactionId = createOrderCallbackRequest.TransCode;
            if (paymentLog.PartnerPaymentStatus is PaymentStatus.Fail or PaymentStatus.Success)
            {
                return GetBaseResult(CodeMessage._200, data: false);
            }

            // Update payment status
            paymentLog.PartnerPaymentStatus = createOrderCallbackRequest.TransStatus switch
            {
                PayGwStatus.NotPaid => PaymentStatus.Pending,
                PayGwStatus.Paid => PaymentStatus.Success,
                PayGwStatus.Fail => PaymentStatus.Fail,
                PayGwStatus.Pending => PaymentStatus.Pending,
                PayGwStatus.UserCanceled => PaymentStatus.Cancel,
                PayGwStatus.WaitPartnerProcess => PaymentStatus.Pending,
                _ => throw new ArgumentOutOfRangeException()
            };

            var updateResult = await _paymentLogDAO.UpdatePartnerStatusAsync(paymentLog);
            if (!updateResult.isSuccess)
            {
                return GetBaseResult<bool>(CodeMessage._99, status: StatusEnum.Failed);
            }

            await _unitOfWork.SaveChangesAsync();

            // Process result
            return GetBaseResult(CodeMessage._200,
                data: paymentLog.PartnerPaymentStatus == PaymentStatus.Success ? true : false);
        }

        /// <summary>
        /// Chức năng: lấy các cấu hình cần thiết cho việc request
        /// </summary>
        /// <returns></returns>
        /// <exception cref="MessageResultException"></exception>
        private GeneratePaymentResponse MappingReCreateResult(PaymentLog paymentLog)
        {
            GeneratePaymentResponse result = new GeneratePaymentResponse
            {
                PartnerPaymentStatus = paymentLog.PartnerPaymentStatus,
                IsofhPaymentStatus = paymentLog.IsofhPaymentStatus,
                Type = paymentLog.PaymentType,
                TransactionId = paymentLog.TransactionId.ConvertTransactionIdToEpayId(),
                OrderId = paymentLog.OrderId,
                PaymentUrl = paymentLog.PaymentUrl,
                QrImage = string.IsNullOrEmpty(paymentLog.Qr) ? string.Empty : paymentLog.Qr.GenerateQrBase64(),
                RequestDatetimeUtc = paymentLog.RequestDatetimeUtc,
                ResponseDatetimeUtc = paymentLog.ResponseDatetimeUtc,
                Invoice = paymentLog.Invoice,
                ChangePriceInformation = new(paymentLog.OldTotalAmount, paymentLog.NewTotalAmount),
                ExpiredSeconds = _kioskExpiredSeconds,
                ExpiredTimeUtc = paymentLog.ExpiredDatetimeUtc
            };

            return result;
        }
    }
}
