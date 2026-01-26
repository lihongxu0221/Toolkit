using BgControls;

namespace BgCommon.Prism.Wpf.Modules.Parameters.Views;

/// <summary>
/// ParameterConfigView.xaml 的交互逻辑
/// </summary>
public partial class ParameterConfigView : UserControl
{
    public ParameterConfigView()
    {
        InitializeComponent();
    }

    private void DataGridMain_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
    {
        // 获取事件的原始来源，即用户实际点击的那个控件
        if (e.OriginalSource is DependencyObject source)
        {
            // 从被点击的控件开始，向上（沿着可视化树）查找其所在的 DataGridRow
            DataGridRow? row = source.FindAncestor<DataGridRow>();
            if (row != null)
            {
                // 如果找到了行，并且这一行当前不是选中状态
                if (!row.IsSelected)
                {
                    // 关键操作：手动将这一行设置为选中状态。
                    // 如果是多选模式，这一行会被添加到现有选中项中，
                    // 但通常右键的期望行为是只选中当前行。
                    // 如果希望右键单击总是只选择当前行，可以先取消所有选择：
                    // (sender as DataGrid).UnselectAll();
                    row.IsSelected = true;
                }

                // 即使行已经是选中状态，也最好设置一下焦点，确保上下文菜单知道操作对象
                if (!row.IsFocused)
                {
                    _ = row.Focus();
                }
            }
        }
    }
}