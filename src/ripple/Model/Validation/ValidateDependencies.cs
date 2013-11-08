namespace ripple.Model.Validation
{
    public class ValidateDependencies : ISolutionValidationRule
    {
        public void Validate(Solution solution, ValidationResult result)
        {
            var child = solution.Dependencies.Validate();
            result.Import(child);
        }
    }
}