# Document Creator

An opinionated solution for creating documents from JSON objects (API payloads) based on Word templates and Excel formula configuration.

![GitHub Workflow Status](https://img.shields.io/github/workflow/status/pkokki/DocumentCreator/.NET%20Core/master) 
![GitHub tag (latest by date)](https://img.shields.io/github/v/tag/pkokki/DocumentCreator) 
![GitHub commits since latest release (by date including pre-releases)](https://img.shields.io/github/commits-since/pkokki/DocumentCreator/0.2.0-alpha/master?include_prereleases) 
![GitHub code size in bytes](https://img.shields.io/github/languages/code-size/pkokki/DocumentCreator) 
![GitHub issues](https://img.shields.io/github/issues/pkokki/DocumentCreator)

## Getting Started

Create Word documents and HTML pages by combining Word templates (documents with content controls) and JSON data via transformations defined as Excel formulas. Try the [quickstart](https://document-creator.azurewebsites.net/#/) to create a document in four steps.

## Transformation examples

![Nuget](https://img.shields.io/nuget/v/JsonExcelExpressions)

Given the data

```json
{
    "x1": 10,
    "x2": 2,
    "x3": { "firstname": "john", "surname": "smith" },
    "x4": [100, 200, 300, 400],
    "x5": {
        "y1": "A quick brown fox jumps over the lazy dog",
        "y2": "brown",
        "y3": "#red#",
        "y4": [ 1, 2, 3]
    }
}
```

we can evaluate the following expressions

| expression | result (in el-GR culture) |
| --- | --- | 
| `8 / 2 * (2 + 2)` | 16 |
| `x1 * x2 - x1 / x2` | 15 |
| `CONCATENATE(x3.firstname, " ", x3.surname)` | john smith |
| `SUM(x4) * 24%` | 240,00 |
| `x4` | ['100','200','300','400'] |
| `PROPER(REPLACE(x5.y1, SEARCH(x5.y2, x5.y1), LEN(x5.y2), x5.y3))` | A Quick #Red# Fox Jumps Over The Lazy Dog |
| `IF(x1 + IFNA(missing.path, x2) > 10, ">10", "<=10")` | >10 |
| `DATE(2020, 4, 28) + x5.y4[1]` | 30/4/2020 |
| `IF(__A1 > __A2, UPPER(__A3), "?")` | JOHN SMITH |

and many more. Test it live [here](https://document-creator.azurewebsites.net/#/expressions).

## Installation

TBD

## Built with

### API

* [ASP.NET Core](https://github.com/dotnet/aspnetcore) 
* [Open XML SDK](https://github.com/OfficeDev/Open-XML-SDK) - to generate documents and extract content from Word and Excel files
* [Open XML PowerTools](https://github.com/OfficeDev/Open-Xml-PowerTools) - to convert Word documents to HTML/CSS

### Web client

* [Angular](https://github.com/angular/angular)
* [Angular Material](https://github.com/angular/components)

### Tests

* [xUnit.net](https://github.com/xunit/xunit)
* [Moq](https://github.com/moq/moq4)

## Contributing

Contributions are welcome. Please contact the [project maintainer](https://twitter.com/pkokki).

## Versions

For the versions available, see the [tags on this repository](https://github.com/pkokki/DocumentCreator/tags). 

## Authors

* **Panos** - *Initial work* - [pkokki](https://twitter.com/pkokki)

## License

This project is licensed under the MIT License - see the [LICENSE.TXT](LICENSE.TXT) file for details

## Credits
* Tim Hall, [JSON conversion and parsing for VBA](https://github.com/VBA-tools/VBA-JSON)
* E. W. Bachtal, [Excel Formula Parsing in C#](https://ewbi.blogs.com/develops/2007/03/excel_formula_p.html)
