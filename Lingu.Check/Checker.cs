namespace Lingu.Check
{
    public class Checker
    {
        public void Check()
        {
            Check1();
        }

        private void Check1()
        {
            new EngineChecker().Check();
        }
    }
}