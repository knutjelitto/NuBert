namespace Pliant.RegularExpressions
{
    public enum RegexIterator
    {
        /// <summary>
        /// optional?
        /// </summary>
        ZeroOrOne,

        /// <summary>
        /// plus-kleene+
        /// </summary>
        OneOrMany,

        /// <summary>
        /// star-kleene*
        /// </summary>
        ZeroOrMany
    }
}