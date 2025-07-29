using System;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace BgControls.Controls;

/// <summary>
/// 一个基于WriteableBitmap的高性能星空动画控件.
/// </summary>
public class FastStarrySky : Image
{
    public static readonly DependencyProperty NumberOfStarsProperty =
        DependencyProperty.Register(nameof(NumberOfStars), typeof(int), typeof(FastStarrySky), new PropertyMetadata(150, OnParameterChanged));

    public static readonly DependencyProperty MaxLineDistanceProperty =
        DependencyProperty.Register(nameof(MaxLineDistance), typeof(double), typeof(FastStarrySky), new PropertyMetadata(120.0, OnParameterChanged));

    public static readonly DependencyProperty StarSizeProperty =
        DependencyProperty.Register(nameof(StarSize), typeof(double), typeof(FastStarrySky), new PropertyMetadata(8.0d, OnParameterChanged));

    public static readonly DependencyProperty LineThicknessProperty =
       DependencyProperty.Register(nameof(LineThickness), typeof(double), typeof(FastStarrySky), new PropertyMetadata(1.0d, OnParameterChanged));

    // 定义一个内部类来存储单个星星的信息
    private class Star
    {
        public float X { get; set; }

        public float Y { get; set; } // 位置

        public float VelX { get; set; }

        public float VelY { get; set; } // 速度

        public float Radius { get; set; } // 半径

        public float Color { get; set; } // 颜色
    }

    // 定义常量和颜色
    private const float StarSpeed = 0.5f;
    private readonly Color lineColor = Colors.White;
    private readonly Color backgroundColor = Colors.Black;
    private readonly Random random = new(); // 用于生成随机数
    private List<Star>? stars; // 存储所有星星的列表
    private WriteableBitmap? writeableBitmap; // 可写入的位图，我们的“画布”
    private byte[]? pixelBuffer; // 像素数据缓冲区，直接操作这块内存
    private int width; // 画布的宽（一行像素占用的字节数）
    private int height; // 画布的高（一行像素占用的字节数）
    private int stride; // 画布的跨距（一行像素占用的字节数）
    private int frameNo = 0;

    private volatile bool isInitialized = false; // 关键的标志位，用于判断是否已成功初始化
    private volatile bool needsReinitialization = true; // 用于请求重新初始化

    // 用于存储预先计算好的辉光笔刷
    private byte[]? starBrush;
    private int starBrushSize;
    private int starBrushRadius;

    public FastStarrySky(Random random)
    {
        this.random = random;
    }

    public FastStarrySky()
    {
        this.Stretch = Stretch.Fill; // 让图像填充整个控件区域
        this.HorizontalAlignment = HorizontalAlignment.Stretch;
        this.VerticalAlignment = VerticalAlignment.Stretch;
        this.Loaded += OnLoaded;
        this.Unloaded += OnUnloaded;
    }

    /// <summary>
    /// Gets or sets  获取或设置要渲染的星星数量.
    /// </summary>
    public int NumberOfStars
    {
        get => (int)GetValue(NumberOfStarsProperty);
        set => SetValue(NumberOfStarsProperty, value);
    }

    /// <summary>
    /// Gets or sets 获取或设置星星之间绘制连线的最大距离.
    /// </summary>
    public double MaxLineDistance
    {
        get => (double)GetValue(MaxLineDistanceProperty);
        set => SetValue(MaxLineDistanceProperty, value);
    }

    /// <summary>
    /// Gets or sets 获取或设置星星的亮度/大小 (推荐值 1.0 - 3.0)。
    /// </summary>
    public double StarSize
    {
        get => (double)GetValue(StarSizeProperty);
        set => SetValue(StarSizeProperty, value);
    }

    /// <summary>
    /// Gets or sets 获取或设置线条的视觉粗细 (推荐值 0.1 - 1.0)。
    /// </summary>
    public double LineThickness
    {
        get => (double)GetValue(LineThicknessProperty);
        set => SetValue(LineThicknessProperty, value);
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        // 当控件加载到可视化树中时，订阅必要的事件
        this.SizeChanged += OnSizeChanged;
        CompositionTarget.Rendering += OnRendering;
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        // 当控件从可视化树中移除时，必须取消订阅，防止内存泄漏
        this.SizeChanged -= OnSizeChanged;
        CompositionTarget.Rendering -= OnRendering;

        // 清理重量级资源，帮助GC回收
        writeableBitmap = null;
        pixelBuffer = null;
        stars = null;
    }

    // 当窗口大小改变时触发
    private void OnSizeChanged(object sender, SizeChangedEventArgs e) => RequestReinitialization();

