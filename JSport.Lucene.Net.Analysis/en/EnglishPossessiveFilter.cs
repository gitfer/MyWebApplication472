using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Tokenattributes;

namespace JSport.Lucene.Net.Analysis.En
{
    public sealed class EnglishPossessiveFilter : TokenFilter
    {
        private readonly ITermAttribute termAtt;

        public EnglishPossessiveFilter(TokenStream input)
          : base(input)
        {
            termAtt = AddAttribute<ITermAttribute>();
        }

        public override bool IncrementToken()
        {
            if (input.IncrementToken())
            {
                char[] buffer = termAtt.TermBuffer();
                int bufferLength = termAtt.TermLength();

                if (bufferLength >= 2 && (buffer[bufferLength - 2] == '\'' || ((buffer[bufferLength - 2] == '\u2019' || buffer[bufferLength - 2] == '\uFF07'))) && (buffer[bufferLength - 1] == 's' || buffer[bufferLength - 1] == 'S'))
                    termAtt.SetTermLength(bufferLength - 2); // Strip last 2 characters off
                return true;
            }
            return false;
        }
    }
}