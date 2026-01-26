using System.Windows.Automation.Peers;

namespace BgControls.Controls;

public class TextBoxNoKeyboard : TextBox
{
    protected override AutomationPeer OnCreateAutomationPeer()
    {
        return new FrameworkElementAutomationPeer(this);
    }
}