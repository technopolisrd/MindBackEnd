using Microsoft.Extensions.Configuration;
using Mind.Context.Data.Security;
using Mind.Entity.Tables;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using Common.Core.Contracts.Common;

namespace Mind.Repository.Common
{
    public class Audit
    {

        private readonly IConfiguration Configuration;

        public Audit(IConfiguration configuration) 
        { 
            Configuration = configuration;
        }

        public void FillAudit<T>(ref T entity, int state, int user) where T : IAuditableEntity, IDeferrableEntity
        {
            if (state == 0) // si esta insertando
            {
                entity.Deferred = false;
                entity.CreatedDate = DateTime.Now;
                entity.UpdatedDate = DateTime.Now;

                using (var context = new MindContext(Configuration))
                {
                    entity.CreatedById = (from account in context.Accounts where account.Id == user select account.FirstName + " " + account.LastName).FirstOrDefault();
                    entity.UpdatedById = entity.CreatedById;
                }
            }
            else if (state == 1) // si esta editando
            {
                entity.Deferred = false;
                entity.UpdatedDate = DateTime.Now;

                using (var context = new MindContext(Configuration))
                {
                    entity.UpdatedById = (from account in context.Accounts where account.Id == user select account.FirstName + " " + account.LastName).FirstOrDefault();
                }
            }
            else // si esta borrando
            {
                entity.Deferred = true;
                entity.UpdatedDate = DateTime.Now;

                using (var context = new MindContext(Configuration))
                {
                    entity.UpdatedById = (from account in context.Accounts where account.Id == user select account.FirstName + " " + account.LastName).FirstOrDefault();
                }
            }
        }
    }
}
