# Lists and Expressions

## Lists

A list is a sequence of ***words*** separated by one or more spaces.  For example:

```
john paul george ringo
```

Each word can be surrounded by the following delimiters:

|Delimiters|Meaning|
|-|-|
|'xxx'|Text|
|"xxx"|Text|
|[xxx]|Field|
|#xxx#|Statement|

## Fields

The following formats are available:

|Format|Meanung|
|-|-|
|[objectname:field]|A reference to an object.  If the object name is ommitted the current active object is used|
|[*storename:field]|A reference to a store.  If the store name is ommitted the current active store is used|

## Expressions

An expression is a list that when evaluated, each ***word*** will be acted upon depending on its meaning and each result
will be joined into a resultant list.  For example:

```
'My name is' [*name] 'and  today is' #today()#
```

May be evaluated to:

```
My name is John and today is December 7, 1941
```
Expressions are used through the system.

## Statements

Statements are evaluated using a set of operations and functions:

### Operators

|Syntax|Meaning|
|-|-|
|( ... )|The sub-expression is evaluated|
|Unary||
|!|Not operator, operand will be converted to boolean|
|~|Not operator, operand will be converted to boolean|
|-|Unary minus operator, operand will be converted to number|
|+|Unary plus operator, operand will be converted to number|
|Dyadic (high precedence)||
|^|Power operator, operands will be converted to numbers (a^b^c -> (a^b)^c)|
|\*|Multiplication operator, operands will be converted to numbers|
|/|Division operator, operands will be converted to numbers|
|%|Modulus operator, operands will be converted to numbers|
|Dyadic (medium precedence)||
|+|Addition operator, operands will be converted to numbers|
|-|Subtraction operator, operands will be converted to numbers|
|Dyadic (low precedence)||
|&|String concantenation operator, operands will be converted to string|
|Comparison (high precedence)||
|\<|Less than operator|
|\<=|Less than or equal to operator|
|\>|Greater than operator|
|\>=|Greater than or equal to operator|
|Comparison (low precedence)||
|=|Equal to operator|
|!=|Not equal to operator|
|\<\>|Not equal to operator|
|Conditional||
|?:|Conditional if operator|
|&&|Logical AND, operands will be converted to boolean|
|\|\||Logical OR, operands will be converted to boolean|

### Functions