    private static void OnParameterChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => (d as FastStarrySky)?.RequestReinitialization();

    private void RequestReinitialization() => needsReinitialization = true;

    /// <summary>
    /// 初始化或重新初始化所有渲染资源.
    /// </summary>
    public void Initialize(int width, int height)
    {
        // 获取控件当前的实际尺寸
        this.width = width; // (int)this.ActualWidth;
        this.height = height; // (int)this.ActualHeight;

        RequestReinitialization();

        this.Width = width;
        this.Height = height;

        // 创建可写入位图和像素缓冲区
        writeableBitmap = new WriteableBitmap(width, height, 96, 96, PixelFormats.Pbgra32, null);
        stride = writeableBitmap.BackBufferStride;
        pixelBuffer = new byte[height * stride];
        this.Source = writeableBitmap; // 将Image控件的源设置为我们的位图

        // 根据参数创建一堆新的星星
        stars = new List<Star>(NumberOfStars);
        for (int i = 0; i < NumberOfStars; i++)
        {
            stars.Add(new Star
            {
                X = random.Next(0, width),
                Y = random.Next(0, height),
                VelX = (float)(random.NextDouble() - 0.5) * StarSpeed,
                VelY = (float)(random.NextDouble() - 0.5) * StarSpeed,
                Radius = 200 + (800 * (float)random.NextDouble()),
                Color = ((200 + (800 * (float)random.NextDouble())) / 1000) + (frameNo / 250.0f)
            });

            frameNo++;
        }

        CreateStarBrush();

        // 所有资源都已成功创建，将标志位置为 true
        isInitialized = true;
    }

    /// <summary>
    /// 线程安全的初始化方法。
    /// </summary>
    private void InitializeInternal()
    {
        // 如果控件还没有获得有效尺寸，则初始化失败，直接返回
        if (width <= 0 || height <= 0)
        {
            isInitialized = false;
            return;
        }

        // 双重检查锁定模式，确保只有一个线程能进行初始化
        if (isInitialized && !needsReinitialization)
        {
            return;
        }

        lock (random) // 使用一个已有的对象作为锁，避免创建新锁对象
        {
            if (!isInitialized || needsReinitialization)
            {
                var wb = new WriteableBitmap(width, height, 96, 96, PixelFormats.Pbgra32, null);
                var buffer = new byte[height * wb.BackBufferStride];
                var stars = new List<Star>(NumberOfStars);

                frameNo = 0;
                for (int i = 0; i < NumberOfStars; i++)
                {
                    stars.Add(new Star
                    {
                        X = random.Next(0, width),
                        Y = random.Next(0, height),
                        VelX = (float)(random.NextDouble() - 0.5) * StarSpeed,
                        VelY = (float)(random.NextDouble() - 0.5) * StarSpeed,
                        Radius = 200 + (800 * (float)random.NextDouble()),
                        Color = ((200 + (800 * (float)random.NextDouble())) / 1000) + (frameNo / 250.0f)
                    });

                    frameNo++;
                }

                CreateStarBrush();

                // 原子性地替换所有资源
                stride = wb.BackBufferStride;
                pixelBuffer = buffer;
                writeableBitmap = wb;
                this.stars = stars;
                _ = this.Dispatcher.Invoke(() => this.Source = writeableBitmap);

                needsReinitialization = false;
                isInitialized = true;
            }
        }
    }

    /// <summary>
    /// 根据 StarSize 属性，动态创建一个带有高斯衰减效果的辉光笔刷。
    /// </summary>
    private void CreateStarBrush()
    {
        // StarSize 现在直接控制笔刷的半径
        starBrushRadius = (int)Math.Max(1, StarSize);
        starBrushSize = (starBrushRadius * 2) + 1;
        starBrush = new byte[starBrushSize * starBrushSize * 4]; // 4 bytes per pixel (B,G,R,A)

        double falloff = 1.0 / starBrushRadius; // 衰减率

        for (int y = 0; y < starBrushSize; y++)
        {
            for (int x = 0; x < starBrushSize; x++)
            {
                double dx = x - starBrushRadius;
                double dy = y - starBrushRadius;
                double dist = Math.Sqrt((dx * dx) + (dy * dy));

                if (dist <= starBrushRadius)
                {
                    // 使用平滑的指数衰减 (ease-out)
                    double intensity = 1.0 - (dist * falloff);
                    intensity = intensity * intensity; // 平方使其更亮

                    byte alpha = (byte)(255 * intensity);

                    int index = ((y * starBrushSize) + x) * 4;
                    starBrush[index] = 255;      // B
                    starBrush[index + 1] = 255;  // G
                    starBrush[index + 2] = 255;  // R
                    starBrush[index + 3] = alpha; // A
                }
            }
        }
    }

