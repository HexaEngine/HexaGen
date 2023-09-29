// See https://aka.ms/new-console-template for more information

using HexaGen.Core;
using System.Text;

//WordList.CreateFromTxt("de_DE.txt", "de_DE.wordlist");
//WordList.CreateFromTxt("en_EN.txt", "en_EN.wordlist");

WordList wordlist = new("en_EN");

var words = wordlist.SplitWords("D3D11ARRAYAXISADDRESSRANGEBITCOUNT");

StringBuilder sb = new();

foreach (var word in words)
{
    sb.Append(char.ToUpper(word[0]));
    for (int i = 1; i < word.Length; i++)
    {
        sb.Append(char.ToLower(word[i]));
    }
}

Console.WriteLine(sb.ToString());

for (int i = 0; i < words.Length; i++)
    Console.WriteLine(words[i]);

wordlist.Write("");