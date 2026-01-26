using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace BgCommon;

public class ImageHelper
{
    /// <summary>
    /// 异步加载本地图片（跨线程安全）
    /// </summary>
    /// <param name="filePath">图片文件路径</param>
    /// <returns>返回BitmapImage示例</returns>
    public static async Task<BitmapImage?> GetImageAsync(string filePath)
    {
        BitmapImage? bitmap = null;
        try
        {
            // 在后台线程处理图像
            bitmap = await Task.Run(() =>
            {
                // 异步获取图片字节数据
                byte[] imageData = File.ReadAllBytes(filePath);

                using (var memStream = new MemoryStream(imageData))
                {
                    var image = new BitmapImage();
                    image.BeginInit();
                    image.CacheOption = BitmapCacheOption.OnLoad; // 确保立即缓存
                    image.StreamSource = memStream;
                    image.EndInit();
                    image.Freeze(); // 冻结对象，使其跨线程安全
                    return image;
                }
            });
        }
        catch (Exception)
        {
            // 处理异常（无效图像等）
            throw;
        }

        return bitmap;
    }

    /// <summary>
    /// 异步加载本地图片（跨线程安全）
    /// </summary>
    /// <param name="filePath">图片文件路径</param>
    /// <param name="cancellationToken">取消</param>
    /// <param name="progress">报告进度</param>
    /// <returns>返回BitmapImage示例</returns>
    public static async Task<BitmapImage?> LoadImageAsync(string filePath, CancellationToken cancellationToken, IProgress<int>? progress = null)
    {
        BitmapImage? bitmap = null;
        try
        {
            // 在后台线程处理图像
            bitmap = await Task.Run(
                async () =>
                {
                    // 异步获取图片字节数据
                    byte[] imageData = await File.ReadAllBytesAsync(filePath);
                    using (var memStream = new MemoryStream(imageData))
                    {
                        memStream.Position = 0;
                        var image = new BitmapImage();
                        image.BeginInit();
                        image.CacheOption = BitmapCacheOption.OnLoad; // 确保立即缓存
                        image.StreamSource = memStream;
                        image.EndInit();
                        image.Freeze(); // 冻结对象，使其跨线程安全
                        return image;
                    }
                }, cancellationToken);
        }
        catch (Exception)
        {
            throw;
        }

        return bitmap;
    }
}