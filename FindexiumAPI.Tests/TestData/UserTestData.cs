using Castle.Core.Configuration;
using FindexiumAPI.Domain;
using FindexiumAPI.Models;
using System.ComponentModel.DataAnnotations;

namespace FindexiumAPI.Tests.TestData
{
    public class UserTestData
    {
        public static IEnumerable<object[]> GetUsersScenarios()
        {
            yield return new object[]
            {
                new List<User>()
            };

            yield return new object[]
            {
                new List<User>
                {
                    new User { Id = 1.ToString(), UserName = "abc", FullName = "Abc", Role = "Admin"}
                }
            };

            yield return new object[]
            {
                new List<User>()
                {
                    new User { Id = 2.ToString(), UserName = "def", FullName = "Def", Role = "Admin"},
                    new User { Id = 3.ToString(), UserName = "ghi", FullName = "Ghi", Role = "User"}
                }
            };
        }

        public static IEnumerable<object[]> GetCreateUsersScenarios()
        {
            yield return new object[]
            {
                new CreateUserDto()
                {
                    UserName = "abc",
                    FullName = "Abc",
                    Role = "Admin",
                    Password = "Bonjour123!",
                    ConfirmPassword = "Bonjour123!"
                }
            };

            yield return new object[]
            {
                new CreateUserDto()
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
                new CreateUserDto()
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
                new CreateUserDto()
                {
                    UserName = "jkl",
                    FullName = "Jkl",
                    Role = "NotARole",
                    Password = "Bonjour123!",
                    ConfirmPassword = "Bonjour123!"
                }
            };

            yield return new object[]
            {
                new CreateUserDto()
                {
                    UserName = "mno",
                    FullName = "Mno",
                    Password = "Bonjour123!",
                    ConfirmPassword = "Bonjour123!"
                }
            };

            yield return new object[]
            {
                new CreateUserDto()
                {
                    UserName = "pqr",
                    FullName = "Pqr",
                    Role = "Admin",
                    Password = "Bonjour123!",
                    ConfirmPassword = "Aurevoir456?"
                }
            };
        }


        public static IEnumerable<object[]> GetUsersToUpdateScenarios()
        {
            yield return new object[]
            {
                new List<User>()
                {
                    new User { Id = 1.ToString(), UserName = "abc", FullName = "Abc", Role = "Admin"},
                    new User { Id = 2.ToString(), UserName = "def", FullName = "Def", Role = "Admin"},
                    new User { Id = 3.ToString(), UserName = "ghi", FullName = "Ghi", Role = "User"},
                    new User { Id = 4.ToString(), UserName = "alreadyExists", FullName = "Already Exists", Role = "User"}
                }
            };
        }

        public static IEnumerable<object[]> GetUserUpdatesScenarios()
        {
            yield return new object[]
            {
                new UserDto { Id = 1.ToString(), UserName = "abc_updated", FullName = "Abc Updated", Role = "User"}
            };
            yield return new object[]
            {
                new UserDto { Id = 2.ToString(), UserName = "def_updated", FullName = "Def Updated", Role = ""}
            };
            yield return new object[]
            {
                new UserDto { Id = 3.ToString(), UserName = "ghi_updated", FullName = "Ghi Updated", Role = "Admin"}
            };
            yield return new object[]
            {
                new UserDto { Id = 1.ToString(), UserName = "alreadyExists", FullName = "Already Exists", Role = "User"}
            };
            yield return new object[]
            {
                new UserDto { Id = 5.ToString(), UserName = "wrongId", FullName = "Wrong Id", Role = "Admin"}
            };
        }

        public static IEnumerable<object[]> GetUpdateUserScenariosWithData()
        {
            yield return new object[]
            {
                new List<User>()
                {
                    new User { Id = 1.ToString(), UserName = "abc", FullName = "Abc", Role = "Admin"},
                    new User { Id = 2.ToString(), UserName = "def", FullName = "Def", Role = "Admin"},
                    new User { Id = 3.ToString(), UserName = "ghi", FullName = "Ghi", Role = "User"},
                    new User { Id = 4.ToString(), UserName = "alreadyExists", FullName = "Already Exists", Role = "User"}
                },
                new UserDto { Id = 1.ToString(), UserName = "abc_updated", FullName = "Abc Updated", Role = "User"}
            };
            yield return new object[]
            {
                new List<User>()
                {
                    new User { Id = 1.ToString(), UserName = "abc", FullName = "Abc", Role = "Admin"},
                    new User { Id = 2.ToString(), UserName = "def", FullName = "Def", Role = "Admin"},
                    new User { Id = 3.ToString(), UserName = "ghi", FullName = "Ghi", Role = "User"},
                    new User { Id = 4.ToString(), UserName = "alreadyExists", FullName = "Already Exists", Role = "User"}
                },
                new UserDto { Id = 2.ToString(), UserName = "def_updated", FullName = "Def Updated", Role = ""}
            };
            yield return new object[]
            {
                new List<User>()
                {
                    new User { Id = 1.ToString(), UserName = "abc", FullName = "Abc", Role = "Admin"},
                    new User { Id = 2.ToString(), UserName = "def", FullName = "Def", Role = "Admin"},
                    new User { Id = 3.ToString(), UserName = "ghi", FullName = "Ghi", Role = "User"},
                    new User { Id = 4.ToString(), UserName = "alreadyExists", FullName = "Already Exists", Role = "User"}
                },
                new UserDto { Id = 3.ToString(), UserName = "ghi_updated", FullName = "Ghi Updated", Role = "Admin"}
            };
            yield return new object[]
            {
                new List<User>()
                {
                    new User { Id = 1.ToString(), UserName = "abc", FullName = "Abc", Role = "Admin"},
                    new User { Id = 2.ToString(), UserName = "def", FullName = "Def", Role = "Admin"},
                    new User { Id = 3.ToString(), UserName = "ghi", FullName = "Ghi", Role = "User"},
                    new User { Id = 4.ToString(), UserName = "alreadyExists", FullName = "Already Exists", Role = "User"}
                },
                new UserDto { Id = 1.ToString(), UserName = "alreadyExists", FullName = "Already Exists", Role = "User"}
            };
        }
    }
}