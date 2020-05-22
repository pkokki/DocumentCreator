using System;
using System.Collections.Generic;
using System.Net.Http;

namespace JsonExcelExpressions.Eval
{
    internal partial class Functions
    {
        public static readonly Functions INSTANCE = new Functions();

        private readonly Dictionary<string, Func<List<ExcelValue>, ExpressionScope, ExcelValue>> Registry
            = new Dictionary<string, Func<List<ExcelValue>, ExpressionScope, ExcelValue>>();

        // https://docs.microsoft.com/en-us/dotnet/api/system.net.http.httpclient?view=netcore-3.1
        // HttpClient is intended to be instantiated once per application, rather than per-use.
        private static readonly Lazy<HttpClient> httpClient = new Lazy<HttpClient>();

        private static readonly Lazy<Random> random = new Lazy<Random>();

        private Functions()
        {
            // https://support.office.com/en-us/article/excel-functions-by-category-5f91f4e9-7b42-46d2-9bd1-63f26a86c0eb
            RegisterCubeFunctions();
            RegisterDatabaseFunctions();
            RegisterDateTimeFunctions();
            RegisterEngineeringFunctions();
            RegisterFinancialFunctions();
            RegisterInformationFunctions();
            RegisterLogicalFunctions();
            RegisterLookupAndReferenceFunctions();
            RegisterMathTrigonometryFunctions();
            RegisterStatisticalFunctions();
            RegisterTextFunctions();
            RegisterWebFunctions();
            RegisterUserDefinedFunctions();
        }

        private void RegisterCubeFunctions()
        {
            //Registry.Add("CUBEKPIMEMBER", CUBEKPIMEMBER); // Returns a key performance indicator (KPI) property and displays the KPI name in the cell. A KPI is a quantifiable measurement, such as monthly gross profit or quarterly employee turnover, that is used to monitor an organization's performance.
            //Registry.Add("CUBEMEMBER", CUBEMEMBER); // Returns a member or tuple from the cube. Use to validate that the member or tuple exists in the cube.
            //Registry.Add("CUBEMEMBERPROPERTY", CUBEMEMBERPROPERTY); // Returns the value of a member property from the cube. Use to validate that a member name exists within the cube and to return the specified property for this member.
            //Registry.Add("CUBERANKEDMEMBER", CUBERANKEDMEMBER); // Returns the nth, or ranked, member in a set. Use to return one or more elements in a set, such as the top sales performer or the top 10 students.
            //Registry.Add("CUBESET", CUBESET); // Defines a calculated set of members or tuples by sending a set expression to the cube on the server, which creates the set, and then returns that set to Microsoft Office Excel.
            //Registry.Add("CUBESETCOUNT", CUBESETCOUNT); // Returns the number of items in a set.
            //Registry.Add("CUBEVALUE", CUBEVALUE); // Returns an aggregated value from the cube.
        }

        private void RegisterDatabaseFunctions()
        {
            //Registry.Add("DAVERAGE", DAVERAGE); // Returns the average of selected database entries
            //Registry.Add("DCOUNT", DCOUNT); // Counts the cells that contain numbers in a database
            //Registry.Add("DCOUNTA", DCOUNTA); // Counts nonblank cells in a database
            //Registry.Add("DGET", DGET); // Extracts from a database a single record that matches the specified criteria
            //Registry.Add("DMAX", DMAX); // Returns the maximum value from selected database entries
            //Registry.Add("DMIN", DMIN); // Returns the minimum value from selected database entries
            //Registry.Add("DPRODUCT", DPRODUCT); // Multiplies the values in a particular field of records that match the criteria in a database
            //Registry.Add("DSTDEV", DSTDEV); // Estimates the standard deviation based on a sample of selected database entries
            //Registry.Add("DSTDEVP", DSTDEVP); // Calculates the standard deviation based on the entire population of selected database entries
            //Registry.Add("DSUM", DSUM); // Adds the numbers in the field column of records in the database that match the criteria
            //Registry.Add("DVAR", DVAR); // Estimates variance based on a sample from selected database entries
            //Registry.Add("DVARP", DVARP); // Calculates variance based on the entire population of selected database entries
        }

        private void RegisterDateTimeFunctions()
        {
            Registry.Add("DATE", DATE); // Returns the serial number of a particular date
            Registry.Add("DATEDIF", DATEDIF); // Calculates the number of days, months, or years between two dates. This function is useful in formulas where you need to calculate an age.
            Registry.Add("DATEVALUE", DATEVALUE); // Converts a date in the form of text to a serial number
            Registry.Add("DAY", DAY); // Converts a serial number to a day of the month
            Registry.Add("DAYS", DAYS); // Returns the number of days between two dates
            //Registry.Add("DAYS360", DAYS360); // Calculates the number of days between two dates based on a 360-day year
            //Registry.Add("EDATE", EDATE); // Returns the serial number of the date that is the indicated number of months before or after the start date
            //Registry.Add("EOMONTH", EOMONTH); // Returns the serial number of the last day of the month before or after a specified number of months
            Registry.Add("HOUR", HOUR); // Converts a serial number to an hour
            //Registry.Add("ISOWEEKNUM", ISOWEEKNUM); // Returns the number of the ISO week number of the year for a given date
            Registry.Add("MINUTE", MINUTE); // Converts a serial number to a minute
            Registry.Add("MONTH", MONTH); // Converts a serial number to a month
            //Registry.Add("NETWORKDAYS", NETWORKDAYS); // Returns the number of whole workdays between two dates
            //Registry.Add("NETWORKDAYS.INTL", NETWORKDAYS_INTL); // Returns the number of whole workdays between two dates using parameters to indicate which and how many days are weekend days
            Registry.Add("NOW", NOW); // Returns the serial number of the current date and time
            Registry.Add("SECOND", SECOND); // Converts a serial number to a second
            Registry.Add("TIME", TIME); // Returns the serial number of a particular time
            Registry.Add("TIMEVALUE", TIMEVALUE); // Converts a time in the form of text to a serial number
            Registry.Add("TODAY", TODAY); // Returns the serial number of today's date
            //Registry.Add("WEEKDAY", WEEKDAY); // Converts a serial number to a day of the week
            //Registry.Add("WEEKNUM", WEEKNUM); // Converts a serial number to a number representing where the week falls numerically with a year
            //Registry.Add("WORKDAY", WORKDAY); // Returns the serial number of the date before or after a specified number of workdays
            //Registry.Add("WORKDAY.INTL", WORKDAY.INTL); // Returns the serial number of the date before or after a specified number of workdays using parameters to indicate which and how many days are weekend days
            Registry.Add("YEAR", YEAR); // Converts a serial number to a year
            //Registry.Add("YEARFRAC", YEARFRAC); // Returns the year fraction representing the number of whole days between start_date and end_date
        }

