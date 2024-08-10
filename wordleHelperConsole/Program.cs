// See https://aka.ms/new-console-template for more information

// initialize the functionality class
using WordleHelper;
using WordleSolverCoreFunctionality;


List<string> wordList = new List<string>();
List<char> alphabetChars = "abcdefghijklmnopqrstuvwxyz".ToList();


var functionality = new Functionality();


// load the dictionary
functionality.FillWordList(wordList);


// rebuild the list of known letters.
List<KnownLetter> availableLetters = functionality.BuildNewAvailableLetters(alphabetChars);


bool endApplication = false;

while(!endApplication)
{
    Console.Write("command:");
    var command = Console.ReadLine().ToLower().Trim();

    if (command == "help")
    {
        Console.WriteLine(" commands \n reset = clear everything in the known letters. \n current = show all the known letters. \n -X = remove a letter that is not found anywhere \n +X = readd a letter with no known positions");
        Console.WriteLine(" X-# = add a known bad for a letter. \n X+# = add a known good position for a letter. \n X*# = remove that number position from X \n cover = find a word that has the most unused letters ");
        Console.WriteLine(" bestguess = find a word that matches known positions or known letters ");
    }

    if (command == "current")
    {
        // TODO: Sort this so it's easier to read after removing and adding.
        foreach(var letter in availableLetters) 
        {
            Console.Write($"{letter.Letter} Good: ");
            foreach(var pos in letter.knownGoodPositions) 
            {
                Console.Write($"{pos},");
            }
        
            if(letter.knownGoodPositions.Count == 0)
            {
                Console.Write("None ");
            }

            Console.Write(" Bad: ");

            foreach (var pos in letter.knownBadPositions)
            {
                Console.Write($"{pos},");
            }

            if (letter.knownBadPositions.Count == 0)
            {
                Console.Write("None ");
            }
            Console.WriteLine();

        }
    }

    if (command == "reset")
    {
        availableLetters = functionality.BuildNewAvailableLetters(alphabetChars);
    }

    if (command[0] == '-')
    {
        // get the letter from the command
        foreach (var letter in command.Substring(1))
        {
        
        var knownLetter = availableLetters.FirstOrDefault(x => x.Letter == letter);
            if (knownLetter == null)
            {
                Console.WriteLine($"{letter} does not exist");
            }
            else
            {
                availableLetters.Remove(knownLetter);
            }
        }
    }

    if (command[0] == '+')
    {
        // get the letter from the command
        foreach (var letter in command.Substring(1))
        {

            var knownLetter = availableLetters.FirstOrDefault(x => x.Letter == letter);
            if (knownLetter != null)
            {
                Console.WriteLine($"{letter} already exists");
            }
            else
            {
                availableLetters.Add(new KnownLetter() { Letter = letter});
            }
        }
    }

    if (command[1] == '-')
    {
        var letter = command[0];

        if (letter > 96 && letter < 123)

        {
            var knownLetter = availableLetters.FirstOrDefault(x => x.Letter == letter);
            if(knownLetter == null)
            {
                Console.WriteLine($"{letter} is not an active letter");
            }

            foreach(var num in command.Substring(2).Select(x => x-48))
            {
                knownLetter.knownBadPositions.Add(num);
            }
        }
    }

    if (command[1] == '+')
    {
        var letter = command[0];

        if (letter > 96 && letter < 123)

        {
            var knownLetter = availableLetters.FirstOrDefault(x => x.Letter == letter);
            if (knownLetter == null)
            {
                Console.WriteLine($"{letter} is not an active letter");
                
            }

            foreach (var num in command.Substring(2).Select(x => x-48))
            {
                knownLetter.knownGoodPositions.Add(num);
            }
        }
    }

    if (command[1] == '*')
    {
        var letter = command[0];

        if (letter > 96 && letter < 123)

        {
            var knownLetter = availableLetters.FirstOrDefault(x => x.Letter == letter);
            if (knownLetter == null)
            {
                Console.WriteLine($"{letter} is not an active letter");
            }

            foreach (var num in command.Substring(2).Select(x => x - 48))
            {
                if(knownLetter.knownBadPositions.Contains(num))
                {
                    knownLetter.knownBadPositions.Remove(num);
                }
                if(knownLetter.knownGoodPositions.Contains(num))
                {
                    knownLetter.knownGoodPositions.Remove(num);
                }
            }
        }
    }

    if (command == "guess" || command == "bestguess")
    {
        var currentLetterList = availableLetters.Select(x => x.Letter).ToList();

        // to improve speed, work out the known good positions.
        
        var knownGoodPositions = functionality.findKnownGoodPositions(availableLetters);
        var badLetterList = functionality.GetBadLetterList(availableLetters);

        bool ComparisonLetters(string word)
        {
            return functionality.ComparisonLetters(word, currentLetterList);
        }

        bool filterKnownBadLetters(string word)
        {
            return functionality.negativeletterUnmatch(word, availableLetters);
        }

        bool filterNotMatchKnownGood(string word)
        {
            for(int i=0; i < word.Length; i++)
            {
                var positionMatch = i + 1;
                if (knownGoodPositions.ContainsKey(positionMatch) && knownGoodPositions[positionMatch] != word[i])
                {
                    return false;
                }
                    
            }

            return true;
        }

        bool badlettersMustExist(string word)
        {
            foreach(var letter in badLetterList)
            {
                if(!word.Contains(letter))
                {
                    return false;
                }
            }

            return true;
        }

        var filteredWordList = wordList.Where(word => ComparisonLetters(word) && filterNotMatchKnownGood(word) && filterKnownBadLetters(word) && badlettersMustExist(word)).ToList();

        var outputLimit = 50;

        if (filteredWordList.Count < 50)
        {
            outputLimit = filteredWordList.Count;
        }
        for (int i = 0; i < outputLimit; i++)
        {
            Console.WriteLine($"{filteredWordList[i]}");
        }

    }

    if (command == "cover")
    {
        var currentLetterList = availableLetters.Select(x => x.Letter).ToList();

        foreach(var letter in functionality.findKnownGoodPositions(availableLetters).Select(x => x.Value).ToList())
        {
            currentLetterList.Remove(letter);
        }

        foreach(var letter in functionality.GetBadLetterList(availableLetters))
        {
            currentLetterList.Remove(letter);
        }
        
        bool ComparisonLetters(string word)
        {
            return functionality.ComparisonLetters(word, currentLetterList);
        }

        bool negativeletterUnmatch(string word)
        {
            return functionality.negativeletterUnmatch(word, availableLetters);
        }        



        ScoredWord scoreWord(string word)
        {
            var returnValue = 30;

            var findDuplicates = new HashSet<char>();

            foreach (char c in word)
            {
                var knownLetter = availableLetters.Single(x => x.Letter == c);
                returnValue -= knownLetter.knownGoodPositions.Count;
                returnValue -= knownLetter.knownBadPositions.Count;
                findDuplicates.Add(c);
            }

            // also decrease for duplicate letters.
            returnValue -= ((5 - findDuplicates.Count)*2);

            return new ScoredWord() {  Score = returnValue, Word = word };
        }

        // remove all the words that have a disallowed letter.
        var filteredWordList = wordList.Where(word => ComparisonLetters(word) && negativeletterUnmatch(word)).Select(word => scoreWord(word)).OrderByDescending(word => word.Score).ToList();

        var outputLimit = 50;

        if (filteredWordList.Count < 50)
        {
            outputLimit = filteredWordList.Count;
        }
        for(int i =0; i < outputLimit; i++) 
        {
            Console.WriteLine($"{filteredWordList[i].Word}");
        }

    }
}

