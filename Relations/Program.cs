using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Relations
{
    public class Male
    {
        public string Name { get; set; }
    }
    public class Female
    {
        public string Name { get; set; }
    }

    public class Parent
    {
        public string parent { get; set; }
        public string child { get; set; }

    }

    public class Sibling
    {
        public string sib1 { get; set; }
        public string sib2 { get; set; }

    }

    public class Father
    {
        public string father { get; set; }
        public string child { get; set; }
    }

    public class Mother
    {
        public string mother { get; set; }
        public string child { get; set; }
    }

    public class Brother
    {
        public string sibling1 { get; set; }
        public string sibling2 { get; set; }
    }

    public class Sister
    {
        public string sibling1 { get; set; }
        public string sibling2 { get; set; }
    }

    class Program
    {
        static List<Male> Males = new List<Male>
        {
            new Male { Name = "Paul" },
            new Male { Name = "Fred" },
               new Male { Name = "Bill" },
        };
        static List<Female> Females = new List<Female>
        {
            new Female { Name = "Mary" },
            new Female { Name = "Victoria" },
               new Female { Name = "Jemima" },        };

        static List<Father> Fathers = new List<Father>()
        {
            new Father {  father="Paul", child="Fred" },
            new Father {  father="Paul", child="Jemima" },
        };

        static List<Mother> Mothers = new List<Mother>()
        {
            new Mother {  mother="Mary", child="Fred" },
            new Mother {  mother="Mary", child="Jemima" },
        };

        static List<Brother> Brothers = new List<Brother>()
        {
            new Brother { sibling1="Bill", sibling2 = "Paul" }
        };

        static List<Sister> Sisters = new List<Sister>()
        {
            new Sister { sibling1="Bill", sibling2 = "Paul" }
        };


        static void Main(string[] args)
        {
            foreach (var item in ParentsOf())
            {
                Console.WriteLine("PrentOf: {0} is a parent of {1}", item.parent, item.child);
            }
            foreach (var item in Siblings())
            {
                Console.WriteLine("Siblings: {0} is a sibling {1}", item.sib1, item.sib2);
            }

            foreach (var item in Sibling("Fred","Jemima"))
            {
                Console.WriteLine("Sibling {0} is a sibling {1}", item.sib1, item.sib2);
            }
            string brother = "Fred";
            string sibling = "Jemima";
            if (BrotherOf(brother, sibling))
                Console.WriteLine("yes. {0} is {1}'s brother", brother,sibling);
            else
                Console.WriteLine("No. {0} is not {1}'s brother", brother, sibling);

            foreach (var item in BrotherOf())
            {
                Console.WriteLine("{0} is a brother of {1}", item.sib1, item.sib2);
            }


            Console.ReadKey();
        }

        // General Query

        static List<Parent> ParentsOf()
        {
            var FathersOf = Fathers
                .Select(p =>
            new Parent {
                parent = p.father,
                child = p.child
            }).ToList();

            var MotherOf = Mothers.Select(p =>
                    new Parent {
                        parent = p.mother,
                        child = p.child
                    }).ToList();
            return FathersOf.Union(MotherOf).ToList();
        }

        static List<Parent> ParentsOf(string sib)
        {
            var FathersOf = Fathers.Where(p => p.child == sib)
                .Select(p =>
            new Parent {
                parent = p.father,
                child = p.child
            }).ToList();

            var MotherOf = Mothers.Where(p => p.child == sib)
                .Select(p =>
                    new Parent {
                        parent = p.mother,
                        child = p.child
                    }).ToList();
            return FathersOf.Union(MotherOf).ToList<Parent>();
        }

        // General Query

        static List<Sibling> Siblings()
        {
            List<Parent> parentsOfSib1 = ParentsOf();
            List<Sibling> siblings =
                parentsOfSib1.Join(parentsOfSib1,
                s1 => s1.parent,
                s2 => s2.parent,
                (s1, s2) => new Sibling { sib1 = s1.child, sib2 = s2.child })
                .Where(s => s.sib1 != s.sib2)
                .ToList();

            // Post processing to remove duplicates as Join works from both sides
            // and sibling is a mirror relationship
            List<Sibling> setof = new List<Sibling>();
            foreach (var item in siblings)
            {
                if(setof
                    .FirstOrDefault(dup => 
                            dup.sib1 == item.sib1 
                            && dup.sib2 == item.sib2) == null)
                {
                    setof.Add(item);
                }

            }
            return setof;

        }


        static List<Sibling> Sibling(string sib1 , string sib2)
        {
            var parent1 = ParentsOf(sib1);
            var parent2 = ParentsOf(sib2);
            List<Sibling> siblings =
                parent1.Join(parent2,
                s1 => s1.parent,
                s2 => s2.parent,
                (s1, s2) => new Sibling { sib1 = s1.child, sib2 = s2.child })
                .Where(s => s.sib1 == sib1 && s.sib2 == sib2)
                .Distinct()
                .ToList();
            return siblings;
        }

        // General Query

        static List<Sibling> BrotherOf()
        {

            var AllSiblings = Siblings()
                .Where(b => Males.Select(m => m.Name).Contains(b.sib1))
//                        || Males.Select(m => m.Name).Contains(b.sib2))
                        .ToList();

            return AllSiblings.ToList();
        }

        static bool BrotherOf(string brotherOf, string sibling)
        {

            // if the potential brother is not male then nowhere to go
            if (Males.FirstOrDefault(m => m.Name != brotherOf) == null)
                return false;

            // Get the brother of the named brother
            var parents1 = ParentsOf(brotherOf)
                .Where(b => b.child == brotherOf)
                .Select(p => new { parent = p.parent });
            // get the parent of the 
            var parents2 = ParentsOf(sibling)
                .Where(b => b.child == sibling)
                .Select(p => p.parent);

            return parents1.Where(p =>parents2.Contains(p.parent)).Count() > 0;

        }
    }
}
