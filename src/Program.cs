using System;
using System.IO;

static bool MatchPattern(string inputLine, string pattern)
{
    if (pattern.Length == 1)
    {
        return inputLine.Contains(pattern);
    }
    else if (pattern == @"\d")
    {
        foreach(char character in inputLine)
        {
            if (character >= '0' && character <='9')
            {
                return true;
            }
        }
        return false;
    }
    else if (pattern == @"\w")
    {
        foreach(char character in inputLine)
        {
            if (IsAlpha(character)) return true;
        }
        return false;
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
    else
    {
        throw new ArgumentException($"Unhandled pattern: {pattern}");
    }
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
