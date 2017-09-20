namespace ToolKit.Cache
{
  public interface IDataCacheStore
  {
    object this[int index] { get; set; }
    object this[string key] { get; set; }

    bool ContainsKey(string key);
  }
}