    // 此事件与显示器刷新同步，是执行逐帧动画的最佳位置
    private void OnRendering(object? sender, EventArgs e)
    {
        // 如果初始化仍然失败（例如窗口被最小化，宽高为0），则跳过本帧的渲染
        if (needsReinitialization)
        {
            // 使用当前的实际尺寸进行初始化
            InitializeInternal();
        }

        if (!isInitialized)
        {
            return;
        }

        // 1. 清空上一帧的画面
        ClearBuffer();

        // 2. 更新星星位置并画出新的星星
        UpdateAndDrawStars();

        // 3. 根据星星之间的距离画出连线
        // DrawLines();
        DrawAntiAliasedLines();

        // 4. 将我们修改过的像素缓冲区数据，一次性更新到WriteableBitmap上
        if (writeableBitmap != null && pixelBuffer != null)
        {
            try
            {
                writeableBitmap.Lock(); // 锁定缓冲区以进行写入
                System.Runtime.InteropServices.Marshal.Copy(pixelBuffer, 0, writeableBitmap.BackBuffer, pixelBuffer.Length);
                writeableBitmap.AddDirtyRect(new Int32Rect(0, 0, width, height)); // 标记整个区域为“脏”，需要刷新
            }
            finally
            {
                writeableBitmap.Unlock(); // 必须解锁
            }
        }
    }

    /// <summary>
    /// 用背景色快速填充整个像素缓冲区.
    /// </summary>
    private void ClearBuffer()
    {
        ArgumentNullException.ThrowIfNull(pixelBuffer, nameof(pixelBuffer));

        int bufferLength = pixelBuffer.Length;
        byte colorB = backgroundColor.B;
        byte colorG = backgroundColor.G;
        byte colorR = backgroundColor.R;
        byte colorA = backgroundColor.A;

        for (int i = 0; i < bufferLength; i += 4)
        {
            pixelBuffer[i] = colorB;
            pixelBuffer[i + 1] = colorG;
            pixelBuffer[i + 2] = colorR;
            pixelBuffer[i + 3] = colorA;
        }
    }

    /// <summary>
    /// 更新所有星星的位置，并在新位置上绘制它们.
    /// </summary>
    private void UpdateAndDrawStars()
    {
        ArgumentNullException.ThrowIfNull(stars, nameof(stars));

        foreach (Star star in stars)
        {
            star.X += star.VelX;
            star.Y += star.VelY;

            // 边界检测（飞出屏幕的星星从另一边回来）
            if (star.X < 0)
            {
                star.X = width;
            }

            if (star.X > width)
            {
                star.X = 0;
            }

            if (star.Y < 0)
            {
                star.Y = height;
            }

            if (star.Y > height)
            {
                star.Y = 0;
            }

            // // 在新位置画一个像素点
            // DrawPixel((int)star.X, (int)star.Y, star.Color);
            // 使用新的方法绘制一个更亮的星星
            DrawStar((int)star.X, (int)star.Y, Rgb(star.Color));
        }
    }

    /// <summary>
    /// 绘制星星的方法.
    /// </summary>
    /// <param name="x">x坐标.</param>
    /// <param name="y">y坐标.</param>
    /// <param name="color">颜色.</param>
    private void DrawStar(int x, int y, Color color)
    {
        ArgumentNullException.ThrowIfNull(starBrush, nameof(starBrush));
        ArgumentNullException.ThrowIfNull(pixelBuffer, nameof(pixelBuffer));

        int startX = x - starBrushRadius;
        int startY = y - starBrushRadius;

        for (int brushY = 0; brushY < starBrushSize; brushY++)
        {
            for (int brushX = 0; brushX < starBrushSize; brushX++)
            {
                int canvasX = startX + brushX;
                int canvasY = startY + brushY;

                if (canvasX < 0 || canvasX >= width || canvasY < 0 || canvasY >= height)
                {
                    continue;
                }

                int brushIndex = ((brushY * starBrushSize) + brushX) * 4;
                byte brushAlpha = starBrush[brushIndex + 3];

                if (brushAlpha > 0)
                {
                    // 进行Alpha混合
                    int canvasIndex = (canvasY * stride) + (canvasX * 4);
                    byte bgB = pixelBuffer[canvasIndex];
                    byte bgG = pixelBuffer[canvasIndex + 1];
                    byte bgR = pixelBuffer[canvasIndex + 2];

                    float brushAlphaFactor = brushAlpha / 255.0f;
                    float starR = color.R / 255.0f;
                    float starG = color.G / 255.0f;
                    float starB = color.B / 255.0f;

                    pixelBuffer[canvasIndex] = (byte)((starB * brushAlpha) + (bgB * (255 - brushAlpha) / 255));
                    pixelBuffer[canvasIndex + 1] = (byte)((starG * brushAlpha) + (bgG * (255 - brushAlpha) / 255));
                    pixelBuffer[canvasIndex + 2] = (byte)((starR * brushAlpha) + (bgR * (255 - brushAlpha) / 255));
                    pixelBuffer[canvasIndex + 3] = 255; // 背景是不透明的，所以最终结果也是不透明
                }
            }
        }
    }

