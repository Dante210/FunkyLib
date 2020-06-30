using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;

namespace funkylib.common {
  public class File {
    public readonly string path;
    public File(string path) { this.path = path; }

    public Try<StreamReader> reader => () => System.IO.File.OpenText(path);
    public Try<StreamWriter> writer => () => System.IO.File.CreateText(path);
  }

  public static class FileExt {
    public static Try<R> readFile<R>(this File @this, Func<StreamReader, R> func) =>
      @this.reader.map(streamReader => F.@using(streamReader, func));

    public static Try<ImmutableList<string>> readAllLines(this File @this) {
      return readFile(@this, reader => {
        string line;
        var temp = new List<string>();
        while ((line = reader.ReadLine()) != null) {
          temp.Add(line);
        }
        return temp.ToImmutableList();
      });
    }
  }
}