        private void RegisterEngineeringFunctions()
        {
            // Registry.Add("BESSELI", BESSELI);  // Returns the modified Bessel function In(x)
            // Registry.Add("BESSELJ", BESSELJ);  // Returns the Bessel function Jn(x)
            // Registry.Add("BESSELK", BESSELK);  // Returns the modified Bessel function Kn(x)
            // Registry.Add("BESSELY", BESSELY);  // Returns the Bessel function Yn(x)
            // Registry.Add("BIN2DEC", BIN2DEC);  // Converts a binary number to decimal
            // Registry.Add("BIN2HEX", BIN2HEX);  // Converts a binary number to hexadecimal
            // Registry.Add("BIN2OCT", BIN2OCT);  // Converts a binary number to octal
            // Registry.Add("BITAND", BITAND);  // Returns a 'Bitwise And' of two numbers
            // Registry.Add("BITLSHIFT", BITLSHIFT);  // Returns a value number shifted left by shift_amount bits
            // Registry.Add("BITOR", BITOR);  // Returns a bitwise OR of 2 numbers
            // Registry.Add("BITRSHIFT", BITRSHIFT);  // Returns a value number shifted right by shift_amount bits
            // Registry.Add("BITXOR", BITXOR);  // Returns a bitwise 'Exclusive Or' of two numbers
            // Registry.Add("COMPLEX", COMPLEX);  // Converts real and imaginary coefficients into a complex number
            // Registry.Add("CONVERT", CONVERT);  // Converts a number from one measurement system to another
            // Registry.Add("DEC2BIN", DEC2BIN);  // Converts a decimal number to binary
            // Registry.Add("DEC2HEX", DEC2HEX);  // Converts a decimal number to hexadecimal
            // Registry.Add("DEC2OCT", DEC2OCT);  // Converts a decimal number to octal
            // Registry.Add("DELTA", DELTA);  // Tests whether two values are equal
            // Registry.Add("ERF", ERF);  // Returns the error function
            // Registry.Add("ERF.PRECISE", ERF.PRECISE);  // Returns the error function
            // Registry.Add("ERFC", ERFC);  // Returns the complementary error function
            // Registry.Add("ERFC.PRECISE", ERFC.PRECISE);  // Returns the complementary ERF function integrated between x and infinity
            // Registry.Add("GESTEP", GESTEP);  // Tests whether a number is greater than a threshold value
            // Registry.Add("HEX2BIN", HEX2BIN);  // Converts a hexadecimal number to binary
            // Registry.Add("HEX2DEC", HEX2DEC);  // Converts a hexadecimal number to decimal
            // Registry.Add("HEX2OCT", HEX2OCT);  // Converts a hexadecimal number to octal
            // Registry.Add("IMABS", IMABS);  // Returns the absolute value (modulus) of a complex number
            // Registry.Add("IMAGINARY", IMAGINARY);  // Returns the imaginary coefficient of a complex number
            // Registry.Add("IMARGUMENT", IMARGUMENT);  // Returns the argument theta, an angle expressed in radians
            // Registry.Add("IMCONJUGATE", IMCONJUGATE);  // Returns the complex conjugate of a complex number
            // Registry.Add("IMCOS", IMCOS);  // Returns the cosine of a complex number
            // Registry.Add("IMCOSH", IMCOSH);  // Returns the hyperbolic cosine of a complex number
            // Registry.Add("IMCOT", IMCOT);  // Returns the cotangent of a complex number
            // Registry.Add("IMCSC", IMCSC);  // Returns the cosecant of a complex number
            // Registry.Add("IMCSCH", IMCSCH);  // Returns the hyperbolic cosecant of a complex number
            // Registry.Add("IMDIV", IMDIV);  // Returns the quotient of two complex numbers
            // Registry.Add("IMEXP", IMEXP);  // Returns the exponential of a complex number
            // Registry.Add("IMLN", IMLN);  // Returns the natural logarithm of a complex number
            // Registry.Add("IMLOG10", IMLOG10);  // Returns the base-10 logarithm of a complex number
            // Registry.Add("IMLOG2", IMLOG2);  // Returns the base-2 logarithm of a complex number
            // Registry.Add("IMPOWER", IMPOWER);  // Returns a complex number raised to an integer power
            // Registry.Add("IMPRODUCT", IMPRODUCT);  // Returns the product of from 2 to 255 complex numbers
            // Registry.Add("IMREAL", IMREAL);  // Returns the real coefficient of a complex number
            // Registry.Add("IMSEC", IMSEC);  // Returns the secant of a complex number
            // Registry.Add("IMSECH", IMSECH);  // Returns the hyperbolic secant of a complex number
            // Registry.Add("IMSIN", IMSIN);  // Returns the sine of a complex number
            // Registry.Add("IMSINH", IMSINH);  // Returns the hyperbolic sine of a complex number
            // Registry.Add("IMSQRT", IMSQRT);  // Returns the square root of a complex number
            // Registry.Add("IMSUB", IMSUB);  // Returns the difference between two complex numbers
            // Registry.Add("IMSUM", IMSUM);  // Returns the sum of complex numbers
            // Registry.Add("IMTAN", IMTAN);  // Returns the tangent of a complex number
            // Registry.Add("OCT2BIN", OCT2BIN);  // Converts an octal number to binary
            // Registry.Add("OCT2DEC", OCT2DEC);  // Converts an octal number to decimal
            // Registry.Add("OCT2HEX", OCT2HEX);  // Converts an octal number to hexadecimal
        }

