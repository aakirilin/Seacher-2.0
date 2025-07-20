using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Seacher
{
    public class licenseFlyoutMenuItem
    {
        public licenseFlyoutMenuItem()
        {
            TargetType = typeof(licenseFlyoutMenuItem);
        }
        public int Id { get; set; }
        public string Title { get; set; }

        public Type TargetType { get; set; }
    }
}