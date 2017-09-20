using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Web;

namespace ToolKit.Cache
{
  public abstract class ApplicationCache
  {
    private static Dictionary<string, object> _locks = new Dictionary<string, object>();
    private static Dictionary<string, object> _listFunctions = new Dictionary<string, object>();

    private static void CreateLock<T>() where T : class
    {
      _locks.Add(typeof(T).ToString(), new object());
    }

    /// <summary>
    /// This is used as the public method for creating lookup Listing operations.  This calls CreateListOperation and allows the Type to be called like List<T>();
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <param name="f"></param>
    /// <example>Create<States>("StateID", () => States.ListStates());</example>
    public static void Create<T>(string key, Func<IEnumerable<T>> f) where T : class
    {
      CreateListOperation<T>(key, f);
    }

    /// <summary>
    /// This method is used to load the initial cache.  This should only be called by the APPLICATION using the application cache scheme.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    public static void Load<T>(string key) where T : class
    {
      CacheManager.LoadList<T>(List<T>(), key);
    }

    private static void CreateListOperation<T>(string key, Func<IEnumerable<T>> f) where T : class
    {
      CreateLock<T>();
      _listFunctions.Add(typeof(T).ToString(), new LoadListDescription<T> { ItemIdPropertyName = key, ListFunc = f });
    }

    public static IEnumerable<T> List<T>() where T : class
    {
      IEnumerable<T> list = List<T>(HttpContext.Current.Cache);
      return list;
    }

    public static IEnumerable<T> List<T>(System.Web.Caching.Cache cache) where T : class
    {
      IEnumerable<T> items = CacheManager.GetItemsOfType<T>(cache);

      if (items == null || items.Count() == 0)
      {
        if (!_locks.ContainsKey(typeof(T).ToString()))
        {
          return new List<T>();
        }

        object theLock = _locks[typeof(T).ToString()];
        lock (theLock)
        {
          // Need to double check because we could have been locked
          // out during the earlier read.
          items = CacheManager.GetItemsOfType<T>(cache);

          if (items == null || items.Count() == 0)
          {
            LoadListDescription<T> description = _listFunctions[typeof(T).ToString()] as LoadListDescription<T>;
            if (description != null)
            {
              items = CacheManager.LoadList<T>(description.ListFunc, description.ItemIdPropertyName);
            }
          }
        }
      }

      return items;
    }

    public static void AddEntry<T>(int id, T item) where T : class
    {
      if (_locks.ContainsKey(typeof(T).ToString()))
      {
        lock (_locks[typeof(T).ToString()])
        {
          CacheManager.AddItem<T>(id, item);
        }
      }
    }

    public static void RemoveEntry<T>(int id) where T : class
    {
      if (_locks.ContainsKey(typeof(T).ToString()))
      {
        lock (_locks[typeof(T).ToString()])
        {
          CacheManager.RemoveItem<T>(id);
        }
      }
    }

    public static void RefreshEntry<T>(int id, T item) where T : class
    {
        ApplicationCache.RemoveEntry<T>(id);
        ApplicationCache.AddEntry<T>(id, item);
    }
  }

  class LoadListDescription<T>
  {
    public string ItemIdPropertyName { get; set; }
    public Func<IEnumerable<T>> ListFunc { get; set; }
  }
}