        private void RegisterFinancialFunctions()
        {
            // Registry.Add("ACCRINT", ACCRINT);  // Returns the accrued interest for a security that pays periodic interest
            // Registry.Add("ACCRINTM", ACCRINTM);  // Returns the accrued interest for a security that pays interest at maturity
            // Registry.Add("AMORDEGRC", AMORDEGRC);  // Returns the depreciation for each accounting period by using a depreciation coefficient
            // Registry.Add("AMORLINC", AMORLINC);  // Returns the depreciation for each accounting period
            // Registry.Add("COUPDAYBS", COUPDAYBS);  // Returns the number of days from the beginning of the coupon period to the settlement date
            // Registry.Add("COUPDAYS", COUPDAYS);  // Returns the number of days in the coupon period that contains the settlement date
            // Registry.Add("COUPDAYSNC", COUPDAYSNC);  // Returns the number of days from the settlement date to the next coupon date
            // Registry.Add("COUPNCD", COUPNCD);  // Returns the next coupon date after the settlement date
            // Registry.Add("COUPNUM", COUPNUM);  // Returns the number of coupons payable between the settlement date and maturity date
            // Registry.Add("COUPPCD", COUPPCD);  // Returns the previous coupon date before the settlement date
            // Registry.Add("CUMIPMT", CUMIPMT);  // Returns the cumulative interest paid between two periods
            // Registry.Add("CUMPRINC", CUMPRINC);  // Returns the cumulative principal paid on a loan between two periods
            // Registry.Add("DB", DB);  // Returns the depreciation of an asset for a specified period by using the fixed-declining balance method
            // Registry.Add("DDB", DDB);  // Returns the depreciation of an asset for a specified period by using the double-declining balance method or some other method that you specify
            // Registry.Add("DISC", DISC);  // Returns the discount rate for a security
            // Registry.Add("DOLLARDE", DOLLARDE);  // Converts a dollar price, expressed as a fraction, into a dollar price, expressed as a decimal number
            // Registry.Add("DOLLARFR", DOLLARFR);  // Converts a dollar price, expressed as a decimal number, into a dollar price, expressed as a fraction
            // Registry.Add("DURATION", DURATION);  // Returns the annual duration of a security with periodic interest payments
            // Registry.Add("EFFECT", EFFECT);  // Returns the effective annual interest rate
            // Registry.Add("FV", FV);  // Returns the future value of an investment
            // Registry.Add("FVSCHEDULE", FVSCHEDULE);  // Returns the future value of an initial principal after applying a series of compound interest rates
            // Registry.Add("INTRATE", INTRATE);  // Returns the interest rate for a fully invested security
            // Registry.Add("IPMT", IPMT);  // Returns the interest payment for an investment for a given period
            // Registry.Add("IRR", IRR);  // Returns the internal rate of return for a series of cash flows
            // Registry.Add("ISPMT", ISPMT);  // Calculates the interest paid during a specific period of an investment
            // Registry.Add("MDURATION", MDURATION);  // Returns the Macauley modified duration for a security with an assumed par value of $100
            // Registry.Add("MIRR", MIRR);  // Returns the internal rate of return where positive and negative cash flows are financed at different rates
            // Registry.Add("NOMINAL", NOMINAL);  // Returns the annual nominal interest rate
            // Registry.Add("NPER", NPER);  // Returns the number of periods for an investment
            // Registry.Add("NPV", NPV);  // Returns the net present value of an investment based on a series of periodic cash flows and a discount rate
            // Registry.Add("ODDFPRICE", ODDFPRICE);  // Returns the price per $100 face value of a security with an odd first period
            // Registry.Add("ODDFYIELD", ODDFYIELD);  // Returns the yield of a security with an odd first period
            // Registry.Add("ODDLPRICE", ODDLPRICE);  // Returns the price per $100 face value of a security with an odd last period
            // Registry.Add("ODDLYIELD", ODDLYIELD);  // Returns the yield of a security with an odd last period
            // Registry.Add("PDURATION", PDURATION);  // Returns the number of periods required by an investment to reach a specified value
            // Registry.Add("PMT", PMT);  // Returns the periodic payment for an annuity
            // Registry.Add("PPMT", PPMT);  // Returns the payment on the principal for an investment for a given period
            // Registry.Add("PRICE", PRICE);  // Returns the price per $100 face value of a security that pays periodic interest
            // Registry.Add("PRICEDISC", PRICEDISC);  // Returns the price per $100 face value of a discounted security
            // Registry.Add("PRICEMAT", PRICEMAT);  // Returns the price per $100 face value of a security that pays interest at maturity
            // Registry.Add("PV", PV);  // Returns the present value of an investment
            // Registry.Add("RATE", RATE);  // Returns the interest rate per period of an annuity
            // Registry.Add("RECEIVED", RECEIVED);  // Returns the amount received at maturity for a fully invested security
            // Registry.Add("RRI", RRI);  // Returns an equivalent interest rate for the growth of an investment
            // Registry.Add("SLN", SLN);  // Returns the straight-line depreciation of an asset for one period
            // Registry.Add("SYD", SYD);  // Returns the sum-of-years' digits depreciation of an asset for a specified period
            // Registry.Add("TBILLEQ", TBILLEQ);  // Returns the bond-equivalent yield for a Treasury bill
            // Registry.Add("TBILLPRICE", TBILLPRICE);  // Returns the price per $100 face value for a Treasury bill
            // Registry.Add("TBILLYIELD", TBILLYIELD);  // Returns the yield for a Treasury bill
            // Registry.Add("VDB", VDB);  // Returns the depreciation of an asset for a specified or partial period by using a declining balance method
            // Registry.Add("XIRR", XIRR);  // Returns the internal rate of return for a schedule of cash flows that is not necessarily periodic
            // Registry.Add("XNPV", XNPV);  // Returns the net present value for a schedule of cash flows that is not necessarily periodic
            // Registry.Add("YIELD", YIELD);  // Returns the yield on a security that pays periodic interest
            // Registry.Add("YIELDDISC", YIELDDISC);  // Returns the annual yield for a discounted security; for example, a Treasury bill
            // Registry.Add("YIELDMAT", YIELDMAT);  // Returns the annual yield of a security that pays interest at maturity
        }

