using FluentEcpay;
using Microsoft.AspNetCore.Mvc;

namespace Dotnet8EcpaySdkSample.Controllers;
public class PaymentController : Controller
{
    private readonly string _merchantId;
    private readonly string _hashKey;
    private readonly string _hashIV;
    private readonly string _domainUrl;
    
    public PaymentController(IConfiguration configuration)
    {
        _merchantId = configuration["EcpayMerchantId"];
        _hashKey = configuration["EcpayHashKey"];
        _hashIV = configuration["EcpayHashIV"];
        _domainUrl = configuration["DomainUrl"];
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult CreateOrder()
    {
        // ... 建立訂單等相關資訊
        return RedirectToAction("CheckOut");
    }

    public IActionResult CheckOut()
    {
        var service = new
        {
            Url = "https://payment-stage.ecpay.com.tw/Cashier/AioCheckOut/V5",
            MerchantId = _merchantId,
            HashKey = _hashKey,
            HashIV = _hashIV,
            ServerUrl = $"{_domainUrl}/api/payment/callback",
            ClientUrl = $"{_domainUrl}/payment/success"
        };
        
        // ...根據已建立訂單撈取訂單、明細資訊
        var transaction = new
        {
            No = "test00003",
            Description = "測試購物系統",
            Date = DateTime.Now,
            Method = EPaymentMethod.Credit,
            Items = new List<Item>
            {
                new Item
                {
                    Name = "手機",
                    Price = 14000,
                    Quantity = 2
                },
                new Item
                {
                    Name = "隨身碟",
                    Price = 900,
                    Quantity = 10
                }
            }
        };
        IPayment payment = new PaymentConfiguration()
            .Send.ToApi(
                url: service.Url)
            .Send.ToMerchant(
                service.MerchantId)
            .Send.UsingHash(
                key: service.HashKey,
                iv: service.HashIV)
            .Return.ToServer(
                url: service.ServerUrl)
            .Return.ToClient(
                url: service.ClientUrl)
            .Transaction.New(
                no: transaction.No,
                description: transaction.Description,
                date: transaction.Date)
            .Transaction.UseMethod(
                method: transaction.Method)
            .Transaction.WithItems(
                items: transaction.Items)
            .Generate();

        return View(payment);
    }

    public IActionResult Success()
    {
        return View();
    }
}