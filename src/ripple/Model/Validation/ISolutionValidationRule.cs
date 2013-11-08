namespace ripple.Model.Validation
{
    public interface ISolutionValidationRule
    {
        void Validate(Solution solution, ValidationResult result);
    }
}