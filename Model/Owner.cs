namespace Sem3_kurs.Model
{
    public class Owner
    {
        public string FullName { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }

        public List<Property> Properties { get; } = new List<Property>();

        public void AddProperty(Property property)
        {
            if (property == null) throw new ArgumentNullException(nameof(property));
            property.Owner = this;
            Properties.Add(property);
        }
    }

}
