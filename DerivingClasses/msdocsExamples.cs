using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Addendum.MSdocsExamples
{
    /// <summary>
    /// примеры с docs.microsoft.com
    /// </summary>
    class LINQ
    {
        public static void RunExample()
        {
            // LINQ with query syntax
            var startingDeck = (from s in Suits()
                                from r in Ranks()
                                select new { Suit = s, Rank = r }).LogQuery("создание колоды").ToArray();
            // LINQ with method call syntax
            var startingDeck2 = Suits().SelectMany(suit => Ranks().Select(rank => new { Suit = suit, Rank = rank }));

            foreach (var card in startingDeck) Console.WriteLine(card);

            int times = 0;
            var shuffle = startingDeck;
            do
            {
                //shuffle = shuffle.Take(26).InterleaveSequenceWith(shuffle.Skip(26)).ToArray();  // out shuffle
                //shuffle = shuffle.Skip(26).InterleaveSequenceWith(shuffle.Take(26)).ToArray();  // in shuffle
                //shuffle = shuffle.Take(26).LogQuery("верхняя половина").InterleaveSequenceWith(shuffle.Skip(26).LogQuery("нижняя половина")).LogQuery("тасованная колода").ToArray();  // in shuffle
                shuffle = shuffle.Skip(26).LogQuery("нижняя половина").InterleaveSequenceWith(shuffle.Take(26).LogQuery("верхняя половина")).LogQuery("тасованная колода").ToArray();  // in shuffle

                foreach (var card in shuffle)
                {
                    Console.WriteLine(card);
                }
                Console.WriteLine(++times);

            } while (!startingDeck.SequenceEquals(shuffle));
            Console.WriteLine(times);

            Console.ReadLine();
        }

        public static void RunExample_unoptimized()  // OutOfMemoryException !
        {
            // LINQ with query syntax
            var startingDeck = (from s in Suits()
                                from r in Ranks()
                                select new { Suit = s, Rank = r }).LogQuery("создание колоды");
            // LINQ with method call syntax
            var startingDeck2 = Suits().SelectMany(suit => Ranks().Select(rank => new { Suit = suit, Rank = rank }));

            foreach (var card in startingDeck) Console.WriteLine(card);

            //var top = startingDeck.Take(26);
            //var bottom = startingDeck.Skip(26);
            //var shuffle = top.InterleaveSequenceWith(bottom);

            int times = 0;
            var shuffle = startingDeck;
            do
            {
                //shuffle = shuffle.Take(26).InterleaveSequenceWith(shuffle.Skip(26));  // out shuffle
                //shuffle = shuffle.Skip(26).InterleaveSequenceWith(shuffle.Take(26));  // in shuffle
                //shuffle = shuffle.Take(26).LogQuery("верхняя половина").InterleaveSequenceWith(shuffle.Skip(26).LogQuery("нижняя половина")).LogQuery("тасованная колода");  // in shuffle
                shuffle = shuffle.Skip(26).LogQuery("нижняя половина").InterleaveSequenceWith(shuffle.Take(26).LogQuery("верхняя половина")).LogQuery("тасованная колода");  // in shuffle

                foreach (var card in shuffle)
                {
                    Console.WriteLine(card);
                }
                Console.WriteLine(++times);

            } while (!startingDeck.SequenceEquals(shuffle));

            Console.WriteLine(times);

            Console.ReadLine();
        }

        static IEnumerable<string> Suits()
        {
            yield return "clubs";
            yield return "diamonds";
            yield return "hearts";
            yield return "spades";
        }

        static IEnumerable<string> Ranks()
        {
            yield return "two";
            yield return "three";
            yield return "four";
            yield return "five";
            yield return "six";
            yield return "seven";
            yield return "eight";
            yield return "nine";
            yield return "ten";
            yield return "jack";
            yield return "queen";
            yield return "king";
            yield return "ace";
        }
    }

    public static class Extensions
    {
        public static IEnumerable<T> InterleaveSequenceWith<T>(this IEnumerable<T> first, IEnumerable<T> second)
        {
            var firstIter = first.GetEnumerator();
            var secondIter = second.GetEnumerator();

            while (firstIter.MoveNext() && secondIter.MoveNext())
            {
                yield return firstIter.Current;
                yield return secondIter.Current;
            }
        }

        public static bool SequenceEquals<T>(this IEnumerable<T> first, IEnumerable<T> second)
        {
            var firstIter = first.GetEnumerator();
            var secondIter = second.GetEnumerator();

            while (firstIter.MoveNext() && secondIter.MoveNext())
            {
                if (!firstIter.Current.Equals(secondIter.Current))
                {
                    return false;
                }
            }
            return true;
        }

        public static IEnumerable<T> LogQuery<T>(this IEnumerable<T> sequence, string tag)
        {
            using (var writer = File.AppendText("debug.log"))
            {
                writer.WriteLine($"Executing Query {tag}");
            }
            return sequence;
        }

    }
}
