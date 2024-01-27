
using System.ComponentModel;

public enum SpellType
{
    /// <summary>
    /// 锁定 [已完成]
    /// </summary>
    [Description("锁定")]
    Lock = 0,

    /// <summary>
    /// 调虎离山 [已完成]
    /// </summary>
    [Description("调虎离山")]
    Away = 1,

    /// <summary>
    /// 增援 [已完成]
    /// </summary>
    [Description("增援")]
    Help = 2,

    /// <summary>
    /// 转移 [已完成]
    /// </summary>
    [Description("转移")]
    Redirect = 3,

    /// <summary>
    /// 博弈
    /// </summary>
    [Description("博弈")]
    Gamble = 4,

    /// <summary>
    /// 截获
    /// </summary>
    [Description("截获")]
    Intercept = 5,

    /// <summary>
    /// 试探
    /// </summary>
    [Description("试探")]
    Test = 6,

    /// <summary>
    /// 烧毁情报
    /// </summary>
    [Description("烧毁")]
    Burn = 7,

    /// <summary>
    /// 调包
    /// </summary>
    [Description("调包")]
    Swap = 8,

    /// <summary>
    /// 破译
    /// </summary>
    [Description("破译")] 
    Decrypt = 9,

    /// <summary>
    /// 真伪莫辨
    /// </summary>
    [Description("真伪莫辨")] 
    GambleAll = 10,

    /// <summary>
    /// 公开文本
    /// </summary>
    [Description("公开文本")]
    PublicContent = 11,

    /// <summary>
    /// 识破
    /// </summary>
    [Description("识破")]
    Cancel = 12,

    /// <summary>
    /// 权衡
    /// </summary>
    [Description("权衡")]
    Trade = 13,

    /// <summary>
    /// 烧毁功能牌
    /// </summary>
    [Description("烧毁功能牌")]
    BurnSpell = 14,
}
