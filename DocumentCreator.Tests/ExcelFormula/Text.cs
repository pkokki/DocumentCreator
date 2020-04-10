using Xunit;
using Xunit.Abstractions;

namespace DocumentCreator.ExcelFormula
{
    public class Text : BaseTest
    {
        public Text(ITestOutputHelper output) : base(output) { }

        // [Fact] public void ASC() { /* Changes full-width (double-byte) English letters or katakana within a character string to half-width (single-byte) characters */ }
        // [Fact] public void BAHTTEXT() { /* Converts a number to text, using the ß (baht) currency format */ }
        // [Fact] public void CHAR() { /* Returns the character specified by the code number */ }
        // [Fact] public void CLEAN() { /* Removes all nonprintable characters from text */ }
        // [Fact] public void CODE() { /* Returns a numeric code for the first character in a text string */ }
        // [Fact] public void CONCAT() { /* Combines the text from multiple ranges and/or strings, but it doesn't provide the delimiter or IgnoreEmpty arguments. */ }
        // [Fact] public void CONCATENATE() { /* Joins several text items into one text item */ }
        // [Fact] public void DBCS() { /* Changes half-width (single-byte) English letters or katakana within a character string to full-width (double-byte) characters */ }
        // [Fact] public void DOLLAR() { /* Converts a number to text, using the $ (dollar) currency format */ }
        // [Fact] public void EXACT() { /* Checks to see if two text values are identical */ }
        // [Fact] public void FIND() { /* Finds one text value within another (case-sensitive) */ }
        // [Fact] public void FINDB() { /*  */ }
        // [Fact] public void FIXED() { /* Formats a number as text with a fixed number of decimals */ }
        // [Fact] public void LEFT() { /* Returns the leftmost characters from a text value */ }
        // [Fact] public void LEFTB() { /*  */ }
        [Fact]
        public void LEN()
        {
            /* Returns the number of characters in a text string */
            AssertExpression("=LEN(\"abcd\")", "4");
            AssertExpression("=LEN(42)", "2");
            AssertExpression("=LEN(2.718)", "5");
            AssertExpression("=LEN(-1)", "2");
            AssertExpression("=LEN(TRUE)", "4");
            AssertExpression("=LEN(NA())", "#N/A");
        }
        // [Fact] public void LENB() { /*  */ }
        [Fact]
        public void LOWER()
        {
            /* Converts text to lowercase */
            AssertExpression("=LOWER(\"abc\")", "abc");
            AssertExpression("=LOWER(\"ABC\")", "abc");
            AssertExpression("=LOWER(\"ΑΒΓ\")", "αβγ");
            AssertExpression("=LOWER(\"αβγ\")", "αβγ");
            AssertExpression("=LOWER(\"ΠΆΝΟΣ\")", "πάνος");
        }
        // [Fact] public void MID() { /* Returns a specific number of characters from a text string starting at the position you specify */ }
        // [Fact] public void MIDB() { /*  */ }
        // [Fact] public void NUMBERVALUE() { /* Converts text to number in a locale-independent manner */ }
        // [Fact] public void PHONETIC() { /* Extracts the phonetic (furigana) characters from a text string */ }
        [Fact]
        public void PROPER()
        {
            /* Capitalizes the first letter in each word of a text value */
            AssertExpression("=PROPER(\"abc\")", "Abc");
            AssertExpression("=PROPER(\"abc def\")", "Abc Def");
        }
        // [Fact] public void REPLACE() { /* Replaces characters within text */ }
        // [Fact] public void REPLACEB() { /*  */ }
        // [Fact] public void REPT() { /* Repeats text a given number of times */ }
        // [Fact] public void RIGHT() { /* Returns the rightmost characters from a text value */ }
        // [Fact] public void RIGHTB() { /*  */ }
        // [Fact] public void SEARCH() { /* Finds one text value within another (not case-sensitive) */ }
        // [Fact] public void SEARCHB() { /*  */ }
        // [Fact] public void SUBSTITUTE() { /* Substitutes new text for old text in a text string */ }
        // [Fact] public void T() { /* Converts its arguments to text */ }
        // [Fact] public void TEXT() { /* Formats a number and converts it to text */ }
        // [Fact] public void TEXTJOIN() { /* Combines the text from multiple ranges and/or strings, and includes a delimiter you specify between each text value that will be combined. If the delimiter is an empty text string, this function will effectively concatenate the ranges. */ }
        // [Fact] public void TRIM() { /* Removes spaces from text */ }
        // [Fact] public void UNICHAR() { /* Returns the Unicode character that is references by the given numeric value */ }
        // [Fact] public void UNICODE() { /* Returns the number (code point) that corresponds to the first character of the text */ }
        [Fact]
        public void UPPER()
        {
            /* Converts text to uppercase */
            AssertExpression("=UPPER(\"abc\")", "ABC");
            AssertExpression("=UPPER(\"ABC\")", "ABC");
            AssertExpression("=UPPER(\"ΑΒΓ\")", "ΑΒΓ");
            AssertExpression("=UPPER(\"αβγ\")", "ΑΒΓ");
            AssertExpression("=UPPER(\"Πάνος\")", "ΠΑΝΟΣ");
        }
        // [Fact] public void VALUE() { /* Converts a text argument to a number */ }

    }
}
