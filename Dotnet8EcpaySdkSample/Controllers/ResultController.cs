using FluentEcpay;
using Microsoft.AspNetCore.Mvc;

namespace Dotnet8EcpaySdkSample.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class ResultController : ControllerBase
{
    private readonly string _hashKey;
    private readonly string _hashIV;
    
    public ResultController(IConfiguration configuration)
    {
        _hashKey = configuration["EcpayHashKey"];
        _hashIV = configuration["EcpayHashIV"];
    }
    public IActionResult Callback(PaymentResult result)
    {
        // 務必判斷檢查碼是否正確。
        if (!CheckMac.PaymentResultIsValid(result, _hashKey, _hashIV)) return BadRequest();

        // 處理後續訂單狀態的更動等等...。

        return Ok("1|OK");
    }
}