        private void RegisterInformationFunctions()
        {
            //Registry.Add("CELL", CELL); // Returns information about the formatting, location, or contents of a cell
            //Registry.Add("ERROR.TYPE", ERROR_TYPE); // Returns a number corresponding to an error type
            //Registry.Add("INFO", INFO); // Returns information about the current operating environment
            Registry.Add("ISBLANK", ISBLANK); // Returns TRUE if the value is blank
            Registry.Add("ISERR", ISERR); // Returns TRUE if the value is any error value except #N/A
            Registry.Add("ISERROR", ISERROR); // Returns TRUE if the value is any error value
            Registry.Add("ISEVEN", ISEVEN); // Returns TRUE if the number is even
            //Registry.Add("ISFORMULA", ISFORMULA); // Returns TRUE if there is a reference to a cell that contains a formula
            Registry.Add("ISLOGICAL", ISLOGICAL); // Returns TRUE if the value is a logical value
            Registry.Add("ISNA", ISNA); // Returns TRUE if the value is the #N/A error value
            Registry.Add("ISNONTEXT", ISNONTEXT); // Returns TRUE if the value is not text
            Registry.Add("ISNUMBER", ISNUMBER); // Returns TRUE if the value is a number
            Registry.Add("ISODD", ISODD); // Returns TRUE if the number is odd
            //Registry.Add("ISREF", ISREF); // Returns TRUE if the value is a reference
            Registry.Add("ISTEXT", ISTEXT); // Returns TRUE if the value is text
            Registry.Add("N", N); // Returns a value converted to a number
            Registry.Add("NA", NA); // Returns the error value #N/A
            //Registry.Add("SHEET", SHEET); // Returns the sheet number of the referenced sheet
            //Registry.Add("SHEETS", SHEETS); // Returns the number of sheets in a reference
            Registry.Add("TYPE", TYPE); // Returns a number indicating the data type of a value
        }

        private void RegisterLogicalFunctions()
        {
            Registry.Add("AND", AND); // Returns TRUE if all of its arguments are TRUE
            Registry.Add("IF", IF); // Specifies a logical test to perform
            Registry.Add("IFERROR", IFERROR); // Returns a value you specify if a formula evaluates to an error; otherwise, returns the result of the formula
            Registry.Add("IFNA", IFNA); // Returns the value you specify if the expression resolves to #N/A, otherwise returns the result of the expression
            //Registry.Add("IFS", IFS); // [2019] Checks whether one or more conditions are met and returns a value that corresponds to the first TRUE condition.
            Registry.Add("NOT", NOT); // Reverses the logic of its argument
            Registry.Add("OR", OR); // Returns TRUE if any argument is TRUE
            //Registry.Add("SWITCH", SWITCH); // [2016] Evaluates an expression against a list of values and returns the result corresponding to the first matching value. If there is no match, an optional default value may be returned.
            Registry.Add("XOR", XOR); // Returns a logical exclusive OR of all arguments
        }

        private void RegisterLookupAndReferenceFunctions()
        {
            // Registry.Add("ADDRESS", ADDRESS);  // Returns a reference as text to a single cell in a worksheet
            // Registry.Add("AREAS", AREAS);  // Returns the number of areas in a reference
            Registry.Add("CHOOSE", CHOOSE);  // Chooses a value from a list of values
            // Registry.Add("COLUMN", COLUMN);  // Returns the column number of a reference
            Registry.Add("COLUMNS", COLUMNS);  // Returns the number of columns in a reference
            // Registry.Add("FILTER", FILTER);  // [Office 365 button] Filters a range of data based on criteria you define
            // Registry.Add("FORMULATEXT", FORMULATEXT);  // Returns the formula at the given reference as text
            // Registry.Add("GETPIVOTDATA", GETPIVOTDATA);  // Returns data stored in a PivotTable report
            Registry.Add("HLOOKUP", HLOOKUP);  // Looks in the top row of an array and returns the value of the indicated cell
            Registry.Add("HYPERLINK", HYPERLINK);  // Creates a shortcut or jump that opens a document stored on a network server, an intranet, or the Internet
            Registry.Add("INDEX", INDEX);  // Uses an index to choose a value from a reference or array
            // Registry.Add("INDIRECT", INDIRECT);  // Returns a reference indicated by a text value
            Registry.Add("LOOKUP", LOOKUP);  // Looks up values in a vector or array
            Registry.Add("MATCH", MATCH);  // Looks up values in a reference or array
            // Registry.Add("OFFSET", OFFSET);  // Returns a reference offset from a given reference
            // Registry.Add("ROW", ROW);  // Returns the row number of a reference
            Registry.Add("ROWS", ROWS);  // Returns the number of rows in a reference
            // Registry.Add("RTD", RTD);  // Retrieves real-time data from a program that supports COM automation
            // Registry.Add("SORT", SORT);  // [Office 365 button] Sorts the contents of a range or array
            // Registry.Add("SORTBY", SORTBY);  // [Office 365 button] Sorts the contents of a range or array based on the values in a corresponding range or array
            // Registry.Add("TRANSPOSE", TRANSPOSE);  // Returns the transpose of an array
            Registry.Add("UNIQUE", UNIQUE);  // [Office 365 button] Returns a list of unique values in a list or range
            Registry.Add("VLOOKUP", VLOOKUP);  // Looks in the first column of an array and moves across the row to return the value of a cell
            Registry.Add("XLOOKUP", XLOOKUP);  // [Office 365 button] Searches a range or an array, and returns an item corresponding to the first match it finds. If a match doesn't exist, then XLOOKUP can return the closest (approximate) match. 
            // Registry.Add("XMATCH", XMATCH);  // [Office 365 button] Returns the relative position of an item in an array or range of cells. 
        }

