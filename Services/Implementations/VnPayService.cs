using OnlineShopping.Services.Interfaces;
using ShoppingOnline.Models;
using System.Security.Cryptography;
using System.Text;

public class VnPayService : IVnPayService
{
    private readonly IConfiguration _config;

    public VnPayService(IConfiguration config)
    {
        _config = config;
    }

    public string CreatePaymentUrl(HttpContext context, Order order)
    {
        var vnp = new SortedList<string, string>();

        // Basic info
        vnp.Add("vnp_Version", "2.1.0");
        vnp.Add("vnp_Command", "pay");
        vnp.Add("vnp_TmnCode", _config["VnPay:TmnCode"]);

        // Amount (x100)
        vnp.Add("vnp_Amount", ((long)(order.TotalAmount * 100)).ToString());

        vnp.Add("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss"));
        vnp.Add("vnp_CurrCode", "VND");

        // IP (fix IPv6)
        var ipAddress = context.Connection.RemoteIpAddress?.ToString();
        if (string.IsNullOrEmpty(ipAddress) || ipAddress == "::1")
        {
            ipAddress = "127.0.0.1";
        }
        vnp.Add("vnp_IpAddr", ipAddress);

        vnp.Add("vnp_Locale", "vn");

        // 🔥 FIX: tránh space gây lỗi hash
        vnp.Add("vnp_OrderInfo", $"Thanh_toan_don_hang_{order.Id}");

        vnp.Add("vnp_OrderType", "other");

        // Return URL
        vnp.Add("vnp_ReturnUrl", _config["VnPay:ReturnUrl"]);

        //// 🔥 (khuyến nghị thêm IPN)
        //vnp.Add("vnp_IpnUrl", _config["VnPay:IpnUrl"]);

        // Unique transaction
        vnp.Add("vnp_TxnRef", order.Id.ToString());

        var sortedParams = new SortedList<string, string>(vnp);

        // =========================
        // 🔴 RAW DATA (SIGN DATA)
        // =========================
        string signData = string.Join("&",
     sortedParams.Select(kvp => $"{kvp.Key}={Uri.EscapeDataString(kvp.Value)}"));

        Console.WriteLine("========= SIGN DATA =========");
        Console.WriteLine(signData);
        Console.WriteLine("========= END SIGN DATA =========");

        // =========================
        // 🟢 ENCODE QUERY
        // =========================
        string query = string.Join("&",
            sortedParams.Select(kvp => $"{kvp.Key}={Uri.EscapeDataString(kvp.Value)}"));

        // =========================
        // 🔐 HASH
        // =========================
        string hash = HmacSHA512(_config["VnPay:HashSecret"], signData);

        Console.WriteLine("========= HASH =========");
        Console.WriteLine(hash);
        Console.WriteLine("========= END HASH =========");

        // =========================
        // FINAL URL
        // =========================
        var paymentUrl = _config["VnPay:BaseUrl"]
            + "?" + query
            + "&vnp_SecureHashType=HmacSHA512"
            + "&vnp_SecureHash=" + hash;

        Console.WriteLine("========= PAYMENT URL =========");
        Console.WriteLine(paymentUrl);
        Console.WriteLine("========= END PAYMENT URL =========");

        return paymentUrl;
    }

    public bool ValidateSignature(IQueryCollection query)
    {
        var hashSecret = _config["VnPay:HashSecret"];

        var vnpData = query
            .Where(k => k.Key.StartsWith("vnp_")
                && k.Key != "vnp_SecureHash"
                && k.Key != "vnp_SecureHashType")
            .ToDictionary(k => k.Key, v => v.Value.ToString());

        var sorted = new SortedList<string, string>(vnpData);

        string signData = string.Join("&",
            sorted.Select(kvp => $"{kvp.Key}={kvp.Value}"));

        string hash = HmacSHA512(hashSecret, signData);

        return hash.Equals(query["vnp_SecureHash"], StringComparison.InvariantCultureIgnoreCase);
    }

    private string HmacSHA512(string key, string inputData)
    {
        var hash = new StringBuilder();

        byte[] keyBytes = Encoding.UTF8.GetBytes(key);
        byte[] inputBytes = Encoding.UTF8.GetBytes(inputData);

        using (var hmac = new HMACSHA512(keyBytes))
        {
            byte[] hashValue = hmac.ComputeHash(inputBytes);

            foreach (var b in hashValue)
            {
                hash.Append(b.ToString("x2"));
            }
        }

        return hash.ToString();
    }
}