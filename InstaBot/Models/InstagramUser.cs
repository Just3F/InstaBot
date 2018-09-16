namespace InstaBot.Service.Models
{
    public class InstagramUser
    {
        public string UserName { get; }
        public string Password { get; }

        public InstagramUser(string userName, string password)
        {
            this.UserName = userName;
            this.Password = password;
        }

        public override bool Equals(object obj)
        {
            var item = obj as InstagramUser;

            if (item == null)
            {
                return false;
            }

            return this.UserName.Equals(item.UserName) && this.Password.Equals(item.Password);
        }

        public override int GetHashCode()
        {
            return this.UserName.GetHashCode() + this.Password.GetHashCode();
        }
    }
}
