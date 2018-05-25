namespace DataProvider.Interfaces
{
    public delegate void DataChangeEventHandler(DataReceivedEventArgs e);

    public interface IDataProvider
    {
        event DataChangeEventHandler DataChanged;

        bool Start();
        //bool Stop();

        //int GetTagProviderId();

        //DataReceivedEventArgs ReadValue(string[] tags);

        //bool AddTag(DTO.TagInfo tag);
        //bool Reload();
    }
}
