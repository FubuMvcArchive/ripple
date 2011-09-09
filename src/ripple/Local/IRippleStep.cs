namespace ripple.Local
{
    public interface IRippleStep
    {
        void Execute(IRippleStepRunner runner);
    }
}