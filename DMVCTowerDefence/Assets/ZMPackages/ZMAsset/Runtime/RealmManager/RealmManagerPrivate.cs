using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using Realms;
using UnityEngine;

/// <summary>
/// RealmManager 的部分类实现，包含核心功能和具体的数据库操作实现
/// </summary>
public partial class RealmManager
{
    /// <summary>
    /// Realm 数据库实例
    /// </summary>
    private Realm _realm;
    
    /// <summary>
    /// Realm 数据库配置对象
    /// </summary>
    private RealmConfiguration _realmConfiguration;

    /// <summary>
    /// 初始化 Realm 数据库
    /// 创建一个新的 Realm 实例并处理可能的异常
    /// </summary>
    private void InitializeRealm()
    {
        try
        {
            _realm = Realm.GetInstance(_realmConfiguration);
            Debug.Log("Realm database initialized successfully");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to initialize Realm: {ex.Message}");
        }
    }

    /// <summary>
    /// 异步初始化 Realm 数据库
    /// 在后台线程创建 Realm 实例，避免阻塞主线程
    /// </summary>
    private async Task InitializeRealmAsync()
    {
        try
        {
            await Task.Run(() =>
            {
                _realm = Realm.GetInstance(_realmConfiguration);
            });
            Debug.Log("Realm database initialized asynchronously");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to initialize Realm asynchronously: {ex.Message}");
        }
    }

    #region 通用操作方法

    /// <summary>
    /// 执行Realm写入操作的通用方法
    /// 在事务中对单个对象执行操作，并处理异常
    /// </summary>
    /// <typeparam name="T">继承自 RealmObject 的数据类型</typeparam>
    /// <param name="obj">要操作的对象</param>
    /// <param name="action">针对对象执行的具体操作</param>
    private void ExecuteRealmWrite<T>(T obj, Action<Realm, T> action) where T : RealmObject
    {
        try
        {
            var realm = GetRealm();
            realm.Write(() => action(realm, obj));
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to execute Realm write operation: {ex.Message}");
        }
    }

    /// <summary>
    /// 异步执行Realm写入操作的通用方法
    /// 在后台线程的事务中对单个对象执行操作，避免阻塞主线程
    /// </summary>
    /// <typeparam name="T">继承自 RealmObject 的数据类型</typeparam>
    /// <param name="obj">要操作的对象</param>
    /// <param name="action">针对对象执行的具体操作</param>
    /// <returns>表示异步操作的任务</returns>
    private async Task ExecuteRealmWriteAsync<T>(T obj, Action<Realm, T> action) where T : RealmObject
    {
        try
        {
            var realm = await GetRealmAsync();
            await Task.Run(() =>
            {
                realm.Write(() => action(realm, obj));
            });
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to execute Realm write operation asynchronously: {ex.Message}");
        }
    }

    /// <summary>
    /// 执行Realm集合写入操作的通用方法
    /// 在单个事务中对多个对象执行相同的操作，提高性能
    /// </summary>
    /// <typeparam name="T">继承自 RealmObject 的数据类型</typeparam>
    /// <param name="objects">要操作的对象集合</param>
    /// <param name="itemAction">针对每个对象执行的具体操作</param>
    private void ExecuteRealmWriteAll<T>(IEnumerable<T> objects, Action<Realm, T> itemAction) where T : RealmObject
    {
        try
        {
            var realm = GetRealm();
            realm.Write(() =>
            {
                foreach (var obj in objects)
                {
                    itemAction(realm, obj);
                }
            });
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to execute Realm batch write operation: {ex.Message}");
        }
    }

    /// <summary>
    /// 异步执行Realm集合写入操作的通用方法
    /// 在后台线程的单个事务中对多个对象执行相同的操作
    /// </summary>
    /// <typeparam name="T">继承自 RealmObject 的数据类型</typeparam>
    /// <param name="objects">要操作的对象集合</param>
    /// <param name="itemAction">针对每个对象执行的具体操作</param>
    /// <returns>表示异步操作的任务</returns>
    private async Task ExecuteRealmWriteAllAsync<T>(IEnumerable<T> objects, Action<Realm, T> itemAction) where T : RealmObject
    {
        try
        {
            var realm = await GetRealmAsync();
            await Task.Run(() =>
            {
                realm.Write(() =>
                {
                    foreach (var obj in objects)
                    {
                        itemAction(realm, obj);
                    }
                });
            });
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to execute Realm batch write operation asynchronously: {ex.Message}");
        }
    }

    #endregion

    #region 具体实现方法

