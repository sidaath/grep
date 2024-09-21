using System;
using System.IO;
using System.Reflection.PortableExecutable;

static bool MatchPattern(string inputLine, string pattern)
{
    if (pattern.Length == 1)
    {
        return inputLine.Contains(pattern);
    }
    else if (pattern == @"\d")
    {
        return MatchDigit(inputLine);
    }
    else if (pattern == @"\w")
    {
        return MatchAlpha(inputLine);
    }
    else if(pattern.StartsWith('[') && pattern.EndsWith(']'))
    {
        if(pattern[1] == '^')
        {
            return MatchNegativeGroup(pattern, inputLine);
        }else
        {
            return MatchPositiveGroup(pattern, inputLine);
        }
    }
    else if(pattern.StartsWith('^'))
    {
        List<string> formattedPattern = FormatPattern(pattern[1..]);

        if(MatchSubstring([.. formattedPattern], inputLine[..(pattern.Length - 1)]))
        {
            return true;
        }
        return false;
    }
    else if(pattern.EndsWith('$'))
    {
        List<string> formattedPattern = FormatPattern(pattern[..^1]);

        int patternLen = pattern.Length - 1;
        int startIndexForMatching = inputLine.Length - patternLen;

        if(MatchSubstring([.. formattedPattern], inputLine[startIndexForMatching..]))
        {
            return true;
        }
        return false;
    }
    else
    {
        List<string> formattedPattern = FormatPattern(pattern);
        bool repeatingPattern = false;
        

        foreach(char c in pattern)
        {
            if(c == '+' || c == '?') repeatingPattern = true;
        }
        
        for(int i = 0; i < inputLine.Length; i++)
        {
            if( inputLine.Length - i < formattedPattern.Count && !repeatingPattern)
            {
                return false;
            }else if(!repeatingPattern && MatchSubstring([.. formattedPattern], inputLine.Substring(i, formattedPattern.Count)))
            {
                return true;
            }else if(MatchSubstring([.. formattedPattern], inputLine[i..]))
            {
                return true;
            }
        }
        
        return false;
    }
}

static bool MatchDigit(string input)
{
    foreach(char character in input)
    {
        if (character >= '0' && character <='9')
        {
            return true;
        }
    }
    return false;
}

static bool MatchAlpha(string input)
{
    foreach(char character in input)
    {
        if (IsAlpha(character)) return true;
    }
    return false;
}

static bool IsAlpha(char character)
{
    if(character >= 'a' && character<='z')
    {
        return true;
    }
    if(character >='A' && character<='Z')
    {
        return true;
    }
    if(character == '_')
    {
        return true;
    }
    return false;
}

static bool MatchPositiveGroup(string pattern, string line)
{
    Dictionary<char, int> collector = [];
    foreach (char character in pattern)
    {
        if (IsAlpha(character))
        {
            collector[character] = 0;
        }
    }
    foreach(char character in line)
    {
        if(collector.ContainsKey(character)) return true;
    }
    return false;
}

static bool MatchNegativeGroup(string pattern, string line)
{
    Dictionary<char, int> collector = [];
    foreach(char character in pattern)
    {
        if (IsAlpha(character))
        {
            collector[character] = 0;
        }
    }
    foreach(char character in line)
    {
        if (!collector.ContainsKey(character)) return true;
    }
    return false;
}

static List<string> FormatPattern(string rawPattern)
{
    List<string> formattedPattern = [];
    bool skipCharacter = false;

    for (int i =0; i < rawPattern.Length; i++)
    {
        if (rawPattern[i] == '\\')
        {
            if(rawPattern[i+1] != '\\')
            {
                formattedPattern.Add(rawPattern.Substring(i,2));
                skipCharacter = true;
            }else
            {
                formattedPattern.Add(rawPattern.Substring(i,1));
            }
        }else if(!skipCharacter)
        {
            if(i+1 < rawPattern.Length && rawPattern[i+1] == '+')
            {
                formattedPattern.Add(rawPattern.Substring(i,2));
                skipCharacter = true;
            }else if(i+1 < rawPattern.Length && rawPattern[i+1] == '?')
            {
                formattedPattern.Add(rawPattern.Substring(i,2));
                skipCharacter = true;
            }
            else
            {
                formattedPattern.Add(rawPattern.Substring(i,1));
            }            
        }else
        {
            skipCharacter = false;
        }
    }

    foreach(string s in formattedPattern)
    {
        Console.WriteLine("s = "+s);
    }
    return formattedPattern;

}

static bool MatchSubstring(string[] pattern, string line)
{
    int lineIndex = 0;
    for(int i = 0; i <pattern.Length; i++)
        {
            string letter = pattern[i];
            switch (letter)
            {
                case @"\d":
                    if (!MatchDigit(line[lineIndex].ToString())) return false;
                    lineIndex += 1;
                    break;
                
                case @"\w":
                    if(!MatchAlpha(line[lineIndex].ToString())) return false;
                    lineIndex += 1;
                    break;

                default:
                    if (letter.Length > 1 && letter[1] == '+')
                    {
                        if(line[lineIndex] != letter[0]) return false;
                        while(lineIndex < line.Length && line[lineIndex] == letter[0])
                        {
                            lineIndex += 1;
                        }
                    }else if(letter.Length > 1 && letter[1] == '?')
                    {
                        if(line[lineIndex] == letter[0])
                        {
                            lineIndex += 1;
                        }
                    }
                    else
                    {
                        if (letter != line[lineIndex].ToString())
                        {
                            return false;
                        }
                        lineIndex += 1;
                    }
                    break;
            }
        }
        
        return true;
}

if (args[0] != "-E")
{
    Console.WriteLine("Expected first argument to be '-E'");
    Environment.Exit(2);
}

string pattern = args[1];
string inputLine = Console.In.ReadToEnd();


if (MatchPattern(inputLine, pattern))
{
    Environment.Exit(0);
}
else
{
    Environment.Exit(1);
}
