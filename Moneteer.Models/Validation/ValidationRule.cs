namespace Moneteer.Models.Validation
{
    public abstract class ValidationRule<T> where T: class
    {
        public abstract void Validate(T model);

    }
}
