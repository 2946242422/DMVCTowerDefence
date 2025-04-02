using Realms;
using System;

public class UserSettings : RealmObject
{
    [PrimaryKey]
    public string Id { get; set; }
    
    public string SavedAccount { get; set; }
    public string EncryptedPassword { get; set; } // 保留原字段名，但现在直接存储密码
    public bool RememberPassword { get; set; }
    public DateTimeOffset LastLoginTime { get; set; }
    
    // 为向后兼容保留此属性
    [Ignored] 
    public string DecryptedPassword { get; set; }
}