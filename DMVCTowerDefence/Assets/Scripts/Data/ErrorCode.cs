// ErrorCode.cs
public static class ErrorCode
{
    public const int Success = 0;
    public const int InvalidAccount = 1;
    public const int UserNotFound = 2;
    public const int PasswordIncorrect = 3;
    public const int OtherError = 4;
    public const int InvalidPassword = 5; // 新增：密码无效错误码
    public const int UsernameAlreadyExists = 6; // 新增：用户名已存在错误码
    // 可以根据需要添加更多错误码
}