        private void RegisterMathTrigonometryFunctions()
        {
            Registry.Add("ABS", ABS);  // Returns the absolute value of a number
            Registry.Add("ACOS", ACOS);  // Returns the arccosine of a number
            Registry.Add("ACOSH", ACOSH);  // Returns the inverse hyperbolic cosine of a number
            Registry.Add("ACOT", ACOT);  // Returns the arccotangent of a number
            Registry.Add("ACOTH", ACOTH);  // Returns the hyperbolic arccotangent of a number
            // Registry.Add("AGGREGATE", AGGREGATE);  // Returns an aggregate in a list or database
            // Registry.Add("ARABIC", ARABIC);  // Converts a Roman number to Arabic, as a number
            Registry.Add("ASIN", ASIN);  // Returns the arcsine of a number
            Registry.Add("ASINH", ASINH);  // Returns the inverse hyperbolic sine of a number
            Registry.Add("ATAN", ATAN);  // Returns the arctangent of a number
            Registry.Add("ATAN2", ATAN2);  // Returns the arctangent from x- and y-coordinates
            Registry.Add("ATANH", ATANH);  // Returns the inverse hyperbolic tangent of a number
            // Registry.Add("BASE", BASE);  // Converts a number into a text representation with the given radix (base)
            Registry.Add("CEILING", CEILING);  // Rounds a number to the nearest integer or to the nearest multiple of significance
            Registry.Add("CEILING.MATH", CEILING_MATH);  // Rounds a number up, to the nearest integer or to the nearest multiple of significance
            Registry.Add("CEILING.PRECISE", CEILING_PRECISE);  // Rounds a number the nearest integer or to the nearest multiple of significance. Regardless of the sign of the number, the number is rounded up.
            // Registry.Add("COMBIN", COMBIN);  // Returns the number of combinations for a given number of objects
            // Registry.Add("COMBINA", COMBINA);  // Returns the number of combinations with repetitions for a given number of items
            Registry.Add("COS", COS);  // Returns the cosine of a number
            Registry.Add("COSH", COSH);  // Returns the hyperbolic cosine of a number
            Registry.Add("COT", COT);  // Returns the cotangent of an angle
            Registry.Add("COTH", COTH);  // Returns the hyperbolic cotangent of a number
            Registry.Add("CSC", CSC);  // Returns the cosecant of an angle
            Registry.Add("CSCH", CSCH);  // Returns the hyperbolic cosecant of an angle
            // Registry.Add("DECIMAL", DECIMAL);  // Converts a text representation of a number in a given base into a decimal number
            // Registry.Add("DEGREES", DEGREES);  // Converts radians to degrees
            // Registry.Add("EVEN", EVEN);  // Rounds a number up to the nearest even integer
            Registry.Add("EXP", EXP);  // Returns e raised to the power of a given number
            // Registry.Add("FACT", FACT);  // Returns the factorial of a number
            // Registry.Add("FACTDOUBLE", FACTDOUBLE);  // Returns the double factorial of a number
            Registry.Add("FLOOR", FLOOR);  // Rounds a number down, toward zero
            Registry.Add("FLOOR.MATH", FLOOR_MATH);  // Rounds a number down, to the nearest integer or to the nearest multiple of significance
            Registry.Add("FLOOR.PRECISE", FLOOR_PRECISE);  // Rounds a number down to the nearest integer or to the nearest multiple of significance. Regardless of the sign of the number, the number is rounded down.
            // Registry.Add("GCD", GCD);  // Returns the greatest common divisor
            Registry.Add("INT", INT);  // Rounds a number down to the nearest integer
            // Registry.Add("ISO.CEILING", ISO.CEILING);  // Returns a number that is rounded up to the nearest integer or to the nearest multiple of significance
            // Registry.Add("LCM", LCM);  // Returns the least common multiple
            Registry.Add("LN", LN);  // Returns the natural logarithm of a number
            Registry.Add("LOG", LOG);  // Returns the logarithm of a number to a specified base
            Registry.Add("LOG10", LOG10);  // Returns the base-10 logarithm of a number
            // Registry.Add("MDETERM", MDETERM);  // Returns the matrix determinant of an array
            // Registry.Add("MINVERSE", MINVERSE);  // Returns the matrix inverse of an array
            // Registry.Add("MMULT", MMULT);  // Returns the matrix product of two arrays
            Registry.Add("MOD", MOD);  // Returns the remainder from division
            // Registry.Add("MROUND", MROUND);  // Returns a number rounded to the desired multiple
            // Registry.Add("MULTINOMIAL", MULTINOMIAL);  // Returns the multinomial of a set of numbers
            // Registry.Add("MUNIT", MUNIT);  // Returns the unit matrix or the specified dimension
            // Registry.Add("ODD", ODD);  // Rounds a number up to the nearest odd integer
            Registry.Add("PI", PI);  // Returns the value of pi
            Registry.Add("POWER", POWER);  // Returns the result of a number raised to a power
            Registry.Add("PRODUCT", PRODUCT);  // Multiplies its arguments
            // Registry.Add("QUOTIENT", QUOTIENT);  // Returns the integer portion of a division
            // Registry.Add("RADIANS", RADIANS);  // Converts degrees to radians
            Registry.Add("RAND", RAND);  // Returns a random number between 0 and 1
            // Registry.Add("RANDARRAY", RANDARRAY);  // [Office 365 button] Returns an array of random numbers between 0 and 1. However, you can specify the number of rows and columns to fill, minimum and maximum values, and whether to return whole numbers or decimal values.
            Registry.Add("RANDBETWEEN", RANDBETWEEN);  // Returns a random number between the numbers you specify
            // Registry.Add("ROMAN", ROMAN);  // Converts an Arabic numeral to Roman, as text
            Registry.Add("ROUND", ROUND);  // Rounds a number to a specified number of digits
            Registry.Add("ROUNDDOWN", ROUNDDOWN);  // Rounds a number down, toward zero
            Registry.Add("ROUNDUP", ROUNDUP);  // Rounds a number up, away from zero
            Registry.Add("SEC", SEC);  // Returns the secant of an angle
            Registry.Add("SECH", SECH);  // Returns the hyperbolic secant of an angle
            // Registry.Add("SERIESSUM", SERIESSUM);  // Returns the sum of a power series based on the formula
            // Registry.Add("SEQUENCE", SEQUENCE);  // [Office 365 button] Generates a list of sequential numbers in an array, such as 1, 2, 3, 4
            Registry.Add("SIGN", SIGN);  // Returns the sign of a number
            Registry.Add("SIN", SIN);  // Returns the sine of the given angle
            Registry.Add("SINH", SINH);  // Returns the hyperbolic sine of a number
            // Registry.Add("SQRT", SQRT);  // Returns a positive square root
            // Registry.Add("SQRTPI", SQRTPI);  // Returns the square root of (number * pi)
            // Registry.Add("SUBTOTAL", SUBTOTAL);  // Returns a subtotal in a list or database
            Registry.Add("SUM", SUM);  // Adds its arguments
            Registry.Add("SUMIF", SUMIF);  // Adds the cells specified by a given criteria
            // Registry.Add("SUMIFS", SUMIFS);  // [2019] Adds the cells in a range that meet multiple criteria
            // Registry.Add("SUMPRODUCT", SUMPRODUCT);  // Returns the sum of the products of corresponding array components
            // Registry.Add("SUMSQ", SUMSQ);  // Returns the sum of the squares of the arguments
            // Registry.Add("SUMX2MY2", SUMX2MY2);  // Returns the sum of the difference of squares of corresponding values in two arrays
            // Registry.Add("SUMX2PY2", SUMX2PY2);  // Returns the sum of the sum of squares of corresponding values in two arrays
            // Registry.Add("SUMXMY2", SUMXMY2);  // Returns the sum of squares of differences of corresponding values in two arrays
            Registry.Add("TAN", TAN);  // Returns the tangent of a number
            Registry.Add("TANH", TANH);  // Returns the hyperbolic tangent of a number
            Registry.Add("TRUNC", TRUNC);  // Truncates a number to an integer
        }

