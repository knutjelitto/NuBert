namespace Lingu.Check
{
    public class Checker
    {
        public void Check()
        {
            Check2();
        }

        private void Check1()
        {
            new EngineChecker().Check();
        }

        private void Check2()
        {
            new ExprChecker().Check();
        }
    }
}