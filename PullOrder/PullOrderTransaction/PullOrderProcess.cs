using DataBase.Models;
using PullOrder.Base.Models;
using PullOrderTransaction.Models;
using System;
using System.Threading.Tasks;
using PullOrder.Base;

namespace PullOrderTransaction
{
    public class PullOrderProcess : Base
    {
        public async Task Process(QueuePullOrderUserToken EbayUser)
        {
            PullOrderLog = new LogPullOrder()
            {
                ProcessStartTime = DateTime.UtcNow,
                ProcessStatus = (int)EnumLogStatus.Pending,
                ApiUserPlatformTokenId = EbayUser.ApiUserPlatformTokenId
            };
            try
            {
                PullOrdersPayload payload = new PullOrdersPayload()
                {
                    apiUserPlatformTokenId = EbayUser.ApiUserPlatformTokenId,
                    //天數小於一天算一天 大於一個月算一個月
                    numberOfDays = EbayUser.NumberOfDays < 1 ? 1 : (EbayUser.NumberOfDays > 30 ? 30 : EbayUser.NumberOfDays)
                };
                PullOrdersWorking PullOrdersWorker = new PullOrdersWorking(payload)
                { workinglog = PullOrderLog };

                PullOrderLog = await PullOrdersWorker.WorkingAsync();
            }
            catch (Exception ex)
            {
                this.SetFailLog(ex.ToString());
            }

            this.SaveLogToDB();
        }
    }
}
