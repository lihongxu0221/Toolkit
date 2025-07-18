using BgControls.Controls.Datas;

namespace BgControls.Controls;

public class IconElement
{
    /// <summary>
    /// 路径图标
    /// </summary>
    public static readonly DependencyProperty GeometryProperty = DependencyProperty.RegisterAttached("Geometry", typeof(Geometry), typeof(IconElement), new PropertyMetadata(default(Geometry)));

    /// <summary>
    /// 路径图标 选中
    /// </summary>
    public static readonly DependencyProperty GeometrySelectedProperty = DependencyProperty.RegisterAttached("GeometrySelected", typeof(Geometry), typeof(IconElement), new PropertyMetadata(default(Geometry)));

    /// <summary>
    /// 图片图标
    /// </summary>
    public static readonly DependencyProperty ImageProperty = DependencyProperty.RegisterAttached("Image", typeof(ImageSource), typeof(IconElement), new PropertyMetadata(default(ImageSource)));

    /// <summary>
    /// 图片图标 选中
    /// </summary>
    public static readonly DependencyProperty ImageSelectedProperty = DependencyProperty.RegisterAttached("ImageSelected", typeof(ImageSource), typeof(IconElement), new PropertyMetadata(default(ImageSource)));

    /// <summary>
    /// 字体图标
    /// </summary>
    public static readonly DependencyProperty FontIconProperty = DependencyProperty.RegisterAttached("FontIcon", typeof(object), typeof(IconElement), new PropertyMetadata(default(object)));

    /// <summary>
    /// 字体图标 选中
    /// </summary>
    public static readonly DependencyProperty FontIconSelectedProperty = DependencyProperty.RegisterAttached("FontIconSelected", typeof(object), typeof(IconElement), new PropertyMetadata(default(object)));

    /// <summary>
    /// 字体大小
    /// </summary>
    public static readonly DependencyProperty FontSizeProperty = DependencyProperty.RegisterAttached("FontSize", typeof(double), typeof(IconElement), new PropertyMetadata(13.6));

    /// <summary>
    /// 边距
    /// </summary>
    public static readonly DependencyProperty MarginProperty = DependencyProperty.RegisterAttached("Margin", typeof(Thickness), typeof(IconElement), new PropertyMetadata(new Thickness(0)));

    /// <summary>
    /// 拉伸  （路径，字体）
    /// </summary>
    public static readonly DependencyProperty StretchProperty = DependencyProperty.RegisterAttached("Stretch", typeof(Stretch), typeof(IconElement), new PropertyMetadata(default(Stretch)));

    /// <summary>
    /// 文本和图标的相对关系
    /// </summary>
    public static readonly DependencyProperty TextImageRelationProperty = DependencyProperty.RegisterAttached("TextImageRelation", typeof(TextImageRelation), typeof(IconElement), new PropertyMetadata(TextImageRelation.ImageBeforeText));

    /// <summary>
    /// 显示方式
    /// </summary>
    public static readonly DependencyProperty DisplayStyleProperty = DependencyProperty.RegisterAttached("DisplayStyle", typeof(DisplayStyle), typeof(IconElement), new PropertyMetadata(DisplayStyle.ImageAndText));

    /// <summary>
    /// 宽度
    /// </summary>
    public static readonly DependencyProperty WidthProperty = DependencyProperty.RegisterAttached("Width", typeof(double), typeof(IconElement), new PropertyMetadata(double.NaN));

    /// <summary>
    /// 高度
    /// </summary>
    public static readonly DependencyProperty HeightProperty = DependencyProperty.RegisterAttached("Height", typeof(double), typeof(IconElement), new PropertyMetadata(double.NaN));

    /// <summary>
    /// 高度
    /// </summary>
    public static readonly DependencyProperty ShowSelectedLineProperty = DependencyProperty.RegisterAttached("ShowSelectedLine", typeof(bool), typeof(IconElement), new PropertyMetadata(ValueBoxes.TrueBox));

    /// <summary>
    /// 旋转的角度
    /// </summary>
    public static readonly DependencyProperty RotateAngleProperty = DependencyProperty.RegisterAttached("RotateAngle", typeof(double), typeof(IconElement), new PropertyMetadata(0.0));

    public static readonly DependencyProperty RenderTransformOriginProperty = DependencyProperty.RegisterAttached("RenderTransformOrigin", typeof(Point), typeof(IconElement), new PropertyMetadata(new Point(0.5, 0.5)));

    public static readonly DependencyProperty RotateCenterXProperty = DependencyProperty.RegisterAttached("RotateCenterX", typeof(double), typeof(IconElement), new PropertyMetadata(0.0));

    public static readonly DependencyProperty RotateCenterYProperty = DependencyProperty.RegisterAttached("RotateCenterY", typeof(double), typeof(IconElement), new PropertyMetadata(0.0));


