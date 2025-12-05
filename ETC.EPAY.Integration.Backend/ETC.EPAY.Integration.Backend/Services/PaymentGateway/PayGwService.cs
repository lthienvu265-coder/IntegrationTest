using AutoMapper;
using ETC.EPAY.Integration.Backend;
using ETC.EPAY.Integration.Backend.EpayRequest;
using ETC.EPAY.Integration.Backend.Resources;
using ETC.EPAY.Integration.DataAccess;
using ETC.EPAY.Integration.DataAccess.UnitOfWork;
using ETC.EPAY.Integration.Extensions;
using ETC.EPAY.Integration.Request;
using ETC.EPAY.Integration.Resources;
using ETC.EPAY.Integration.Resources.Enums;
using ETC.EPAY.Integration.Response;
using ETC.EPAY.Integration.Results;
using Newtonsoft.Json;
using Org.BouncyCastle.Crypto.Agreement.JPake;
using System.Net.WebSockets;
using System.Text;

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
        private string _secretKey;
        private IPaymentLogDAO _paymentLogDAO;
        private IUnitOfWork _unitOfWork;
        private readonly WebSocketCreateOrderConnectionManager _wsManagerCreateOrder;
        private readonly WebSocketRefundConnectionManager _wsManagerRefund;
        protected readonly ResponseMessage ResponseMessage = new ResponseMessage();

        public PayGwService(string baseAddress, string merchantCode, string merchantUser, string merchantPassword, string privateKey, string secretKey, IPaymentLogDAO paymentLogDAO, IUnitOfWork unitOfWork, WebSocketCreateOrderConnectionManager wsManagerCreateOrder, WebSocketRefundConnectionManager wsManagerRefund)
        {
            _baseAddress = baseAddress;
            _merchantCode = merchantCode;
            _merchantUser = merchantUser;
            _merchantPassword = merchantPassword;
            _privateKey = privateKey;
            _secretKey = secretKey;
            _paymentLogDAO = paymentLogDAO;
            _unitOfWork = unitOfWork;
            _wsManagerCreateOrder = wsManagerCreateOrder;
            _wsManagerRefund = wsManagerRefund;
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
            catch (Exception)
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
            catch (Exception)
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
            catch (Exception)
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
            catch (Exception)
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
            catch (Exception)
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
            catch (Exception)
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
            catch (Exception)
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
            catch (Exception)
            {
                return GetBaseResult<PayGwRetrieveTokenResponse>(CodeMessage._102, status: StatusEnum.Failed);
            }
        }

        public async Task<BaseResult<PayGwQrCusPaymentResponse>> QrCusPaymentAsync(QrCusPaymentRequest request, CancellationToken cancellationToken, string token)
        {
            var _httpClient = new HttpClient();
            try
            {
                PayGwRequest<QrCusPaymentRequest> payload =
                    PayGwRequest<QrCusPaymentRequest>.Create(request, _merchantCode, _secretKey, _privateKey);

                string url = $"{_baseAddress}{Resources.PaymentGateway.QrCusPayment}";

                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                _httpClient.DefaultRequestHeaders.Add("merchantCode", "IACV");
                _httpClient.DefaultRequestHeaders.Add("messageType", "token");
                _httpClient.DefaultRequestHeaders.Add("lang", "vi");
                _httpClient.DefaultRequestHeaders.Add("version", "1.0.0");
                _httpClient.DefaultRequestHeaders.Add("clientIp", "127.0.0.1");
                return await PostAsync<PayGwQrCusPaymentResponse>(url, payload, cancellationToken, _httpClient);
            }
            catch (Exception)
            {
                return GetBaseResult<PayGwQrCusPaymentResponse>(CodeMessage._102, status: StatusEnum.Failed);
            }
        }

        public async Task<BaseResult<bool>> CreateOrderReturnUrlAsync(string epayResult, CancellationToken cancellationToken)
        {
            string epayResultJson = Encoding.UTF8.GetString(Convert.FromBase64String(epayResult));
            var payGwReturnUrlRequest = JsonConvert.DeserializeObject<ReturnUrlRequest>(epayResultJson);
            if (string.IsNullOrEmpty(payGwReturnUrlRequest?.TransCode))
                return GetBaseResult<bool>(CodeMessage._98, status: StatusEnum.Failed);

            var paymentLog = await _paymentLogDAO.GetByIdAsync(payGwReturnUrlRequest.OrderCode);

            // Validate data
            if (paymentLog == null)
                return GetBaseResult<bool>(CodeMessage._211, status: StatusEnum.Failed);


            paymentLog.partner_transaction_id = payGwReturnUrlRequest.TransCode;

            // Update payment status
            paymentLog.partner_payment_status = payGwReturnUrlRequest.TransStatus switch
            {
                TransStatus.NotPaid => TransStatus.Pending,
                TransStatus.Paid => TransStatus.Paid,
                TransStatus.Fail => TransStatus.Fail,
                TransStatus.Pending => TransStatus.Pending,
                TransStatus.UserCanceled => TransStatus.Fail,
                TransStatus.WaitPartnerProcess => TransStatus.Pending,
                _ => throw new ArgumentOutOfRangeException()
            };

            var updateResult = await _paymentLogDAO.UpdatePartnerStatusAsync(paymentLog);
            if (!updateResult.isSuccess)
            {
                return GetBaseResult<bool>(CodeMessage._99, status: StatusEnum.Failed);
            }

            await _unitOfWork.SaveChangesAsync();

            // Process result
            return GetBaseResult(CodeMessage._200, data: paymentLog.partner_payment_status == TransStatus.Paid ? true : false);
        }

        public async Task<BaseResult<bool>> CreateOrderCallbackAsync(PayGwResponse<IpnPayGatewayRequest> request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(request.Data))
            {
                return GetBaseResult<bool>(CodeMessage._98, status: StatusEnum.Failed);
            }
            var createOrderCallbackRequest = request.DecryptedData(_secretKey);

            if (string.IsNullOrEmpty(createOrderCallbackRequest?.TransCode))
                return GetBaseResult<bool>(CodeMessage._98, status: StatusEnum.Failed);

            var paidOrderResult = await _paymentLogDAO.GetPaidOrderAsync(createOrderCallbackRequest.OrderCode);
            var paymentLog = paidOrderResult.data;

            // Validate data
            if (paymentLog == null)
                return GetBaseResult<bool>(CodeMessage._211, status: StatusEnum.Failed);

            paymentLog.partner_transaction_id = createOrderCallbackRequest.TransCode;
            // Update payment status
            paymentLog.partner_payment_status = createOrderCallbackRequest.TransStatus switch
            {
                TransStatus.NotPaid => TransStatus.Pending,
                TransStatus.Paid => TransStatus.Paid,
                TransStatus.Fail => TransStatus.Fail,
                TransStatus.Pending => TransStatus.Pending,
                TransStatus.UserCanceled => TransStatus.Fail,
                TransStatus.WaitPartnerProcess => TransStatus.Pending,
                _ => throw new ArgumentOutOfRangeException()
            };

            var updateResult = await _paymentLogDAO.UpdatePartnerStatusAsync(paymentLog);
            if (!updateResult.isSuccess)
            {
                return GetBaseResult<bool>(CodeMessage._99, status: StatusEnum.Failed);
            }

            await _unitOfWork.SaveChangesAsync();

            var webSocketCreateOrderCallbackRequest = new
            {
                invoiceType = "Phí thành viên",
                invoiceCode = paymentLog.order_id,
                paymentMethod = paymentLog.payment_method == "04" ? "QRcode" : "Thanh toán qua thẻ ngân hàng",
                paymentCode = createOrderCallbackRequest.TransCode,
                paymentTime = DateTimeOffset.FromUnixTimeMilliseconds(createOrderCallbackRequest.TimeRequest).UtcDateTime,
                amount = paymentLog.new_total_amount,
                discount = 0,
                totalAmount = paymentLog.old_total_amount,
                status = paymentLog.partner_payment_status == TransStatus.Paid ? "Thành công" : "Thất bại"
            };

            var json = System.Text.Json.JsonSerializer.Serialize(webSocketCreateOrderCallbackRequest);
            var bytes = System.Text.Encoding.UTF8.GetBytes(json);

            foreach (var socket in _wsManagerCreateOrder.GetAllSockets())
            {
                if (socket.Key == paymentLog.device_id)
                {
                    if (socket.Value.State == WebSocketState.Open)
                    {
                        await socket.Value.SendAsync(
                            bytes,
                            WebSocketMessageType.Text,
                            endOfMessage: true,
                            cancellationToken: CancellationToken.None
                        );
                        break;
                    }
                }
            }

            // Process result
            return GetBaseResult(CodeMessage._200,
                data: paymentLog.partner_payment_status == TransStatus.Paid ? true : false);
        }

        public async Task<BaseResult<bool>> RefundCallbackAsync(PayGwResponse<IpnPayGatewayRefundRequest> request, CancellationToken cancellationToken)
        {
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

            paymentLog.partner_transaction_id = createOrderCallbackRequest.TransCode;

            // Update payment status
            paymentLog.partner_payment_status = createOrderCallbackRequest.TransStatus switch
            {
                TransStatus.NotPaid => TransStatus.Pending,
                TransStatus.Paid => TransStatus.Paid,
                TransStatus.Fail => TransStatus.Fail,
                TransStatus.Pending => TransStatus.Pending,
                TransStatus.UserCanceled => TransStatus.Fail,
                TransStatus.WaitPartnerProcess => TransStatus.Pending,
                _ => throw new ArgumentOutOfRangeException()
            };

            var updateResult = await _paymentLogDAO.UpdatePartnerStatusAsync(paymentLog);
            if (!updateResult.isSuccess)
            {
                return GetBaseResult<bool>(CodeMessage._99, status: StatusEnum.Failed);
            }

            await _unitOfWork.SaveChangesAsync();

            var json = System.Text.Json.JsonSerializer.Serialize(createOrderCallbackRequest);
            var bytes = System.Text.Encoding.UTF8.GetBytes(json);

            foreach (var socket in _wsManagerRefund.GetAllSockets())
            {
                if (socket.Key == paymentLog.device_id)
                {
                    if (socket.Value.State == WebSocketState.Open)
                    {
                        await socket.Value.SendAsync(
                            bytes,
                            WebSocketMessageType.Text,
                            endOfMessage: true,
                            cancellationToken: CancellationToken.None
                        );
                        break;
                    }
                }
            }

            // Process result
            return GetBaseResult(CodeMessage._200,
                data: paymentLog.partner_payment_status == TransStatus.NotPaid ? true : false);
        }
    }
}
