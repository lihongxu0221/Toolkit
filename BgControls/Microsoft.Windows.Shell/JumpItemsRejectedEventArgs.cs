namespace Microsoft.Windows.Shell;

public sealed class JumpItemsRejectedEventArgs : EventArgs
{
    private readonly IList<JumpItem> rejectedItems = new List<JumpItem>();
    private readonly IList<JumpItemRejectionReason> reasons = new List<JumpItemRejectionReason>();

    public IList<JumpItem> RejectedItems => rejectedItems;

    public IList<JumpItemRejectionReason> RejectionReasons => reasons;

    public JumpItemsRejectedEventArgs()
        : this(null, null)
    {
    }

    public JumpItemsRejectedEventArgs(IList<JumpItem>? rejectedItems, IList<JumpItemRejectionReason>? reasons)
    {
        if (rejectedItems == null || reasons == null || rejectedItems.Count != reasons.Count)
        {
            throw new ArgumentException("The counts of rejected items doesn't match the count of reasons.");
        }

        if (rejectedItems != null)
        {
            this.rejectedItems = new List<JumpItem>(rejectedItems).AsReadOnly();
            this.reasons = new List<JumpItemRejectionReason>(reasons).AsReadOnly();
            return;
        }

        this.rejectedItems = new List<JumpItem>().AsReadOnly();
        this.reasons = new List<JumpItemRejectionReason>().AsReadOnly();
    }
}