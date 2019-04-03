namespace Lingu.Check
{
    public class Checker
    {
        public void Check()
        {
            Check3();
        }

        private void Check1()
        {
            new EngineChecker().Check();
        }

        private void Check2()
        {
            new ExprChecker().Check();
        }

        private void Check3()
        {
            new AmbiChecker().Check();
        }
    }
}