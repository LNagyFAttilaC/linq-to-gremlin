namespace LINQtoGremlin.Core.Graph
{
    public class GDbModelDescriptorEntry 
        : IGDbModelDescriptorEntry
    {
        #region Properties

        public string EntryName
        {
            get;
            set;
        }

        public object EntryValue
        {
            get;
            set;
        }

        public object Extra
        {
            get;
            set;
        }

        #endregion
    }
}
