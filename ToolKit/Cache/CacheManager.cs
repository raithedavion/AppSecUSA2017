using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Caching;
using System.Web;
using System.Web.Caching;
using ToolKit.Configuration;

namespace ToolKit.Cache
{
  public static class CacheManager
  {
    public static System.Web.Caching.Cache Cache
    {
      get;
      set;
    }

    public static void AddItem<T>(int itemID, T item) where T : class
    {
      ResetCache();
      string key = ConstructCacheKey(item.GetType(), itemID);
      Cache.Insert(key, item, null, DateTime.Now.AddHours(Config.CacheTimeoutInHours), TimeSpan.Zero);
    }

    public static T RemoveItem<T>(int itemID) where T : class
    {
      ResetCache();
      T item = GetItem<T>(itemID);
      if (item != null)
      {
        string key = ConstructCacheKey(typeof(T), itemID);
        Cache.Remove(key);
      }
      return item;
    }

    public static T GetItem<T>(int itemID) where T : class
    {
      ResetCache();
      string key = ConstructCacheKey(typeof(T), itemID);
      return Cache.Get(key) as T;
    }

    public static IEnumerable<T> GetItemsOfType<T>(System.Web.Caching.Cache cache) where T : class
    {
      ResetCache();
      return cache.OfType<DictionaryEntry>()
                                      .Where(de => de.Value is T)
                                      .Select(de => (T)de.Value);
    }

    public static IEnumerable<T> LoadList<T>(Func<IEnumerable<T>> listFunc, string itemIDPropertyName) where T : class
    {
      ResetCache();
      IEnumerable<T> items = listFunc();

      if (items == null)
      {
        return new List<T>();
      }

      foreach (T item in items)
      {
        PropertyInfo propertyInfo = item.GetType().GetProperty(itemIDPropertyName);
        int itemID = Convert.ToInt32(propertyInfo.GetValue(item, null));
        CacheManager.AddItem(itemID, item);
      }

      return items;
    }

    public static void LoadList<T>(IEnumerable<T> list, string itemIDPropertyName) where T : class
    {
      ResetCache();
      foreach (T item in list)
      {
        PropertyInfo propertyInfo = item.GetType().GetProperty(itemIDPropertyName);
        int itemID = Convert.ToInt32(propertyInfo.GetValue(item, null));
        CacheManager.AddItem(itemID, item);
      }
    }

    private static string ConstructCacheKey(Type itemType, int itemID)
    {
      return string.Format("{0}_{1}", itemType.Name, itemID.ToString());
    }

    private static void ResetCache()
    {
      if (Cache == null)
        Cache = HttpContext.Current.Cache;
    }
  }
}
