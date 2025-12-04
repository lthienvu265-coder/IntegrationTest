using ETC.EPAY.Integration.Backend;
using ETC.EPAY.Integration.DataAccess;
using ETC.EPAY.Integration.DataAccess.UnitOfWork;
using ETC.EPAY.Integration.Services.Payment;
using ETC.EPAY.Integration.Services.PaymentGateway;
using Microsoft.Extensions.Configuration;
using System.Net.WebSockets;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddSingleton<WebSocketCreateOrderConnectionManager>();
builder.Services.AddSingleton<WebSocketRefundConnectionManager>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<IPaymentLogDAO, PaymentLogDAO>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddScoped<IPayGwService>(provider =>
{
    var baseAddress = "https://payment-gw-core-uat.epayservices.com.vn";
    var merchantCode = "IACV";
    var merchantUser = "IACV";
    var merchantPassword = "123456";
    var privateKey = "MIIEvAIBADANBgkqhkiG9w0BAQEFAASCBKYwggSiAgEAAoIBAQDWy27T19I1slRsJDV5niuBTdLKjesE4VYgVzKWj2TmqMRRxWHVqRvptx/9IFMSGzf7Mib9aZuOrHg9s7mHsCBJkLaqabhU/zgHegU5NJUzC28XqBDOvJcok3WdJ64EdZFw3mpM7sLN5+DGfcFgoeRhE2t/SmFh3oRPMqSf3D30Dc0NC9LkRNOAR+5VdvoAWyZCRKpvDMTd+fHgCPYX8jr+tSOEvGGjJvhORO4XDQpYTE7tJfBfK6FeTmHhJ2Y26Q3H4WiHzKFnW7AEyAs8b6G+4YlijEXlAs5M9Gf6psmXx5BI2oDdxPF0UKOHON1ravUpWXWTCvXnYmuXfoSjzOTxAgMBAAECggEARhEtkYBlK6wFGxPCt+4PTgpXeUjLQhfJsxoOwAt9xDqIdA45yGt2VgqtLjLVtmHXpKMrEd5Z5rQ/Mw2GN4uONME2vSdogJ2Spum4e306MGEJ98MU3IOroYpcwwu3GfBKJfH/5iG6vVVnPwi1xLJ63+noVqkqVDU0nusrxZQgR5SBnGz6la6rkYaVeXWQlbgapgWyST3y27Zr6KWFHLBL6B9pV7DSiKtoUsIOE2k6LIdE5oJ83JZL/K3OzLY4Ci0ahLlPDmD8oNCa+WZ0Us16tO93XnUAx5EHZMPplEs11pRv88ZZ/gfumITNyxWko9b2Gf5UXH9Pb14Y3350xA7XuwKBgQD6lM32Hy1R5NyOKuDuXJzBUX0xrdWcEUFeXhiaWAPiMTyjP2hJjKSr2PwCJnrAyjHbm/Pe3azry2O3TOfvatioy+Hi6XAmpl2e42up9BLiuH0aJfV3xKD2EFrY1YZwZ0ATeZscSNuHDwJo9SLY+hE2QcArCml5ul+2mXldGlc6nwKBgQDbcIRVQ0oh0EYRkfsRrWxHnPVg+U1Jje233iaI2xESfQuP5wnDx8/yxyzIgEyr9CVdv1uRbJLHJBgAqiMosZkttJnzG0Y2arx7+uMdtm+ELr0jmYgmwAGtQA0FRDcOhi/EUdJQCNtyFwvlwWwsGNa4RvpJm5WpMT/MfEVY8C1GbwKBgFBwYEV38KZvX1XL8fTSigeMzzGZag71gxR2BFPXmTeNMrf87M9fdKUtvIg8Lf+haKvkvj52zsHKwrHe4D19ARv+hv6+jR/7G+abfr5J+Z4KNy6jrM7LshgLW/5rXQLMQkd+LArCYKZUbSOgPZWFgOz34mZuqzlGWN9XGRyIDiRlAoGANR1T+qtR60NVYnVaNxoqBl94iWNukKo46vNrkl6sEDSSMt+yXmAj9li9fz6G0GxFCx8BU/7avDcBVE/aOMRcFlMLnGtyoENPohbFGHELJwyFHXPRH8gD3+KMBEVRqkNSlotTg6nRLBll+NPLkhTlDC+p710oaY6RKba+tltUKiMCgYBB5tiLLd9EGBpdeKMURJ/HygT0sTIEdXxMRCfM1tuLwlMFE613ZQ4rZ9BDpi7uyVtj6iY54ddXtAfccpN8VCyq6GVbCfHCaxMTHHVTQdSYkkPWCTQTI7TKEyLL5CLVfEDIWHOiyV5XvNVok0XMo1L1Rf50p103gGo5bnqV0vCUbg==";
    var secretKey = "0123456789ABCDEF76543210";

    var paymentLogDAO = provider.GetRequiredService<IPaymentLogDAO>();
    var unitOfWork = provider.GetRequiredService<IUnitOfWork>();

    var webSocketCreateOrderConnectionManager = provider.GetRequiredService<WebSocketCreateOrderConnectionManager>();
    var webSocketRefundConnectionManager = provider.GetRequiredService<WebSocketRefundConnectionManager>();


    return new PayGwService(
        baseAddress,
        merchantCode,
        merchantUser,
        merchantPassword,
        privateKey,
        secretKey,
        paymentLogDAO,
        unitOfWork,
        webSocketCreateOrderConnectionManager,
        webSocketRefundConnectionManager
    );
});

