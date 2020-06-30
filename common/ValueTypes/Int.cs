﻿namespace funkylib.common.ValueTypes {
  public static class Int {
    /// <summary>
    /// Safely parses int from string.
    /// </summary>
    /// <param name="s">String to parse</param>
    /// <returns>If parse was successful returns `Some`, else returns `None`</returns>
    public static Option<int> parse(string s) =>
      int.TryParse(s, out var temp) ? temp.some() : Option.None;
  }
}