    /// <summary>
    /// 检查所有星星对之间的距离，如果足够近则绘制连线.
    /// </summary>
    private void DrawLines()
    {
        ArgumentNullException.ThrowIfNull(stars, nameof(stars));

        double maxDistSq = MaxLineDistance * MaxLineDistance;

        // LineThickness 现在控制线条的“粒子密度”和“亮度”
        double lineDensity = Math.Clamp(LineThickness, 0.1, 1.0);

        for (int i = 0; i < stars.Count; i++)
        {
            for (int j = i + 1; j < stars.Count; j++)
            {
                Star star1 = stars[i];
                Star star2 = stars[j];

                float dx = star1.X - star2.X;
                float dy = star1.Y - star2.Y;
                float distSq = (dx * dx) + (dy * dy);

                if (distSq < maxDistSq)
                {
                    // 1. 计算基础透明度，距离越近越亮
                    double opacityFactor = 1.0 - (distSq / maxDistSq);

                    // 基础Alpha值非常低，以匹配参考图的暗淡效果
                    byte baseAlpha = (byte)(50 * opacityFactor * opacityFactor);

                    // 如果太暗，直接跳过，提高性能
                    if (baseAlpha < 5)
                    {
                        continue;
                    }

                    // 2. 计算线上需要绘制的粒子数量
                    // 距离越长，粒子越多；LineThickness越大，粒子越密集
                    float distance = (float)Math.Sqrt(distSq);
                    int particleCount = (int)(distance / 4 * lineDensity);

                    // 至少保证2个粒子
                    if (particleCount < 2)
                    {
                        particleCount = 2;
                    }

                    // 3. 沿着两点间的矢量方向，插值绘制粒子
                    for (int p = 1; p < particleCount; p++)
                    {
                        float t = (float)p / particleCount; // 插值比例 (0.0 to 1.0)
                        int x = (int)(star1.X + ((star2.X - star1.X) * t));
                        int y = (int)(star1.Y + ((star2.Y - star1.Y) * t));

                        byte finalAlpha = baseAlpha;

                        if (finalAlpha > 0)
                        {
                            DrawPixel(x, y, Color.FromArgb(finalAlpha, lineColor.R, lineColor.G, lineColor.B));
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// 在指定的坐标绘制一个单像素点.
    /// </summary>
    private void DrawPixel(int x, int y, Color color, bool blend = false)
    {
        // 边界检查
        if (x < 0 || x >= width || y < 0 || y >= height)
        {
            return;
        }

        // 计算像素在缓冲区中的索引
        int index = (y * stride) + (x * 4);
        if (blend && color.A < 255)
        {
            byte bgB = pixelBuffer[index];
            byte bgG = pixelBuffer[index + 1];
            byte bgR = pixelBuffer[index + 2];
            float alphaFactor = color.A / 255f;

            pixelBuffer[index] = (byte)((color.B * alphaFactor) + (bgB * (1 - alphaFactor)));
            pixelBuffer[index + 1] = (byte)((color.G * alphaFactor) + (bgG * (1 - alphaFactor)));
            pixelBuffer[index + 2] = (byte)((color.R * alphaFactor) + (bgR * (1 - alphaFactor)));
            pixelBuffer[index + 3] = 255;
        }
        else
        {
            // 设置BGRA颜色值
            pixelBuffer[index] = color.B;
            pixelBuffer[index + 1] = color.G;
            pixelBuffer[index + 2] = color.R;
            pixelBuffer[index + 3] = color.A;
        }
    }

    /// <summary>
    /// 使用改进的抗锯齿算法绘制所有线条，以实现视觉上的纤细效果。
    /// </summary>
    private void DrawAntiAliasedLines()
    {
        double maxDistSq = MaxLineDistance * MaxLineDistance;

        // LineThickness 现在控制线条的最大亮度 (Alpha)
        double maxBrightness = 60 * Math.Clamp(LineThickness, 0.0, 1.0);

        if (maxBrightness < 1)
        {
            return;
        }

        foreach (Star star1 in stars)
        {
            // 只需检查距离 star1 更近的星星，避免重复计算
            // (此处为简化，仍使用完整循环，但实际项目中可用空间分割优化)
            foreach (Star star2 in stars)
            {
                if (star1 == star2)
                {
                    continue;
                }

                float dx = star1.X - star2.X;
                float dy = star1.Y - star2.Y;
                float distSq = (dx * dx) + (dy * dy);

                if (distSq < maxDistSq)
                {
                    double opacityFactor = 1.0 - (distSq / maxDistSq);
                    double brightness = maxBrightness * opacityFactor * opacityFactor;

                    DrawAALine((int)star1.X, (int)star1.Y, (int)star2.X, (int)star2.Y, brightness);
                }
            }
        }
    }

    /// <summary>
    /// 使用 Xiaolin Wu算法的思想绘制一条抗锯齿线。
    /// </summary>
    /// <param name="brightness">线条的亮度 (0-255)。</param>
    private void DrawAALine(float x0, float y0, float x1, float y1, double brightness)
    {
        bool steep = Math.Abs(y1 - y0) > Math.Abs(x1 - x0);
        if (steep)
        {
            Swap(ref x0, ref y0);
            Swap(ref x1, ref y1);
        }

        if (x0 > x1)
        {
            Swap(ref x0, ref x1);
            Swap(ref y0, ref y1);
        }

        float dx = x1 - x0;
        float dy = y1 - y0;
        float gradient = dy / dx;
        if (dx == 0.0)
        {
            gradient = 1.0f;
        }

        float xend = Round(x0);
        float yend = y0 + (gradient * (xend - x0));
        float xgap = 1 - Frac(x0 + 0.5f);
        float xpxl1 = xend;
        float ypxl1 = Floor(yend);

        if (steep)
        {
            Plot(ypxl1, xpxl1, (1 - Frac(yend)) * xgap, brightness);
            Plot(ypxl1 + 1, xpxl1, Frac(yend) * xgap, brightness);
        }
        else
        {
            Plot(xpxl1, ypxl1, (1 - Frac(yend)) * xgap, brightness);
            Plot(xpxl1, ypxl1 + 1, Frac(yend) * xgap, brightness);
        }

        float intery = yend + gradient;

        xend = Round(x1);
        yend = y1 + (gradient * (xend - x1));
        xgap = Frac(x1 + 0.5f);
        float xpxl2 = xend;
        float ypxl2 = Floor(yend);

        if (steep)
        {
            Plot(ypxl2, xpxl2, (1 - Frac(yend)) * xgap, brightness);
            Plot(ypxl2 + 1, xpxl2, Frac(yend) * xgap, brightness);
        }
        else
        {
            Plot(xpxl2, ypxl2, (1 - Frac(yend)) * xgap, brightness);
            Plot(xpxl2, ypxl2 + 1, Frac(yend) * xgap, brightness);
        }

        if (steep)
        {
            for (float x = xpxl1 + 1; x <= xpxl2 - 1; x++)
            {
                Plot(Floor(intery), x, 1 - Frac(intery), brightness);
                Plot(Floor(intery) + 1, x, Frac(intery), brightness);
                intery += gradient;
            }
        }
        else
        {
            for (float x = xpxl1 + 1; x <= xpxl2 - 1; x++)
            {
                Plot(x, Floor(intery), 1 - Frac(intery), brightness);
                Plot(x, Floor(intery) + 1, Frac(intery), brightness);
                intery += gradient;
            }
        }
    }

    // 绘制一个带亮度的点
    private void Plot(float x, float y, float coverage, double brightness)
    {
        byte alpha = (byte)(brightness * coverage);
        DrawPixel((int)x, (int)y, Color.FromArgb(alpha, lineColor.R, lineColor.G, lineColor.B), true);
    }

    private void Swap<T>(ref T a, ref T b)
    {
        T temp = a;
        a = b;
        b = temp;
    }

    private float Frac(float x) => x - (float)Math.Floor(x);

    private float Round(float x) => (float)Math.Round(x);

    private float Floor(float x) => (float)Math.Floor(x);

    private Color Rgb(float col)
    {
        col += 0.000001f;
        byte r = (byte)((0.5 + (Math.Sin(col) * 0.5)) * 255);
        byte g = (byte)((0.5 + (Math.Cos(col) * 0.5)) * 255);
        byte b = (byte)((0.5 - (Math.Sin(col) * 0.5)) * 255);
        return Color.FromArgb(255, r, g, b);
    }
}