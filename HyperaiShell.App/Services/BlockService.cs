using HyperaiShell.App.Models;
using HyperaiShell.Foundation.Data;
using HyperaiShell.Foundation.Services;
using System;

namespace HyperaiShell.App.Services
{
    public class BlockService : IBlockService
    {
        private readonly IRepository _repository;

        public BlockService(IRepository repository)
        {
            _repository = repository;
        }

        public void Ban(long id, string reason)
        {
            _repository.Upsert(new BlockedUser() { UserId = id, Reason = reason, Enrollment = DateTime.Now, IsBanned = true });
        }

        public void Deban(long id)
        {
            BlockedUser user = _repository.Query<BlockedUser>().Where(x => x.UserId == id).FirstOrDefault();
            if (user != null)
            {
                user.IsBanned = false;
                _repository.Update<BlockedUser>(user);
            }
        }

        public bool IsBanned(long id, out string reason)
        {
            BlockedUser user = _repository.Query<BlockedUser>().Where(x => x.UserId == id).FirstOrDefault();
            if (user == null)
            {
                reason = null;
                return false;
            }
            else
            {
                reason = user.IsBanned ? user.Reason : null;
                return user.IsBanned;
            }
        }
    }
}