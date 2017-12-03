namespace LINQtoGremlin.Core.Graph
{
    public interface IGDbModelDescriptorEntry
    {
        string EntryName
        {
            get;
        }

        object EntryValue
        {
            get;
        }

        object Extra
        {
            get;
        }
    }
}
