using NUnit.Framework.Interfaces;

namespace GodotNUnitRunner
{
    public class MatchEverythingTestFilter : ITestFilter
    {
        public TNode AddToXml(TNode parentNode, bool recursive) => null;
        public TNode ToXml(bool recursive) => null;

        public bool IsExplicitMatch(ITest test) => true;
        public bool Pass(ITest test) => true;
    }

    public class MatchSpecificTestFilter : ITestFilter
    {
        private readonly ITest _test;

        public MatchSpecificTestFilter(ITest test)
        {
            _test = test;
        }

        public TNode AddToXml(TNode parentNode, bool recursive) => null;
        public TNode ToXml(bool recursive) => null;

        public bool IsExplicitMatch(ITest test) => test == _test;
        public bool Pass(ITest test) => test == _test;
    }
}
