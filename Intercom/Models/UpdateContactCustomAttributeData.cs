
namespace Intercom.Models
{
    public class UpdateContactCustomAttributeData
    {

        public string AttributeName { get; }
        public string AttributeValue { get; }

        public UpdateContactCustomAttributeData(string name, string value)
        {
            AttributeName = name;
            AttributeValue = value;
        }

    }
}
