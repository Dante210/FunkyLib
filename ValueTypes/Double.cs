namespace funkylib.ValueTypes
{
    public static class Double
    {
        public static Option<double> parse(string s) {
            double temp;
            return double.TryParse(s, out temp) ? temp.some() : Option.None;
        }
    }
}