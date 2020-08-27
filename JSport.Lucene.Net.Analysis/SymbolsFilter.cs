using System;
using System.Collections.Generic;
using System.Text;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Tokenattributes;

namespace JSport.Lucene.Net.Analysis
{
    public class SymbolsFilter : TokenFilter
    {
        private readonly ITermAttribute termAtt;
        private readonly IPositionIncrementAttribute posAtt;
        private State currentState;

        private readonly Queue<string> splittedQueue = new Queue<string>();

        //http://www.opten.ch/blog/2014/05/30/writing-a-custom-synonym-token-filter-in-lucenenet/
        public SymbolsFilter(TokenStream input)
          : base(input)
        {
            termAtt = AddAttribute<ITermAttribute>();
            posAtt = AddAttribute<IPositionIncrementAttribute>();
        }

        public override bool IncrementToken()
        {
            //Finchè ci sono termini nella coda ritorniamo e svuotiamo un temrine alla volta
            if (splittedQueue.Count > 0)
            {
                var splitted = splittedQueue.Dequeue();
                RestoreState(currentState);
                termAtt.SetTermBuffer(splitted);
                posAtt.PositionIncrement = 0;
                return true;
            }
            //Se siamo alla fine ci fermiamo subito
            if (!input.IncrementToken())
                return false;

            var currentTerm = new string(termAtt.TermBuffer(), 0, termAtt.TermLength());
            if (!string.IsNullOrEmpty(currentTerm))
            {
                var splittedWords = GetSplittedWord(currentTerm);
                //Non ci sono parole, non elaboriamo e proseguiamo
                if (splittedWords == null || splittedWords.Length == 0)
                    return true;
                foreach (var splittedWord in splittedWords)
                    splittedQueue.Enqueue(splittedWord);
            }
            currentState = CaptureState();
            return true;
        }

        private string[] GetSplittedWord(string term)
        {
            //Qui si può poi creare una lista per cui deve essere splittata la parola
            if (string.IsNullOrWhiteSpace(term) || term.IndexOf('-') == -1)
                return null;
            var result = term.Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
            if (result.Length == 1) //se solo una parola evito di generare un doppione
                return null;
            //Aggiungo anche la parola senza i -
            Array.Resize<string>(ref result, result.Length + 1);
            result[result.Length - 1] = term.Replace("-", "");
            return result;
        }
    }
}
