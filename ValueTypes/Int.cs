namespace funkylib.ValueTypes {
  public static class Int {
    public static Option<int> parse(string s) {
      int temp;
      return int.TryParse(s, out temp) ? temp.some() : Option.None;
    }
  }
}