    /// <summary>
    /// 删除指定类型的所有对象实现
    /// 在单个事务中移除所有对象，高效执行批量删除
    /// </summary>
    /// <typeparam name="T">继承自 RealmObject 的数据类型</typeparam>
    private void DeleteAllImpl<T>() where T : RealmObject
    {
        try
        {
            var realm = GetRealm();
            realm.Write(() =>
            {
                var allObjects = realm.All<T>();
                realm.RemoveRange(allObjects);
            });
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to delete all objects of type {typeof(T).Name}: {ex.Message}");
        }
    }

    /// <summary>
    /// 异步删除指定类型的所有对象实现
    /// 在后台线程的事务中移除所有对象
    /// </summary>
    /// <typeparam name="T">继承自 RealmObject 的数据类型</typeparam>
    /// <returns>表示异步操作的任务</returns>
    private async Task DeleteAllImplAsync<T>() where T : RealmObject
    {
        try
        {
            var realm = await GetRealmAsync();
            await Task.Run(() =>
            {
                realm.Write(() =>
                {
                    var allObjects = realm.All<T>();
                    realm.RemoveRange(allObjects);
                });
            });
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to delete all objects of type {typeof(T).Name} asynchronously: {ex.Message}");
        }
    }

    /// <summary>
    /// 获取指定类型的所有对象实现
    /// 返回可查询的集合，支持后续的 LINQ 操作
    /// </summary>
    /// <typeparam name="T">继承自 RealmObject 的数据类型</typeparam>
    /// <returns>包含所有对象的可查询集合</returns>
    private IQueryable<T> GetAllImpl<T>() where T : RealmObject
    {
        try
        {
            var realm = GetRealm();
            return realm.All<T>();
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to get all objects of type {typeof(T).Name}: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// 异步获取指定类型的所有对象实现
    /// 在后台线程查询并转换结果为 List，避免线程问题
    /// </summary>
    /// <typeparam name="T">继承自 RealmObject 的数据类型</typeparam>
    /// <returns>包含所有对象的可查询集合的任务</returns>
    private async Task<IQueryable<T>> GetAllImplAsync<T>() where T : RealmObject
    {
        try
        {
            var realm = await GetRealmAsync();
            // 注意：必须将结果转换成List才能在异步上下文中安全使用
            var results = await Task.Run(() => realm.All<T>().ToList());
            return results.AsQueryable();
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to get all objects of type {typeof(T).Name} asynchronously: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// 根据条件查询对象实现
    /// 使用内存中过滤进行查询
    /// </summary>
    /// <typeparam name="T">继承自 RealmObject 的数据类型</typeparam>
    /// <param name="predicate">用于筛选对象的条件函数</param>
    /// <returns>满足条件的对象集合</returns>
    private IQueryable<T> QueryImpl<T>(Func<T, bool> predicate) where T : RealmObject
    {
        try
        {
            var realm = GetRealm();
            return realm.All<T>().Where(predicate).AsQueryable();
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to query objects: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// 异步根据条件查询对象实现
    /// 在后台线程执行查询并转换结果为 List
    /// </summary>
    /// <typeparam name="T">继承自 RealmObject 的数据类型</typeparam>
    /// <param name="predicate">用于筛选对象的条件函数</param>
    /// <returns>包含满足条件的对象集合的任务</returns>
    private async Task<IQueryable<T>> QueryImplAsync<T>(Func<T, bool> predicate) where T : RealmObject
    {
        try
        {
            var realm = await GetRealmAsync();
            var results = await Task.Run(() => realm.All<T>().Where(predicate).ToList());
            return results.AsQueryable();
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to query objects asynchronously: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// 使用 LINQ 表达式查询对象实现
    /// 支持复杂的 LINQ 查询操作
    /// </summary>
    /// <typeparam name="T">继承自 RealmObject 的数据类型</typeparam>
    /// <param name="query">LINQ 查询表达式</param>
    /// <returns>查询结果集合</returns>
    private IQueryable<T> QueryWithLinqImpl<T>(Func<IQueryable<T>, IQueryable<T>> query) where T : RealmObject
    {
        try
        {
            var realm = GetRealm();
            return query(realm.All<T>());
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to query with LINQ: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// 异步使用 LINQ 表达式查询对象实现
    /// 在后台线程执行 LINQ 查询并转换结果为 List
    /// </summary>
    /// <typeparam name="T">继承自 RealmObject 的数据类型</typeparam>
    /// <param name="query">LINQ 查询表达式</param>
    /// <returns>包含查询结果集合的任务</returns>
    private async Task<IQueryable<T>> QueryWithLinqImplAsync<T>(Func<IQueryable<T>, IQueryable<T>> query) where T : RealmObject
    {
        try
        {
            var realm = await GetRealmAsync();
            var results = await Task.Run(() => query(realm.All<T>()).ToList());
            return results.AsQueryable();
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to query with LINQ asynchronously: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// 在写入事务中执行自定义操作实现
    /// 支持执行复杂的事务逻辑
    /// </summary>
    /// <param name="action">要在事务中执行的操作</param>
    private void ExecuteTransactionImpl(Action<Realm> action)
    {
        try
        {
            var realm = GetRealm();
            realm.Write(() => action(realm));
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to execute transaction: {ex.Message}");
        }
    }

    /// <summary>
    /// 异步执行事务实现
    /// 在后台线程执行事务操作
    /// </summary>
    /// <param name="action">要在事务中执行的操作</param>
    /// <returns>表示异步操作的任务</returns>
    private async Task ExecuteTransactionImplAsync(Action<Realm> action)
    {
        try
        {
            var realm = await GetRealmAsync();
            await Task.Run(() => realm.Write(() => action(realm)));
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to execute async transaction: {ex.Message}");
        }
    }

    /// <summary>
    /// 通用方法：根据主键查找对象实现
    /// 支持多种主键类型（string, long, Guid, ObjectId）
    /// </summary>
    /// <typeparam name="T">继承自 RealmObject 的数据类型</typeparam>
    /// <typeparam name="TKey">主键的数据类型</typeparam>
    /// <param name="primaryKey">要查找的主键值</param>
    /// <returns>找到的对象，如果不存在则返回 null</returns>
    private T FindByPrimaryKeyImpl<T, TKey>(TKey primaryKey) where T : RealmObject
    {
        try
        {
            var realm = GetRealm();
            // 根据不同的TKey类型调用对应的Find方法
            if (primaryKey is string stringKey)
            {
                return realm.Find<T>(stringKey);
            }
            else if (primaryKey is long longKey)
            {
                return realm.Find<T>(longKey);
            }
            else if (primaryKey is Guid guidKey)
            {
                return realm.Find<T>(guidKey);
            }
            else if (primaryKey is ObjectId objectIdKey)
            {
                return realm.Find<T>(objectIdKey);
            }
            else
            {
                Debug.LogError($"Unsupported primary key type: {typeof(TKey).Name}");
                return null;
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to find object by primary key: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// 通用异步方法：根据主键查找对象实现
    /// 在后台线程查找对象
    /// </summary>
    /// <typeparam name="T">继承自 RealmObject 的数据类型</typeparam>
    /// <typeparam name="TKey">主键的数据类型</typeparam>
    /// <param name="primaryKey">要查找的主键值</param>
    /// <returns>包含找到的对象的任务，如果不存在则返回 null</returns>
    private async Task<T> FindByPrimaryKeyImplAsync<T, TKey>(TKey primaryKey) where T : RealmObject
    {
        try
        {
            var realm = await GetRealmAsync();
            return await Task.Run(() => {
                // 根据不同的TKey类型调用对应的Find方法
                if (primaryKey is string stringKey)
                {
                    return realm.Find<T>(stringKey);
                }
                else if (primaryKey is long longKey)
                {
                    return realm.Find<T>(longKey);
                }
                else if (primaryKey is Guid guidKey)
                {
                    return realm.Find<T>(guidKey);
                }
                else if (primaryKey is ObjectId objectIdKey)
                {
                    return realm.Find<T>(objectIdKey);
                }
                else
                {
                    Debug.LogError($"Unsupported primary key type: {typeof(TKey).Name}");
                    return null;
                }
            });
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to find object by primary key asynchronously: {ex.Message}");
            return null;
        }
    }
    
    /// <summary>
    /// 关闭 Realm 数据库实现
    /// 安全地释放 Realm 资源
    /// </summary>
    private void CloseRealmImpl()
    {
        if (_realm != null && !_realm.IsClosed)
        {
            _realm.Dispose();
            _realm = null;
            Debug.Log("Realm database closed");
        }
    }

    /// <summary>
    /// 异步关闭 Realm 数据库实现
    /// 在后台线程安全地释放 Realm 资源
    /// </summary>
    /// <returns>表示异步操作的任务</returns>
    private async Task CloseRealmImplAsync()
    {
        await Task.Run(() =>
        {
            if (_realm != null && !_realm.IsClosed)
            {
                _realm.Dispose();
                _realm = null;
            }
        });
        Debug.Log("Realm database closed asynchronously");
    }

    #endregion
}
