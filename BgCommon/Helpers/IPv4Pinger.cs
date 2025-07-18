namespace BgCommon.Helpers;

public class IPv4Pinger
{
    /// <summary>
    /// 校验 IPv4 地址合法性
    /// </summary>
    /// <param name="ipAddress">待校验的IP地址字符串</param>
    /// <returns>校验结果</returns>
    public static bool ValidateIPv4(string ipAddress)
    {
        // 使用 .NET 内置方法解析IP地址
        if (IPAddress.TryParse(ipAddress, out IPAddress? parsedAddress))
        {
            // 确认是 IPv4 地址（排除 IPv6 和其他类型）
            return parsedAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork;
        }

        return false;
    }

    /// <summary>
    /// 执行 Ping 操作
    /// </summary>
    /// <param name="ipAddress">目标 IPv4 地址</param>
    /// <param name="timeout">超时时间（毫秒，默认5000ms）</param>
    /// <returns>Ping 结果字符串</returns>
    public static async Task<string> PingAsync(string ipAddress, int timeout = 5000)
    {
        // 参数校验
        if (!ValidateIPv4(ipAddress))
        {
            return $"Invalid IPv4 address: {ipAddress}";
        }

        try
        {
            using Ping ping = new Ping();
            PingReply reply = await ping.SendPingAsync(ipAddress, timeout);

            // 解析 Ping 结果
            return reply.Status switch
            {
                IPStatus.Success =>
                    $"Ping succeeded!\n" +
                    $"Address: {reply.Address}\n" +
                    $"Roundtrip: {reply.RoundtripTime}ms\n" +
                    $"TTL: {reply.Options?.Ttl ?? 128}",

                // 其他状态处理
                _ => $"Ping failed! Status: {reply.Status}"
            };
        }
        catch (PingException ex)
        {
            // 处理 Ping 特定异常
            return $"Ping error: {ex.InnerException?.Message ?? ex.Message}";
        }
        catch (Exception ex)
        {
            // 处理其他异常
            return $"General error: {ex.Message}";
        }
    }

    /// <summary>
    /// 执行并打印 Ping 结果
    /// </summary>
    public static async Task ExecutePing(string ipAddress)
    {
        Debug.WriteLine($"Pinging {ipAddress}...");
        string result = await PingAsync(ipAddress);
        Debug.WriteLine(result);
        Debug.WriteLine(new string('-', 50));
    }
}