namespace funkylib
{
    public class Error
    {
        public virtual string message { get; }
        public override string ToString() => message;
        protected Error() { }
        public Error(string message) { this.message = message; }
        public static implicit operator Error(string s) => new Error(s);
    }
}