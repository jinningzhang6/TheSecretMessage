using Assets.Scripts.Models;
using System;

/// <summary>
/// 转换传输的数据 变成可读文字
/// </summary>
public static class ObjectMapper
{
    public static SpellContent MapSpellContent(SpellContent origin, SpellContent target)
    {
        origin.IsCanceled = target.IsCanceled;
        origin.IsActive = target.IsActive;

        return origin;
    }

    public static T MapToObjectArray<T>(object[] dataArray) where T : new()
    {
        if (dataArray == null)
        {
            throw new ArgumentNullException(nameof(dataArray), "Data array is null.");
        }

        if (dataArray.Length != typeof(T).GetProperties().Length)
        {
            throw new ArgumentException("Data array length does not match the number of properties in the target type.");
        }

        T result = new T();

        var properties = typeof(T).GetProperties();
        for (int i = 0; i < properties.Length; i++)
        {
            var propertyType = properties[i].PropertyType;
            properties[i].SetValue(result, Convert.ChangeType(dataArray[i], propertyType));
        }

        return result;
    }

    public static SpellContent MapSpellEvent(SpellContent originalContent, SpellContent content)
    {
        originalContent.FromPlayer = content.FromPlayer;
        originalContent.ToPlayer = content.ToPlayer;
        originalContent.CurrPassingCardId = content.CurrPassingCardId;
        originalContent.ParentCardId = content.ParentCardId;
        originalContent.IsActive = content.IsActive;
        originalContent.IsCanceled = content.IsCanceled;

        originalContent.CurrPosition = content.CurrPosition;
        originalContent.PrevPosition = content.PrevPosition;
        originalContent.CurrDrawableCard = content.CurrDrawableCard;
        originalContent.PreviousDrawableCard = content.PreviousDrawableCard;

        return originalContent;
    }
}
