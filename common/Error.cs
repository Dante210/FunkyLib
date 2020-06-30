namespace funkylib.common {
  /// <summary>
  /// Convenience class to hold error.
  /// <![CDATA[
  /// Useful with Either<Error, A>, when Error is `Left` and A is wanted value.
  /// ]]>
  /// </summary>
  public class Error {
    public readonly string message;
    public Error(string message) => this.message = message;

    public override string ToString() => message;
    public static implicit operator Error(string s) => new Error(s);
  }
}