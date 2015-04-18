namespace Model.Settings
{ 
    public interface ISettings
    {
    }

    public class Setting : BaseEntity
    {
        public Setting() { }

        public Setting(string name, string value, int storeId = 0)
        {
            this.Name = name;
            this.Value = value;
        }

        public string Name { get; set; }

        public string Value { get; set; }

    }
}
