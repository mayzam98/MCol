using MCol.DAL.Modelo;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCol.BLL.Controller
{
    public abstract class BaseControllerBLL
    {
        protected readonly IDbContextFactory<ColegiosCOLContext> _contextFactory;

        protected BaseControllerBLL(IDbContextFactory<ColegiosCOLContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        protected ColegiosCOLContext CreateDbContext()
        {
            return _contextFactory.CreateDbContext();
        }
    }
}
