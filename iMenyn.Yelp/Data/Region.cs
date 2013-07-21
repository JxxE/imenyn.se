namespace iMenyn.Yelp.Data
{
    /// <summary>
    /// Suggested bounds in a map to display results in
    /// </summary>
	public class Region
	{
        /// <summary>
        /// Center position of map bounds
        /// </summary>
		public Coordinate center { get; set; }

        /// <summary>
        /// Span of suggested map bounds
        /// </summary>
		public Span span { get; set;  }		
	}
}