        private void RegisterStatisticalFunctions()
        {
            // Registry.Add("AVEDEV", AVEDEV);  // Returns the average of the absolute deviations of data points from their mean
            // Registry.Add("AVERAGE", AVERAGE);  // Returns the average of its arguments
            // Registry.Add("AVERAGEA", AVERAGEA);  // Returns the average of its arguments, including numbers, text, and logical values
            // Registry.Add("AVERAGEIF", AVERAGEIF);  // Returns the average (arithmetic mean) of all the cells in a range that meet a given criteria
            // Registry.Add("AVERAGEIFS", AVERAGEIFS);  // [2019] Returns the average (arithmetic mean) of all cells that meet multiple criteria
            // Registry.Add("BETA.DIST", BETA.DIST);  // Returns the beta cumulative distribution function
            // Registry.Add("BETA.INV", BETA.INV);  // Returns the inverse of the cumulative distribution function for a specified beta distribution
            // Registry.Add("BINOM.DIST", BINOM.DIST);  // Returns the individual term binomial distribution probability
            // Registry.Add("BINOM.DIST.RANGE", BINOM.DIST.RANGE);  // Returns the probability of a trial result using a binomial distribution
            // Registry.Add("BINOM.INV", BINOM.INV);  // Returns the smallest value for which the cumulative binomial distribution is less than or equal to a criterion value
            // Registry.Add("CHISQ.DIST", CHISQ.DIST);  // Returns the cumulative beta probability density function
            // Registry.Add("CHISQ.DIST.RT", CHISQ.DIST.RT);  // Returns the one-tailed probability of the chi-squared distribution
            // Registry.Add("CHISQ.INV", CHISQ.INV);  // Returns the cumulative beta probability density function
            // Registry.Add("CHISQ.INV.RT", CHISQ.INV.RT);  // Returns the inverse of the one-tailed probability of the chi-squared distribution
            // Registry.Add("CHISQ.TEST", CHISQ.TEST);  // Returns the test for independence
            // Registry.Add("CONFIDENCE.NORM", CONFIDENCE.NORM);  // Returns the confidence interval for a population mean
            // Registry.Add("CONFIDENCE.T", CONFIDENCE.T);  // Returns the confidence interval for a population mean, using a Student's t distribution
            // Registry.Add("CORREL", CORREL);  // Returns the correlation coefficient between two data sets
            // Registry.Add("COUNT", COUNT);  // Counts how many numbers are in the list of arguments
            // Registry.Add("COUNTA", COUNTA);  // Counts how many values are in the list of arguments
            // Registry.Add("COUNTBLANK", COUNTBLANK);  // Counts the number of blank cells within a range
            // Registry.Add("COUNTIF", COUNTIF);  // Counts the number of cells within a range that meet the given criteria
            // Registry.Add("COUNTIFS", COUNTIFS);  // [2019] Counts the number of cells within a range that meet multiple criteria
            // Registry.Add("COVARIANCE.P", COVARIANCE.P);  // Returns covariance, the average of the products of paired deviations
            // Registry.Add("COVARIANCE.S", COVARIANCE.S);  // Returns the sample covariance, the average of the products deviations for each data point pair in two data sets
            // Registry.Add("DEVSQ", DEVSQ);  // Returns the sum of squares of deviations
            // Registry.Add("EXPON.DIST", EXPON.DIST);  // Returns the exponential distribution
            // Registry.Add("F.DIST", F.DIST);  // Returns the F probability distribution
            // Registry.Add("F.DIST.RT", F.DIST.RT);  // Returns the F probability distribution
            // Registry.Add("F.INV", F.INV);  // Returns the inverse of the F probability distribution
            // Registry.Add("F.INV.RT", F.INV.RT);  // Returns the inverse of the F probability distribution
            // Registry.Add("F.TEST", F.TEST);  // Returns the result of an F-test
            // Registry.Add("FISHER", FISHER);  // Returns the Fisher transformation
            // Registry.Add("FISHERINV", FISHERINV);  // Returns the inverse of the Fisher transformation
            // Registry.Add("FORECAST", FORECAST);  // Returns a value along a linear trend. Note: In Excel 2016, this function is replaced with FORECAST.LINEAR as part of the new Forecasting functions, but it's still available for compatibility with earlier versions.
            // Registry.Add("FORECAST.ETS", FORECAST.ETS);  // [2016] Returns a future value based on existing (historical) values by using the AAA version of the Exponential Smoothing (ETS) algorithm
            // Registry.Add("FORECAST.ETS.CONFINT", FORECAST.ETS.CONFINT);  // [2016] Returns a confidence interval for the forecast value at the specified target date
            // Registry.Add("FORECAST.ETS.SEASONALITY", FORECAST.ETS.SEASONALITY);  // [2016] Returns the length of the repetitive pattern Excel detects for the specified time series
            // Registry.Add("FORECAST.ETS.STAT", FORECAST.ETS.STAT);  // [2016] Returns a statistical value as a result of time series forecasting
            // Registry.Add("FORECAST.LINEAR", FORECAST.LINEAR);  // [2016] Returns a future value based on existing values
            // Registry.Add("FREQUENCY", FREQUENCY);  // Returns a frequency distribution as a vertical array
            // Registry.Add("GAMMA", GAMMA);  // Returns the Gamma function value
            // Registry.Add("GAMMA.DIST", GAMMA.DIST);  // Returns the gamma distribution
            // Registry.Add("GAMMA.INV", GAMMA.INV);  // Returns the inverse of the gamma cumulative distribution
            // Registry.Add("GAMMALN", GAMMALN);  // Returns the natural logarithm of the gamma function, Γ(x)
            // Registry.Add("GAMMALN.PRECISE", GAMMALN.PRECISE);  // Returns the natural logarithm of the gamma function, Γ(x)
            // Registry.Add("GAUSS", GAUSS);  // Returns 0.5 less than the standard normal cumulative distribution
            // Registry.Add("GEOMEAN", GEOMEAN);  // Returns the geometric mean
            // Registry.Add("GROWTH", GROWTH);  // Returns values along an exponential trend
            // Registry.Add("HARMEAN", HARMEAN);  // Returns the harmonic mean
            // Registry.Add("HYPGEOM.DIST", HYPGEOM.DIST);  // Returns the hypergeometric distribution
            // Registry.Add("INTERCEPT", INTERCEPT);  // Returns the intercept of the linear regression line
            // Registry.Add("KURT", KURT);  // Returns the kurtosis of a data set
            // Registry.Add("LARGE", LARGE);  // Returns the k-th largest value in a data set
            // Registry.Add("LINEST", LINEST);  // Returns the parameters of a linear trend
            // Registry.Add("LOGEST", LOGEST);  // Returns the parameters of an exponential trend
            // Registry.Add("LOGNORM.DIST", LOGNORM.DIST);  // Returns the cumulative lognormal distribution
            // Registry.Add("LOGNORM.INV", LOGNORM.INV);  // Returns the inverse of the lognormal cumulative distribution
            // Registry.Add("MAX", MAX);  // Returns the maximum value in a list of arguments
            // Registry.Add("MAXA", MAXA);  // Returns the maximum value in a list of arguments, including numbers, text, and logical values
            // Registry.Add("MAXIFS", MAXIFS);  // [2019] Returns the maximum value among cells specified by a given set of conditions or criteria
            // Registry.Add("MEDIAN", MEDIAN);  // Returns the median of the given numbers
            // Registry.Add("MIN", MIN);  // Returns the minimum value in a list of arguments
            // Registry.Add("MINA", MINA);  // Returns the smallest value in a list of arguments, including numbers, text, and logical values
            // Registry.Add("MINIFS", MINIFS);  // [2019] Returns the minimum value among cells specified by a given set of conditions or criteria.
            // Registry.Add("MODE.MULT", MODE.MULT);  // Returns a vertical array of the most frequently occurring, or repetitive values in an array or range of data
            // Registry.Add("MODE.SNGL", MODE.SNGL);  // Returns the most common value in a data set
            // Registry.Add("NEGBINOM.DIST", NEGBINOM.DIST);  // Returns the negative binomial distribution
            // Registry.Add("NORM.DIST", NORM.DIST);  // Returns the normal cumulative distribution
            // Registry.Add("NORM.INV", NORM.INV);  // Returns the inverse of the normal cumulative distribution
            // Registry.Add("NORM.S.DIST", NORM.S.DIST);  // Returns the standard normal cumulative distribution
            // Registry.Add("NORM.S.INV", NORM.S.INV);  // Returns the inverse of the standard normal cumulative distribution
            // Registry.Add("PEARSON", PEARSON);  // Returns the Pearson product moment correlation coefficient
            // Registry.Add("PERCENTILE.EXC", PERCENTILE.EXC);  // Returns the k-th percentile of values in a range, where k is in the range 0..1, exclusive
            // Registry.Add("PERCENTILE.INC", PERCENTILE.INC);  // Returns the k-th percentile of values in a range
            // Registry.Add("PERCENTRANK.EXC", PERCENTRANK.EXC);  // Returns the rank of a value in a data set as a percentage (0..1, exclusive) of the data set
            // Registry.Add("PERCENTRANK.INC", PERCENTRANK.INC);  // Returns the percentage rank of a value in a data set
            // Registry.Add("PERMUT", PERMUT);  // Returns the number of permutations for a given number of objects
            // Registry.Add("PERMUTATIONA", PERMUTATIONA);  // Returns the number of permutations for a given number of objects (with repetitions) that can be selected from the total objects
            // Registry.Add("PHI", PHI);  // Returns the value of the density function for a standard normal distribution
            // Registry.Add("POISSON.DIST", POISSON.DIST);  // Returns the Poisson distribution
            // Registry.Add("PROB", PROB);  // Returns the probability that values in a range are between two limits
            // Registry.Add("QUARTILE.EXC", QUARTILE.EXC);  // Returns the quartile of the data set, based on percentile values from 0..1, exclusive
            // Registry.Add("QUARTILE.INC", QUARTILE.INC);  // Returns the quartile of a data set
            // Registry.Add("RANK.AVG", RANK.AVG);  // Returns the rank of a number in a list of numbers
            // Registry.Add("RANK.EQ", RANK.EQ);  // Returns the rank of a number in a list of numbers
            // Registry.Add("RSQ", RSQ);  // Returns the square of the Pearson product moment correlation coefficient
            // Registry.Add("SKEW", SKEW);  // Returns the skewness of a distribution
            // Registry.Add("SKEW.P", SKEW.P);  // Returns the skewness of a distribution based on a population: a characterization of the degree of asymmetry of a distribution around its mean
            // Registry.Add("SLOPE", SLOPE);  // Returns the slope of the linear regression line
            // Registry.Add("SMALL", SMALL);  // Returns the k-th smallest value in a data set
            // Registry.Add("STANDARDIZE", STANDARDIZE);  // Returns a normalized value
            // Registry.Add("STDEV.P", STDEV.P);  // Calculates standard deviation based on the entire population
            // Registry.Add("STDEV.S", STDEV.S);  // Estimates standard deviation based on a sample
            // Registry.Add("STDEVA", STDEVA);  // Estimates standard deviation based on a sample, including numbers, text, and logical values
            // Registry.Add("STDEVPA", STDEVPA);  // Calculates standard deviation based on the entire population, including numbers, text, and logical values
            // Registry.Add("STEYX", STEYX);  // Returns the standard error of the predicted y-value for each x in the regression
            // Registry.Add("T.DIST", T.DIST);  // Returns the Percentage Points (probability) for the Student t-distribution
            // Registry.Add("T.DIST.2T", T.DIST.2T);  // Returns the Percentage Points (probability) for the Student t-distribution
            // Registry.Add("T.DIST.RT", T.DIST.RT);  // Returns the Student's t-distribution
            // Registry.Add("T.INV", T.INV);  // Returns the t-value of the Student's t-distribution as a function of the probability and the degrees of freedom
            // Registry.Add("T.INV.2T", T.INV.2T);  // Returns the inverse of the Student's t-distribution
            // Registry.Add("T.TEST", T.TEST);  // Returns the probability associated with a Student's t-test
            // Registry.Add("TREND", TREND);  // Returns values along a linear trend
            // Registry.Add("TRIMMEAN", TRIMMEAN);  // Returns the mean of the interior of a data set
            // Registry.Add("VAR.P", VAR.P);  // Calculates variance based on the entire population
            // Registry.Add("VAR.S", VAR.S);  // Estimates variance based on a sample
            // Registry.Add("VARA", VARA);  // Estimates variance based on a sample, including numbers, text, and logical values
            // Registry.Add("VARPA", VARPA);  // Calculates variance based on the entire population, including numbers, text, and logical values
            // Registry.Add("WEIBULL.DIST", WEIBULL.DIST);  // Returns the Weibull distribution
            // Registry.Add("Z.TEST", Z.TEST);  // Returns the one-tailed probability-value of a z-test
        }

