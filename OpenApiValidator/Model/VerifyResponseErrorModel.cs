namespace OpenApiValidator.Model
{
    public sealed class VerifyResponseErrorModel
    {
        public void Add(string error)
        {
            Errors.Add(error);
        }

        public string SchemaName
        {
            get;
            set;
        }



        public List<string> Errors { get; set; } = new List<string>();
    }
}
