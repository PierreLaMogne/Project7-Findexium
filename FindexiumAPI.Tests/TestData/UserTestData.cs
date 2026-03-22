using FindexiumAPI.Domain;
using FindexiumAPI.Models;

namespace FindexiumAPI.Tests.TestData
{
    public class UserTestData
    {
        // Scenarios for GetAllUserAsync and GetUserByIdAsync
        public static IEnumerable<object[]> GetUsersScenarios()
        {
            yield return new object[] { new List<User>() };

            yield return new object[]
            {
                new List<User>
                {
                    new User { Id = "1", UserName = "abc", FullName = "Abc", Role = "Admin"}
                }
            };

            yield return new object[]
            {
                new List<User>
                {
                    new User { Id = "2", UserName = "def", FullName = "Def", Role = "Admin"},
                    new User { Id = "3", UserName = "ghi", FullName = "Ghi", Role = "User"}
                }
            };
        }

        // Scénarios pour CreateUserAsync
        public static IEnumerable<object[]> GetCreateUserDuplicateScenarios()
        {
            yield return new object[]
            {
                new CreateUserDto
                {
                    UserName = "abc",
                    FullName = "Abc",
                    Role = "Admin",
                    Password = "Bonjour123!",
                    ConfirmPassword = "Bonjour123!"
                }
            };
        }

        public static IEnumerable<object[]> GetCreateUserPasswordMismatchScenarios()
        {
            yield return new object[]
            {
                new CreateUserDto
                {
                    UserName = "pqr",
                    FullName = "Pqr",
                    Role = "Admin",
                    Password = "Bonjour123!",
                    ConfirmPassword = "Aurevoir456?"
                }
            };
        }

        public static IEnumerable<object[]> GetCreateUserValidScenarios()
        {
            yield return new object[]
            {
                new CreateUserDto
                {
                    UserName = "def",
                    FullName = "Def",
                    Role = "Admin",
                    Password = "Bonjour123!",
                    ConfirmPassword = "Bonjour123!"
                }
            };

            yield return new object[]
            {
                new CreateUserDto
                {
                    UserName = "ghi",
                    FullName = "Ghi",
                    Role = "User",
                    Password = "Bonjour123!",
                    ConfirmPassword = "Bonjour123!"
                }
            };

            yield return new object[]
            {
                new CreateUserDto
                {
                    UserName = "jkl",
                    FullName = "Jkl",
                    Role = "NotARole", // Role should default to "User"
                    Password = "Bonjour123!",
                    ConfirmPassword = "Bonjour123!"
                }
            };

            yield return new object[]
            {
                new CreateUserDto // No role provided, should default to "User"
                {
                    UserName = "mno",
                    FullName = "Mno",
                    Password = "Bonjour123!",
                    ConfirmPassword = "Bonjour123!"
                }
            };
        }

        // Scenarios for UpdateUserAsync
        public static IEnumerable<object[]> GetUpdateUserScenariosWithData()
        {
            yield return new object[]
            {
                new List<User>()
                {
                    new User { Id = "1", UserName = "abc", FullName = "Abc", Role = "Admin"},
                    new User { Id = "2", UserName = "def", FullName = "Def", Role = "Admin"},
                    new User { Id = "3", UserName = "ghi", FullName = "Ghi", Role = "User"},
                    new User { Id = "4", UserName = "alreadyExists", FullName = "Already Exists", Role = "User"}
                },
                new UserDto { Id = "1", UserName = "abc_updated", FullName = "Abc Updated", Role = "User"}
            };

            yield return new object[]
            {
                new List<User>()
                {
                    new User { Id = "1", UserName = "abc", FullName = "Abc", Role = "Admin"},
                    new User { Id = "2", UserName = "def", FullName = "Def", Role = "Admin"},
                    new User { Id = "3", UserName = "ghi", FullName = "Ghi", Role = "User"},
                    new User { Id = "4", UserName = "alreadyExists", FullName = "Already Exists", Role = "User"}
                },
                new UserDto { Id = "2", UserName = "def_updated", FullName = "Def Updated", Role = ""}
            };

            yield return new object[]
            {
                new List<User>()
                {
                    new User { Id = "1", UserName = "abc", FullName = "Abc", Role = "Admin"},
                    new User { Id = "2", UserName = "def", FullName = "Def", Role = "Admin"},
                    new User { Id = "3", UserName = "ghi", FullName = "Ghi", Role = "User"},
                    new User { Id = "4", UserName = "alreadyExists", FullName = "Already Exists", Role = "User"}
                },
                new UserDto { Id = "3", UserName = "ghi_updated", FullName = "Ghi Updated", Role = "Admin"}
            };

            yield return new object[]
            {
                new List<User>()
                {
                    new User { Id = "1", UserName = "abc", FullName = "Abc", Role = "Admin"},
                    new User { Id = "2", UserName = "def", FullName = "Def", Role = "Admin"},
                    new User { Id = "3", UserName = "ghi", FullName = "Ghi", Role = "User"},
                    new User { Id = "4", UserName = "alreadyExists", FullName = "Already Exists", Role = "User"}
                },
                new UserDto { Id = "1", UserName = "alreadyExists", FullName = "Already Exists", Role = "User"}
            };
        }
    }
}
