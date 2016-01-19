using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrays
{
    class Program
    {
        static void Main(string[] args)
        {
            string[][] donutConstraintPairs =  {
                new string[] {"cruller", "vegan"},
                new string[] {"eclair", "chocolate"},
                new string[] { "zefir", "chocolate" }
            };
            string[][] candidateConstraintPairs =  {
                new string[] {"jose", "vegan"},
                new string[] {"john", "chocolate"},
                new string[] { "ian", "*" } //all items for the type
            };

            var res = JoinArrays(donutConstraintPairs, candidateConstraintPairs);
        }

        private static string[][] JoinArrays(string[][] donutConstraintPairs, string[][] candidateConstraintPairs)
        {
            return (from d in donutConstraintPairs
                                       .Select((data, row) => new { data, row })
                                       .Select(q => new { Name = q.data[0], Type = q.data[1], Row = q.row })
                         from c in candidateConstraintPairs
                                       .Select((data, row) => new { data, row })
                                       .Select(q => new { Name = q.data[0], Type = q.data[1], Row = q.row })
                    where d.Type == c.Type || c.Type == "*"
                    select new { Person = c.Name, Donut = d.Name }
               ).OrderBy(e => e.Person)
                .ThenBy(e => e.Donut)
                .Select(e=> new[] { e.Person, e.Donut })
                .ToArray();
        }
    }
}
