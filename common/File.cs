﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;

namespace funkylib.common {
  /// <summary>
  /// Wrapper with helper methods for <see cref="System.IO.File"/>
  /// </summary>
  public class File {
    public readonly string path;
    public File(string path) { this.path = path; }

    public Try<StreamReader> getReader => () => System.IO.File.OpenText(path);
    public Try<StreamWriter> getWriter => () => System.IO.File.CreateText(path);
  }

  public static class FileExt {
    public static Try<R> readFile<R>(this File @this, Func<StreamReader, R> func) =>
      @this.getReader.map(streamReader => F.@using(streamReader, func));

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