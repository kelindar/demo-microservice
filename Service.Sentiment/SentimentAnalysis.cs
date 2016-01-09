using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Concurrent;

namespace Service.Sentiment
{
    /// <summary>
    /// Represents a text sentiment.
    /// </summary>
    [Serializable]
    public sealed class SentimentAnalysis
    {
        #region Constructor
        /// <summary>
        /// Constructs a new text sentiment.
        /// </summary>
        /// <param name="tokens">The list of tokens on which the sentiment was scored.</param>
        public SentimentAnalysis(IEnumerable<string> tokens)
        {
            this.Tokens = tokens;
            this.Words = new List<string>();
            this.Negative = new List<string>();
            this.Positive = new List<string>();
        }
        #endregion

        #region Properties

        /// <summary>
        /// Tokens which were scored
        /// </summary>
        public IEnumerable<string> Tokens
        {
            get;
            set;
        }

        /// <summary>
        /// Total sentiment score of the tokens
        /// </summary>
        public int Sentiment
        {
            get;
            set;
        }

        /// <summary>
        /// Average sentiment score Sentiment/Tokens.Count
        /// </summary>
        public double AverageSentimentTokens
        {
            get
            {
                var tokens = Tokens.Count();
                if (tokens == 0)
                    return 0;
                return (double)Sentiment / tokens;
            }
        }

        /// <summary>
        /// Average sentiment score Sentiment/Words.Count
        /// </summary>
        public double AverageSentimentWords
        {
            get
            {
                var words = Words.Count();
                if (words == 0)
                    return 0;
                return (double)Sentiment / words;
            }
        }

        /// <summary>
        /// Words that were used from AFINN
        /// </summary>
        public IList<string> Words
        {
            get;
            set;
        }

        /// <summary>
        /// Words that had negative sentiment
        /// </summary>
        public IList<string> Negative
        {
            get;
            set;
        }

        /// <summary>
        /// Words that had positive sentiment
        /// </summary>
        public IList<string> Positive
        {
            get;
            set;
        }
        #endregion

        #region Static Members
        /// <summary>
        /// The scoring word map.
        /// </summary>
        private static readonly ConcurrentDictionary<string, int> English;

        /// <summary>
        /// Static constructor.
        /// </summary>
        static SentimentAnalysis()
        {
            // Map the english language
            English = MapLanguage(File.ReadAllText("Database.txt"));
        }

        /// <summary>
        /// Maps the language by reading from a corpus of words.
        /// </summary>
        /// <param name="corpus">The tab separated list of annotated sentiment words.</param>
        /// <returns>The dictionary with annotations.</returns>
        private static ConcurrentDictionary<string, int> MapLanguage(string corpus)
        {
            var map = new ConcurrentDictionary<string, int>();
            try
            {
                using (var file = new StringReader(corpus))
                {
                    string line;
                    while ((line = file.ReadLine()) != null)
                    {
                        var bits = line.Split('\t');
                        map.TryAdd(bits[0], int.Parse(bits[1]));
                    }
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error during parsing the sentiment corpus.", ex);
            }
            return map;
        }

        /// <summary>
        /// Tokenizes a string. This method first removes non-alpha characters,
        /// removes multiple spaces, and lowercases every word. Then splits the
        /// string into an array of words.
        /// </summary>
        /// <param name="input">String to be tokenized</param>
        /// <returns>Array of words (tokens)</returns>
        private static IEnumerable<string> Tokenize(string input)
        {
            input = Regex.Replace(input, "[^a-zA-Z ]+", "");
            input = Regex.Replace(input, @"\s+", " ");
            input = input.ToLower();
            return input.Split(' ');
        }

        /// <summary>
        /// Calculates sentiment score of a sentence
        /// </summary>
        /// <param name="input">Sentence</param>
        /// <returns>Score object</returns>
        public static SentimentAnalysis Analyze(string input)
        {
            var score = new SentimentAnalysis(Tokenize(input));

            foreach (var token in score.Tokens)
            {
                if (!English.ContainsKey(token)) continue;

                var item = English[token];
                score.Words.Add(token);

                if (item > 0) score.Positive.Add(token);
                if (item < 0) score.Negative.Add(token);

                score.Sentiment += item;
            }

            return score;
        }
        #endregion

    }
}