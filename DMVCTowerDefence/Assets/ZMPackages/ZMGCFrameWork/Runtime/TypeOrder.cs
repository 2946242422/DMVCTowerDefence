using System;


// TypeOrder 类用于封装一个类型及其排序顺序
public class TypeOrder
{
    // 只读的整数类型，表示该类型的排序顺序
    // 使用 readonly 关键字表示该成员变量只能在构造函数中赋值，赋值后不可修改
    public readonly int order;

    // 只读的 Type 类型，表示要排序的类型
    // 使用 readonly 关键字表示该成员变量只能在构造函数中赋值，赋值后不可修改
    public readonly Type type;

    // TypeOrder 类的构造函数
    // 用于初始化 order 和 type 成员变量
    // 参数：
    //   order：类型的排序顺序
    //   type：要排序的类型
    public TypeOrder(int order, Type type)
    {
        // 将传入的 order 参数赋值给类的 order 成员变量
        this.order = order;
        // 将传入的 type 参数赋值给类的 type 成员变量
        this.type = type;
    }
}