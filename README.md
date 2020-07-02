# Lightweight functional programming library in C#

“Funkylib” is intended for programmers who have only started to learn FP in C#, but do not want to write a whole library from scratch. It is also useful for experienced programmers who find that they only need a few basic FP types and a highly customizable library in their projects.

## How to install

Inside your project run:

```dotnet add package funkylib --version 2.1.0```

Latest version of the “funkylib” can be found at https://www.nuget.org/, also many IDE’s (Rider, Visual Studio) have Nuget package tooling included where you can find “funkylib

## Example

```
Either<String, double> parseDouble(String s) {
  if (double.TryParse(s, out var temp)) return temp.right();
  return "Error while parsing".left();
}

var powerOfTwo = parseDouble("not-a-number").map(number => number * number);
powerOfTwo.fold(error => Console.Write(error), number => Console.Write(number));
```

For extensive example on how is “funkylib” is being used check https://github.com/Donatas-L/funkylib/tree/master/examples

## Interactive documentation
Please have a look at the documentation which is hosted at https://donatas-l.github.io/funkylib/ you can find there a whole documentation with descriptions and examples for main data types and methods.

## Further development

The project is still lacking 100% documentation coverage and some helper methods which can be found at latest version Scala. Simpler examples for beginner programmers also would help project to meet its intended audience. 

If you have specific needs for your project and think that it should be in the library create an issue.
