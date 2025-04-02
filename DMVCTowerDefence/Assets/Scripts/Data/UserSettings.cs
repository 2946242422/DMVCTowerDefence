using Realms;
using System;

public class UserSettings : RealmObject
{
    [PrimaryKey]
    public string Id { get; set; }
    
    public string SavedAccount { get; set; }
    public string EncryptedPassword { get; set; } // ����ԭ�ֶ�����������ֱ�Ӵ洢����
    public bool RememberPassword { get; set; }
    public DateTimeOffset LastLoginTime { get; set; }
    
    // Ϊ�����ݱ���������
    [Ignored] 
    public string DecryptedPassword { get; set; }
}