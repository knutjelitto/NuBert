using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Lingu.Commons;

namespace Lingu.Automata
{
    // ReSharper disable UnusedMember.Global
    // ReSharper disable UnusedMember.Local
    public static class UnicodeSets
    {
        public static readonly IntegerSet Any = new IntegerSet((char.MinValue, char.MaxValue));

        public static readonly CatergorySets Category = new CatergorySets();

        public class CatergorySets
        {
            // https://docs.microsoft.com/de-de/dotnet/standard/base-types/character-classes-in-regular-expressions#SupportedUnicodeGeneralCategories
            private readonly (string name, UnicodeCategory[] categories)[] categories =
            {
                ("Lu", new[] {UnicodeCategory.UppercaseLetter}),
                ("Ll", new[] {UnicodeCategory.LowercaseLetter}),
                ("Lt", new[] {UnicodeCategory.TitlecaseLetter}),
                ("Lm", new[] {UnicodeCategory.ModifierLetter}),
                ("Lo", new[] {UnicodeCategory.OtherLetter}),
                ("L",
                    new[]
                    {
                        UnicodeCategory.UppercaseLetter, UnicodeCategory.LowercaseLetter, UnicodeCategory.TitlecaseLetter,
                        UnicodeCategory.ModifierLetter, UnicodeCategory.OtherLetter
                    }),
                ("Mn", new[] {UnicodeCategory.NonSpacingMark}),
                ("Mc", new[] {UnicodeCategory.SpacingCombiningMark}),
                ("Me", new[] {UnicodeCategory.EnclosingMark}),
                ("M", new[] {UnicodeCategory.NonSpacingMark, UnicodeCategory.SpacingCombiningMark, UnicodeCategory.EnclosingMark}),
            };

            public IntegerSet this[string category]
            {
                get
                {
                    if (this.sets.TryGetValue(category, out var result))
                    {
                        return result;
                    }

                    var def = this.categories.FirstOrDefault(cat => cat.name == category);

                    if (def.name == null)
                    {
                        return null;
                    }

                    if (def.categories.Length == 1)
                    {
                        this.sets.Add(def.name, this[def.categories[0]]);

                        return this[def.categories[0]];
                    }

                    var set = new IntegerSet();

                    foreach (var unicodeCategory in def.categories)
                    {
                        set.Add(this[unicodeCategory]);
                    }

                    this.sets.Add(def.name, set);

                    return set;
                }
            }

            public IntegerSet this[UnicodeCategory category]
            {
                get
                {
                    var index = (int) category;
                    return this.basicSets[index] ?? (this.basicSets[index] = Generate(category));
                }
            }

            private static IntegerSet Generate(UnicodeCategory category)
            {
                var set = new IntegerSet();

                for (int ch = char.MinValue; ch <= char.MaxValue; ++ch)
                {
                    if (CharUnicodeInfo.GetUnicodeCategory((char) ch) == category)
                    {
                        set.Add(ch);
                    }
                }

                return set;
            }

            private readonly IntegerSet[] basicSets = new IntegerSet[30];
            private readonly Dictionary<string, IntegerSet> sets = new Dictionary<string, IntegerSet>();
        }
    }
}