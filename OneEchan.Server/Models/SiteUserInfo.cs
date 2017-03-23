using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OneEchan.Server.Models
{
    public class SiteUserInfo
    {
        public enum State
        {
            Normal,
            Blocked,
        }
        public int Id { get; set; }
        public Guid UserId { get; set; }
        public State UserState { get; set; }

    }
}
