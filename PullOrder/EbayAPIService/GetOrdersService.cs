using DataBase.Models;
using EbayAPIService.Models;
using PullOrder.Base;
using PullOrder.Base.Models;
using ServiceReference1;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using static ServiceReference1.eBayAPIInterfaceClient;

namespace EbayAPIService
{
    public class GetOrdersService : Base, IDisposable
    {
        private EbayAPISettings EbayApiSettings;
        private ApiUserPlatformToken ApiUser;
        private int currentPage;
        private eBayAPIInterfaceClient service;
        private List<EbayOrderResult> ebayOrderResults;
        public LogPullOrder EbayApilog;
        public GetOrdersService(ApiUserPlatformToken apiuser)
        {
            EbayApiSettings = SetEbayAPIConfig();
            this.ApiUser = apiuser;
            currentPage = 1;
            string requestURL = "https://api.ebay.com/wsapi"
               + "?callname=" + "GetOrders"
               + "&siteid=" + EbayApiSettings.SiteID
               + "&appid=" + EbayApiSettings.AppID
               + "&version=" + EbayApiSettings.Version
               + "&routing=new";
            service = new eBayAPIInterfaceClient(EndpointConfiguration.eBayAPI, requestURL);
        }

        public async Task<List<EbayOrderResult>> GetOrdersAsync(EbayOrderRequest ebayOrderRequest)
        {
            PullOrderLog = EbayApilog;
            ebayOrderResults = new List<EbayOrderResult>();
            CustomSecurityHeaderType requesterCredentials = new CustomSecurityHeaderType
            {
                eBayAuthToken = ApiUser.AccessToken,
                Credentials = new UserIdPasswordType()
                {
                    AppId = EbayApiSettings.AppID,
                    DevId = EbayApiSettings.DevID,
                    AuthCert = EbayApiSettings.AuthCert
                }
            };

            while (true)
            {
                try
                {
                    GetOrdersRequestType request = new GetOrdersRequestType()
                    {
                        Pagination = new PaginationType()
                        {
                            PageNumber = currentPage++,
                            PageNumberSpecified = true,
                            EntriesPerPage = 100,
                            EntriesPerPageSpecified = true
                        },

                        NumberOfDays = ebayOrderRequest.numberOfDays,
                        //設定為True 則Request參數僅需要此為條件否則會默認列入其他條件
                        NumberOfDaysSpecified = true,
                        Version = EbayApiSettings.Version
                    };

                    GetOrdersResponseType response = (await service.GetOrdersAsync(requesterCredentials, request)).GetOrdersResponse1;
                    if (response.Ack == AckCodeType.Failure || response.Ack == AckCodeType.PartialFailure)
                    {
                        string errors = string.Empty;
                        foreach (ErrorType e in response.Errors)
                        {
                            errors += e.ShortMessage + e.LongMessage + " /";
                        }
                        SetFailLog(errors);
                        break;
                    }

                    ProcessResponse(response);

                    if (response.HasMoreOrdersSpecified && response.HasMoreOrders)
                    {
                        continue;
                    }
                    break;
                }
                catch (Exception ex)
                {
                    SetFailLog(ex.ToString());
                    break;
                }
            }
            return ebayOrderResults;
        }

       
        private void ProcessResponse(GetOrdersResponseType response)
        {
            if (response.OrderArray == null)
                return;

            foreach (OrderType orderType in response.OrderArray)
            {
                List<EbayOrderResult> r = ProcessOrder(orderType);
                ebayOrderResults.AddRange(r);
            }
        }

        private List<EbayOrderResult> ProcessOrder(OrderType order)
        {
            List<EbayOrderResult> r = new List<EbayOrderResult>();
            if (order.TransactionArray != null)
            {
                foreach (TransactionType transactionType in order.TransactionArray)
                {
                    EbayOrderResult orderResult = ProcessTransaction(transactionType);
                    orderResult.orderId = order.OrderID;
                    r.Add(orderResult);
                }
            }
            return r;
        }

        private EbayOrderResult ProcessTransaction(TransactionType transaction)
        {
            EbayOrderResult ebayOrderResult = new EbayOrderResult()
            {
                transactionSiteId = transaction.TransactionSiteID.ToString()
            };
            return ebayOrderResult;
        }

        public void Dispose()
        {

        }
    }
}
