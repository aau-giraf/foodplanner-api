using System.Reflection;

namespace FoodplannerDataAccessSql;

public static class EntityDbTranslation
{
    // Children
        public const string Children = "children";
        public const string ChildId = "child_id";
        public const string FirstName = "first_name";
        public const string LastName = "last_name";
        public const string parentId = "parent_id";
        public const string classId = "class_id";
        // User
        public const string User = "users";
        public const string Id = "id";
        public const string Email = "email";
        public const string Password = "password";
        public const string Role = "role";
        public const string RoleApproved = "role_approved";
        public const string PinCode = "pincode";
        public const string Archived = "archived";

        // Classroom
        public const string Classroom = "classroom";
        public const string ClassName = "class_name";
        
    
        private static readonly Dictionary<string, string> _map =
            typeof(EntityDbTranslation)
                .GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)
                .ToDictionary(f => f.Name, f => f.GetValue(null)!.ToString()!);

        public static string ToDb(string propertyName)
        {
            return _map.TryGetValue(propertyName, out var db) ? db : propertyName.ToLower();
        }
}
