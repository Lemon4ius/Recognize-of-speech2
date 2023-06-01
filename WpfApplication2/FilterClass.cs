using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApplication2
{
    class FilterClass
    {
        private string[] BadWords = File.ReadAllLines(@"..\..\bad-words.txt");

        private string text { get; set; } 
        public FilterClass(string Text)
        {
            this.text = Text;
        }


        public string FilterCensoredWords()
        {
            string filteredText = text;
            string[] word = text.Split(' ');
            foreach (string censoredWord in BadWords)
            {
                foreach (string word2 in word)
                {
                    int index = BoyerMooreSearch(word2, censoredWord);
                
                while (index >= 0)
                {
                    filteredText = filteredText.Remove(index, censoredWord.Length).Insert(index, new string('*', censoredWord.Length));
                    index = BoyerMooreSearch(filteredText, censoredWord);
                }
                }
            }

            return filteredText;
        }

        private int BoyerMooreSearch(string text, string pattern)
        {
            int n = text.Length;
            int m = pattern.Length;

            // Если паттерн длиннее текста, значит совпадений не будет
            if (m > n)
                return -1;

            // Создаем таблицу смещений для всех символов в паттерне
            int[] badChar = new int[256];
            for (int i = 0; i < badChar.Length; i++)
            {
                badChar[i] = m;
            }

            for (int i = 0; i < m - 1; i++)
            {
                badChar[(int)pattern[i]] = m - i - 1;
            }

            // Инициализируем указатель на конец паттерна и указатель на конец сравниваемой части текста
            int endOfPattern = m - 1;
            int i1 = endOfPattern;
            int j = i1;

            // Проходим по тексту пока не найдем паттерн
            while (i1 < n)
            {
                if (text[i1] == pattern[j])
                {
                    if (j == 0)
                        return i1;
                    i1--;
                    j--;
                }
                else
                {
                    i1 += badChar[(int)text[i1]];
                    j = endOfPattern;
                }
            }

            return -1;
        }

    }
}