        private void RegisterTextFunctions()
        {
            //Registry.Add("ASC", ASC); // Changes full-width (double-byte) English letters or katakana within a character string to half-width (single-byte) characters
            //Registry.Add("BAHTTEXT", BAHTTEXT); // Converts a number to text, using the ß (baht) currency format
            //Registry.Add("CHAR", CHAR); // Returns the character specified by the code number
            //Registry.Add("CLEAN", CLEAN); // Removes all nonprintable characters from text
            //Registry.Add("CODE", CODE); // Returns a numeric code for the first character in a text string
            //Registry.Add("CONCAT", CONCAT); // [2019] Combines the text from multiple ranges and/or strings, but it doesn't provide the delimiter or IgnoreEmpty arguments.
            Registry.Add("CONCATENATE", CONCATENATE); // Joins several text items into one text item
            //Registry.Add("DBCS", DBCS); // Changes half-width (single-byte) English letters or katakana within a character string to full-width (double-byte) characters
            //Registry.Add("DOLLAR", DOLLAR); // Converts a number to text, using the $ (dollar) currency format
            Registry.Add("EXACT", EXACT); // Checks to see if two text values are identical
            Registry.Add("FIND", FIND); // Finds one text value within another (case-sensitive)
            Registry.Add("FIXED", FIXED); // Formats a number as text with a fixed number of decimals
            Registry.Add("LEFT", LEFT); // Returns the leftmost characters from a text value
            Registry.Add("LEN", LEN); // Returns the number of characters in a text string
            Registry.Add("LOWER", LOWER); // Converts text to lowercase
            Registry.Add("MID", MID); // Returns a specific number of characters from a text string starting at the position you specify
            //Registry.Add("NUMBERVALUE", NUMBERVALUE); // Converts text to number in a locale-independent manner
            //Registry.Add("PHONETIC", PHONETIC); // Extracts the phonetic (furigana) characters from a text string
            Registry.Add("PROPER", PROPER); // Capitalizes the first letter in each word of a text value
            Registry.Add("REPLACE", REPLACE); // Replaces characters within text
            //Registry.Add("REPT", REPT); // Repeats text a given number of times
            Registry.Add("RIGHT", RIGHT); // Returns the rightmost characters from a text value
            Registry.Add("SEARCH", SEARCH); // Finds one text value within another (not case-sensitive)
            Registry.Add("SUBSTITUTE", SUBSTITUTE); // Substitutes new text for old text in a text string
            Registry.Add("T", T); // Converts its arguments to text
            Registry.Add("TEXT", TEXT); // Formats a number and converts it to text
            //Registry.Add("TEXTJOIN", TEXTJOIN); // [2019] Combines the text from multiple ranges and/or strings, and includes a delimiter you specify between each text value that will be combined. If the delimiter is an empty text string, this function will effectively concatenate the ranges.
            Registry.Add("TRIM", TRIM); // Removes spaces from text
            //Registry.Add("UNICHAR", UNICHAR); // Returns the Unicode character that is references by the given numeric value
            //Registry.Add("UNICODE", UNICODE); // Returns the number (code point) that corresponds to the first character of the text
            Registry.Add("UPPER", UPPER); // Converts text to uppercase
            Registry.Add("VALUE", VALUE); // Converts a text argument to a number
        }

