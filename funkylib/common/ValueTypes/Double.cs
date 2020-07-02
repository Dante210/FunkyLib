﻿namespace funkylib.common.ValueTypes {
  public static class Double {
    /// <summary>
    /// Safely parses double from string.
    /// </summary>
    /// <param name="s">String to parse</param>
    /// <returns>If parse was successful returns `Some`, else returns `None`</returns>
    public static Option<double> parse(string s) =>
      double.TryParse(s, out var temp) ? temp.some() : Option.None;
  }
}