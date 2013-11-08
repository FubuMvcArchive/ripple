using System.Collections.Generic;

namespace ripple.Model.Validation
{
    public class SolutionValidator : ISolutionValidator
    {
        private static readonly IList<ISolutionValidationRule> Rules;

        static SolutionValidator()
        {
            Rules = new List<ISolutionValidationRule>();
            Reset();
        }

        public static void Reset()
        {
            Clear();
            RegisterRule<ValidateDependencies>();
        }

        public static void Clear()
        {
            Rules.Clear();
        }

        public static void RegisterRule<T>()
            where T : ISolutionValidationRule, new()
        {
            RegisterRule(new T());
        }

        public static void RegisterRule(ISolutionValidationRule rule)
        {
            Rules.Add(rule);
        }

        public ValidationResult Validate(Solution solution)
        {
            var result = new ValidationResult(solution);
            Rules.Each(x => x.Validate(solution, result));

            return result;
        }
    }
}