    public static void SetGeometry(DependencyObject element, Geometry value)
    {
        element.SetValue(GeometryProperty, value);
    }

    public static Geometry GetGeometry(DependencyObject element)
    {
        return (Geometry)element.GetValue(GeometryProperty);
    }

    public static void SetGeometrySelected(DependencyObject element, Geometry value)
    {
        element.SetValue(GeometrySelectedProperty, value);
    }

    public static Geometry GetGeometrySelected(DependencyObject element)
    {
        return (Geometry)element.GetValue(GeometrySelectedProperty);
    }

    public static void SetImage(DependencyObject element, ImageSource value)
    {
        element.SetValue(ImageProperty, value);
    }

    public static ImageSource GetImage(DependencyObject element)
    {
        return (ImageSource)element.GetValue(ImageProperty);
    }

    public static void SetImageSelected(DependencyObject element, ImageSource value)
    {
        element.SetValue(ImageSelectedProperty, value);
    }

    public static ImageSource GetImageSelected(DependencyObject element)
    {
        return (ImageSource)element.GetValue(ImageSelectedProperty);
    }

    public static void SetFontIcon(DependencyObject element, object value)
    {
        element.SetValue(FontIconProperty, value);
    }

    public static object GetFontIcon(DependencyObject element)
    {
        return element.GetValue(FontIconProperty);
    }

    public static void SetFontIconSelected(DependencyObject element, object value)
    {
        element.SetValue(FontIconSelectedProperty, value);
    }

    public static object GetFontIconSelected(DependencyObject element)
    {
        return element.GetValue(FontIconSelectedProperty);
    }

    public static void SetFontSize(DependencyObject element, double value)
    {
        element.SetValue(FontSizeProperty, value);
    }

    public static double GetFontSize(DependencyObject element)
    {
        return (double)element.GetValue(FontSizeProperty);
    }

    public static void SetMargin(DependencyObject element, Thickness value)
    {
        element.SetValue(MarginProperty, value);
    }

    public static Thickness GetMargin(DependencyObject element)
    {
        return (Thickness)element.GetValue(MarginProperty);
    }

    public static void SetStretch(DependencyObject element, Stretch value)
    {
        element.SetValue(StretchProperty, value);
    }

    public static Stretch GetStretch(DependencyObject element)
    {
        return (Stretch)element.GetValue(StretchProperty);
    }

    public static void SetTextImageRelation(DependencyObject element, TextImageRelation value)
    {
        element.SetValue(TextImageRelationProperty, value);
    }

    public static TextImageRelation GetTextImageRelation(DependencyObject element)
    {
        return (TextImageRelation)element.GetValue(TextImageRelationProperty);
    }

    public static void SetDisplayStyle(DependencyObject element, DisplayStyle value)
    {
        element.SetValue(DisplayStyleProperty, value);
    }

    public static DisplayStyle GetDisplayStyle(DependencyObject element)
    {
        return (DisplayStyle)element.GetValue(DisplayStyleProperty);
    }

    public static void SetWidth(DependencyObject element, double value)
    {
        element.SetValue(WidthProperty, value);
    }

    public static double GetWidth(DependencyObject element)
    {
        return (double)element.GetValue(WidthProperty);
    }

    public static void SetHeight(DependencyObject element, double value)
    {
        element.SetValue(HeightProperty, value);
    }

    public static double GetHeight(DependencyObject element)
    {
        return (double)element.GetValue(HeightProperty);
    }

    public static void SetShowSelectedLine(DependencyObject element, bool value)
    {
        element.SetValue(ShowSelectedLineProperty, ValueBoxes.BooleanBox(value));
    }

    public static bool GetShowSelectedLine(DependencyObject element)
    {
        return (bool)element.GetValue(ShowSelectedLineProperty);
    }

    public static void SetRotateAngle(DependencyObject element, double value)
    {
        element.SetValue(RotateAngleProperty, value);
    }

    public static double GetRotateAngle(DependencyObject element)
    {
        return (double)element.GetValue(RotateAngleProperty);
    }

    public static void SetRenderTransformOrigin(DependencyObject element, Point value)
    {
        element.SetValue(RenderTransformOriginProperty, value);
    }

    public static Point GetRenderTransformOrigin(DependencyObject element)
    {
        return (Point)element.GetValue(RenderTransformOriginProperty);
    }

    public static void SetRotateCenterX(DependencyObject element, double value)
    {
        element.SetValue(RotateCenterXProperty, value);
    }

    public static double GetRotateCenterX(DependencyObject element)
    {
        return (double)element.GetValue(RotateCenterXProperty);
    }

    public static void SetRotateCenterY(DependencyObject element, double value)
    {
        element.SetValue(RotateCenterYProperty, value);
    }

    public static double GetRotateCenterY(DependencyObject element)
    {
        return (double)element.GetValue(RotateCenterYProperty);
    }
}
