using System;
using System.Collections.Generic;
using FubuCore;
using ripple.Model;

namespace ripple.Local
{
    public class RippleRunner
    {
        private readonly IRippleLogger _logger;
        private readonly IRippleStepRunner _runner;

        public RippleRunner(IRippleLogger logger, IRippleStepRunner runner)
        {
            _logger = logger;
            _runner = runner;
        }

        public void RunPlan(SolutionGraph graph, RipplePlanRequirements requirements)
        {
            var solutions = requirements.SelectSolutions(graph);
            var plan = new RipplePlan(solutions);

            var number = 0;
            try
            {
                plan.Each(step => runStep(step, ++number, plan.Count));
            }
            finally
            {
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine("The log file is at " + RippleFileSystem.RippleLogsDirectory().AppendPath("ripple.log"));
                Console.WriteLine("or type 'ripple open-log'");
                _logger.WriteLogFile("ripple.log");
                Console.WriteLine();
                Console.WriteLine();
            }
        }

        private void runStep(IRippleStep step, int number, int total)
        {
            _logger.Trace(string.Empty);
            _logger.Trace("{0} / {1} - {2}", number.ToString().PadLeft(3), total.ToString().PadLeft(3), step.ToString());

            _logger.Indent(() =>
            {
                try
                {
                    step.Execute(_runner);
                }
                catch (Exception ex)
                {
                    var start = Console.ForegroundColor;
                    Console.ForegroundColor = ConsoleColor.Red;

                    Console.WriteLine();
                    Console.WriteLine(ex.ToString());
                    Console.WriteLine();

                    Console.ForegroundColor = start;

                    throw;
                }
            });
        }
    }
}