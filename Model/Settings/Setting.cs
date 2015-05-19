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

        /// <summary>
        /// Gets or sets the name
        /// </summary> 
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the value
        /// </summary> 
        public string Value { get; set; }

    }
}
