namespace SevColApp.Models
{
    public class Bank
    {
        public int Id { get; set; }
        public string Name { get; set; }        
        public string Abbreviation { get; set; }
        public int ColonyId { get; set; }
        public virtual Colony Colony { get; set; }

        public Bank(int id, string name, string abbreviation, int colonyId)
        {
            Id = id;
            Name = name;
            Abbreviation = abbreviation;
            ColonyId = colonyId;
        }
    }
}
