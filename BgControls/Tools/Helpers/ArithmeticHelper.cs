namespace BgControls.Tools.Helpers;

/// <summary>
/// 包含内部使用的一些简单算法.
/// </summary>
internal class ArithmeticHelper
{
    /// <summary>
    /// 平分一个整数到一个数组中.
    /// 例如，将 num = 10平均分配到 count = 3个元素中，结果为[4,3,3].
    /// </summary>
    /// <param name="num">要分配的整数.</param>
    /// <param name="count">分配的份数.</param>
    /// <returns>分配结果数组，每个元素为分得的整数.</returns>
    public static int[] DivideInt2Arr(int num, int count)
    {
        var arr = new int[count];
        var div = num / count;
        var rest = num % count;
        for (var i = 0; i < count; i++)
        {
            arr[i] = div;
        }

        for (var i = 0; i < rest; i++)
        {
            arr[i] += 1;
        }

        return arr;
    }

    /// <summary>
    /// 计算控件在窗口中的可见坐标.
    /// 保证控件不会超出屏幕工作区.
    /// </summary>
    /// <param name="element">参考的控件.</param>
    /// <param name="showElement">需要显示的控件.</param>
    /// <param name="thickness">额外的边距.</param>
    /// <returns>可见区域内的坐标点.</returns>
    public static Point CalSafePoint(FrameworkElement element, FrameworkElement showElement, Thickness thickness = default)
    {
        if (element == null || showElement == null)
        {
            return default;
        }

        Point point = element.PointToScreen(new Point(0, 0));
        if (point.X < 0)
        {
            point.X = 0;
        }

        if (point.Y < 0)
        {
            point.Y = 0;
        }

        var maxLeft = SystemParameters.WorkArea.Width -
                      ((double.IsNaN(showElement.Width) ? showElement.ActualWidth : showElement.Width) +
                       thickness.Left + thickness.Right);
        var maxTop = SystemParameters.WorkArea.Height -
                     ((double.IsNaN(showElement.Height) ? showElement.ActualHeight : showElement.Height) +
                      thickness.Top + thickness.Bottom);
        return new Point(maxLeft > point.X ? point.X : maxLeft, maxTop > point.Y ? point.Y : maxTop);
    }

    /// <summary>
    /// 获取布局范围框.
    /// 根据控件的实际尺寸、对齐方式和外边距计算其在父容器中的布局矩形.
    /// </summary>
    /// <param name="element">需要计算的控件.</param>
    /// <returns>控件的布局矩形.</returns>
    public static Rect GetLayoutRect(FrameworkElement element)
    {
        var num1 = element.ActualWidth;
        var num2 = element.ActualHeight;
        if (element is Image || element is MediaElement)
        {
            if (element.Parent is Canvas)
            {
                num1 = double.IsNaN(element.Width) ? num1 : element.Width;
                num2 = double.IsNaN(element.Height) ? num2 : element.Height;
            }
            else
            {
                num1 = element.RenderSize.Width;
                num2 = element.RenderSize.Height;
            }
        }

        var width = element.Visibility == Visibility.Collapsed ? 0.0 : num1;
        var height = element.Visibility == Visibility.Collapsed ? 0.0 : num2;
        Thickness margin = element.Margin;
        Rect layoutSlot = LayoutInformation.GetLayoutSlot(element);
        var x = 0.0;
        var y = 0.0;
        x = element.HorizontalAlignment switch
        {
            HorizontalAlignment.Left => layoutSlot.Left + margin.Left,
            HorizontalAlignment.Center => ((layoutSlot.Left + margin.Left + layoutSlot.Right - margin.Right) / 2.0) -
                                          (width / 2.0),
            HorizontalAlignment.Right => layoutSlot.Right - margin.Right - width,
            HorizontalAlignment.Stretch => Math.Max(
                layoutSlot.Left + margin.Left,
                ((layoutSlot.Left + margin.Left + layoutSlot.Right - margin.Right) / 2.0) - (width / 2.0)),
            _ => x
        };

        y = element.VerticalAlignment switch
        {
            VerticalAlignment.Top => layoutSlot.Top + margin.Top,
            VerticalAlignment.Center => ((layoutSlot.Top + margin.Top + layoutSlot.Bottom - margin.Bottom) / 2.0) -
                                        (height / 2.0),
            VerticalAlignment.Bottom => layoutSlot.Bottom - margin.Bottom - height,
            VerticalAlignment.Stretch => Math.Max(
                layoutSlot.Top + margin.Top,
                ((layoutSlot.Top + margin.Top + layoutSlot.Bottom - margin.Bottom) / 2.0) - (height / 2.0)),
            _ => y
        };

        return new Rect(x, y, width, height);
    }

    /// <summary>
    /// 计算两点的连线和x轴的夹角.
    /// 返回值为角度，范围[-180, 180].
    /// </summary>
    /// <param name="center">起点.</param>
    /// <param name="p">终点.</param>
    /// <returns>与x轴的夹角（角度）.</returns>
    public static double CalAngle(Point center, Point p) => Math.Atan2(p.Y - center.Y, p.X - center.X) * 180 / Math.PI;

    /// <summary>
    /// 计算三维空间中三点确定的平面的法线向量.
    /// </summary>
    /// <param name="p0">第一个点.</param>
    /// <param name="p1">第二个点.</param>
    /// <param name="p2">第三个点.</param>
    /// <returns>法线向量.</returns>
    public static Vector3D CalNormal(Point3D p0, Point3D p1, Point3D p2)
    {
        var v0 = new Vector3D(p1.X - p0.X, p1.Y - p0.Y, p1.Z - p0.Z);
        var v1 = new Vector3D(p2.X - p1.X, p2.Y - p1.Y, p2.Z - p1.Z);
        return Vector3D.CrossProduct(v0, v1);
    }
}
