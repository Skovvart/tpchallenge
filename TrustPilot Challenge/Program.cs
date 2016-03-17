using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace TrustPilot_Challenge {
    /// <summary>
    /// See http://followthewhiterabbit.trustpilot.com/cs/step3.html
    /// </summary>
    class Program {
        private const string ChallengeMd5Hash = "4624d200580677270a54ccff86b9610e";
        private const string WordListFile = "wordlist";
        private const string AnagramPhrase = "poultry outwits ants";
        private static Node _trie;
        private static Stopwatch _sw;

        static void Main() {
            _trie = new Node();
            foreach (var word in File.ReadAllLines(WordListFile).Where(word => !string.IsNullOrEmpty(word))) {
                _trie.Add(word);
            }
            FindAllAnagrams("macklemore lewis");
            TrustPilotChallenge(ChallengeMd5Hash, AnagramPhrase);
            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
        }

        private static void FindAllAnagrams(string word) {
            _sw = Stopwatch.StartNew();
            _trie.MinWordSize = 1;
            word = word.ToLower().Replace(" ", "");
            var c = 0;
            foreach (var anagram in _trie.Anagram(word)) {
                Console.WriteLine(anagram);
                c++;
            }
            Console.WriteLine("Done - found {0} anagrams in {1}.", c, _sw.Elapsed);
        }

        private static void TrustPilotChallenge(string challengeMd5Hash, string anagramPhrase) {
            _trie.MinWordSize = 4;
            _sw = Stopwatch.StartNew();
            var letters = anagramPhrase.ToLower().Replace(" ", "");
            Console.WriteLine("Starting search for letters: {0}", letters);
            var c = 0;
            foreach (var anagram in _trie.Anagram(letters)) {
                if (c % 5000 == 0) Console.Write(".");
                var md5 = Md5(anagram);
                if (md5.Equals(challengeMd5Hash)) {
                    Console.WriteLine();
                    Console.WriteLine("Anagram {0,6:0}: MD5(\"{1}\") == {2}", c, anagram, md5);
                    break; //remove to iterate all anagrams
                }
                c++;
            }
            Console.WriteLine("Done - checked {0} anagrams in {1}.", c, _sw.Elapsed);
        }

        /// <summary>
        /// Calculate the MD5 hex-value of a string
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private static string Md5(string input) {
            var md5 = MD5.Create();
            var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(input));
            var sb = new StringBuilder();
            foreach (var b in hash) {
                sb.Append(b.ToString("x2"));
            }
            return sb.ToString();
        }
    }
}
