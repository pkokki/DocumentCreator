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
        [Fact]
        public void EXACT()
        {
            /* Checks to see if two text values are identical */
            AssertExpression("=\"a\"=\"A\"", "TRUE");
            AssertExpression("=\"ω\"=\"Ω\"", "TRUE");
            AssertExpression("=EXACT(\"a\", \"A\")", "FALSE");
            AssertExpression("=EXACT(\"Ω\", \"ω\")", "FALSE");
        }
        // [Fact] public void FINDB() { /*  */ }
        [Fact]
        public void FIND()
        {
            /* Finds one text value within another (case-sensitive) */
            AssertExpression("=FIND(\"a\", \"Bananarama\")", "2");
            AssertExpression("=FIND(\"a\", \"Bananarama\", 5)", "6");
            AssertExpression("=FIND(\"o\", \"Bananarama\")", "#VALUE!");
            AssertExpression("=FIND(\"A\", \"Bananarama\")", "#VALUE!");
        }
        [Fact]
        public void FIXED()
        {
            // /* Formats a number as text with a fixed number of decimals */
            AssertExpression("=FIXED(PI()*100000,2,TRUE)", "314159,27");
            AssertExpression("=FIXED(PI()*100000,2,FALSE)", "314.159,27");
            AssertExpression("=FIXED(PI()*100000,0,FALSE)", "314.159");
            AssertExpression("=FIXED(PI()*100000,0,TRUE)", "314159");
            AssertExpression("=FIXED(PI()*100000)", "314.159,27");
            AssertExpression("=FIXED(PI()*100000,2)", "314.159,27");
            AssertExpression("=FIXED(PI()*100000,-2)", "314.200");
        }
        // [Fact] public void LEFTB() { /*  */ }
        [Fact]
        public void LEFT()
        {
            /* Returns the leftmost characters from a text value */
            AssertExpression("=LEFT(\"ABCDEF\",2)", "AB");
            AssertExpression("=LEFT(\"ABCDEF\",5)", "ABCDE");
            AssertExpression("=LEFT(\"ABCDEF\",6)", "ABCDEF");
            AssertExpression("=LEFT(\"ABCDEF\",7)", "ABCDEF");
            AssertExpression("=LEFT(\"ABCDEF\")", "A");
        }
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
        // [Fact] public void MIDB() { /*  */ }
        [Fact]
        public void MID()
        {
            /* Returns a specific number of characters from a text string starting at the position you specify */
            AssertExpression("=MID(\"abcdef\",3,1)", "c");
            AssertExpression("=MID(\"abcdef\",1,1)", "a");
            AssertExpression("=MID(\"abcdef\",6,1)", "f");
            AssertExpression("=MID(\"abcdef\",3,13)", "cdef");
        }
        // [Fact] public void NUMBERVALUE() { /* Converts text to number in a locale-independent manner */ }
        // [Fact] public void PHONETIC() { /* Extracts the phonetic (furigana) characters from a text string */ }
        [Fact]
        public void PROPER()
        {
            /* Capitalizes the first letter in each word of a text value */
            AssertExpression("=PROPER(\"abc\")", "Abc");
            AssertExpression("=PROPER(\"abc def\")", "Abc Def");
        }
        // [Fact] public void REPLACEB() { /*  */ }
        [Fact]
        public void REPLACE()
        {
            /* Replaces characters within text */
            AssertExpression("=REPLACE(\"123456789\",1,1,\"\")", "23456789");
            AssertExpression("=REPLACE(\"123456789\",1,1,\"A\")", "A23456789");
            AssertExpression("=REPLACE(\"123456789\",20,1,\"A\")", "123456789A");
            AssertExpression("=REPLACE(\"123456789\",5,1,\"A\")", "1234A6789");
            AssertExpression("=REPLACE(\"123456789\",0,1,\"A\")", "#N/A");
            AssertExpression("=REPLACE(\"123456789\",1,2,\"A\")", "A3456789");
            AssertExpression("=REPLACE(\"123456789\",20,2,\"A\")", "123456789A");
            AssertExpression("=REPLACE(\"123456789\",5,2,\"A\")", "1234A789");
            AssertExpression("=REPLACE(\"123456789\",1,0,\"A\")", "A123456789");
            AssertExpression("=REPLACE(\"123456789\",1,2,\"AB\")", "AB3456789");
            AssertExpression("=REPLACE(\"123456789\",20,2,\"AB\")", "123456789AB");
            AssertExpression("=REPLACE(\"123456789\",5,2,\"AB\")", "1234AB789");
            AssertExpression("=REPLACE(\"123456789\",1,0,\"AB\")", "AB123456789");
        }
        // [Fact] public void REPT() { /* Repeats text a given number of times */ }
        // [Fact] public void RIGHTB() { /*  */ }
        [Fact]
        public void RIGHT()
        {
            /* Returns the rightmost characters from a text value */
            AssertExpression("=RIGHT(\"ABCDEF\",2)", "EF");
            AssertExpression("=RIGHT(\"ABCDEF\")", "F");
            AssertExpression("=RIGHT(\"ABCDEF\",5)", "BCDEF");
            AssertExpression("=RIGHT(\"ABCDEF\",6)", "ABCDEF");
            AssertExpression("=RIGHT(\"ABCDEF\",7)", "ABCDEF");
        }
        // [Fact] public void SEARCHB() { /*  */ }
        [Fact]
        public void SEARCH()
        {
            /* Finds one text value within another (not case-sensitive) */
            AssertExpression("=SEARCH(\"a\",\"bananarama\")", "2");
            AssertExpression("=SEARCH(\"o\",\"bananarama\")", "#N/A");
            AssertExpression("=SEARCH(\"1\",\"654321\")", "6");
            AssertExpression("=SEARCH(\"1,1\",\"Section 1,1\")", "9");
            AssertExpression("=SEARCH(1,\"Section 1,1\")", "9");
            AssertExpression("=SEARCH(1.1,\"Section 1,1\")", "9");
            AssertExpression("=SEARCH(1.1,\"Section 1.1\")", "#N/A");
            AssertExpression("=SEARCH(\"a\",\"bananarama\",0)", "#N/A");
            AssertExpression("=SEARCH(\"a\",\"bananarama\",10)", "10");
            AssertExpression("=SEARCH(\"a\",\"bananarama\",11)", "#N/A");
        }
        [Fact]
        public void SUBSTITUTE()
        {
            /* Substitutes new text for old text in a text string */
            AssertExpression("=SUBSTITUTE(\"ABCDFGH\",\"A\",\"_\")", "_BCDFGH");
            AssertExpression("=SUBSTITUTE(\"ABCDFGH\",\"ABC\",\"_\")", "_DFGH");
            AssertExpression("=SUBSTITUTE(\"ABCDFGH\",\"A\",\"___\")", "___BCDFGH");
            AssertExpression("=SUBSTITUTE(\"AaAa\",\"A\",\"_\")", "_a_a");
            AssertExpression("=SUBSTITUTE(\"ABCDFGH\",\"\",\"_\")", "ABCDFGH");
            AssertExpression("=SUBSTITUTE(\"ABCDFGH\",\"B\",\"\")", "ACDFGH");
            AssertExpression("=SUBSTITUTE(\"\",\"B\",\"_\")", "");
            AssertExpression("=SUBSTITUTE(\"A\",\"B\",\"_\")", "A");
            AssertExpression("=SUBSTITUTE(1234,3,\"_\")", "12_4");
            AssertExpression("=SUBSTITUTE(1234.56,\"4,5\",\"_\")", "123_6");
            AssertExpression("=SUBSTITUTE(TRUE,\"R\",\"_\")", "T_UE");
            AssertExpression("=SUBSTITUTE(\"Bananarama\",\"a\",\"_\",0)", "#N/A");
            AssertExpression("=SUBSTITUTE(\"Bananarama\",\"a\",\"_\",3)", "Banan_rama");
            AssertExpression("=SUBSTITUTE(\"Bananarama\",\"a\",\"_\",5)", "Bananaram_");
            AssertExpression("=SUBSTITUTE(\"Bananarama\",\"a\",\"_\",6)", "Bananarama");

        }
        [Fact]
        public void T()
        {
            /* Converts its arguments to text */
            AssertExpression("=T(\"\")", "");
            AssertExpression("=T(\"12345\")", "12345");
            AssertExpression("=T(\"ABC\")", "ABC");
            AssertExpression("=T(TRUE)", "");
            AssertExpression("=T(FALSE)", "");
            AssertExpression("=T(0)", "");
            AssertExpression("=T(1234)", "");
            AssertExpression("=T(123.4)", "");
            AssertExpression("=T(NA())", "#N/A");
        }
        //[Fact]
        //public void TEXT()
        //{
        //    /* Formats a number and converts it to text */
        //}
        // [Fact] public void TEXTJOIN() { /* Combines the text from multiple ranges and/or strings, and includes a delimiter you specify between each text value that will be combined. If the delimiter is an empty text string, this function will effectively concatenate the ranges. */ }
        [Fact]
        public void TRIM()
        {
            /* Removes spaces from text */
            AssertExpression("=TRIM(\"\")", "");
            AssertExpression("=TRIM(\"ABC\")", "ABC");
            AssertExpression("=TRIM(NA())", "#N/A");
            AssertExpression("=TRIM(\"ABC \")", "ABC");
            AssertExpression("=TRIM(\" ABC\")", "ABC");
            AssertExpression("=TRIM(\" ABC \")", "ABC");
            AssertExpression("=TRIM(\"   ABC   \")", "ABC");
        }
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
        [Fact]
        public void VALUE()
        {
            /* Converts a text argument to a number */
            AssertExpression("=VALUE(1)", "1");
            AssertExpression("=VALUE(TRUE)", "#VALUE!");
            AssertExpression("=VALUE(FALSE)", "#VALUE!");
            AssertExpression("=VALUE(NA())", "#N/A");
            AssertExpression("=VALUE(\"\")", "#VALUE!");
            AssertExpression("=VALUE(\"123\")", "123");
            AssertExpression("=VALUE(\"123,12\")", "123,12");
            AssertExpression("=VALUE(\"123.12\")", "#VALUE!");
            AssertExpression("=VALUE(1.2)", "1,2");
        }
    }
}
