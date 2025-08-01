namespace Microsoft.Windows.Shell;

public class ThumbButtonInfoCollection : FreezableCollection<ThumbButtonInfo>
{
    private static ThumbButtonInfoCollection? s_empty = null;

    protected override Freezable CreateInstanceCore()
    {
        return new ThumbButtonInfoCollection();
    }

    internal static ThumbButtonInfoCollection Empty
    {
        get
        {
            if (s_empty == null)
            {
                ThumbButtonInfoCollection thumbButtonInfoCollection = new ThumbButtonInfoCollection();
                thumbButtonInfoCollection.Freeze();
                s_empty = thumbButtonInfoCollection;
            }

            return s_empty;
        }
    }
}