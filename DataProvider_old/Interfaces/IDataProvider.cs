using DataProvider.DTO;
using System;


namespace DataProvider.Interfaces
{
    public delegate void DataChangeEventHandler(object sender, DataReceivedEventArgs e);
    public interface IDataProvider
    {
        event DataChangeEventHandler DataChanged;

        bool Start();
        bool Stop();

        int GetTagProviderId();

        DataReceivedEventArgs ReadValue(string[] tags);

        bool AddTag(TagInfo tag);
        bool Reload();
        bool IsActive();
    }
}
