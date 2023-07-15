namespace Clientes.Domain.Common;

public abstract class BaseValueObject
{
    protected static bool EqualOperator(BaseValueObject left, BaseValueObject right)
    {
        if (ReferenceEquals(left, null) ^ ReferenceEquals(right, null))
            return false;

        return ReferenceEquals(left, right) || left!.Equals(right);
    }

    protected static bool NotEqualOperator(BaseValueObject left, BaseValueObject right) => !EqualOperator(left, right);

    protected abstract IEnumerable<object?> GetEqualityComponents();

    public override bool Equals(object? obj)
    {
        if (obj == null || obj.GetType() != GetType())
            return false;

        var other = (BaseValueObject)obj;

        return GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());
    }

    public override int GetHashCode()
    {
        return GetEqualityComponents()
            .Select(x => x?.GetHashCode() ?? 0)
            .Aggregate((x, y) => x ^ y);
    }

    public static bool operator ==(BaseValueObject left, BaseValueObject right) => EqualOperator(left, right);

    public static bool operator !=(BaseValueObject left, BaseValueObject right) => NotEqualOperator(left, right);
}