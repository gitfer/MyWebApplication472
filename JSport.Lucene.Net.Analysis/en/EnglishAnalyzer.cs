using System.Collections.Generic;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Version = Lucene.Net.Util.Version;

namespace JSport.Lucene.Net.Analysis.En
{
    public sealed class EnglishAnalyzer : Analyzer
    {
        private ISet<string> stemExclutionSet = global::Lucene.Net.Support.Compatibility.SetFactory.CreateHashSet<string>();
        private readonly Version matchVersion;

        private readonly ISet<string> stopTable;

        public static ISet<string> GetDefaultStopSet()
        {
            return DefaultSetHolder.DEFAULT_STOP_SET;
        }

        private static class DefaultSetHolder
        {
            internal static ISet<string> DEFAULT_STOP_SET = CharArraySet.UnmodifiableSet(new CharArraySet(StandardAnalyzer.STOP_WORDS_SET, false));
        }

        public EnglishAnalyzer(Version matchVersion)
          : this(matchVersion, DefaultSetHolder.DEFAULT_STOP_SET)
        {
        }

        public EnglishAnalyzer(Version matchVersion, ISet<string> stopwords)
          : this(matchVersion, stopwords, CharArraySet.EMPTY_SET)
        {
        }

        public EnglishAnalyzer(Version matchVersion, ISet<string> stopwords, ISet<string> stemExclusionSet)
        {
            this.matchVersion = matchVersion;
            this.stopTable = CharArraySet.UnmodifiableSet(CharArraySet.Copy(stopwords));
            this.stemExclutionSet = CharArraySet.UnmodifiableSet(CharArraySet.Copy(stemExclutionSet));
        }

        public override TokenStream TokenStream(string fieldName, System.IO.TextReader reader)
        {
            //TokenStream result = new StandardTokenizer(matchVersion, reader);
            TokenStream result = new WhitespaceTokenizer(@reader);
            result = new StandardFilter(result);
            result = new EnglishPossessiveFilter(result);
            result = new LowerCaseFilter(result);
            //La ricerca nel titolo deve indicizzare tutto, pertanto niente stopwords
            //result = new StopFilter(StopFilter.GetEnablePositionIncrementsVersionDefault(matchVersion), result, stopTable);
            result = new PorterStemFilter(result);
            //Per gestire la creazione di parole tagliando i simboli
            result = new SymbolsFilter(result);
            return result;
        }
    }
}