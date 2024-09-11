namespace _006_Swagger
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class CXLSwaggerGroupAttribute : Attribute
    {
        public string GroupName { get; }

        public CXLSwaggerGroupAttribute(string groupName)
        {
            GroupName = groupName;
        }

    }
}