builder.Services.AddScoped<IUnitOfWorkContext>(provider =>
{
    var cs = "Host=10.0.229.177;Port=5432;Database=integration;Username=iacv;Password=Etc@1234;";
    return new UnitOfWorkContext(cs);
});

builder.Services.AddOpenApi();
builder.WebHost.UseUrls("http://0.0.0.0:5006");

var app = builder.Build();

app.UseWebSockets();
app.Map("/ws_createorder", async (HttpContext context, WebSocketCreateOrderConnectionManager manager) =>
{
    var query = context.Request.Query;
    string deviceId = query["deviceId"];
    string model = query["model"];

    if (string.IsNullOrEmpty(deviceId))
        deviceId = Guid.NewGuid().ToString(); // fallback

    if (context.WebSockets.IsWebSocketRequest)
    {
        var socket = await context.WebSockets.AcceptWebSocketAsync();

        manager.AddSocket(deviceId, socket);

        Console.WriteLine($"Client Create Order connected: {deviceId}, Model: {model}");

        var buffer = new byte[1024];
        while (socket.State == WebSocketState.Open)
        {
            var result = await socket.ReceiveAsync(buffer, CancellationToken.None);

            if (result.MessageType == WebSocketMessageType.Close)
                await manager.RemoveSocketAsync(deviceId);
        }
    }
});

app.Map("/ws_refund", async (HttpContext context, WebSocketRefundConnectionManager manager) =>
{
    var query = context.Request.Query;
    string deviceId = query["deviceId"];
    string model = query["model"];

    if (string.IsNullOrEmpty(deviceId))
        deviceId = Guid.NewGuid().ToString(); // fallback

    if (context.WebSockets.IsWebSocketRequest)
    {
        var socket = await context.WebSockets.AcceptWebSocketAsync();

        manager.AddSocket(deviceId, socket);

        Console.WriteLine($"Client Refund connected: {deviceId}, Model: {model}");

        var buffer = new byte[1024];
        while (socket.State == WebSocketState.Open)
        {
            var result = await socket.ReceiveAsync(buffer, CancellationToken.None);

            if (result.MessageType == WebSocketMessageType.Close)
                await manager.RemoveSocketAsync(deviceId);
        }
    }
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
app.UseAuthorization();

app.MapControllers();

app.Run();
