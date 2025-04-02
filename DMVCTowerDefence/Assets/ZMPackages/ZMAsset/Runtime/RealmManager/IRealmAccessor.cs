using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Realms;

/// <summary>
/// Realm ���ݿ�����ӿڣ������˶� Realm ���ݿ���� CRUD �����ı�׼������
/// </summary>
public interface IRealmAccessor
{
    /// <summary>
    /// ��ȡ Realm ���ݿ�ʵ��
    /// </summary>
    /// <returns>Realm ���ݿ�ʵ��</returns>
    Realm GetRealm();
    
    /// <summary>
    /// �첽��ȡ Realm ���ݿ�ʵ��
    /// </summary>
    /// <returns>���� Realm ���ݿ�ʵ��������</returns>
    Task<Realm> GetRealmAsync();
    
    /// <summary>
    /// ���ӻ���µ��� Realm ����
    /// </summary>
    /// <typeparam name="T">�̳��� RealmObject ����������</typeparam>
    /// <param name="obj">Ҫ���ӻ���µĶ���</param>
    void AddOrUpdate<T>(T obj) where T : RealmObject;
    
    /// <summary>
    /// �첽���ӻ���µ��� Realm ����
    /// </summary>
    /// <typeparam name="T">�̳��� RealmObject ����������</typeparam>
    /// <param name="obj">Ҫ���ӻ���µĶ���</param>
    /// <returns>��ʾ�첽����������</returns>
    Task AddOrUpdateAsync<T>(T obj) where T : RealmObject;
    
    /// <summary>
    /// �������ӻ���¶�� Realm ����
    /// </summary>
    /// <typeparam name="T">�̳��� RealmObject ����������</typeparam>
    /// <param name="objects">Ҫ���ӻ���µĶ��󼯺�</param>
    void AddOrUpdateAll<T>(IEnumerable<T> objects) where T : RealmObject;
    
    /// <summary>
    /// �첽�������ӻ���¶�� Realm ����
    /// </summary>
    /// <typeparam name="T">�̳��� RealmObject ����������</typeparam>
    /// <param name="objects">Ҫ���ӻ���µĶ��󼯺�</param>
    /// <returns>��ʾ�첽����������</returns>
    Task AddOrUpdateAllAsync<T>(IEnumerable<T> objects) where T : RealmObject;
    
    /// <summary>
    /// ɾ������ Realm ����
    /// </summary>
    /// <typeparam name="T">�̳��� RealmObject ����������</typeparam>
    /// <param name="obj">Ҫɾ���Ķ���</param>
    void Delete<T>(T obj) where T : RealmObject;
    
    /// <summary>
    /// �첽ɾ������ Realm ����
    /// </summary>
    /// <typeparam name="T">�̳��� RealmObject ����������</typeparam>
    /// <param name="obj">Ҫɾ���Ķ���</param>
    /// <returns>��ʾ�첽����������</returns>
    Task DeleteAsync<T>(T obj) where T : RealmObject;
    
    /// <summary>
    /// ����ɾ����� Realm ����
    /// </summary>
    /// <typeparam name="T">�̳��� RealmObject ����������</typeparam>
    /// <param name="objects">Ҫɾ���Ķ��󼯺�</param>
    void DeleteAll<T>(IEnumerable<T> objects) where T : RealmObject;
    
    /// <summary>
    /// �첽����ɾ����� Realm ����
    /// </summary>
    /// <typeparam name="T">�̳��� RealmObject ����������</typeparam>
    /// <param name="objects">Ҫɾ���Ķ��󼯺�</param>
    /// <returns>��ʾ�첽����������</returns>
    Task DeleteAllAsync<T>(IEnumerable<T> objects) where T : RealmObject;
    
    /// <summary>
    /// ɾ��ָ�����͵����� Realm ����
    /// </summary>
    /// <typeparam name="T">�̳��� RealmObject ����������</typeparam>
    void DeleteAll<T>() where T : RealmObject;
    
    /// <summary>
    /// �첽ɾ��ָ�����͵����� Realm ����
    /// </summary>
    /// <typeparam name="T">�̳��� RealmObject ����������</typeparam>
    /// <returns>��ʾ�첽����������</returns>
    Task DeleteAllAsync<T>() where T : RealmObject;
    
