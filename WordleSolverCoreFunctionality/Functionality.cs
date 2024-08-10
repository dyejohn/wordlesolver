using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordleSolverCoreFunctionality
{
    public class Functionality
    {
        public void FillWordList(List<string> wordList)
        {
            using (var wordsFile = new System.IO.FileStream("words_alpha.txt", FileMode.Open, FileAccess.Read))
            {
                using (var reader = new StreamReader(wordsFile))
                {
                    // for now, only pull 5 width words
                    wordList.AddRange(reader.ReadToEnd().Split("\r\n").Where(x => x.Length == 5));
                }
            }
        }
        public List<KnownLetter> BuildNewAvailableLetters(List<char> listOfChars)
        {
            var response = new List<KnownLetter>();
            foreach(var individualChar in listOfChars)
            {
                response.Add(new KnownLetter() { Letter = individualChar, knownBadPositions = new List<int>(), knownGoodPositions = new List<int>() });
            }

            return response;
        }

        public Dictionary<int, char> findKnownGoodPositions(List<KnownLetter> availableLetters)
        {
            var response = new Dictionary<int, char>();
            foreach (var letter in availableLetters)
            {
                foreach (var pos in letter.knownGoodPositions)
                {
                    response.Add(pos, letter.Letter);
                }
            }
            return response;
        }

        public bool ComparisonLetters(string word, List<char> allowedLetters)
        {
            foreach (var c in word)
            {
                if (!allowedLetters.Contains(c))
                {
                    return false;
                }
            }
            return true;
        }

        public bool negativeletterUnmatch(string word, List<KnownLetter> availableLetters)
        {
            for (int i = 0; i < word.Length; i++)
            {
                var letter = availableLetters.SingleOrDefault(x => x.Letter == word[i]);
                var positionMatch = i + 1; // just to make this less confusing. humans are 1 based.

                if (letter == null)
                {
                    Console.WriteLine($"error in search {letter} is not an available letter");
                    return false;
                }

                if (letter.knownBadPositions.Contains(positionMatch))
                {
                    // this is a word we know won't work.
                    return false;
                }
            }

            return true;
        }

        public List<char> GetBadLetterList(List<KnownLetter> availableLetters)
        {
            List<char> response = new List<char>();
            foreach(var letter in availableLetters)
            { 
                if(letter.knownBadPositions.Count > 0)
                {
                    response.Add(letter.Letter);
                }
            }

            return response;

        }

    }
}
