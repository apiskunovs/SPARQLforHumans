namespace SparqlForHumans.Models
{
    public interface IProperty : ISubject, IHasRank<double>, IHasAltLabel, IHasDescription, IHasDomain, IHasRange
    {
        //string Value { get; set; }
    }
}