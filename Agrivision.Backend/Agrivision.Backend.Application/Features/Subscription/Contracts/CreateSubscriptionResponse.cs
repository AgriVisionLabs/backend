using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agrivision.Backend.Application.Features.Subscription.Contracts;
public record CreateSubscriptionResponse
(
    string CheckOutSession_URL
 );
