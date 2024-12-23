public interface IDataService
{
    bool SaveData<T>(string RelativePath, T Data);

    T LoadData<T>(string RelativePath);

    void DeleteData<T>(string relativePath);
}
