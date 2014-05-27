namespace iMenyn.Data.Models
{
    public class FailedEnterprise
    {
        public Enterprise Enterprise { get; set; }
        public Menu Menu { get; set; }

        public string ErrorMessage { get; set; }
    }
}