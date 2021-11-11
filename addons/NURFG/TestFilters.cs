using NUnit.Framework.Interfaces;

namespace NURFG
{
    public class MatchEverythingTestFilter : ITestFilter
    {
        public TNode AddToXml(TNode parentNode, bool recursive) => null;
        public TNode ToXml(bool recursive) => null;

        public bool IsExplicitMatch(ITest test) => true;
        public bool Pass(ITest test) => true;
    }

    public class MatchDescendantsOfFilter : ITestFilter
    {
        private readonly ITest _possibleParent;

        public MatchDescendantsOfFilter(ITest test)
        {
            _possibleParent = test;
        }

        public TNode AddToXml(TNode parentNode, bool recursive) => null;
        public TNode ToXml(bool recursive) => null;

        public bool IsExplicitMatch(ITest test) => IsDescendantOf(_possibleParent, test);
        public bool Pass(ITest test) => IsDescendantOf(_possibleParent, test);

        private bool IsDescendantOf(ITest possibleParent, ITest possibleChild)
        {
            if (possibleChild == possibleParent)
                return true;
            
            if (possibleChild.Parent == null)
                return false;

            return IsDescendantOf(possibleParent, possibleChild.Parent);
        }
    }
}
