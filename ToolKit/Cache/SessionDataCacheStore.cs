using System.Web.SessionState;

namespace ToolKit.Cache
{
  public class SessionDataCacheStore : IDataCacheStore
  {
    private HttpSessionState session;

    public SessionDataCacheStore(HttpSessionState session)
    {
      this.session = session;
    }

    public object this[int index]
    {
      get { return session[index]; }
      set { session[index] = value; }
    }

    public object this[string key]
    {
      get { return session[key]; }
      set { session[key] = value; }
    }

    public bool ContainsKey(string key)
    {
      foreach (string sessionKey in session.Keys)
      {
        if (sessionKey == key)
        {
          return true;
        }
      }

      return false;
    }
  }
}
