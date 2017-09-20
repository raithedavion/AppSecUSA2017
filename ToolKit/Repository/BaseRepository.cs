using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ToolKit.Cache;
using System.Web;

namespace ToolKit.Repository
{
  public class BaseRepository
  {
    protected IDataCacheStore Cache { get; private set; }
    protected BaseRepository()
      : this(new SessionDataCacheStore(HttpContext.Current.Session))
    {

    }

    protected BaseRepository(IDataCacheStore cache)
    {
      Cache = cache;
    }

    protected void ClearEntry(string key)
    {
      if (Cache.ContainsKey(key))
      {
        Cache[key] = null;
      }
    }

    public bool ExistsInCache(string key)
    {
      return Cache.ContainsKey(key);
    }
  }
}
