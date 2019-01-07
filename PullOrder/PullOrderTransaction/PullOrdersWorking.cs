using DataBase;
using DataBase.Models;
using EbayAPIService;
using EbayAPIService.Models;
using PullOrder.Base.Models;
using PullOrder.Base;
using PullOrderTransaction.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PullOrderTransaction
{
    public class PullOrdersWorking : Base
    {

        private ApiUserPlatformToken apiUserPlatformToken;
        private readonly PullOrdersPayload PayLoad;
        private List<EbayOrderResult> orders;
        public LogPullOrder workinglog;
        public PullOrdersWorking(PullOrdersPayload payload)
        {
            this.PayLoad = payload;
        }

        public async Task<LogPullOrder> WorkingAsync()
        {
            this.PullOrderLog = workinglog;
            try
            {
                PullOrderLog = await GetOrdersFromEBayAsync();
                if (PullOrderLog.ProcessStatus == (int)EnumLogStatus.Fail)
                    return PullOrderLog;

                InSertPullOrderTransactionToDB();
                PullOrderLog.ProcessMessage = "Success : Get" + orders.Count.ToString() + " Orders";
                PullOrderLog.ProcessEndTime = DateTime.UtcNow;
                PullOrderLog.ProcessStatus = (int)EnumLogStatus.Success;
            }
            catch (Exception ex)
            {
                this.SetFailLog(ex.ToString());
            }
            return PullOrderLog;
        }

        private void InSertPullOrderTransactionToDB()
        {
            if (orders == null)
                return;

            using (mardevContext context = new mardevContext())
            {
                string LastOrderID = string.Empty;

                if (orders != null && orders.Count > 0)
                    LastOrderID = orders.Last().orderId;

                Guid NewTokenFromID = Guid.NewGuid();
                foreach (EbayOrderResult ebayOrderResult in orders)
                {
                    QueuePullOrder NewQueue = new QueuePullOrder()
                    {
                        ApiUserPlatformTokenId = PayLoad.apiUserPlatformTokenId,
                        PullOrderFromID = NewTokenFromID,
                        OrderId = ebayOrderResult.orderId,
                        SiteCode = ebayOrderResult.transactionSiteId,                 
                    };
                    context.QueuePullOrder.Add(NewQueue);
                };
                context.SaveChanges();
            }
        }

        private async Task<LogPullOrder> GetOrdersFromEBayAsync()
        {
            try
            {
                string error = string.Empty;
                using (mardevContext context = new mardevContext())
                {
                    apiUserPlatformToken = context.ApiUserPlatformToken
                         .Where(x => x.ApiUserPlatformTokenId == PayLoad.apiUserPlatformTokenId).FirstOrDefault();
                };

                if (apiUserPlatformToken == null)
                {
                    error = PayLoad.apiUserPlatformTokenId.ToString() + " is not exist in Database";
                    return SetFailLog(error);
                }
                if (string.IsNullOrWhiteSpace(apiUserPlatformToken.AccessToken))
                {
                    error = PayLoad.apiUserPlatformTokenId.ToString() + " hasn't AccessToken in ApiUserPlatformToken";
                    return SetFailLog(error);
                }

                using (GetOrdersService service = new GetOrdersService(apiUserPlatformToken))
                {
                    service.EbayApilog = PullOrderLog;
                    EbayOrderRequest ebayOrderRequest = new EbayOrderRequest()
                    { numberOfDays = PayLoad.numberOfDays };

                    orders = await service.GetOrdersAsync(ebayOrderRequest);
                }

                if (orders != null && orders.Count > 0)
                    orders = orders.GroupBy(x => x.orderId).Select(x => new EbayOrderResult
                    {
                        orderId = x == null ? "" : x.FirstOrDefault().orderId,
                        transactionSiteId = x == null ? "" : x.FirstOrDefault().transactionSiteId
                    }).ToList();
            }
            catch (Exception ex)
            {
                SetFailLog(ex.ToString());
            }

            return PullOrderLog;
        }
    }
}