    /// <summary>
    /// ��ȡָ�����͵����� Realm ����
    /// </summary>
    /// <typeparam name="T">�̳��� RealmObject ����������</typeparam>
    /// <returns>�������ж���Ŀɲ�ѯ����</returns>
    IQueryable<T> GetAll<T>() where T : RealmObject;
    
    /// <summary>
    /// �첽��ȡָ�����͵����� Realm ����
    /// </summary>
    /// <typeparam name="T">�̳��� RealmObject ����������</typeparam>
    /// <returns>�������ж���Ŀɲ�ѯ���ϵ�����</returns>
    Task<IQueryable<T>> GetAllAsync<T>() where T : RealmObject;
    
    /// <summary>
    /// ʹ��ν�ʺ�����ѯ Realm ����
    /// </summary>
    /// <typeparam name="T">�̳��� RealmObject ����������</typeparam>
    /// <param name="predicate">����ɸѡ�������������</param>
    /// <returns>���������Ķ��󼯺�</returns>
    IQueryable<T> Query<T>(Func<T, bool> predicate) where T : RealmObject;
    
    /// <summary>
    /// �첽ʹ��ν�ʺ�����ѯ Realm ����
    /// </summary>
    /// <typeparam name="T">�̳��� RealmObject ����������</typeparam>
    /// <param name="predicate">����ɸѡ�������������</param>
    /// <returns>�������������Ķ��󼯺ϵ�����</returns>
    Task<IQueryable<T>> QueryAsync<T>(Func<T, bool> predicate) where T : RealmObject;
    
    /// <summary>
    /// ʹ�� LINQ ����ʽ��ѯ Realm ����
    /// </summary>
    /// <typeparam name="T">�̳��� RealmObject ����������</typeparam>
    /// <param name="query">LINQ ��ѯ����ʽ</param>
    /// <returns>��ѯ�������</returns>
    IQueryable<T> QueryWithLinq<T>(Func<IQueryable<T>, IQueryable<T>> query) where T : RealmObject;
    
    /// <summary>
    /// �첽ʹ�� LINQ ����ʽ��ѯ Realm ����
    /// </summary>
    /// <typeparam name="T">�̳��� RealmObject ����������</typeparam>
    /// <param name="query">LINQ ��ѯ����ʽ</param>
    /// <returns>������ѯ������ϵ�����</returns>
    Task<IQueryable<T>> QueryWithLinqAsync<T>(Func<IQueryable<T>, IQueryable<T>> query) where T : RealmObject;
    
    /// <summary>
    /// ��������ִ�����ݿ����
    /// </summary>
    /// <param name="action">Ҫ��������ִ�еĲ���</param>
    void ExecuteTransaction(Action<Realm> action);
    
    /// <summary>
    /// �첽��������ִ�����ݿ����
    /// </summary>
    /// <param name="action">Ҫ��������ִ�еĲ���</param>
    /// <returns>��ʾ�첽����������</returns>
    Task ExecuteTransactionAsync(Action<Realm> action);
    
    /// <summary>
    /// ������������ Realm ����
    /// </summary>
    /// <typeparam name="T">�̳��� RealmObject ����������</typeparam>
    /// <typeparam name="TKey">��������������</typeparam>
    /// <param name="primaryKey">Ҫ���ҵ�����ֵ</param>
    /// <returns>�ҵ��Ķ�������������򷵻� null</returns>
    T FindByPrimaryKey<T, TKey>(TKey primaryKey) where T : RealmObject;
    
    /// <summary>
    /// �첽������������ Realm ����
    /// </summary>
    /// <typeparam name="T">�̳��� RealmObject ����������</typeparam>
    /// <typeparam name="TKey">��������������</typeparam>
    /// <param name="primaryKey">Ҫ���ҵ�����ֵ</param>
    /// <returns>�����ҵ��Ķ������������������򷵻� null</returns>
    Task<T> FindByPrimaryKeyAsync<T, TKey>(TKey primaryKey) where T : RealmObject;
    
    /// <summary>
    /// �رղ��ͷ� Realm ���ݿ�ʵ��
    /// </summary>
    void CloseRealm();
    
    /// <summary>
    /// �첽�رղ��ͷ� Realm ���ݿ�ʵ��
    /// </summary>
    /// <returns>��ʾ�첽����������</returns>
    Task CloseRealmAsync();
}
