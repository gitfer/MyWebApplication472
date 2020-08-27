using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Tokenattributes;

namespace JSport.Lucene.Net.Analysis.En
{
    public sealed class PorterStemFilter : TokenFilter
    {
        private readonly PorterStemmer stemmer;
        private readonly ITermAttribute termAtt;

        public PorterStemFilter(TokenStream input)
          : base(input)
        {
            termAtt = AddAttribute<ITermAttribute>();
            stemmer = new PorterStemmer();
        }

        public override bool IncrementToken()
        {
            if (input.IncrementToken())
            {
                if (stemmer.Stem(termAtt.TermBuffer(), termAtt.TermLength()))
                    termAtt.SetTermBuffer(stemmer.ToString());
                return true;
            }
            return false;
        }
    }
}