|Function|Format|Returns|a|b|c|
|-|-|-|-|-|-|
|abbrev|abbrev(a,b)|An abbreviated value of a given max length|The value (Required)|The max length (Required)|
|abs|abs(a)|Absolute value of a value|The value (Required)|
|acos|acos(a)|Arc cosine of a value|The value (Required)|
|adjusttimezone|adjusttimezone(a,b)|An timezone adjusted date and time|The date value (Required)|The timezone (Required)|True is the reverse adjustemnt is to take place (Optional)|
|and|and(a,b)|True if all the values are true|One or more values (Required)|
|arraycount|arraycount(a)|The count of values in an array|The name of the array (Required)|
|asamt|asamt(a)|A value as a number amount|The value (Required)|
|asin|asin(a)|Arc sine of a value|The value (Required)|
|asname|asname(a)|A value formatted as a name|The value (Required)|
|aspassword|aspassword(a)|The value encrypted as a password|The value (Required)|
|atan|atan(a)|Arc tangent of a value|The value (Required)|
|atan2|atan2(a,b)|Arc tangent2 of a value|The value (Required)|
|avg|avg(a)|Average of a list of values|One or more values (Required)|
|capword|capword(a)|The value with each word capitalized|The value (Required)|
|case|case(a)|A value from a list, using teh first value as the selector|The seletion value (Required)|One or more values to be selected (Required)|
|ceiling|ceiling(a)|Next higher integer value of a value|The value (Required)|
|choice|choice(a)|A value from a list, using teh first value as the selector|The seletion value (Required)|One or more values to be selected (Required)|
|concat|concat(a)|A list of values concantenated|One or more values (Required)|
|concatdelim|concatdelim(a)|A list of values concantenated with a delimiter in between each value|The delimiter (Required)|One or more values (Required)|
|concatif|concatif(a,b,c)|The concatenation of two values. The delimiter is used only if both values are not empty|The leading value (Required)|The trailing value (Required)|The delimiter (Required)|
|concattext|concattext(a,b)|The concantenation f values, with the first value being the default delimiter and the second value being the delimiter used for the last value pair|The default delimiter (Required)|The last delimiter (Required)|One or more values (Optional)|
|cos|cos(a)|Cosine of a value|The value (Required)|
|cosh|cosh(a)|Cosinehy[erbolic of a value|The value (Required)|
|cr|cr()|A carriage return|
|crlf|crlf()|A carriage return and line feed|
|date|date(a)|A date value formatted am MM/dd/yyyy|The date value (Required)|
|dateadd|dateadd(a,b,c)|Adds a value to a date|The date value (Required)|The amount to add (Required)|What the amount represents (y: years, m: months, w: weeks,d: days, h: hours, mi: minutes) (Required)|
|dateadjust|dateadjust(a,b)|The timezone adjusted date an time|The value (Required)|The timezone (Required)|
|dateafter|dateafter(a,b)|True if the first date is after the second date|A date value (Required)|A date value (Required)|
|dateasfilename|dateasfilename(a)|The date formatted as yyyMMddhhmm|The date value (Required)|
|datebefore|datebefore(a,b)|True if the first date is before the second date|A date value (Required)|A date value (Required)|
|datebusiness|datebusiness(a)|The next business date|The date value (Required)|
|datecompare|datecompare(a,b)|The number of days difference between to dates|The base date value (Required)|The second date value (Required)|
|dateday|dateday(a)|The day portion of a date|The date value (Required)|
|datedayofweek|datedayofweek(a)|The day of the week|The date value (Required)|
|datedayordinal|datedayordinal(a)|The day of the month in ordinal format|The date value (Required)|
|datediff|datediff(a,b,c)|The difference between two dates|The base date value (Required)|The second date value (Required)|The difference format (d: days, h: hours, mi: minutes) (Required)|
|dateformal|dateformal(a)|A date value in a formal format|The date value (Required)|
|datehour|datehour(a)|The hours portion of a date|The date value (Required)|
|datemax|datemax(a)|Last date of a list of date values|One or more date values (Required)|
|datemin|datemin(a)|First date of a list of date values|One or more date values (Required)|
|dateminute|dateminute(a)|The minutes portion of a date|The date value (Required)|
|datemonth|datemonth(a)|The month portion of a date|The date value (Required)|
|datemonthabbrev|datemonthabbrev(a)|The month name abbreviation for a date value|The date value (Required)|
|datemonthname|datemonthname(a)|The name of the month|The date value (Required)|
|dateonly|dateonly(a)|Date portion of a date value|A date value (Required)|
|dateonlysortable|dateonlysortable(a)|The date formatted as yyy-MM-dd|The date value (Required)|The timezone (Optional)|
|dateonorafter|dateonorafter(a,b)|True if the first date is the same or after the second date|A date value (Required)|A date value (Required)|
|dateonorbefore|dateonorbefore(a,b)|True if the first date isthe same or before the second date|A date value (Required)|A date value (Required)|
|datesame|datesame(a,b)|True if two dates are the same|A date value (Required)|A date value (Required)|
|datesecond|datesecond(a)|The seconds portion of a date|The date value (Required)|
|datesortable|datesortable(a)|The date and time formatted as yyy-MM-dd HH:mm|The date value (Required)|The timezone (Optional)|
|datetimefull|datetimefull(a)|A date and time value formatted|The date value (Required)|
|dateyear|dateyear(a)|The year portion of a date|The date value (Required)|
|docexists|docexists(a)|True if the document exists|The document reference (Required)|
|docextension|docextension(a)|The document extension|The document reference (Required)|
|docname|docname(a)|The document name|The document reference (Required)|
|docshortname|docshortname(a)|The document name without extension|The document reference (Required)|
|endswith|endswith(a,b)|True if a value ends with a value|The value (Required)|The ending value (Required)|
|eval|eval(a)|Evaluates an expression|The expression (Required)|
|exp|exp(a)|Exponential of a value|The value (Required)|
|fact|fact(a)|Factorial of a value|The value (Required)|
|floor|floor(a)|Previous interger of a value|The value (Required)|
|format|format(a,b)|A formatted value|The format as a C# format (Required)|The value (Required)|
|hash|hash(a)|The MD5 hash of a value|The value (Required)|
|hex|hex(a)|The hex representation of a numeric value|The value (Required)|
|html|html(a,b)|An HTML tag block|The value in the block (Required)|The tag (Required)|
|htmlb|htmlb(a)|An HTML b block|The value in the block (Required)|
|htmli|htmli(a)|An HTML i block|The value in the block (Required)|
|htmlp|htmlp(a)|An HTML p block|The value in the block (Required)|
|htmlu|htmlu(a)|An HTML u block|The value in the block (Required)|
|httpget|httpget(a)|The result of an HTTP GET|The URL (Required)|
|httppost|httppost(a,b)|The result of an HTTP POST|The URL (Required)|The value (Required)|
|if|if(a,b)|The first value where the comparison is true. If there is an odd number of parameters, the last is assumed to be the default if no comparion is true|The comparison value (Required)|The returned value (Required)|
|ifempty|ifempty(a,b)|A default value if the passed pavue is empty|The value (Required)|The default value (Required)|
|ifte|ifte(a,b)|If then/else|The comparison value (Required)|Value returned if the comparison is true (Required)|Value returned if the coparison is false (Optional)|
|indexof|indexof(a,b)|The index posiion of the second value in the first value|The value to be searched (Required)|The value to be found (Required)|
|int|int(a)|Integer of a value|The value (Required)|
|istimezonevalid|istimezonevalid(a)|True if the value is a valid timezone|The value (Required)|
|lastindexof|lastindexof(a,b)|The last index posiion of the second value in the first value|The value to be searched (Required)|The value to be found (Required)|
|left|left(a,b)|The n most leftsome characters in a value|The value (Required)|The number of characters to return (Required)|
|len|len(a)|The length of a value|The value (Required)|
|lf|lf()|A line feed|
|linkdesc|linkdesc(a)|The placeholder of the object|The object name (Required)|
|linkdscaption|linkdscaption(a)|The link dataset captio|The link value (Optional)|
|ln|ln(a)|Log n of a value|The value (Required)|
|log|log(a)|Log of a value|The value (Required)|The base (default: 10) (Optional)|
|lower|lower(a)|The lowercase representation of a value|The value (Required)|
|ltrim|ltrim(a)|The value with leading spaces removed|The value (Required)|
|max|max(a)|Largest of a list of values|One or more values (Required)|
|median|median(a)|Median of a list of values|One or more values (Required)|
|mid|mid(a,b,c)|The n characters in a value starting at position p|The value (Required)|The starting position (Required)|The number of characters.  If omitted the characters to the end (Optional)|
|min|min(a)|Smallest of a list of values|One or more values (Required)|
|namefirst|namefirst(a)|The first name in a name value|The value (Required)|
|namejob|namejob(a)|The job in a name value|The value (Required)|
|namelast|namelast(a)|The last name in a name value|The value (Required)|
|namemi|namemi(a)|The middle initial in a name value|The value (Required)|
|namemiddle|namemiddle(a)|The middle name in a name value|The value (Required)|
|namesuffix|namesuffix(a)|The suffix in a name value|The value (Required)|
|not|not(a)|The nlogical negation of a value|The value (Required)|
|now|now()|The current date and time|The timezone (Optional)|
|objds|objds(a)|True if the object is new|The object name (Required)|
|objfield|objfield(a)|The eval field reference of a field in an object|Either the field name for the default object object name or the object name (Required)|The field name if the previous parameter is the object name (Optional)|
|objid|objid(a)|The UUID of the object|The object name (Required)|
|objidonly|objidonly(a)|The ID portion of the UUID of the object|The object name (Required)|
|objisnew|objisnew(a)|True if the object is new|The object name (Required)|
|objlistcount|objlistcount(a)|The count of values in a list|The name of the object list (Required)|
|objvalue|objvalue(a)|The field value for an optional object and field|The object name if a second value is give, otherwise the field name (Required)|The field name (Optional)|
|or|or(a,b)|True if any value is true|One or more values (Required)|
|parsename|parsename(a)|Converts a name to a store|The value (Required)|The store name (Optional)|The default value (default: '') (Required)|
|placeholder|placeholder(a)|The placeholder of the object|The object name (Required)|
|pow|pow(a,b)|Value raised to a power|The value (Required)|The power (Required)|
|random|random(a)|A random number between zero and a given value|The value (Required)|
|randomstring|randomstring(a)|A random string of a given length|The length (Required)|
|right|right(a,b)|The n most rightsome characters in a value|The value (Required)|The number of characters to return (Required)|
|round|round(a)|Value rounded|The value (Required)|The number of fractional dgits (default: 0) (Optional)|
|rtrim|rtrim(a)|The value with trailing spaces removed|The value (Required)|
|shortdate|shortdate(a)|A date value formatted am MM/dd/yy|The date value (Required)|
|sign|sign(a)|Sign of a value|The value (Required)|
|sin|sin(a)|Sine of a value|The value (Required)|
|sinh|sinh(a)|Sine hyperbolic of a value|The value (Required)|
|split|split(a,b)|The nth piece in a value using a given delimiter|The value (Required)|The word position (n) (Required)|The delimiter (default: tab) (Required)|
|sqr|sqr(a)|Square of a value|The value (Required)|
|sqrt|sqrt(a)|Square root of a value|The value (Required)|
|startswith|startswith(a,b)|True if a value starts with a value|The value (Required)|The starting value (Required)|
|stdev|stdev(a)|Standard deviation of a list of values|One or more values (Required)|
|sum|sum(a)|Sum of a list of values|One or more values (Required)|
|time|time(a)|A date formatted in HH:mm format|The datevalue (Required)|
|timeap|timeap(a)|A date formatted in hh:mm tt format|The date value (Required)|
|timeelapsed|timeelapsed(a)|The elapsed time between two dates|The base date (Required)|The second date (default: today) (Optional)|The time frame (d: days (default), h: hours, m: minutes, s: seconds) (Optional)|
|timestamp|timestamp()|The current date and time as a timestamp|The date and time (default now()) (Optional)|The format (default: MM/dd/yyyy hh:mm tt) (Optional)|
|timezoneoffset|timezoneoffset(a)|The offset in minutes for a given timezone|The timezone (Required)|
|todate|todate(a)|Date value as ISO string|A date value (Required)|
|today|today()|Today's date|The timezone (Optional)|
|trim|trim(a)|Retruns the value with leading and trailing spaces emoved|The value (Required)|
|trunc|trunc(a)|Value tuncated|The value (Required)|
|tzlist|tzlist()|A list of time zones|
|upper|upper(a)|The uppercase representation of a value|The value (Required)|
|user|user()|The current user name|
|utcnow|utcnow()|The current UTC date and time|
|var|var(a)|Variance of a list of values|One or more values (Required)|
|wl|wl(a,b)|The value truncated or expanded to a given length|The value (Required)|The length (Required)|
|wm|wm(a,b)|The value truncated to a given length|The value (Required)|The length (Required)|
|word|word(a,b)|The nth word in a value|The value (Required)|The word position (n) (Required)|
|yesno|yesno(a)|The strings 'Yes' or 'No' depending on a value being true|The value (Required)|

## Directives

Directives are macro-like operations.  They are defined with the following format:

[[definition]]

There are two types of definitions:

|Type|-Meaning|
|-|-|
|Field|Defines a field.  Similar to [xxx] above|
|Command|Defines a command.  Starts with a [[#cmd]] and ends with a [[/cmd]]|

### Commands

|Command|Parameters|Meaning|
|-|-|-|
|if|field|Directive block is used if the field exists|
|iff|field comparison  value|Directive block is used if field comparison with value is true|
||=|Equality comparison, case insensitive|
||==|Equality comparison, case insensitive|
||!=|Inequiality comparino, case insensitive|
|!==|Inequality comparison, case sensitive|
||>|Greater than comparison|
||>=|Greater than or equal to comparison|
||<|Less than comparison|
||<=|Less tha or equal to comparison|
|is|field value|Directive block is used if the field is the same as the value, case insensitive|
||isnt|fiedl value|Directive block is used if the field is not the same as the value, case insensitive|
|unless||field|Directive block is used if field does not exist|
|each|field|Directive block is used once for each entry in the field list|
|with|field|Directive block is used with the field as the root value|

[Home](../README.md)
