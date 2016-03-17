using System.Collections.Generic;
using System.Linq;

namespace TrustPilot_Challenge {
    public class Node {
        public int MinWordSize { get; set; } = 4;
        //private const int MinWordSize = 4;
        private readonly string _letter;
        private readonly bool _final;
        private readonly int _depth;
        private readonly Dictionary<string, Node> _children;

        private Node(string letter, bool final, int depth) {
            _letter = letter;
            _final = final;
            _depth = depth;
            _children = new Dictionary<string, Node>();
        }

        public Node() : this(string.Empty, false, 0) { }

        public void Add(string letters) {
            var node = this;
            for (var i = 0; i < letters.Length; i++) {
                var letter = letters[i].ToString();
                if (!node._children.ContainsKey(letter))
                    node._children.Add(letter, new Node(letter, i == letters.Length - 1, i + 1));
                node = node._children[letter];
            }
        }

        public IEnumerable<string> Anagram(string letters) {
            var tiles = new Dictionary<string, int>();
            foreach (var letter in letters.Select(c => c.ToString())) {
                if (tiles.ContainsKey(letter))
                    tiles[letter] = tiles[letter] + 1;
                else
                    tiles.Add(letter, 1);
            }
            return Anagram(tiles, new Stack<string>(), this, letters.Length);
        }

        private IEnumerable<string> Anagram(IDictionary<string, int> tiles, Stack<string> path, Node root, int anagramCharacterLength) {
            if (_final && _depth >= MinWordSize) {
                var pathWord = string.Join("", path.Reverse());
                var pathWordLength = pathWord.Replace(" ", "").Length;
                if (pathWordLength >= anagramCharacterLength)
                    yield return pathWord;
                path.Push(" ");
                foreach (var word in root.Anagram(tiles, path, root, anagramCharacterLength)) {
                    yield return word;
                }
                path.Pop();
            }
            foreach (var node in _children.Values) {
                var letter = node._letter;
                var count = 0;
                if (tiles.ContainsKey(letter))
                    count = tiles[letter];
                if (count == 0) continue;
                tiles[letter] = count - 1;
                path.Push(letter);
                foreach (var word in node.Anagram(tiles, path, root, anagramCharacterLength)) {
                    yield return word;
                }
                path.Pop();
                tiles[letter] = count;
            }
        }
    }
}