        private void RegisterWebFunctions()
        {
            Registry.Add("ENCODEURL", ENCODEURL); // Returns a URL-encoded string
            Registry.Add("FILTERXML", FILTERXML); // Returns specific data from the XML content by using the specified XPath
            Registry.Add("WEBSERVICE", WEBSERVICE); // Returns data from a web service
        }

        private void RegisterUserDefinedFunctions()
        {
            // ######################### Custom functions #########################
            Registry.Add("SYSDATE", SYSDATE);
            Registry.Add("SOURCE", SOURCE);
            Registry.Add("RQD", RQD);
            Registry.Add("RQL", RQL);
            Registry.Add("RQR", RQR);
            Registry.Add("CONTENT", CONTENT);
            Registry.Add("MAPVALUE", MAPVALUE);
            Registry.Add("MAPITEM", MAPITEM);
            Registry.Add("GETITEM", GETITEM);
            Registry.Add("GETLIST", GETLIST);
        }


        public ExcelValue Evaluate(string name, List<ExcelValue> args, ExpressionScope scope)
        {
            if (Registry.TryGetValue(name, out var function))
                return function(args, scope);
            else if (name == "ARRAY")
                return ARRAY(args, scope);
            else if (name == "ARRAYROW")
                return ARRAYROW(args, scope);
            else
                throw new InvalidOperationException($"Unknown function name: {name}");
        }

        private ExcelValue ARRAY(List<ExcelValue> args, ExpressionScope scope)
        {
            return new ExcelValue.ArrayValue(args, scope.OutLanguage);
        }
        private ExcelValue ARRAYROW(List<ExcelValue> args, ExpressionScope scope)
        {
            return new ExcelValue.ArrayValue(args, scope.OutLanguage);
        }
    }
}
