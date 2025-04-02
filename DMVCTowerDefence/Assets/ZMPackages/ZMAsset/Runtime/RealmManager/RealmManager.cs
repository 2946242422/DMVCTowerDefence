using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using Realms;
using UnityEngine;

/// <summary>
/// Realm ���ݿ�����������ڴ����κ� Realm �������͵� CRUD ����
/// ʵ��Ϊ MonoBehaviour �������ṩͬ�����첽���ݿ��������
/// </summary>
public partial class RealmManager : MonoBehaviour, IRealmAccessor
{
    /// <summary>
    /// ����ʵ��
    /// </summary>
    private static RealmManager _instance;
    
    /// <summary>
    /// ������������������������Զ�����
    /// </summary>
    public static RealmManager Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject go = new GameObject("RealmManager");
                _instance = go.AddComponent<RealmManager>();
                DontDestroyOnLoad(go);
            }
            return _instance;
        }
    }

    /// <summary>
    /// Unity �������ڷ��� - �����ʼ��ʱ����
    /// ���������߼��� Realm ���ó�ʼ��
    /// </summary>
    private void Awake()
    {
        // ����ģʽʵ�� - ȷ��ֻ��һ��ʵ������
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);
        
        // ���� Realm ���ݿ�����
        _realmConfiguration = new RealmConfiguration
        {
            SchemaVersion = 1,
            MigrationCallback = (migration, oldVersion) =>
            {
                // �������ݿ�Ǩ���߼�
                // �����ݿ�ģʽ�汾���ʱ�ᴥ���˻ص�
            }
        };
        
        // ��ʼ�����ݿ�
        InitializeRealm();
    }

    #region IRealmAccessor �ӿ�ʵ��

    /// <summary>
    /// ��ȡ Realm ���ݿ�ʵ�������δ��ʼ�����ѹر������³�ʼ��
    /// </summary>
    /// <returns>Realm ���ݿ�ʵ��</returns>
    public Realm GetRealm()
    {
        if (_realm == null || _realm.IsClosed)
        {
            InitializeRealm();
        }
        return _realm;
    }

    /// <summary>
    /// �첽��ȡ Realm ���ݿ�ʵ�������δ��ʼ�����ѹر����첽���³�ʼ��
    /// </summary>
    /// <returns>���� Realm ���ݿ�ʵ��������</returns>
    public async Task<Realm> GetRealmAsync()
    {
        if (_realm == null || _realm.IsClosed)
        {
            await InitializeRealmAsync();
        }
        return _realm;
    }

    /// <summary>
    /// ���ӻ���µ��� Realm ����
    /// </summary>
    /// <typeparam name="T">�̳��� RealmObject ����������</typeparam>
    /// <param name="obj">Ҫ���ӻ���µĶ���</param>
    public void AddOrUpdate<T>(T obj) where T : RealmObject
    {
        ExecuteRealmWrite(obj, (realm, o) => realm.Add(o, true));
    }

    /// <summary>
    /// �첽���ӻ���µ��� Realm ����
    /// </summary>
    /// <typeparam name="T">�̳��� RealmObject ����������</typeparam>
    /// <param name="obj">Ҫ���ӻ���µĶ���</param>
    /// <returns>��ʾ�첽����������</returns>
    public async Task AddOrUpdateAsync<T>(T obj) where T : RealmObject
    {
        await ExecuteRealmWriteAsync(obj, (realm, o) => realm.Add(o, true));
    }

    /// <summary>
    /// �������ӻ���¶�� Realm ����
    /// </summary>
    /// <typeparam name="T">�̳��� RealmObject ����������</typeparam>
    /// <param name="objects">Ҫ���ӻ���µĶ��󼯺�</param>
    public void AddOrUpdateAll<T>(IEnumerable<T> objects) where T : RealmObject
    {
        ExecuteRealmWriteAll(objects, (realm, obj) => realm.Add(obj, true));
    }

    /// <summary>
    /// �첽�������ӻ���¶�� Realm ����
    /// </summary>
    /// <typeparam name="T">�̳��� RealmObject ����������</typeparam>
    /// <param name="objects">Ҫ���ӻ���µĶ��󼯺�</param>
    /// <returns>��ʾ�첽����������</returns>
    public async Task AddOrUpdateAllAsync<T>(IEnumerable<T> objects) where T : RealmObject
    {
        await ExecuteRealmWriteAllAsync(objects, (realm, obj) => realm.Add(obj, true));
    }

    /// <summary>
    /// ɾ������ Realm ����
    /// </summary>
    /// <typeparam name="T">�̳��� RealmObject ����������</typeparam>
    /// <param name="obj">Ҫɾ���Ķ���</param>
    public void Delete<T>(T obj) where T : RealmObject
    {
        ExecuteRealmWrite(obj, (realm, o) => realm.Remove(o));
    }

    /// <summary>
    /// �첽ɾ������ Realm ����
    /// </summary>
    /// <typeparam name="T">�̳��� RealmObject ����������</typeparam>
    /// <param name="obj">Ҫɾ���Ķ���</param>
    /// <returns>��ʾ�첽����������</returns>
    public async Task DeleteAsync<T>(T obj) where T : RealmObject
    {
        await ExecuteRealmWriteAsync(obj, (realm, o) => realm.Remove(o));
    }

    /// <summary>
    /// ����ɾ����� Realm ����
    /// </summary>
    /// <typeparam name="T">�̳��� RealmObject ����������</typeparam>
    /// <param name="objects">Ҫɾ���Ķ��󼯺�</param>
    public void DeleteAll<T>(IEnumerable<T> objects) where T : RealmObject
    {
        ExecuteRealmWriteAll(objects, (realm, obj) => realm.Remove(obj));
    }

    /// <summary>
    /// �첽����ɾ����� Realm ����
    /// </summary>
    /// <typeparam name="T">�̳��� RealmObject ����������</typeparam>
    /// <param name="objects">Ҫɾ���Ķ��󼯺�</param>
    /// <returns>��ʾ�첽����������</returns>
    public async Task DeleteAllAsync<T>(IEnumerable<T> objects) where T : RealmObject
    {
        await ExecuteRealmWriteAllAsync(objects, (realm, obj) => realm.Remove(obj));
    }

    /// <summary>
    /// ɾ��ָ�����͵����� Realm ����
    /// </summary>
    /// <typeparam name="T">�̳��� RealmObject ����������</typeparam>
    public void DeleteAll<T>() where T : RealmObject
    {
        DeleteAllImpl<T>();
    }

    /// <summary>
    /// �첽ɾ��ָ�����͵����� Realm ����
    /// </summary>
    /// <typeparam name="T">�̳��� RealmObject ����������</typeparam>
    /// <returns>��ʾ�첽����������</returns>
    public async Task DeleteAllAsync<T>() where T : RealmObject
    {
        await DeleteAllImplAsync<T>();
    }

    /// <summary>
    /// ��ȡָ�����͵����� Realm ����
    /// </summary>
    /// <typeparam name="T">�̳��� RealmObject ����������</typeparam>
    /// <returns>�������ж���Ŀɲ�ѯ����</returns>
    public IQueryable<T> GetAll<T>() where T : RealmObject
    {
        return GetAllImpl<T>();
    }

    /// <summary>
    /// �첽��ȡָ�����͵����� Realm ����
    /// </summary>
    /// <typeparam name="T">�̳��� RealmObject ����������</typeparam>
    /// <returns>�������ж���Ŀɲ�ѯ���ϵ�����</returns>
    public async Task<IQueryable<T>> GetAllAsync<T>() where T : RealmObject
    {
        return await GetAllImplAsync<T>();
    }

    /// <summary>
    /// ʹ��ν�ʺ�����ѯ Realm ����
    /// </summary>
    /// <typeparam name="T">�̳��� RealmObject ����������</typeparam>
    /// <param name="predicate">����ɸѡ�������������</param>
    /// <returns>���������Ķ��󼯺�</returns>
    public IQueryable<T> Query<T>(Func<T, bool> predicate) where T : RealmObject
    {
        return QueryImpl(predicate);
    }

    /// <summary>
    /// �첽ʹ��ν�ʺ�����ѯ Realm ����
    /// </summary>
    /// <typeparam name="T">�̳��� RealmObject ����������</typeparam>
    /// <param name="predicate">����ɸѡ�������������</param>
    /// <returns>�������������Ķ��󼯺ϵ�����</returns>
    public async Task<IQueryable<T>> QueryAsync<T>(Func<T, bool> predicate) where T : RealmObject
    {
        return await QueryImplAsync(predicate);
    }

    /// <summary>
    /// ʹ�� LINQ ����ʽ��ѯ Realm ����
    /// </summary>
    /// <typeparam name="T">�̳��� RealmObject ����������</typeparam>
    /// <param name="query">LINQ ��ѯ����ʽ</param>
    /// <returns>��ѯ�������</returns>
    public IQueryable<T> QueryWithLinq<T>(Func<IQueryable<T>, IQueryable<T>> query) where T : RealmObject
    {
        return QueryWithLinqImpl(query);
    }

    /// <summary>
    /// �첽ʹ�� LINQ ����ʽ��ѯ Realm ����
    /// </summary>
    /// <typeparam name="T">�̳��� RealmObject ����������</typeparam>
    /// <param name="query">LINQ ��ѯ����ʽ</param>
    /// <returns>������ѯ������ϵ�����</returns>
    public async Task<IQueryable<T>> QueryWithLinqAsync<T>(Func<IQueryable<T>, IQueryable<T>> query) where T : RealmObject
    {
        return await QueryWithLinqImplAsync(query);
    }

    /// <summary>
    /// ��������ִ�����ݿ����
    /// </summary>
    /// <param name="action">Ҫ��������ִ�еĲ���</param>
    public void ExecuteTransaction(Action<Realm> action)
    {
        ExecuteTransactionImpl(action);
    }

    /// <summary>
    /// �첽��������ִ�����ݿ����
    /// </summary>
    /// <param name="action">Ҫ��������ִ�еĲ���</param>
    /// <returns>��ʾ�첽����������</returns>
    public async Task ExecuteTransactionAsync(Action<Realm> action)
    {
        await ExecuteTransactionImplAsync(action);
    }

    /// <summary>
    /// ������������ Realm ����
    /// </summary>
    /// <typeparam name="T">�̳��� RealmObject ����������</typeparam>
    /// <typeparam name="TKey">��������������</typeparam>
    /// <param name="primaryKey">Ҫ���ҵ�����ֵ</param>
    /// <returns>�ҵ��Ķ�������������򷵻� null</returns>
    public T FindByPrimaryKey<T, TKey>(TKey primaryKey) where T : RealmObject
    {
        return FindByPrimaryKeyImpl<T, TKey>(primaryKey);
    }

    /// <summary>
    /// �첽������������ Realm ����
    /// </summary>
    /// <typeparam name="T">�̳��� RealmObject ����������</typeparam>
    /// <typeparam name="TKey">��������������</typeparam>
    /// <param name="primaryKey">Ҫ���ҵ�����ֵ</param>
    /// <returns>�����ҵ��Ķ������������������򷵻� null</returns>
    public async Task<T> FindByPrimaryKeyAsync<T, TKey>(TKey primaryKey) where T : RealmObject
    {
        return await FindByPrimaryKeyImplAsync<T, TKey>(primaryKey);
    }

    /// <summary>
    /// �رղ��ͷ� Realm ���ݿ�ʵ��
    /// </summary>
    public void CloseRealm()
    {
        CloseRealmImpl();
    }

    /// <summary>
    /// �첽�رղ��ͷ� Realm ���ݿ�ʵ��
    /// </summary>
    /// <returns>��ʾ�첽����������</returns>
    public async Task CloseRealmAsync()
    {
        await CloseRealmImplAsync();
    }

    #endregion

    #region ��ݷ��� - Ϊ�����������

    /// <summary>
    /// �����ַ��������������Ҷ���ı�ݷ���
    /// </summary>
    /// <typeparam name="T">�̳��� RealmObject ����������</typeparam>
    /// <param name="primaryKey">�ַ������͵�����ֵ</param>
    /// <returns>�ҵ��Ķ�������������򷵻� null</returns>
    public T FindByStringPrimaryKey<T>(string primaryKey) where T : RealmObject
    {
        return FindByPrimaryKey<T, string>(primaryKey);
    }

    /// <summary>
    /// �첽�����ַ��������������Ҷ���ı�ݷ���
    /// </summary>
    /// <typeparam name="T">�̳��� RealmObject ����������</typeparam>
    /// <param name="primaryKey">�ַ������͵�����ֵ</param>
    /// <returns>�����ҵ��Ķ������������������򷵻� null</returns>
    public Task<T> FindByStringPrimaryKeyAsync<T>(string primaryKey) where T : RealmObject
    {
        return FindByPrimaryKeyAsync<T, string>(primaryKey);
    }

    /// <summary>
    /// ���ݳ������������Ҷ���ı�ݷ���
    /// </summary>
    /// <typeparam name="T">�̳��� RealmObject ����������</typeparam>
    /// <param name="primaryKey">�����͵�����ֵ</param>
    /// <returns>�ҵ��Ķ�������������򷵻� null</returns>
    public T FindByLongPrimaryKey<T>(long primaryKey) where T : RealmObject
    {
        return FindByPrimaryKey<T, long>(primaryKey);
    }

    /// <summary>
    /// �첽���ݳ������������Ҷ���ı�ݷ���
    /// </summary>
    /// <typeparam name="T">�̳��� RealmObject ����������</typeparam>
    /// <param name="primaryKey">�����͵�����ֵ</param>
    /// <returns>�����ҵ��Ķ������������������򷵻� null</returns>
    public Task<T> FindByLongPrimaryKeyAsync<T>(long primaryKey) where T : RealmObject
    {
        return FindByPrimaryKeyAsync<T, long>(primaryKey);
    }

    /// <summary>
    /// ���� Guid �����������Ҷ���ı�ݷ���
    /// </summary>
    /// <typeparam name="T">�̳��� RealmObject ����������</typeparam>
    /// <param name="primaryKey">Guid ���͵�����ֵ</param>
    /// <returns>�ҵ��Ķ�������������򷵻� null</returns>
    public T FindByGuidPrimaryKey<T>(Guid primaryKey) where T : RealmObject
    {
        return FindByPrimaryKey<T, Guid>(primaryKey);
    }

    /// <summary>
    /// �첽���� Guid �����������Ҷ���ı�ݷ���
    /// </summary>
    /// <typeparam name="T">�̳��� RealmObject ����������</typeparam>
    /// <param name="primaryKey">Guid ���͵�����ֵ</param>
    /// <returns>�����ҵ��Ķ������������������򷵻� null</returns>
    public Task<T> FindByGuidPrimaryKeyAsync<T>(Guid primaryKey) where T : RealmObject
    {
        return FindByPrimaryKeyAsync<T, Guid>(primaryKey);
    }

    /// <summary>
    /// ���� ObjectId �����������Ҷ���ı�ݷ���
    /// </summary>
    /// <typeparam name="T">�̳��� RealmObject ����������</typeparam>
    /// <param name="primaryKey">ObjectId ���͵�����ֵ</param>
    /// <returns>�ҵ��Ķ�������������򷵻� null</returns>
    public T FindByObjectIdPrimaryKey<T>(ObjectId primaryKey) where T : RealmObject
    {
        return FindByPrimaryKey<T, ObjectId>(primaryKey);
    }

    /// <summary>
    /// �첽���� ObjectId �����������Ҷ���ı�ݷ���
    /// </summary>
    /// <typeparam name="T">�̳��� RealmObject ����������</typeparam>
    /// <param name="primaryKey">ObjectId ���͵�����ֵ</param>
    /// <returns>�����ҵ��Ķ������������������򷵻� null</returns>
    public Task<T> FindByObjectIdPrimaryKeyAsync<T>(ObjectId primaryKey) where T : RealmObject
    {
        return FindByPrimaryKeyAsync<T, ObjectId>(primaryKey);
    }

    #endregion

    /// <summary>
    /// Unity �������ڷ��� - �������ʱ����
    /// ȷ����ȷ�ر����ݿ�����
    /// </summary>
    private void OnDestroy()
    {
        CloseRealm();
    }
}
