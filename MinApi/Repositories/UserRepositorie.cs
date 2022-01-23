using MinApi.Models;

namespace MinApi.Repositories
{
    public static class UserRepositorie
    {
        public static User Get(string userName, string password)
        {
            var user = new List<User>
            {
                new User{Id = 1, UserName = "Batman", Password = "123",Role ="manager"},
                new User{Id = 1, UserName = "Robin", Password = "456",Role ="employee"},
            };
            return user.Where(x => x.UserName.ToLower()== userName.ToLower() && x.Password == password).FirstOrDefault();
        }
    }
}
